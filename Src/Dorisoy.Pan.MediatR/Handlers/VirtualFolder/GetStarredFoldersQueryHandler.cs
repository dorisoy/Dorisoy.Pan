using Dorisoy.Pan.Data.Dto;
using Dorisoy.Pan.MediatR.Queries;
using Dorisoy.Pan.Repository;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Dorisoy.Pan.MediatR.Handlers
{
    public class GetStarredFoldersQueryHandler : IRequestHandler<GetStarredFoldersQuery, List<VirtualFolderInfoDto>>
    {
        private readonly IVirtualFolderUserRepository _virtualFolderUserRepository;
        private readonly UserInfoToken _userInfo;
        public GetStarredFoldersQueryHandler(
             IVirtualFolderUserRepository virtualFolderUserRepository,
             UserInfoToken userInfo)
        {
            _virtualFolderUserRepository = virtualFolderUserRepository;
            _userInfo = userInfo;
        }
        public async Task<List<VirtualFolderInfoDto>> Handle(GetStarredFoldersQuery request, CancellationToken cancellationToken)
        {
            var entities = await _virtualFolderUserRepository.All
                 .Include(c => c.VirtualFolder)
                 .ThenInclude(cs => cs.PhysicalFolder)
                 .ThenInclude(c => c.PhysicalFolderUsers)
                 .ThenInclude(c => c.User)
                 .Where(d => d.UserId == _userInfo.Id && d.IsStarred)
                 .Select(c => new VirtualFolderInfoDto
                 {
                     Id = c.VirtualFolder.Id,
                     Name = c.VirtualFolder.Name,
                     ParentId = c.VirtualFolder.ParentId,
                     PhysicalFolderId = c.VirtualFolder.PhysicalFolderId,
                     IsShared = c.VirtualFolder.IsShared,
                     IsStarred = c.IsStarred,
                     Users = c.VirtualFolder.PhysicalFolder.PhysicalFolderUsers.Select(cs => new UserInfoDto
                     {
                         Email = cs.User.Email,
                         FirstName = cs.User.FirstName,
                         Id = cs.UserId,
                         LastName = cs.User.LastName,
                         IsOwner = cs.UserId == c.VirtualFolder.PhysicalFolder.CreatedBy
                     }).ToList()
                 }).ToListAsync();
            return entities;

        }
    }
}
