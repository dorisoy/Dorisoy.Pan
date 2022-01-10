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
    public class CopyDocumentCommandHandler : IRequestHandler<CopyDocumentCommand, bool>
    {
        private readonly IPhysicalFolderRepository _physicalFolderRepository;
        private readonly IUnitOfWork<DocumentContext> _uow;
        private readonly IVirtualFolderRepository _virtualFolderRepository;
        private readonly IDocumentRepository _documentRepository;
        private readonly UserInfoToken _userInfoToken;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly PathHelper _pathHelper;
        private readonly UserInfoToken _userInfo;

        public CopyDocumentCommandHandler(
              IUnitOfWork<DocumentContext> uow,
            IVirtualFolderRepository virtualFolderRepository,
            IPhysicalFolderRepository physicalFolderRepository,
            IDocumentRepository documentRepository,
            UserInfoToken userInfoToken,
            IWebHostEnvironment webHostEnvironment,
            PathHelper pathHelper,
            UserInfoToken userInfo
            )
        {
            _uow = uow;
            _virtualFolderRepository = virtualFolderRepository;
            _physicalFolderRepository = physicalFolderRepository;
            _documentRepository = documentRepository;
            _userInfoToken = userInfoToken;
            _webHostEnvironment = webHostEnvironment;
            _pathHelper = pathHelper;
            _userInfo = userInfo;
        }


        public async Task<bool> Handle(CopyDocumentCommand request, CancellationToken cancellationToken)
        {
            var sourceDocument = await _documentRepository
                .FindAsync(request.DocumentId);
            var distinationFolder = await _virtualFolderRepository.All
                .Include(c => c.PhysicalFolder)
                .Where(c => c.Id == request.DestinationFolderId)
                .FirstOrDefaultAsync();
            var documentSystemName = sourceDocument.Path.Split(Path.DirectorySeparatorChar).LastOrDefault();
            var originalPath = sourceDocument.Path;
            var originalThumbPath = sourceDocument.ThumbnailPath;
            var containerPhysicalFolderPath = await _physicalFolderRepository.GetParentFolderPath(distinationFolder.PhysicalFolder.Id);
            var documentName = await _documentRepository.GetDocumentName(sourceDocument.Name, distinationFolder.PhysicalFolder.Id, 0, _userInfo.Id);
            var extension = Path.GetExtension(documentSystemName);

            var newDocumentId = Guid.NewGuid();
            var copyFileName = newDocumentId + extension;
            Document document = new Document
            {
                Id = newDocumentId,
                Name = documentName,
                Path = Path.Combine(_userInfoToken.Id.ToString(), copyFileName),
                ThumbnailPath = ThumbnailHelper.IsSystemThumnails(sourceDocument.ThumbnailPath) ? sourceDocument.ThumbnailPath : Path.Combine(_userInfoToken.Id.ToString(), "_thumbnail_" + copyFileName),
                PhysicalFolderId = distinationFolder.PhysicalFolder.Id,
                Extension = sourceDocument.Extension,
                Size = sourceDocument.Size
            };
            _documentRepository.Add(document);
            if (await _uow.SaveAsync() <= 0)
            {
                return false;
            }
            try
            {
                var physicalContainerFolderPath = Path.Combine(_pathHelper.ContentRootPath, _pathHelper.DocumentPath, _userInfoToken.Id.ToString());
                if (!Directory.Exists(physicalContainerFolderPath))
                {
                    Directory.CreateDirectory(physicalContainerFolderPath);
                }
                var containerPath = Path.Combine(physicalContainerFolderPath, copyFileName);
                originalPath = Path.Combine(_pathHelper.ContentRootPath, _pathHelper.DocumentPath, originalPath);
                File.Copy(originalPath, containerPath, true);

                originalThumbPath = Path.Combine(_webHostEnvironment.WebRootPath, _pathHelper.DocumentPath, originalThumbPath);
                var containerThumbPhysicalFolderPath = Path.Combine(_webHostEnvironment.WebRootPath, _pathHelper.DocumentPath, document.ThumbnailPath);
                if (!ThumbnailHelper.IsSystemThumnails(sourceDocument.ThumbnailPath) && File.Exists(originalThumbPath))
                {
                    if (!Directory.Exists(Path.Combine(_webHostEnvironment.WebRootPath, _pathHelper.DocumentPath, _userInfoToken.Id.ToString())))
                    {
                        Directory.CreateDirectory(Path.Combine(_webHostEnvironment.WebRootPath, _pathHelper.DocumentPath, _userInfoToken.Id.ToString()));
                    }
                    //containerThumbPhysicalFolderPath = $"{containerThumbPhysicalFolderPath}\\_thumbnail_{ copyFileName}";
                    File.Copy(originalThumbPath, containerThumbPhysicalFolderPath, true);
                }
            }
            catch (Exception ex)
            {
                return false;
            }
            return true;
        }
    }
}
