using Dorisoy.Pan.Common.GenericRespository;
using Dorisoy.Pan.Data;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Dorisoy.Pan.Repository
{
    public interface IPhysicalFolderRepository : IGenericRepository<PhysicalFolder>
    {
        Task<string> GetParentFolderPath(Guid childId);
        Task<string> GetParentOriginalFolderPath(Guid childId);

        Task<List<HierarchyFolder>> GetChildsHierarchyById(Guid id);

        Task<List<HierarchyFolder>> GetParentsHierarchyById(Guid id);
    }
}
