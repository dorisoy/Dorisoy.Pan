using Dorisoy.Pan.Common.GenericRespository;
using Dorisoy.Pan.Data;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Dorisoy.Pan.Repository
{
    public interface IVirtualFolderRepository : IGenericRepository<VirtualFolder>
    {
        Task<VirtualFolder> SharedFolderExist(Guid PhysicalFolderId, Guid RootId);
        Guid AddVirtualFolder(Guid? parentId, Guid id, string name, Guid physicalFolderId);
        Task<string> GetParentFolderPath(Guid childId);
        Task<List<HierarchyFolder>> GetChildsHierarchyById(Guid id);
        Task<List<HierarchyFolder>> GetParentsHierarchyById(Guid id);
        Task<List<VirtualFolder>> GetVirualFoldersByPhysicalId(Guid PhysicalFolderId);
        Task<VirtualFolder> GetRootFolder();
        Task<List<Guid>> GetFolderUserByPhysicalFolderId(Guid id);
        Task<List<HierarchyFolder>> GetParentsShared(Guid id);
        Task<List<HierarchyFolder>> GetChildsShared(Guid id);
        Task<string> GetFolderName(string name, Guid Id, int index, Guid userId);
    }
}
