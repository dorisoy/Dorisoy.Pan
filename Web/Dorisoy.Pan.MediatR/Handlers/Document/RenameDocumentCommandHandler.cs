using Dorisoy.Pan.Common.UnitOfWork;
using Dorisoy.Pan.Domain;
using Dorisoy.Pan.Helper;
using Dorisoy.Pan.MediatR.Commands;
using Dorisoy.Pan.Repository;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace Dorisoy.Pan.MediatR.Handlers
{
    public class RenameDocumentCommandHandler
        : IRequestHandler<RenameDocumentCommand, ServiceResponse<bool>>
    {
        private readonly IDocumentRepository _documentRepository;
        private readonly IUnitOfWork<DocumentContext> _uow;

        public RenameDocumentCommandHandler(IDocumentRepository documentRepository,
            IUnitOfWork<DocumentContext> uow)
        {
            _documentRepository = documentRepository;
            _uow = uow;
        }
        public async Task<ServiceResponse<bool>> Handle(RenameDocumentCommand request, CancellationToken cancellationToken)
        {
            var document = await _documentRepository.FindAsync(request.Id);
            if (document == null)
            {
                return ServiceResponse<bool>.Return404();
            }

            document.Name = request.Name;
            _documentRepository.Update(document);
            if (await _uow.SaveAsync() <= 0)
            {
                return ServiceResponse<bool>.Return500();
            }
            return ServiceResponse<bool>.ReturnSuccess();
        }
    }
}
