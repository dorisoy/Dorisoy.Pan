using Dorisoy.Pan.Data.Dto;
using Dorisoy.Pan.MediatR.Queries;
using Dorisoy.Pan.Repository;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace Dorisoy.Pan.MediatR.Handlers
{
    public class GetParentSharedQueryHandler:  IRequestHandler<GetParentSharedQuery, HierarchySharedDto>
    {
        private readonly IVirtualFolderRepository _virtualFolderRepository;
        public GetParentSharedQueryHandler(
             IVirtualFolderRepository virtualFolderRepository
            )
        {
            _virtualFolderRepository = virtualFolderRepository;
        }

        public async Task<HierarchySharedDto> Handle(GetParentSharedQuery request, CancellationToken cancellationToken)
        {
            var hierarchySharedDto = new HierarchySharedDto();
            hierarchySharedDto.Id = request.Id;
            var hierarchyFolder = await _virtualFolderRepository.GetParentsShared(request.Id);
            if (hierarchyFolder.Count > 0)
            {
                hierarchySharedDto.IsParentShared = true;
                hierarchySharedDto.IsChildShared = false;
            }
            return hierarchySharedDto;
        }
    }
}
