using Dorisoy.Pan.Data.Dto;
using Dorisoy.Pan.Helper;
using Dorisoy.Pan.MediatR.Queries;
using Dorisoy.Pan.Repository;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.IO;

namespace Dorisoy.Pan.MediatR.Handlers
{
    public class GetVirtualFoldersQueryHandler : IRequestHandler<GetVirtualFoldersQuery, List<VirtualFolderInfoDto>>
    {
        private readonly IVirtualFolderRepository _virtualFolderRepository;
        private readonly UserInfoToken _userInfo;
        private readonly PathHelper _pathHelper;

        public GetVirtualFoldersQueryHandler(
             IVirtualFolderRepository virtualFolderRepository,
             UserInfoToken userInfo,
             PathHelper pathHelper)
        {
            _virtualFolderRepository = virtualFolderRepository;
            _userInfo = userInfo;
            _pathHelper = pathHelper;
        }
        public async Task<List<VirtualFolderInfoDto>> Handle(GetVirtualFoldersQuery request, CancellationToken cancellationToken)
        {
            var entities = await _virtualFolderRepository.All
                 .Include(c => c.PhysicalFolder)
                 .ThenInclude(c => c.PhysicalFolderUsers)
                 .ThenInclude(c => c.User)
                 .Where(c => c.ParentId == request.Id && c.VirtualFolderUsers.Any(d => d.UserId == _userInfo.Id))
                 .Select(c => new VirtualFolderInfoDto
                 {
                     Id = c.Id,
                     Name = c.Name,
                     ParentId = c.ParentId,
                     PhysicalFolderId = c.PhysicalFolderId,
                     IsShared = c.IsShared,
                     IsStarred = c.VirtualFolderUsers.Any(c => c.IsStarred && c.UserId == _userInfo.Id),
                     Users = c.PhysicalFolder.PhysicalFolderUsers.Select(cs => new UserInfoDto
                     {
                         Email = cs.User.Email,
                         FirstName = cs.User.FirstName,
                         Id = cs.UserId,
                         LastName = cs.User.LastName,
                         IsOwner = cs.UserId == c.PhysicalFolder.CreatedBy,
                         ProfilePhoto = Path.Combine(_pathHelper.UserProfilePath, cs.User.ProfilePhoto)
                     }).ToList()
                 }).ToListAsync();
            return entities;

        }
    }
}
