using Dorisoy.Pan.Common.UnitOfWork;
using Dorisoy.Pan.Data.Dto;
using Dorisoy.Pan.Domain;
using Dorisoy.Pan.Helper;
using Dorisoy.Pan.MediatR.Commands;
using Dorisoy.Pan.Repository;
using MediatR;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Dorisoy.Pan.MediatR.Handlers
{
    public class DeleteDeletedDocumentCommandHandler : IRequestHandler<DeleteDeletedDocumentCommand, ServiceResponse<bool>>
    {
        private readonly IDocumentSharedUserRepository _documentSharedUserRepository;
        private readonly IDocumentRepository _documentRepository;
        private readonly IDocumentDeletedRepository _documentDeletedRepository;
        private readonly IPhysicalFolderRepository _physicalFolderRepository;
        private readonly UserInfoToken _userInfoToken;
        private readonly IUnitOfWork<DocumentContext> _uow;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly PathHelper _pathHelper;
        private readonly ILogger<DeleteDeletedDocumentCommandHandler> _logger;

        public DeleteDeletedDocumentCommandHandler(IDocumentSharedUserRepository documentSharedUserRepository,
            IDocumentRepository documentRepository,
            IDocumentDeletedRepository documentDeletedRepository,
            IPhysicalFolderRepository physicalFolderRepository,
            UserInfoToken userInfoToken,
            IUnitOfWork<DocumentContext> uow,
            IWebHostEnvironment webHostEnvironment,
            PathHelper pathHelper,
            ILogger<DeleteDeletedDocumentCommandHandler> logger)
        {
            _documentSharedUserRepository = documentSharedUserRepository;
            _documentRepository = documentRepository;
            _documentDeletedRepository = documentDeletedRepository;
            _physicalFolderRepository = physicalFolderRepository;
            _userInfoToken = userInfoToken;
            _uow = uow;
            _webHostEnvironment = webHostEnvironment;
            _pathHelper = pathHelper;
            _logger = logger;
        }
        public async Task<ServiceResponse<bool>> Handle(DeleteDeletedDocumentCommand request, CancellationToken cancellationToken)
        {
            var document = await _documentRepository
                .All
                .IgnoreQueryFilters()
                .FirstOrDefaultAsync(c => c.Id == request.Id);

            if (document == null)
            {
                _logger.LogError("Document not found.", request);
                return ServiceResponse<bool>.Return404();
            }

            var isOwner = document.CreatedBy == _userInfoToken.Id;
            if (isOwner)
            {
                var sharedUsers = await _documentSharedUserRepository.All
                        .Where(c => c.DocumentId == request.Id)
                        .ToArrayAsync();
                _documentSharedUserRepository.RemoveRange(sharedUsers);

                var deletedUsers = await _documentDeletedRepository.All
                    .IgnoreQueryFilters()
                     .Where(c => c.DocumentId == request.Id)
                     .ToArrayAsync();

                _documentDeletedRepository.RemoveRange(deletedUsers);

                _documentRepository.Remove(document);
            }
            else
            {
                var sharedUsers = await _documentSharedUserRepository.All
                    .Where(c => c.DocumentId == request.Id && c.UserId == _userInfoToken.Id)
                    .ToArrayAsync();
                _documentSharedUserRepository.RemoveRange(sharedUsers);

                var deletedUsers = await _documentDeletedRepository.All
                    .IgnoreQueryFilters()
                    .Where(c => c.DocumentId == request.Id && c.UserId == _userInfoToken.Id)
                    .ToArrayAsync();

                _documentDeletedRepository.RemoveRange(deletedUsers);
            }

            if (await _uow.SaveAsync() <= 0)
            {
                _logger.LogError("Error while deleting the document.", request);
                return ServiceResponse<bool>.Return500();
            }

            if (isOwner)
            {
                // delete document
                var fullDocumentpath = Path.Combine(_pathHelper.ContentRootPath,_pathHelper.DocumentPath,document.Path);
                if (File.Exists(fullDocumentpath))
                {
                    try
                    {
                        File.Delete(fullDocumentpath);
                    }
                    catch
                    {
                        _logger.LogError("Error while deleting the document from dist.", request);
                    }
                }

                // delete document version
                var rootPath = await _physicalFolderRepository.GetParentFolderPath(document.PhysicalFolderId);
                var versionFolder = Path.Combine(_pathHelper.ContentRootPath,_pathHelper.DocumentPath, _userInfoToken.Id.ToString(), document.Id.ToString());
                if (Directory.Exists(versionFolder))
                {
                    try
                    {
                        Directory.Delete(versionFolder, true);
                    }
                    catch
                    {
                        _logger.LogError("Error while deleting the document version from disk.", request);
                    }
                }
            }
            return ServiceResponse<bool>.ReturnSuccess();
        }
    }
}
