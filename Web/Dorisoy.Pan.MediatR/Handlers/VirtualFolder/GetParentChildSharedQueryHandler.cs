using Dorisoy.Pan.Data.Dto;
using Dorisoy.Pan.MediatR.Queries;
using Dorisoy.Pan.Repository;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace Dorisoy.Pan.MediatR.Handlers
{
    public class GetParentChildSharedQueryHandler : IRequestHandler<GetParentChildSharedQuery, HierarchySharedDto>
    {
        private readonly IVirtualFolderRepository _virtualFolderRepository;
        public GetParentChildSharedQueryHandler(
             IVirtualFolderRepository virtualFolderRepository
            )
        {
            _virtualFolderRepository = virtualFolderRepository;
        }
        public async Task<HierarchySharedDto> Handle(GetParentChildSharedQuery request, CancellationToken cancellationToken)
        {
            var hierarchySharedDto = new HierarchySharedDto();
            hierarchySharedDto.Id = request.Id;
            var hierarchyFolder = await _virtualFolderRepository.GetParentsShared(request.Id);
            if (hierarchyFolder.Count >0)
            {
                hierarchySharedDto.IsParentShared = true;
                hierarchySharedDto.IsChildShared = false;
            }
            else
            {
                hierarchyFolder = await _virtualFolderRepository.GetChildsShared(request.Id);
                if (hierarchyFolder.Count > 0)
                {
                    hierarchySharedDto.IsParentShared = false;
                    hierarchySharedDto.IsChildShared = true;
                }
            }
            return hierarchySharedDto;
        }
    }
}
