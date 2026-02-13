using Dorisoy.Pan.Data.Resources;
using Dorisoy.Pan.Repository;
using MediatR;

namespace Dorisoy.Pan.MediatR.Queries
{
    public class GetAllNotificationQuery : IRequest<UserNotificationList>
    {
        public NotificationSource NotificationSource { get; set; }
    }
}
