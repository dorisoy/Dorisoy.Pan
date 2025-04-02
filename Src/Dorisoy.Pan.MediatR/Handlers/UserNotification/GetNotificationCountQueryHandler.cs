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
    public class GetNotificationCountQueryHandler : IRequestHandler<GetNotificationCountQuery, int>
    {
        private readonly IUserNotificationRepository _userNotificationRepository;
        private readonly UserInfoToken _userInfoToken;

        public GetNotificationCountQueryHandler(IUserNotificationRepository userNotificationRepository,
            UserInfoToken userInfoToken)
        {
            _userNotificationRepository = userNotificationRepository;
            _userInfoToken = userInfoToken;
        }
        public async Task<int> Handle(GetNotificationCountQuery request, CancellationToken cancellationToken)
        {
            return await _userNotificationRepository.All.CountAsync(c => c.ToUserId == _userInfoToken.Id && !c.IsRead);
        }
    }
}
