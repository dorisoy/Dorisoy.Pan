using Dorisoy.Pan.Common.UnitOfWork;
using Dorisoy.Pan.Data.Dto;
using Dorisoy.Pan.Domain;
using Dorisoy.Pan.Helper;
using Dorisoy.Pan.MediatR.Commands;
using Dorisoy.Pan.Repository;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Dorisoy.Pan.MediatR.Handlers.DeletedDocument
{
    public class RestoreDeletedDocumentCommandHandler : IRequestHandler<RestoreDeletedDocumentCommand, ServiceResponse<bool>>
    {
        private readonly UserInfoToken _userInfoToken;
        private readonly IDocumentDeletedRepository _documentDeletedRepository;
        private readonly IUnitOfWork<DocumentContext> _uow;

        public RestoreDeletedDocumentCommandHandler(UserInfoToken userInfoToken,
            IDocumentDeletedRepository documentDeletedRepository,
            IUnitOfWork<DocumentContext> uow)
        {
            _userInfoToken = userInfoToken;
            _documentDeletedRepository = documentDeletedRepository;
            _uow = uow;
        }

        public async Task<ServiceResponse<bool>> Handle(RestoreDeletedDocumentCommand request, CancellationToken cancellationToken)
        {
            var documentPermission = await _documentDeletedRepository.All
                .Where(c => c.UserId == _userInfoToken.Id && c.DocumentId == request.Id)
                .FirstOrDefaultAsync();

            if (documentPermission != null)
            {
                _documentDeletedRepository.Remove(documentPermission);
                if (await _uow.SaveAsync() <= 0)
                {
                    return ServiceResponse<bool>.Return500();
                }
            }

            return ServiceResponse<bool>.ReturnSuccess();
        }
    }
}
