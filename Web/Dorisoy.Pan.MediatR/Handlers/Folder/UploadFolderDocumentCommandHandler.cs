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
    public class UploadFolderDocumentCommandHandler : IRequestHandler<UploadFolderDocumentCommand, ServiceResponse<DocumentDto>>
    {
        private readonly IPhysicalFolderRepository _physicalFolderRepository;
        private readonly IVirtualFolderRepository _virtualFolderRepository;
        private readonly IDocumentVersionRepository _documentVersionRepository;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly PathHelper _pathHelper;
        private readonly IDocumentRepository _documentRepository;
        private readonly IUnitOfWork<DocumentContext> _uow;
        private readonly IMapper _mapper;
        private readonly UserInfoToken _userInfoToken;

        public UploadFolderDocumentCommandHandler(
            IPhysicalFolderRepository physicalFolderRepository,
            IVirtualFolderRepository virtualFolderRepository,
            IDocumentVersionRepository documentVersionRepository,
            IWebHostEnvironment webHostEnvironment,
            PathHelper pathHelper,
            IDocumentRepository documentRepository,
            IUnitOfWork<DocumentContext> uow,
            IMapper mapper,
            UserInfoToken userInfoToken)
        {
            _physicalFolderRepository = physicalFolderRepository;
            _virtualFolderRepository = virtualFolderRepository;
            _documentVersionRepository = documentVersionRepository;
            _webHostEnvironment = webHostEnvironment;
            _pathHelper = pathHelper;
            _documentRepository = documentRepository;
            _uow = uow;
            _mapper = mapper;
            _userInfoToken = userInfoToken;
        }
        public async Task<ServiceResponse<DocumentDto>> Handle(UploadFolderDocumentCommand request, CancellationToken cancellationToken)
        {
            var fullFileName = string.IsNullOrEmpty(request.FullPath) ? request.Documents[0].FileName : request.FullPath;
            var (parentId, childFolderPath) = await GetDocumentParentIdAndPath(request.FolderId, fullFileName);
            if (!parentId.HasValue)
            {
                return ServiceResponse<DocumentDto>.Return404();
            }

            var fileToSave = request.Documents[0];
            var fileName = fullFileName.Split("/").LastOrDefault();
            var extension = Path.GetExtension(fileName);
            if (_pathHelper.ExecutableFileTypes.Any(c => c == extension))
            {
                return ServiceResponse<DocumentDto>.Return400("Executable file is not allowed to upload.");
            }
            var path = $"{Guid.NewGuid()}{extension}";
            var folderPath = await _physicalFolderRepository.GetParentFolderPath(request.FolderId);
            var documentPath = $"{_webHostEnvironment.ContentRootPath}\\{_pathHelper.DocumentPath}{folderPath}\\{childFolderPath}";
            var thumbnailPath = $"{_webHostEnvironment.WebRootPath}\\{_pathHelper.DocumentPath}";

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
                if (!Directory.Exists($"{documentPath}{document.Id}"))
                {
                    Directory.CreateDirectory($"{documentPath}{document.Id}");
                }

                var versionFileName = document.Path.Replace($"{folderPath}\\{childFolderPath}", "");
                var sourceFile = $"{_webHostEnvironment.ContentRootPath}\\{_pathHelper.DocumentPath}{document.Path}";
                var detinationFile = $"{documentPath}{document.Id}\\{versionFileName}";

                File.Move(sourceFile, detinationFile);

                using (var stream = new FileStream($"{documentPath}{path}", FileMode.Create))
                {
                    fileToSave.CopyTo(stream);
                }

                _documentVersionRepository.Add(new DocumentVersion
                {
                    DocumentId = document.Id,
                    Path = $"{folderPath}\\{childFolderPath}{document.Id}\\{versionFileName}",
                    Size = document.Size,
                    CreatedBy = document.CreatedBy,
                    CreatedDate = document.CreatedDate,
                    ModifiedDate = document.ModifiedDate,
                    ModifiedBy = document.ModifiedBy
                });
                document.Path = $"{folderPath}\\{childFolderPath}{path}";
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
                    Path = $"{folderPath}\\{childFolderPath}{path}",
                    Size = fileToSave.Length,
                    Name = fileName,
                    ThumbnailPath = ThumbnailHelper.SaveThumbnailFile(fileToSave, path, $"{folderPath}\\{childFolderPath}{path}", _pathHelper.DocumentPath, _pathHelper.EncryptionKey),
                    PhysicalFolderId = parentId.Value
                };
                try
                {
                    using (var stream = new FileStream($"{documentPath}{path}", FileMode.Create))
                    {
                        fileToSave.CopyTo(stream);
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
            document.ThumbnailPath = $"{_pathHelper.DocumentPath}{document.ThumbnailPath}";
            var documentDto = _mapper.Map<DocumentDto>(document);
            return ServiceResponse<DocumentDto>.ReturnResultWith201(documentDto);
        }

        private async Task<(Guid?, string)> GetDocumentParentIdAndPath(Guid rootId, string fullFileName)
        {
            var folderPaths = fullFileName.Split("/").SkipLast(1).ToList();
            var parentId = rootId;
            var path = "";
            foreach (var folderName in folderPaths)
            {
                var folder = await _physicalFolderRepository.All
                    .FirstOrDefaultAsync(c => c.PhysicalFolderUsers.Any(c => c.UserId == _userInfoToken.Id)
                        && c.ParentId == parentId
                        && c.Name == folderName);
                if (folder == null)
                {
                    return (null, path);
                }
                else
                {
                    parentId = folder.Id;
                    path = $"{path}{folder.SystemFolderName}\\";
                }
            }
            return (parentId, path);
        }
    }
}