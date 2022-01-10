using Dorisoy.Pan.Common.UnitOfWork;
using Dorisoy.Pan.Data;
using Dorisoy.Pan.Data.Dto;
using Dorisoy.Pan.Domain;
using Dorisoy.Pan.Helper;
using Dorisoy.Pan.MediatR.Commands;
using Dorisoy.Pan.Repository;
using MediatR;
using Microsoft.AspNetCore.Hosting;
using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Dorisoy.Pan.MediatR.Handlers
{
    public class RestoreDocumentVersionCommandHandler
        : IRequestHandler<RestoreDocumentVersionCommand, ServiceResponse<bool>>
    {
        private readonly IDocumentVersionRepository _documentVersionRepository;
        private readonly IDocumentRepository _documentRepository;
        private readonly IPhysicalFolderRepository _physicalFolderRepository;
        private readonly IUnitOfWork<DocumentContext> _uow;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly PathHelper _pathHelper;
        private readonly UserInfoToken _userInfoToken;

        public RestoreDocumentVersionCommandHandler(IDocumentVersionRepository documentVersionRepository,
            IDocumentRepository documentRepository,
            IPhysicalFolderRepository physicalFolderRepository,
            IUnitOfWork<DocumentContext> uow,
            IWebHostEnvironment webHostEnvironment,
            PathHelper pathHelper,
            UserInfoToken userInfoToken)
        {
            _documentVersionRepository = documentVersionRepository;
            _documentRepository = documentRepository;
            _physicalFolderRepository = physicalFolderRepository;
            _uow = uow;
            _webHostEnvironment = webHostEnvironment;
            _pathHelper = pathHelper;
            _userInfoToken = userInfoToken;
        }
        public async Task<ServiceResponse<bool>> Handle(RestoreDocumentVersionCommand request, CancellationToken cancellationToken)
        {
            var document = await _documentRepository.FindAsync(request.DocumentId);
            var originalPath = document.Path;
            if (document == null)
            {
                return ServiceResponse<bool>.Return404();
            }
            var version = _documentVersionRepository
                .All.FirstOrDefault(c => c.Id == request.Id && c.DocumentId == request.DocumentId);
            if (version == null)
            {
                return ServiceResponse<bool>.Return404();
            }

            var rootPath = Path.Combine(_pathHelper.ContentRootPath, _pathHelper.DocumentPath);

            var documentSystemName = document.Path.Split(Path.DirectorySeparatorChar).LastOrDefault();
            var documentVersionSystemName = version.Path.Split(Path.DirectorySeparatorChar).LastOrDefault();

            var docSize = document.Size;
            var versionSize = version.Size;

            var versionId = Guid.NewGuid();
            _documentVersionRepository.Add(new DocumentVersion
            {
                Id = versionId,
                DocumentId = document.Id,
                Path = Path.Combine(_userInfoToken.Id.ToString(), document.Id.ToString(), versionId.ToString()+ document.Extension),
                Size = document.Size,
                CreatedBy = document.CreatedBy,
                CreatedDate = document.CreatedDate,
                ModifiedDate = document.ModifiedDate,
                ModifiedBy = document.ModifiedBy
            });

            document.Path = Path.Combine(_userInfoToken.Id.ToString(), documentVersionSystemName);
            document.Size = versionSize;
           
            // Copy Version File
            File.Copy(Path.Combine(rootPath, originalPath), Path.Combine(rootPath, _userInfoToken.Id.ToString(),document.Id.ToString(), versionId.ToString() + document.Extension));

            // Move Document File
            File.Move(Path.Combine(rootPath, version.Path), Path.Combine(rootPath, _userInfoToken.Id.ToString(), documentVersionSystemName));

            _documentRepository.Update(document);
            if (await _uow.SaveAsync() <= 0)
            {
                // Revert Document File
                //File.Move($"{folderpathPath}\\{document.Id}\\{documentSystemName}", $"{folderpathPath}\\{documentSystemName}");
                File.Move(Path.Combine(rootPath, _userInfoToken.Id.ToString(), documentVersionSystemName), Path.Combine(rootPath, version.Path));

                return ServiceResponse<bool>.Return500();
            }
            return ServiceResponse<bool>.ReturnSuccess(); ;
        }
    }
}
