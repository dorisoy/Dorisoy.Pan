using Dorisoy.Pan.Data.Dto;
using MediatR;
using System.Collections.Generic;

namespace Dorisoy.Pan.MediatR.Queries
{
    public class GetNewNotificationsQuery : IRequest<List<UserNotificationDto>>
    {
    }
}
