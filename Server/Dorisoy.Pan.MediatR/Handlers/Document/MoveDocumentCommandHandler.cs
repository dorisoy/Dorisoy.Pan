using Dorisoy.Pan.Common.UnitOfWork;
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
    public class MoveDocumentCommandHandler : IRequestHandler<MoveDocumentCommand, bool>
    {
        private readonly IPhysicalFolderRepository _physicalFolderRepository;
        private readonly IUnitOfWork<DocumentContext> _uow;
        private readonly IVirtualFolderRepository _virtualFolderRepository;
        private readonly IDocumentRepository _documentRepository;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly PathHelper _pathHelper;
        private readonly UserInfoToken _userInfoToken;

        public MoveDocumentCommandHandler(
              IUnitOfWork<DocumentContext> uow,
            IVirtualFolderRepository virtualFolderRepository,
            IPhysicalFolderRepository physicalFolderRepository,
            IDocumentRepository documentRepository,
            IWebHostEnvironment webHostEnvironment,
            PathHelper pathHelper,
            UserInfoToken userInfoToken
            )
        {
            _uow = uow;
            _virtualFolderRepository = virtualFolderRepository;
            _physicalFolderRepository = physicalFolderRepository;
            _documentRepository = documentRepository;
            _webHostEnvironment = webHostEnvironment;
            _pathHelper = pathHelper;
            _userInfoToken = userInfoToken;
        }

        public async Task<bool> Handle(MoveDocumentCommand request, CancellationToken cancellationToken)
        {
            var sourceDocument = await _documentRepository
                .FindAsync(request.DocumentId);
            var distinationFolder = await _virtualFolderRepository.All
                .Include(c => c.PhysicalFolder)
                .Where(c => c.Id == request.DestinationFolderId)
                .FirstOrDefaultAsync();

            var originalPath = sourceDocument.Path;
            var originalThumbPath = sourceDocument.ThumbnailPath;
            //var containerPhysicalFolderPath = await _physicalFolderRepository.GetParentFolderPath(distinationFolder.PhysicalFolder.Id);
            var documentName = await _documentRepository.GetDocumentName(sourceDocument.Name, distinationFolder.PhysicalFolder.Id, 0, _userInfoToken.Id);
            sourceDocument.PhysicalFolderId = distinationFolder.PhysicalFolder.Id;
            sourceDocument.Name = documentName;

            var documentSystemName = sourceDocument.Path.Split(Path.DirectorySeparatorChar).LastOrDefault();

            sourceDocument.Path = Path.Combine(_userInfoToken.Id.ToString(), documentSystemName);
            sourceDocument.ThumbnailPath = ThumbnailHelper.IsSystemThumnails(sourceDocument.ThumbnailPath) ? sourceDocument.ThumbnailPath : Path.Combine(_userInfoToken.Id.ToString(), "_thumbnail_" + documentSystemName);
            _documentRepository.Update(sourceDocument);
            if (await _uow.SaveAsync() <= 0)
            {
                return false;
            }
            try
            {
                var containerPath = Path.Combine(_pathHelper.ContentRootPath, _pathHelper.DocumentPath, sourceDocument.Path);
                originalPath = Path.Combine(_pathHelper.ContentRootPath, _pathHelper.DocumentPath, originalPath);

                var containerThumbPath = Path.Combine(_pathHelper.ContentRootPath, _pathHelper.DocumentPath, sourceDocument.ThumbnailPath);
                originalThumbPath = Path.Combine(_pathHelper.ContentRootPath, _pathHelper.DocumentPath, originalThumbPath);

                if (!Directory.Exists(Path.Combine(_pathHelper.ContentRootPath, _pathHelper.DocumentPath, _userInfoToken.Id.ToString())))
                {
                    Directory.CreateDirectory(Path.Combine(_pathHelper.ContentRootPath, _pathHelper.DocumentPath, _userInfoToken.Id.ToString()));
                }

                File.Move(originalPath, containerPath, true);
                if (!ThumbnailHelper.IsSystemThumnails(originalThumbPath) && File.Exists(originalThumbPath))
                {
                    if (!Directory.Exists(Path.Combine(_pathHelper.ContentRootPath, _pathHelper.DocumentPath, _userInfoToken.Id.ToString())))
                    {
                        Directory.CreateDirectory(Path.Combine(_pathHelper.ContentRootPath, _pathHelper.DocumentPath, _userInfoToken.Id.ToString()));
                    }
                    File.Move(originalThumbPath, containerThumbPath, true);
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
