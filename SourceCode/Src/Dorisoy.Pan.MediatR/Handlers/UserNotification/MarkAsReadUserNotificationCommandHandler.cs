using Dorisoy.Pan.Common.UnitOfWork;
using Dorisoy.Pan.Domain;
using Dorisoy.Pan.MediatR.Commands;
using Dorisoy.Pan.Repository;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace Dorisoy.Pan.MediatR.Handlers
{
    public class MarkAsReadUserNotificationCommandHandler : IRequestHandler<MarkAsReadUserNotificationCommand, bool>
    {
        private readonly IUserNotificationRepository _userNotificationRepository;
        private readonly IUnitOfWork<DocumentContext> _uow;


        public MarkAsReadUserNotificationCommandHandler(IUserNotificationRepository userNotificationRepository,
            IUnitOfWork<DocumentContext> uow)
        {
            _userNotificationRepository = userNotificationRepository;
            _uow = uow;
        }

        public async Task<bool> Handle(MarkAsReadUserNotificationCommand request, CancellationToken cancellationToken)
        {
            var notification = await _userNotificationRepository.FindAsync(request.Id);
            if (notification != null)
            {
                notification.IsRead = true;
                _userNotificationRepository.Update(notification);
                await _uow.SaveAsync();
            }

            return true;
        }
    }
}
