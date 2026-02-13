using AutoMapper;
using Dorisoy.Pan.Data.Dto;
using Dorisoy.Pan.MediatR.Queries;
using Dorisoy.Pan.Repository;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace Dorisoy.Pan.MediatR.Handlers
{
    public class GetVirtualRootFolderQueryHandler : IRequestHandler<GetVirtualRootFolderQuery, VirtualFolderDto>
    {
        private readonly IVirtualFolderRepository _virtualFolderRepository;
        private readonly IMapper _mapper;
        public GetVirtualRootFolderQueryHandler(
            IVirtualFolderRepository virtualFolderRepository,
            IMapper mapper
            )
        {
            _virtualFolderRepository = virtualFolderRepository;
            _mapper = mapper;
        }

        public async Task<VirtualFolderDto> Handle(GetVirtualRootFolderQuery request, CancellationToken cancellationToken)
        {
            var folder = await _virtualFolderRepository.GetRootFolder();
            return _mapper.Map<VirtualFolderDto>(folder);
        }
    }
}
