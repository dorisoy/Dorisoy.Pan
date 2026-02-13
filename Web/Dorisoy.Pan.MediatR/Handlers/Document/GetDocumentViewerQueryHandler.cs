using Dorisoy.Pan.Data.Dto;
using Dorisoy.Pan.Helper;
using Dorisoy.Pan.MediatR.Queries;
using Dorisoy.Pan.Repository;
using MediatR;
using Microsoft.AspNetCore.Hosting;
using System;
using System.Threading;
using System.Threading.Tasks;
using System.IO;

namespace Dorisoy.Pan.MediatR.Handlers
{
    public class GetDocumentViewerQueryHandler : IRequestHandler<GetDocumentViewerQuery, DocumentSource>
    {
        private readonly IDocumentRepository _documentRepository;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly PathHelper _pathHelper;

        public GetDocumentViewerQueryHandler(
              IDocumentRepository documentRepository,
            IWebHostEnvironment webHostEnvironment,
            PathHelper pathHelper)
        {
            _documentRepository = documentRepository;
            _webHostEnvironment = webHostEnvironment;
            _pathHelper = pathHelper;
        }
        public async Task<DocumentSource> Handle(GetDocumentViewerQuery request, CancellationToken cancellationToken)
        {
            var sourceDocument = await _documentRepository
               .FindAsync(request.DocumentId);

            var containerPath = Path.Combine(_pathHelper.ContentRootPath, _pathHelper.DocumentPath, sourceDocument.Path);
            byte[] fileBytes = await System.IO.File.ReadAllBytesAsync(containerPath);
            var documentSource = new DocumentSource();
            documentSource.Source = DocumentType.Get64ContentStartText(sourceDocument.Extension) + Convert.ToBase64String(fileBytes);
            documentSource.Type = DocumentType.GetDocumentType(sourceDocument.Extension);
            return documentSource;
        }
    }
}
