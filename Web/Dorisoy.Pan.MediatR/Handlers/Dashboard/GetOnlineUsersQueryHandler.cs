using Dorisoy.Pan.Common;
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
            var allUserIds = _connectionMappingRepository.GetAllUsersExceptThis(_userInfoToken).Select(c => c.Id).ToList();
            var users = await _userRepository.All.WhereContains(c => c.Id, allUserIds)
                .Select(cs => new UserDto
                {
                    Id = cs.Id,
                    FirstName = cs.FirstName,
                    LastName = cs.LastName,
                    Email = cs.Email,
                    ProfilePhoto = !string.IsNullOrWhiteSpace(cs.ProfilePhoto) ? $"{_pathHelper.UserProfilePath}{cs.ProfilePhoto}" : string.Empty
                }).ToListAsync();
            return users;
        }
    }
}
