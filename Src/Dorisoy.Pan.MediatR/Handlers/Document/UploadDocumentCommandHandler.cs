using AutoMapper;
using Dorisoy.Pan.Common;
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
using NewLife.Redis.Core;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
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
        private readonly INewLifeRedis _redis;
        private readonly object Writing = true;
        private readonly object Combining = true;
        private readonly object cache = true;
        public UploadDocumentCommandHandler(
            IPhysicalFolderRepository physicalFolderRepository,
            IDocumentVersionRepository documentVersionRepository,
            IWebHostEnvironment webHostEnvironment,
            PathHelper pathHelper,
            IDocumentRepository documentRepository,
            IUnitOfWork<DocumentContext> uow,
            IMapper mapper,
            INewLifeRedis redis,
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
            _redis = redis;
        }
        #region 文件保存
        /// <summary>
        ///  文件保存
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
            SetCache(request.Md5);

            var bytesData = AesOperation.ReadAsBytesAsync(request.Documents[0]);
            var size = request.Total.ToString().Length;

            var temp = Path.Combine(tempPath, request.Index.ToString().PadLeft(size, '0'));
            if (!File.Exists(temp))
            {
                if (CanLock(request.Md5))
                {
                    lock (Writing)
                    {
                        WriteFile(temp, bytesData);
                    }
                }
                else
                {
                    WriteFile(temp, bytesData);
                }
            }
            if (request.Index >= request.Total)
            {
                var path = Path.Combine(documentPath, saveName);
                if (CanLock(request.Md5))
                {
                    lock (Combining)
                    {
                        CombinFile(tempPath, path);
                    }
                }
                else
                {
                    CombinFile(tempPath, path);
                }
                if (_redis.ContainsKey(request.Md5))
                {
                    RemoveCache(request.Md5);
                }
            }
        }
        private bool CanLock(string md5)
        {
            if (!_redis.ContainsKey(md5))
                return false;
            return _redis.ListGetAll<string>(md5).Count > 1;
        }
        private void SetCache(string md5)
        {
            lock (cache)
            {
                if (_redis.ContainsKey(md5))
                {
                    var user = _redis.ListIndexOf(md5, $"\"{_userInfoToken.Id}\"");
                    if (user == -1)
                    {
                        _redis.ListAdd(md5, _userInfoToken.Id);
                    }
                }
                else
                {
                    _redis.ListAdd(md5, _userInfoToken.Id);
                }
            }

        }
        private void RemoveCache(string md5)
        {
            _redis.Remove(md5);
        }

        private void WriteFile(string path, byte[] bytes)
        {
            try
            {
                if (File.Exists(path)) return;
                using (var stream = new FileStream(path, FileMode.Create))
                {
                    stream.Write(bytes, 0, bytes.Length);
                }
            }
            catch (IOException ex)
            {
                if (ex.Message.Contains("another process"))
                {
                    return;
                }
                throw ex;
            }

        }
        private void CombinFile(string tempPath, string path)
        {
            try
            {
                if (File.Exists(path)) return;
                LargeFileEncryptor.EncryptFile(tempPath, path, _pathHelper.EncryptionKey);
                Directory.Delete(tempPath, true);
            }
            catch (IOException ex)
            {
                if (ex.Message.Contains("another process"))
                {
                    return;
                }
                throw ex;
            }
        }
        #endregion
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
            if (old != null && oldDocument != null)
            {
                //并发上传一份文件处理
                if (_redis.ContainsKey(request.Md5) && _redis.ListGetAll<string>(request.Md5).Count > 0)
                {
                    RemoveCache(request.Md5);
                    var oldDocumentDto = _mapper.Map<DocumentDto>(oldDocument);
                    return ServiceResponse<DocumentDto>.ReturnResultWith201(oldDocumentDto);
                }
                //正常上传处理
                return ServiceResponse<DocumentDto>.Return400("文件已存在:" + oldDocument.Path);
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
