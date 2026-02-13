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
    public class GetAllDeletedFolderQueryHandler : IRequestHandler<GetAllDeletedFolderQuery, List<DeletedVirtualFolderDto>>
    {
        private readonly IVirtualFolderUserRepository _virtualFolderUserRepository;
        private readonly UserInfoToken _userInfoToken;

        public GetAllDeletedFolderQueryHandler(IVirtualFolderUserRepository virtualFolderUserRepository,
            UserInfoToken userInfoToken)
        {
            _virtualFolderUserRepository = virtualFolderUserRepository;
            _userInfoToken = userInfoToken;
        }
        public async Task<List<DeletedVirtualFolderDto>> Handle(GetAllDeletedFolderQuery request, CancellationToken cancellationToken)
        {
            var folders = await _virtualFolderUserRepository.All
                .IgnoreQueryFilters()
                .Where(c => c.UserId == _userInfoToken.Id
                    && c.IsDeleted
                    && !c.VirtualFolder.Parent.VirtualFolderUsers.Any(cs => cs.IsDeleted && cs.UserId == _userInfoToken.Id))
                .Select(d => new DeletedVirtualFolderDto
                {
                    Id = d.FolderId,
                    IsShared = d.VirtualFolder.IsShared,
                    DeletedDate = d.DeletedDate,
                    Name = d.VirtualFolder.Name
                })
                .ToListAsync();
            return folders;
        }
    }
}
