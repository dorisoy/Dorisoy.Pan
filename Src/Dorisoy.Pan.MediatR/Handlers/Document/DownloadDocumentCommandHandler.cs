using Dorisoy.Pan.Helper;
using Dorisoy.Pan.MediatR.Commands;
using Dorisoy.Pan.Repository;
using MediatR;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using System.Threading;
using System.Threading.Tasks;
using System.IO;

namespace Dorisoy.Pan.MediatR.Handlers
{
    public class DownloadDocumentCommandHandler : IRequestHandler<DownloadDocumentCommand, string>
    {
        private readonly IDocumentRepository _documentRepository;
        private readonly IDocumentVersionRepository _documentVersionRepository;
        private readonly IDocumentShareableLinkRepository _documentShareableLinkRepository;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly PathHelper _pathHelper;


        public DownloadDocumentCommandHandler(IDocumentRepository documentRepository,
            IDocumentVersionRepository documentVersionRepository,
            IDocumentShareableLinkRepository documentShareableLinkRepository,
            IWebHostEnvironment webHostEnvironment,
            PathHelper pathHelper)
        {
            _documentRepository = documentRepository;
            _documentVersionRepository = documentVersionRepository;
            _documentShareableLinkRepository = documentShareableLinkRepository;
            _webHostEnvironment = webHostEnvironment;
            _pathHelper = pathHelper;
        }

        public async Task<string> Handle(DownloadDocumentCommand request, CancellationToken cancellationToken)
        {
            if (request.IsFromPreview)
            {
                var link = await _documentShareableLinkRepository.All.FirstOrDefaultAsync(c => c.LinkCode == request.Token);
                if (link == null)
                {
                    return "";
                }
            }

            if (request.IsVersion)
            {
                var version = await _documentVersionRepository.All.FirstOrDefaultAsync(c => c.Id == request.Id);
                return Path.Combine(_pathHelper.ContentRootPath, _pathHelper.DocumentPath, version?.Path);
            }
            var documentPath = await _documentRepository.All.FirstOrDefaultAsync(c => c.Id == request.Id);
            return Path.Combine(_pathHelper.ContentRootPath, _pathHelper.DocumentPath, documentPath?.Path);
        }
    }
}
