using Dorisoy.Pan.Common.GenericRespository;
using Dorisoy.Pan.Data;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Dorisoy.Pan.Repository
{
    public interface IVirtualFolderUserRepository : IGenericRepository<VirtualFolderUser>
    {
        void AssignPermission(Guid id, Guid folderId);
        void AddFolderUsers(Guid folderId, List<Guid> users);
        Task AddVirtualFolderUsersChildsById(Guid id, List<Guid> users);
        Task RemovedFolderUsers(List<HierarchyFolder> lstFolders, Guid userId);
    }
}
