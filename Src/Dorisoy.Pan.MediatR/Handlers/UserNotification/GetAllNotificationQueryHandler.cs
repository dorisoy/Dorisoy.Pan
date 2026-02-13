using Dorisoy.Pan.Data.Dto;
using Dorisoy.Pan.MediatR.Queries;
using Dorisoy.Pan.Repository;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Dorisoy.Pan.MediatR.Handlers
{
    public class GetAllNotificationQueryHandler
        : IRequestHandler<GetAllNotificationQuery, UserNotificationList>
    {
        private readonly IUserNotificationRepository _userNotificationRepository;
        private readonly IUserRepository _userRepository;

        public GetAllNotificationQueryHandler(IUserNotificationRepository userNotificationRepository,
            IUserRepository userRepository)
        {
            _userNotificationRepository = userNotificationRepository;
            _userRepository = userRepository;
        }

        public async Task<UserNotificationList> Handle(GetAllNotificationQuery request, CancellationToken cancellationToken)
        {
            var entities = await _userNotificationRepository.GetUserNotifications(request.NotificationSource);

            var allUsersIds = entities.Select(c => c.FromUserId).ToList();
            var allUsers = _userRepository.All.Where(c => EF.Constant(allUsersIds).Contains(c.Id)).Select(cs => new UserInfoDto
            {
                Id = cs.Id,
                FirstName = cs.FirstName,
                LastName = cs.LastName
            }).ToList();

            entities.ForEach(entity =>
            {
                var user = allUsers.FirstOrDefault(c => c.Id == entity.FromUserId);
                if (user != null)
                {
                    entity.FromUserName = $"{user.FirstName} {user.LastName}";
                }
            });
            return entities;
        }
    }
}
