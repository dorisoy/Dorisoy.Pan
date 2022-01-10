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
    public class DeleteDocumentShareableLinkCommandHandler
        : IRequestHandler<DeleteDocumentShareableLinkCommand, ServiceResponse<bool>>
    {
        private readonly IDocumentShareableLinkRepository _documentShareableLinkRepository;
        private readonly IUnitOfWork<DocumentContext> _uow;

        public DeleteDocumentShareableLinkCommandHandler(IDocumentShareableLinkRepository documentShareableLinkRepository,
            IUnitOfWork<DocumentContext> uow)
        {
            _documentShareableLinkRepository = documentShareableLinkRepository;
            _uow = uow;
        }
        public async Task<ServiceResponse<bool>> Handle(DeleteDocumentShareableLinkCommand request, CancellationToken cancellationToken)
        {
            var link = await _documentShareableLinkRepository.FindAsync(request.Id);
            if (link == null)
            {
                return ServiceResponse<bool>.Return404();
            }
            _documentShareableLinkRepository.Remove(link);
            if (await _uow.SaveAsync() <= 0)
            {
                return ServiceResponse<bool>.Return500();
            }
            return ServiceResponse<bool>.ReturnSuccess();
        }
    }
}
