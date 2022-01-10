using Dorisoy.Pan.Data.Dto;
using Dorisoy.Pan.MediatR.Commands;
using Dorisoy.Pan.Repository;
using MediatR;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Dorisoy.Pan.MediatR.Handlers
{
    public class NotificationCommandHandler : IRequestHandler<NotificationCommand, bool>
    {
        private readonly IPhysicalFolderUserRepository _physicalFolderUserRepository;
        private readonly UserInfoToken _userInfoToken;
        private readonly IConnectionMappingRepository _signalrService;

        public NotificationCommandHandler(
            IPhysicalFolderUserRepository physicalFolderUserRepository,
            UserInfoToken userInfoToken,
            IConnectionMappingRepository signalrService
            )
        {
            _physicalFolderUserRepository = physicalFolderUserRepository;
            _userInfoToken = userInfoToken;
            _signalrService = signalrService;
        }

        public async Task<bool> Handle(NotificationCommand request, CancellationToken cancellationToken)
        {
            if (request.Users.Any())
            {
                await _signalrService.SendFolderNotification(request.Users, request.FolderId);
                return true;
            }
            var users = _physicalFolderUserRepository
                 .FindBy(c => c.FolderId == request.FolderId)
                 .Select(c => c.UserId)
                 .ToList();
            if (users.Count() == 0)
            {
                users.Add(_userInfoToken.Id);
            }

            await _signalrService.SendFolderNotification(users, request.FolderId);
            return true;
        }
    }
}
