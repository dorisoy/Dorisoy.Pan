using AutoMapper;
using Dorisoy.Pan.Data;
using Dorisoy.Pan.MediatR.Queries;
using Dorisoy.Pan.Repository;
using MediatR;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Dorisoy.Pan.MediatR.Handlers
{
    public class GetFolderParentsByIdQueryHandler : IRequestHandler<GetFolderParentsByIdQuery, IEnumerable<HierarchyFolder>>
    {
        private readonly IVirtualFolderRepository _virtualFolderRepository;
        private readonly IMapper _mapper;
        public GetFolderParentsByIdQueryHandler(
            IVirtualFolderRepository virtualFolderRepository,
            IMapper mapper)
        {
            _virtualFolderRepository = virtualFolderRepository;
            _mapper = mapper;
        }


        public async Task<IEnumerable<HierarchyFolder>> Handle(GetFolderParentsByIdQuery request, CancellationToken cancellationToken)
        {
            try
            {
                return await _virtualFolderRepository.GetParentsHierarchyById(request.Id);
            }
            catch (Exception)
            { 
                return [];
            }
        }
    }
}
