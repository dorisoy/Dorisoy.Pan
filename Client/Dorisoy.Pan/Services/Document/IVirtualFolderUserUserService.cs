namespace Dorisoy.PanClient.Services
{
    public interface IVirtualFolderUserUserService
    {
        Task<VirtualFolderUser> VirtualFolderPemission(Guid userid, Guid physicalFolderId);
        Task AddAsync(VirtualFolderUser virtualFolderUser);
        void AddFolderUsers(Guid folderId, List<Guid> users);
        Task AddRangeAsync(List<VirtualFolderUser> virtualFolderUsers);
        Task AddVirtualFolderUsersChildsById(Guid id, List<Guid> users);
        void AssignPermission(Guid id, Guid folderId);
        Task DeleteAsync(VirtualFolderUser virtualFolderUser);
        Task DeleteRangeAsync(List<VirtualFolderUser> virtualFolderUsers);
        Task RemovedFolderUsers(List<HierarchyFolder> lstFolders, Guid userId);
        Task UpdateAsync(VirtualFolderUser virtualFolderUser);
        Task UpdateRangeAsync(List<VirtualFolderUser> virtualFolderUsers);
    }
}