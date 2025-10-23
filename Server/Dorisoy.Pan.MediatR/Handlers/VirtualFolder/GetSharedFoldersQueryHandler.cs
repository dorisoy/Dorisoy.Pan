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
    public class GetSharedFoldersQueryHandler : IRequestHandler<GetSharedFoldersQuery, List<VirtualFolderInfoDto>>
    {
        private readonly IVirtualFolderRepository _virtualFolderRepository;
        private readonly UserInfoToken _userInfo;
        public GetSharedFoldersQueryHandler(
             IVirtualFolderRepository virtualFolderRepository,
             UserInfoToken userInfo)
        {
            _virtualFolderRepository = virtualFolderRepository;
            _userInfo = userInfo;
        }
        public async Task<List<VirtualFolderInfoDto>> Handle(GetSharedFoldersQuery request, CancellationToken cancellationToken)
        {
            var entities = await _virtualFolderRepository.All
                 .Include(c => c.PhysicalFolder)
                 .ThenInclude(c => c.PhysicalFolderUsers)
                 .ThenInclude(c => c.User)
                 .Where(c => c.VirtualFolderUsers.Any(d => d.UserId == _userInfo.Id) && c.IsShared)
                 .Select(c => new VirtualFolderInfoDto
                 {
                     Id = c.Id,
                     Name = c.Name,
                     ParentId = c.ParentId,
                     PhysicalFolderId = c.PhysicalFolderId.Value,
                     IsShared = c.IsShared,
                     Users = c.PhysicalFolder.PhysicalFolderUsers.Select(cs => new UserInfoDto
                     {
                         Email = cs.User.Email,
                         RaleName = cs.User.RaleName,
                         Id = cs.UserId,
                         IsOwner = cs.UserId == c.PhysicalFolder.CreatedBy
                     }).ToList()
                 }).ToListAsync();
            return entities;

        }
    }
}
