using Dorisoy.Pan.Helper;
using Dorisoy.Pan.MediatR.Commands;
using Dorisoy.Pan.Repository;
using MediatR;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Dorisoy.Pan.MediatR.Handlers
{
    public class GetDocumentPathByTokenCommandHandler : IRequestHandler<GetDocumentPathByTokenCommand, ServiceResponse<string>>
    {
        private readonly IDocumentTokenRepository _documentTokenRepository;
        private readonly IDocumentRepository _documentRepository;
        private readonly IDocumentVersionRepository _documentVersionRepository;
        private readonly PathHelper _pathHelper;

        public GetDocumentPathByTokenCommandHandler(IDocumentTokenRepository documentTokenRepository,
            IDocumentRepository documentRepository,
            IDocumentVersionRepository documentVersionRepository,
            PathHelper pathHelper)
        {
            _documentTokenRepository = documentTokenRepository;
            _documentRepository = documentRepository;
            _documentVersionRepository = documentVersionRepository;
            _pathHelper = pathHelper;
        }
        public async Task<ServiceResponse<string>> Handle(GetDocumentPathByTokenCommand request, CancellationToken cancellationToken)
        {
            if (request.IsVersion)
            {
                if (_documentTokenRepository.All.Any(c => c.DocumentVersionId == request.Id && c.Token == request.Token))
                {
                    var version = await _documentVersionRepository.FindAsync(request.Id);
                    if (version != null)
                    {
                        return ServiceResponse<string>.ReturnResultWith200($"{_pathHelper.DocumentPath}\\{version.Path}");
                    }
                }
            }
            else if (_documentTokenRepository.All.Any(c => c.DocumentId == request.Id && c.Token == request.Token))
            {
                var document = await _documentRepository.FindAsync(request.Id);
                if (document != null)
                {
                    return ServiceResponse<string>.ReturnResultWith200($"{_pathHelper.DocumentPath}\\{document.Path}");
                }
            }
            return ServiceResponse<string>.Return404();
        }
    }
}
