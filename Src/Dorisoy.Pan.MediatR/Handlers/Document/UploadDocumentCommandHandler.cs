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
using System.Collections.Generic;
using System.Diagnostics;
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
        /// <summary>
        ///  文件保存
        /// TODO 并发上传一文件问题
        /// </summary>
        /// <param name="request"></param>
        /// <param name="documentPath"></param>
        /// <param name="saveName"></param>
        private void SaveFile(UploadDocumentCommand request, string documentPath, string saveName)
        {
            var tempPath = Path.Combine(documentPath, request.Md5);
            if (!Directory.Exists(documentPath))
            {
                Directory.CreateDirectory(documentPath);
            }
            if (!Directory.Exists(tempPath))
            {
                Directory.CreateDirectory(tempPath);
            }

            var bytesData = AesOperation.ReadAsBytesAsync(request.Documents[0]);
            var size = request.Total.ToString().Length;

            var temp = Path.Combine(tempPath, request.Index.ToString().PadLeft(size, '0'));
            if (!File.Exists(temp))
            {
                using (var stream = new FileStream(temp, FileMode.Create))
                {
                    stream.Write(bytesData, 0, bytesData.Length);
                }
            }
            if (request.Index >= request.Total)
            {
                var path = Path.Combine(documentPath, saveName);
                var files = new DirectoryInfo(tempPath).GetFiles().OrderBy(p => p.Name);
                if (files.Count() > 0)
                {
                    var list = new List<byte>();
                    foreach (var item in files)
                    {
                        var bytes = File.ReadAllBytes(item.FullName);
                        list.AddRange(bytes);
                        File.Delete(item.FullName);
                    }
                    var datas = list.ToArray();
                    using (var stream = new FileStream(path, FileMode.Create))
                    {
                        var byteArray = AesOperation.EncryptStream(datas, _pathHelper.EncryptionKey);
                        stream.Write(byteArray, 0, byteArray.Length);
                    }
                    Directory.Delete(tempPath, true);
                }
            }
        }
        public async Task<ServiceResponse<DocumentDto>> Handle(UploadDocumentCommand request, CancellationToken cancellationToken)
        {
            var fullFileName = string.IsNullOrEmpty(request.FullPath) ? request.Documents[0].FileName : request.FullPath;

            var parentId = await GetDocumentParentIdAndPath(request.FolderId, fullFileName);
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
            var saveName = $"{request.Md5}{extension}";
            var now = DateTime.Now.ToString("yyyyMMdd");
            //数据库存储相对路径
            var saveFullPath = Path.Combine(now, saveName);
            //物理文件目录
            var documentPath = Path.Combine(_pathHelper.ContentRootPath, _pathHelper.DocumentPath, now);

            //文件是否存在
            var old = await _documentRepository.All.FirstOrDefaultAsync(p => p.Md5 == request.Md5);
            var oldDocument = await _documentRepository.All
                     .Where(c => c.PhysicalFolderId == parentId
                     && c.Md5 == request.Md5
                     && (c.CreatedBy == _userInfoToken.Id
                     || c.SharedDocumentUsers.Any(c => c.UserId == _userInfoToken.Id)
                     || c.Folder.PhysicalFolderUsers.Any(c => c.UserId == _userInfoToken.Id)
                     ))
                     .FirstOrDefaultAsync();
            if (old != null && oldDocument != null && old.Id == oldDocument.Id)
            {
                return ServiceResponse<DocumentDto>.Return400("文件已存在.");
            }
            var include = old != null;
            if (request.Index < request.Total && !include)
            {
                SaveFile(request, documentPath, saveName);
                return ServiceResponse<DocumentDto>.ReturnResultWith201(null);
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
                if (!include)
                {
                    SaveFile(request, documentPath, saveName);
                }

                _documentVersionRepository.Add(new DocumentVersion
                {
                    DocumentId = document.Id,
                    Path = saveFullPath,
                    Size = request.Size,
                    CreatedBy = document.CreatedBy,
                    CreatedDate = document.CreatedDate,
                    ModifiedDate = document.ModifiedDate,
                    ModifiedBy = document.ModifiedBy
                });
                document.Path = saveFullPath;
                document.Size = fileToSave.Length;
                _documentRepository.Update(document);
                if (await _uow.SaveAsync() <= 0)
                {
                    // revert move
                    return ServiceResponse<DocumentDto>.Return500();
                }
            }
            else
            {
                try
                {
                    if (!include)
                    {
                        SaveFile(request, documentPath, saveName);
                    }

                    document = new Document
                    {
                        Id = Guid.NewGuid(),
                        Extension = extension,
                        Path = saveFullPath,
                        Size = request.Size,
                        Md5 = request.Md5,
                        Name = fileName,
                        ThumbnailPath = include ? old.ThumbnailPath : ThumbnailHelper.SaveThumbnailFile(fileToSave, saveName, documentPath, Path.Combine(_pathHelper.ContentRootPath, _pathHelper.DocumentPath), _pathHelper.EncryptionKey),
                        PhysicalFolderId = parentId
                    };

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
            var folderPaths = fullFileName.Split("/").SkipLast(1).ToList();
            if (folderPaths.Count() == 0)
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
