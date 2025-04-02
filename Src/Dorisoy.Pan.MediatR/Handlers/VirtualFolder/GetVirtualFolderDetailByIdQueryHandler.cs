using Dorisoy.Pan.Data.Dto;
using Dorisoy.Pan.Helper;
using Dorisoy.Pan.MediatR.Queries;
using Dorisoy.Pan.Repository;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Dorisoy.Pan.MediatR.Handlers
{
    public class GetVirtualFolderDetailByIdQueryHandler : IRequestHandler<GetVirtualFolderDetailByIdQuery, VirtualFolderInfoDto>
    {
        private readonly IVirtualFolderRepository _virtualFolderRepository;
        private readonly PathHelper _pathHeloper;
        private readonly UserInfoToken _userInfoToken;
        public GetVirtualFolderDetailByIdQueryHandler(
             IVirtualFolderRepository virtualFolderRepository,
             PathHelper pathHeloper,
             UserInfoToken userInfoToken)
        {
            _virtualFolderRepository = virtualFolderRepository;
            _pathHeloper = pathHeloper;
            _userInfoToken = userInfoToken;
        }

        public async Task<VirtualFolderInfoDto> Handle(GetVirtualFolderDetailByIdQuery request, CancellationToken cancellationToken)
        {
            var entity = await _virtualFolderRepository.All
                .Include(c => c.PhysicalFolder)
                .ThenInclude(c => c.PhysicalFolderUsers)
                .Where(c => c.Id == request.Id && c.PhysicalFolder.PhysicalFolderUsers.Any(phu=> phu.UserId== _userInfoToken.Id))
                .Select(c => new VirtualFolderInfoDto
                {
                    Id = c.Id,
                    Name = c.Name,
                    ParentId = c.ParentId,
                    PhysicalFolderId = c.PhysicalFolderId,
                    IsShared = c.IsShared,
                    CreatedDate = c.CreatedDate,
                    Users = c.PhysicalFolder.PhysicalFolderUsers.Select(cs => new UserInfoDto
                    {
                        Email = cs.User.Email,
                        FirstName = cs.User.FirstName,
                        Id = cs.UserId,
                        LastName = cs.User.LastName,
                        IsOwner = cs.User.Id == c.CreatedBy,
                        ProfilePhoto = $"{_pathHeloper.UserProfilePath}/{cs.User.ProfilePhoto}"
                    }).ToList()
                }).FirstOrDefaultAsync(); 
            return entity;
        }
    }
}
