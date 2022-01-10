using AutoMapper;
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
    public class GetFolderChildByIdQueryHandler : IRequestHandler<GetFolderChildByIdQuery, List<VirtualFolderDto>>
    {
        private readonly IVirtualFolderRepository _folderRepository;
        private readonly IMapper _mapper;
        private readonly UserInfoToken _userInfo;
        public GetFolderChildByIdQueryHandler(
            IVirtualFolderRepository folderRepository,
            IMapper mapper,
            UserInfoToken userInfo)
        {
            _folderRepository = folderRepository;
            _mapper = mapper;
            _userInfo = userInfo;
        }
        public async Task<List<VirtualFolderDto>> Handle(GetFolderChildByIdQuery request, CancellationToken cancellationToken)
        {
            var list = await _folderRepository.All
                    .Include(c => c.Children)
                  .Where(c => c.ParentId == request.Id && c.VirtualFolderUsers.Any(d => d.UserId == _userInfo.Id))
                  .ToListAsync();
            return _mapper.Map<List<VirtualFolderDto>>(list);

        }
    }
}
