using Dorisoy.Pan.Data.Dto;
using Dorisoy.Pan.Helper;
using Dorisoy.Pan.MediatR.Commands;
using Dorisoy.Pan.Repository;
using MediatR;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Dorisoy.Pan.MediatR.Handlers
{
    public class SendFolderCommadHandler
        : IRequestHandler<SendFolderCommad, ServiceResponse<bool>>
    {
        private readonly IPhysicalFolderRepository _physicalFolderRepository;
        private readonly IDocumentRepository _documentRepository;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly PathHelper _pathHelper;
        private readonly IEmailSMTPSettingRepository _emailSMTPSettingRepository;
        private readonly ILogger<SendFolderCommadHandler> _logger;
        private readonly UserInfoToken _userInfoToken;

        public SendFolderCommadHandler(IPhysicalFolderRepository physicalFolderRepository,
            IDocumentRepository documentRepository,
            IWebHostEnvironment webHostEnvironment,
            PathHelper pathHelper,
            IEmailSMTPSettingRepository emailSMTPSettingRepository,
            ILogger<SendFolderCommadHandler> logger,
            UserInfoToken userInfoToken)
        {
            _physicalFolderRepository = physicalFolderRepository;
            _documentRepository = documentRepository;
            _webHostEnvironment = webHostEnvironment;
            _pathHelper = pathHelper;
            _emailSMTPSettingRepository = emailSMTPSettingRepository;
            _logger = logger;
            _userInfoToken = userInfoToken;
        }

        public async Task<ServiceResponse<bool>> Handle(SendFolderCommad request, CancellationToken cancellationToken)
        {
            var defaultSmtp = await _emailSMTPSettingRepository.FindBy(c => c.IsDefault).FirstOrDefaultAsync();
            if (defaultSmtp == null)
            {
                _logger.LogError("Default SMTP setting does not exist.");
                return ServiceResponse<bool>.Return404("Default SMTP setting does not exist.");
            }

            var folder = await _physicalFolderRepository.FindAsync(request.Id);

            if (folder == null)
            {
                return ServiceResponse<bool>.Return404("Folder not found.");
            }

            var listOfDocuments = new List<DownloadDocumentDto>();
            var childs = await _physicalFolderRepository.GetChildsHierarchyById(request.Id);
            var parentpath = await _physicalFolderRepository.GetParentFolderPath(request.Id);
            var parentOriginalPath = await _physicalFolderRepository.GetParentOriginalFolderPath(request.Id);
            parentpath = string.Join(Path.DirectorySeparatorChar, parentpath.Split(Path.DirectorySeparatorChar).SkipLast(1).ToList()) + Path.DirectorySeparatorChar;
            parentOriginalPath = string.Join(Path.DirectorySeparatorChar, parentOriginalPath.Split(Path.DirectorySeparatorChar).SkipLast(1).ToList()) + Path.DirectorySeparatorChar;
            var folderIdsToDownload = new List<Guid> { request.Id };
            folderIdsToDownload.AddRange(childs.Select(c => c.Id).ToList());
            var documentsToAdd = await _documentRepository.All.Where(c => EF.Constant(folderIdsToDownload).Contains(c.PhysicalFolderId))
                .Select(c => new DownloadDocumentDto
                {
                    Name = c.Name,
                    Path = c.Path,
                    Id = c.Id,
                    FolderId = c.PhysicalFolderId
                }).ToListAsync();

            var uniquiFolderId = documentsToAdd.Select(c => c.FolderId).Distinct().ToList();
            Dictionary<Guid, string> folderPaths = new Dictionary<Guid, string>();

            foreach (var folderId in uniquiFolderId)
            {
                var path = await _physicalFolderRepository.GetParentOriginalFolderPath(folderId);
                folderPaths.Add(folderId, path);
            }

            documentsToAdd.ForEach(r =>
            {
                folderPaths.TryGetValue(r.FolderId, out var folderPath);
                r.FolderPath = r.Path.Replace(parentpath, "");
                r.OriginalFolderPath = Path.Combine(_pathHelper.ContentRootPath, _pathHelper.DocumentPath, r.Path);
                r.Path = Path.Combine(folderPath.Replace(parentOriginalPath, ""), r.Name);
            });
            listOfDocuments.AddRange(documentsToAdd);

            // using var ms = new MemoryStream();
            using var msFinal = new MemoryStream();
            using (var zipArchive = new ZipArchive(msFinal, ZipArchiveMode.Create, true))
            {
                foreach (var attachment in listOfDocuments)
                {
                    string path = attachment.Name;
                    if (!string.IsNullOrWhiteSpace(attachment.Path))
                    {
                        path = string.Join(Path.DirectorySeparatorChar, attachment.Path.Split(Path.DirectorySeparatorChar).SkipLast(1)) + Path.DirectorySeparatorChar + attachment.Name;
                    }
                    var entry = zipArchive.CreateEntry(path);
                    //using var fileStream = new FileStream(attachment.OriginalFolderPath, FileMode.Open);
                    //using var entryStream = entry.Open();
                    //await fileStream.CopyToAsync(entryStream);


                    byte[] newBytes;

                    using (var stream = new FileStream(attachment.OriginalFolderPath, FileMode.Open))
                    {
                        using (var entryStream = entry.Open())
                        {
                            var ms = new MemoryStream();
                            stream.CopyTo(ms);
                            var latestBytes = ms.ToArray();
                            ms.Close();
                            ms.Dispose();
                            newBytes = AesOperation.DecryptStream(latestBytes, _pathHelper.EncryptionKey);
                            entryStream.Write(newBytes);
                        }
                    }
                }
            }
            msFinal.Position = 0;
            try
            {
                var attachment = new System.Net.Mail.Attachment(msFinal, $"{folder.Name}.zip");
                EmailHelper.SendFileOrFolder(new SendEmailSpecification
                {
                    Body = request.Body,
                    FromAddress = _userInfoToken.Email,
                    Host = defaultSmtp.Host,
                    IsEnableSSL = defaultSmtp.IsEnableSSL,
                    Password = defaultSmtp.Password,
                    Port = defaultSmtp.Port,
                    Subject = request.Subject,
                    ToAddress = request.ToAddress,
                    CCAddress = request.CCAddress,
                    UserName = defaultSmtp.UserName,
                    Attachment = attachment
                });
                return ServiceResponse<bool>.ReturnSuccess();
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message, e);
                return ServiceResponse<bool>.ReturnFailed(500, e.Message);
            }

        }
    }
}
