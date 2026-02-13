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
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Dorisoy.Pan.MediatR.Handlers
{
    public class UploadDocumentsCommandHandler : IRequestHandler<UploadDocumentsCommand, ServiceResponse<DocumentDto>>
    {

        private readonly IDocumentRepository _documentRepository;
        private readonly IVirtualFolderRepository _virtualFolderRepository;
        private readonly IPhysicalFolderRepository _physicalFolderRepository;
        private readonly IUnitOfWork<DocumentContext> _uow;
        private readonly IMapper _mapper;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly PathHelper _pathHelper;
        private readonly UserInfoToken _userInfoToken;
        private readonly IDocumentVersionRepository _documentVersionRepository;
        private readonly IDocumentSharedUserRepository _documentSharedUserRepository;
        private readonly IUserRepository _userRepository;

        public UploadDocumentsCommandHandler(
            IDocumentRepository documentRepository,
            IVirtualFolderRepository virtualFolderRepository,
            IUnitOfWork<DocumentContext> uow,
            IMapper mapper,
            IWebHostEnvironment webHostEnvironment,
            PathHelper pathHelper,
            IPhysicalFolderRepository physicalFolderRepository,
            IDocumentVersionRepository documentVersionRepository,
            UserInfoToken userInfoToken,
            IDocumentSharedUserRepository documentSharedUserRepository,
            IUserRepository userRepository)
        {
            _documentRepository = documentRepository;
            _virtualFolderRepository = virtualFolderRepository;
            _uow = uow;
            _mapper = mapper;
            _webHostEnvironment = webHostEnvironment;
            _pathHelper = pathHelper;
            _physicalFolderRepository = physicalFolderRepository;
            _userInfoToken = userInfoToken;
            _documentVersionRepository = documentVersionRepository;
            _documentSharedUserRepository = documentSharedUserRepository;
            _userRepository = userRepository;
        }

        public async Task<ServiceResponse<DocumentDto>> Handle(UploadDocumentsCommand request, CancellationToken cancellationToken)
        {
            var folder = await _virtualFolderRepository.FindAsync(request.FolderId);
            var fileToSave = request.Documents[0];
            var extension = Path.GetExtension(fileToSave.FileName);
            var fileName = $"{Guid.NewGuid()}{extension}";
            var thumbnailPath = $"{_webHostEnvironment.WebRootPath}\\{_pathHelper.DocumentPath}";
            var folderPath = await _physicalFolderRepository.GetParentFolderPath(folder.PhysicalFolderId);
            var documentPath = $"{_webHostEnvironment.ContentRootPath}\\{_pathHelper.DocumentPath}{folderPath}";
            var isExisting = false;
            if (!Directory.Exists(documentPath))
            {
                Directory.CreateDirectory(documentPath);
            }

            var document = await _documentRepository.All
                .Where(c => c.PhysicalFolderId == folder.PhysicalFolderId
                && c.Name == fileToSave.FileName
                && (c.CreatedBy == _userInfoToken.Id || c.SharedDocumentUsers.Any(c => c.UserId == _userInfoToken.Id)))
                .FirstOrDefaultAsync();

            // Document Version 
            if (document != null)
            {
                isExisting = true;
                if (!Directory.Exists($"{documentPath}\\{document.Id}"))
                {
                    Directory.CreateDirectory($"{documentPath}\\{document.Id}");
                }

                var versionFileName = document.Path.Replace(folderPath, "").Replace("\\", "");
                var sourceFile = $"{_webHostEnvironment.ContentRootPath}\\{_pathHelper.DocumentPath}{document.Path}";
                var detinationFile = $"{documentPath}\\{document.Id}\\{versionFileName}";

                File.Move(sourceFile, detinationFile);

                using (var stream = new FileStream($"{documentPath}\\{fileName}", FileMode.Create))
                {
                    fileToSave.CopyTo(stream);
                }

                _documentVersionRepository.Add(new DocumentVersion
                {
                    DocumentId = document.Id,
                    Path = $"{folderPath}\\{document.Id}\\{versionFileName}",
                    Size = document.Size,
                    CreatedBy = document.CreatedBy,
                    CreatedDate = document.CreatedDate,
                    ModifiedDate = document.ModifiedDate,
                    ModifiedBy = document.ModifiedBy
                });
                document.Path = $"{folderPath}\\{fileName}";
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
                    PhysicalFolderId = folder.PhysicalFolderId,
                    Extension = extension,
                    Path = $"{folderPath}\\{fileName}",
                    Size = fileToSave.Length,
                    Name = fileToSave.FileName,
                    ThumbnailPath = ThumbnailHelper.SaveThumbnailFile(fileToSave, fileName, $"{folderPath}\\{fileName}", _pathHelper.DocumentPath, _pathHelper.EncryptionKey)
                };
                try
                {
                    using (var stream = new FileStream($"{documentPath}\\{fileName}", FileMode.Create))
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

            var documentToReturn = _mapper.Map<DocumentDto>(document);
            documentToReturn.ThumbnailPath = $"{_pathHelper.DocumentPath}{documentToReturn.ThumbnailPath}";
            var documentUsers = new List<UserInfoDto>();
            if (isExisting)
            {
                documentUsers = await _documentSharedUserRepository
                    .All
                    .Where(c => c.DocumentId == document.Id)
                    .Select(cs => new UserInfoDto
                    {
                        Id = cs.User.Id,
                        Email = cs.User.Email,
                        FirstName = cs.User.FirstName,
                        LastName = cs.User.LastName,
                        IsOwner = cs.UserId == document.CreatedBy
                    }).ToListAsync();
            }
            var createdByUser = await _userRepository.FindAsync(document.CreatedBy);
            if (createdByUser != null)
            {
                documentUsers.Add(new UserInfoDto
                {
                    Id = createdByUser.Id,
                    Email = createdByUser.Email,
                    FirstName = createdByUser.FirstName,
                    LastName = createdByUser.LastName,
                    IsOwner = true
                });
            }
            documentToReturn.Users = documentUsers;
            return ServiceResponse<DocumentDto>.ReturnResultWith201(documentToReturn);
        }
    }
}
