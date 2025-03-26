using AutoMapper;
using Dorisoy.Pan.Common.UnitOfWork;
using Dorisoy.Pan.Data;
using Dorisoy.Pan.Data.Dto;
using Dorisoy.Pan.Domain;
using Dorisoy.Pan.Helper;
using Dorisoy.Pan.MediatR.Commands;
using Dorisoy.Pan.Repository;
using MediatR;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Dorisoy.Pan.MediatR.Handlers
{
    public class UploadDocumentCommandHandler : IRequestHandler<UploadDocumentCommand, ServiceResponse<DocumentDto>>
    {
        private readonly IPhysicalFolderRepository _physicalFolderRepository;
        private readonly IDocumentVersionRepository _documentVersionRepository;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly PathHelper _pathHelper;
        private readonly IDocumentRepository _documentRepository;
        private readonly IUnitOfWork<DocumentContext> _uow;
        private readonly IMapper _mapper;
        private readonly UserInfoToken _userInfoToken;

        public UploadDocumentCommandHandler(
            IPhysicalFolderRepository physicalFolderRepository,
            IDocumentVersionRepository documentVersionRepository,
            IWebHostEnvironment webHostEnvironment,
            PathHelper pathHelper,
            IDocumentRepository documentRepository,
            IUnitOfWork<DocumentContext> uow,
            IMapper mapper,
            UserInfoToken userInfoToken)
        {
            _physicalFolderRepository = physicalFolderRepository;
            _documentVersionRepository = documentVersionRepository;
            _webHostEnvironment = webHostEnvironment;
            _pathHelper = pathHelper;
            _documentRepository = documentRepository;
            _uow = uow;
            _mapper = mapper;
            _userInfoToken = userInfoToken;
        }
        public async Task<ServiceResponse<DocumentDto>> Handle(UploadDocumentCommand request, CancellationToken cancellationToken)
        {
            var fullFileName = string.IsNullOrEmpty(request.FullPath) ? request.Documents[0].FileName : request.FullPath;
     
            var parentId  = await GetDocumentParentIdAndPath(request.FolderId, fullFileName);
            if (Guid.Empty == parentId)
            {
                return ServiceResponse<DocumentDto>.Return404();
            }

            var fileToSave = request.Documents[0];
            var fileName = fullFileName.Split("/").LastOrDefault();
            if (string.IsNullOrEmpty(fileName))
            {
                fileName = fullFileName.Split("\\").LastOrDefault();
            }
            var extension = Path.GetExtension(fileName);
            if (_pathHelper.ExecutableFileTypes.Any(c => c == extension))
            {
                return ServiceResponse<DocumentDto>.Return400("Executable file is not allowed to upload.");
            }
            var path = $"{Guid.NewGuid()}{extension}";

            var documentPath = Path.Combine(_pathHelper.ContentRootPath, _pathHelper.DocumentPath, _userInfoToken.Id.ToString());

            if (!Directory.Exists(documentPath))
            {
                Directory.CreateDirectory(documentPath);
            }

            var document = await _documentRepository.All
                .Where(c => c.PhysicalFolderId == parentId
                && c.Name == fileName
                && (c.CreatedBy == _userInfoToken.Id
                || c.SharedDocumentUsers.Any(c => c.UserId == _userInfoToken.Id)
                || c.Folder.PhysicalFolderUsers.Any(c => c.UserId == _userInfoToken.Id)
                ))
                .FirstOrDefaultAsync();
            if (document != null)
            {
                var versionPath = Path.Combine(documentPath, document.Id.ToString());
                if (!Directory.Exists(versionPath))
                {
                    Directory.CreateDirectory(versionPath);
                }

                //var versionFileName = document.Path.Replace($"{folderPath}\\{childFolderPath}", "");

                var versionFileName = $"{Guid.NewGuid()}{extension}";
                // var sourceFile = $"{_webHostEnvironment.ContentRootPath}\\{_pathHelper.DocumentPath}{document.Path}";
                var sourceFile = Path.Combine(_pathHelper.ContentRootPath, _pathHelper.DocumentPath, document.Path);
                //var detinationFile = $"{documentPath}{document.Id}\\{versionFileName}";
                var detinationFile = Path.Combine(versionPath, versionFileName);
                File.Move(sourceFile, detinationFile);

                foreach (var file in request.Documents)
                {
                    var bytesData = AesOperation.ReadAsBytesAsync(file);

                    using (var stream = new FileStream(Path.Combine(documentPath, path), FileMode.Create))
                    {
                        var byteArray = AesOperation.EncryptStream(bytesData, _pathHelper.EncryptionKey);
                        stream.Write(byteArray, 0, byteArray.Length);
                    }
                }
                //Path = Path.Combine(_userInfoToken.Id.ToString(), document.Id.ToString(), versionFileName),
                _documentVersionRepository.Add(new DocumentVersion
                {
                    DocumentId = document.Id,
                    Path = Path.Combine(_userInfoToken.Id.ToString(), document.Id.ToString(), versionFileName),
                    Size = document.Size,
                    CreatedBy = document.CreatedBy,
                    CreatedDate = document.CreatedDate,
                    ModifiedDate = document.ModifiedDate,
                    ModifiedBy = document.ModifiedBy
                });
                // document.Path = $"{folderPath}\\{childFolderPath}{path}";
                document.Path = Path.Combine(_userInfoToken.Id.ToString(), path);
                document.Size = fileToSave.Length;
                _documentRepository.Update(document);
                if (await _uow.SaveAsync() <= 0)
                {
                    // revert move
                    File.Move(detinationFile, sourceFile);
                    return ServiceResponse<DocumentDto>.Return500();
                }
            }
            else
            {
                var documentId = Guid.NewGuid();
                document = new Document
                {
                    Id = documentId,
                    Extension = extension,
                    Path = Path.Combine(_userInfoToken.Id.ToString(), path),
                    Size = fileToSave.Length,
                    Name = fileName,
                    ThumbnailPath = ThumbnailHelper.SaveThumbnailFile(fileToSave, path, _pathHelper.DocumentPath),
                    PhysicalFolderId = parentId
                };
                try
                {
                    foreach (var file in request.Documents)
                    {
                        var bytesData = AesOperation.ReadAsBytesAsync(file);
                        var storePath = Path.Combine(documentPath, path);
                        using (var stream = new FileStream(storePath, FileMode.Create))
                        {
                            var byteArray = AesOperation.EncryptStream(bytesData, _pathHelper.EncryptionKey);
                            stream.Write(byteArray, 0, byteArray.Length);
                        }
                    }

                    _documentRepository.Add(document);

                    if (await _uow.SaveAsync() <= 0)
                    {
                        return ServiceResponse<DocumentDto>.Return500();
                    }
                }
                catch (Exception ex)
                {
                    return ServiceResponse<DocumentDto>.ReturnException(ex);
                }
            }
            document.ThumbnailPath = ThumbnailHelper.GetThumbnailFile(_pathHelper.DocumentPath, document.ThumbnailPath);
            var documentDto = _mapper.Map<DocumentDto>(document);
            return ServiceResponse<DocumentDto>.ReturnResultWith201(documentDto);
        }

        private async Task<Guid> GetDocumentParentIdAndPath(Guid rootId, string fullFileName)
        {
            var folderPaths =  fullFileName.Split("/").SkipLast(1).ToList();
            if (folderPaths.Count()==0)
            {
                folderPaths = fullFileName.Split("\\").SkipLast(1).ToList();
            }
            var parentId = rootId;
            foreach (var folderName in folderPaths)
            {
                var folder = await _physicalFolderRepository.All
                    .FirstOrDefaultAsync(c => c.PhysicalFolderUsers.Any(c => c.UserId == _userInfoToken.Id)
                        && c.ParentId == parentId
                        && c.Name == folderName);
                if (folder == null)
                {
                    return Guid.Empty;
                }
                else
                {
                    parentId = folder.Id;
                }
            }
            return parentId;
        }
    }
}
