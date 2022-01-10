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
    public class GetVirtualFolderForMoveAndCopyQueryCommand : IRequestHandler<GetVirtualFolderForMoveAndCopyQuery, List<VirtualFolderInfoDto>>
    {
        private readonly IVirtualFolderRepository _virtualFolderRepository;
        private readonly UserInfoToken _userInfo;
        public GetVirtualFolderForMoveAndCopyQueryCommand(
             IVirtualFolderRepository virtualFolderRepository,
             UserInfoToken userInfo)
        {
            _virtualFolderRepository = virtualFolderRepository;
            _userInfo = userInfo;
        }

        public async Task<List<VirtualFolderInfoDto>> Handle(GetVirtualFolderForMoveAndCopyQuery request, CancellationToken cancellationToken)
        {
            var entities = await _virtualFolderRepository.All
                 .Include(c => c.PhysicalFolder)
                 .ThenInclude(c => c.PhysicalFolderUsers)
                 .ThenInclude(c => c.User)
                 .OrderBy(c=>c.Name)
                 .Where(c =>
                     c.ParentId == request.ParentId
                     && c.Id != request.SourceId
                     && c.VirtualFolderUsers.Any(d => d.UserId == _userInfo.Id))
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
                         IsOwner = cs.UserId == c.PhysicalFolder.CreatedBy
                     }).ToList()
                 }).ToListAsync();
            return entities;
        }
    }
}
