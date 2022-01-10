using Dorisoy.Pan.Common.GenericRespository;
using Dorisoy.Pan.Data;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Dorisoy.Pan.Repository
{
    public interface IPhysicalFolderUserRepository : IGenericRepository<PhysicalFolderUser>
    {
        void AssignPermission(Guid id, Guid folderId);
        void AddFolderUsers(Guid id, List<Guid> users);
        Task AddPhysicalFolderUsersChildsById(Guid id, List<Guid> users);
        Task RemovedFolderUsers(List<HierarchyFolder> lstFolders, Guid userId);
    }
}
