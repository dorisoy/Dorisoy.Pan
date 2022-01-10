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
    public class SearchVirtualFolderQueryHandler : IRequestHandler<SearchVirtualFolderQuery, List<VirtualFolderInfoDto>>
    {
        private readonly IVirtualFolderRepository _virtualFolderRepository;
        private readonly UserInfoToken _userInfo;
        public SearchVirtualFolderQueryHandler(
             IVirtualFolderRepository virtualFolderRepository,
             UserInfoToken userInfo)
        {
            _virtualFolderRepository = virtualFolderRepository;
            _userInfo = userInfo;
        }
        public async Task<List<VirtualFolderInfoDto>> Handle(SearchVirtualFolderQuery request, CancellationToken cancellationToken)
        {
            var folders = await _virtualFolderRepository.All
                  .OrderByDescending(c => c.CreatedDate)
                 .Include(c => c.PhysicalFolder)
                 .ThenInclude(c => c.PhysicalFolderUsers)
                 .ThenInclude(c => c.User)
                 .Where(c => c.VirtualFolderUsers.Any(d => d.UserId == _userInfo.Id)
                    && EF.Functions.Like(c.Name, $"%{request.SearchString}%"))
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
                         IsOwner = cs.UserId == c.PhysicalFolder.CreatedBy
                     }).ToList()
                 }).Take(30).ToListAsync();
            return folders;
        }
    }
}
