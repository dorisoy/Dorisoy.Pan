using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Dorisoy.Pan.Data.Dto;
using Dorisoy.Pan.Helper;
using Dorisoy.Pan.MediatR.Queries;
using Dorisoy.Pan.Repository;

namespace Dorisoy.Pan.MediatR.Handlers
{
    public class GetOnlineUsersQueryHandler : IRequestHandler<GetOnlineUsersQuery, List<UserDto>>
    {
        private readonly IConnectionMappingRepository _connectionMappingRepository;
        private readonly IUserRepository _userRepository;
        private readonly PathHelper _pathHelper;
        private readonly UserInfoToken _userInfoToken;
        public GetOnlineUsersQueryHandler(IUserRepository userRepository,
            IConnectionMappingRepository connectionMappingRepository,
            PathHelper pathHelper,
            UserInfoToken userInfoToken)
        {
            _userRepository = userRepository;
            _connectionMappingRepository = connectionMappingRepository;
            _pathHelper = pathHelper;
            _userInfoToken = userInfoToken;
        }
        public async Task<List<UserDto>> Handle(GetOnlineUsersQuery request, CancellationToken cancellationToken)
        {
            var allUsers = _connectionMappingRepository.GetAllUsersExceptThis(_userInfoToken).ToList();
            var allUserIds = allUsers.Select(x => x.Id);

            var users = await _userRepository.All.Where(c => allUserIds.Contains(c.Id))
                .Select(cs => new UserDto
                {
                    Id = cs.Id,
                    RaleName = cs.RaleName,
                    Email = cs.Email,
                    IP = "",
                    ProfilePhoto = !string.IsNullOrWhiteSpace(cs.ProfilePhoto) ? $"{_pathHelper.UserProfilePath}{cs.ProfilePhoto}" : string.Empty
                }).ToListAsync();

            if (users.Any()) 
            {
                users.ForEach(x => 
                {
                    x.IP = allUsers.Where(s => s.Id == x.Id).FirstOrDefault()?.IP ?? "";
                });
            }

            return users;
        }
    }
}
