using Dorisoy.Pan.Common.UnitOfWork;
using Dorisoy.Pan.Data.Dto;
using Dorisoy.Pan.Domain;
using Dorisoy.Pan.Helper;
using Dorisoy.Pan.MediatR.Commands;
using Dorisoy.Pan.Repository;
using MediatR;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Dorisoy.Pan.MediatR.Handlers
{
    public class DeleteDocumentCommandHandler : IRequestHandler<DeleteDocumentCommand, ServiceResponse<bool>>
    {
        private readonly UserInfoToken _userInfoToken;
        private readonly IDocumentDeletedRepository _documentDeletedRepository;
        private readonly IDocumentRepository _documentRepository;
        private readonly IUnitOfWork<DocumentContext> _uow;

        public DeleteDocumentCommandHandler(IDocumentDeletedRepository documentDeletedRepository,
            IDocumentRepository documentRepository,
            UserInfoToken userInfoToken,
            IUnitOfWork<DocumentContext> uow)
        {
            _documentDeletedRepository = documentDeletedRepository;
            _documentRepository = documentRepository;
            _userInfoToken = userInfoToken;
            _uow = uow;
        }

        public async Task<ServiceResponse<bool>> Handle(DeleteDocumentCommand request, CancellationToken cancellationToken)
        {
            var document = await _documentRepository.FindAsync(request.Id);
            if (document == null)
            {
                return ServiceResponse<bool>.Return404();
            }

            var deletedDocument = _documentDeletedRepository.All
                .FirstOrDefault(c => c.UserId == _userInfoToken.Id && c.DocumentId == request.Id);

            if (deletedDocument == null)
            {
                _documentDeletedRepository.Add(new Data.DocumentDeleted
                {
                    DocumentId = request.Id,
                    UserId = _userInfoToken.Id
                });

                if (await _uow.SaveAsync() <= 0)
                {
                    return ServiceResponse<bool>.Return500();
                }
            }
            return ServiceResponse<bool>.ReturnSuccess();
        }
    }
}
