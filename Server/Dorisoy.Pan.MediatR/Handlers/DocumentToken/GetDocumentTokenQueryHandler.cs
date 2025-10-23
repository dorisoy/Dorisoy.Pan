using Dorisoy.Pan.Common.UnitOfWork;
using Dorisoy.Pan.Domain;
using Dorisoy.Pan.MediatR.Queries;
using Dorisoy.Pan.Repository;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Dorisoy.Pan.MediatR.Handlers.DocumentToken
{
    public class GetDocumentTokenQueryHandler : IRequestHandler<GetDocumentTokenQuery, string>
    {
        private readonly IDocumentTokenRepository _documentTokenRepository;
        private readonly IDocumentVersionRepository _documentVersionRepository;
        private readonly IUnitOfWork<DocumentContext> _uow;

        public GetDocumentTokenQueryHandler(IDocumentTokenRepository documentTokenRepository,
            IDocumentVersionRepository documentVersionRepository,
            IUnitOfWork<DocumentContext> uow)
        {
            _documentTokenRepository = documentTokenRepository;
            _documentVersionRepository = documentVersionRepository;
            _uow = uow;
        }
        public async Task<string> Handle(GetDocumentTokenQuery request, CancellationToken cancellationToken)
        {
            var token = Guid.NewGuid();
            if (request.IsVersion)
            {
                var version = await _documentVersionRepository.FindAsync(request.Id);
                if (version != null)
                {
                    _documentTokenRepository.Add(new Data.DocumentToken
                    {
                        CreatedDate = DateTime.UtcNow,
                        DocumentId = version.DocumentId,
                        Token = token,
                        DocumentVersionId = request.Id
                    });
                }
            }
            else
            {
                _documentTokenRepository.Add(new Data.DocumentToken
                {
                    CreatedDate = DateTime.UtcNow,
                    DocumentId = request.Id,
                    Token = token
                });
            }
            await _uow.SaveAsync();
            return token.ToString();
        }
    }
}
