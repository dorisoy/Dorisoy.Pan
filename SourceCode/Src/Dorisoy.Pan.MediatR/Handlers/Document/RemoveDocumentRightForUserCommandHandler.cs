using Dorisoy.Pan.Common.UnitOfWork;
using Dorisoy.Pan.Domain;
using Dorisoy.Pan.Helper;
using Dorisoy.Pan.MediatR.Commands;
using Dorisoy.Pan.Repository;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Dorisoy.Pan.MediatR.Handlers
{
    public class RemoveDocumentRightForUserCommandHandler
        : IRequestHandler<RemoveDocumentRightForUserCommand, ServiceResponse<bool>>
    {
        private readonly IDocumentSharedUserRepository _documentSharedUserRepository;
        private readonly IUnitOfWork<DocumentContext> _uow;
        private readonly IUserNotificationRepository _userNotificationRepository;

        public RemoveDocumentRightForUserCommandHandler(IDocumentSharedUserRepository documentSharedUserRepository,
            IUnitOfWork<DocumentContext> uow,
            IUserNotificationRepository userNotificationRepository)
        {
            _documentSharedUserRepository = documentSharedUserRepository;
            _uow = uow;
            _userNotificationRepository = userNotificationRepository;
        }
        public async Task<ServiceResponse<bool>> Handle(RemoveDocumentRightForUserCommand request, CancellationToken cancellationToken)
        {
            var documentPermission = await _documentSharedUserRepository
                .All
                .FirstOrDefaultAsync(c => c.UserId == request.UserId && c.DocumentId == request.DocumentId);

            if (documentPermission == null)
            {
                return ServiceResponse<bool>.ReturnSuccess();
            }

            var notifications = await _userNotificationRepository.All
                .Where(c => c.ToUserId == request.UserId && c.DocumentId == request.DocumentId)
                .ToListAsync();
            _userNotificationRepository.RemoveRange(notifications);

            _documentSharedUserRepository.Remove(documentPermission);
            if (await _uow.SaveAsync() <= 0)
            {
                return ServiceResponse<bool>.Return500();
            }

            return ServiceResponse<bool>.ReturnSuccess();
        }
    }
}
