namespace Dorisoy.PanClient.Services
{
    public interface IPhysicalFolderUserService
    {
        Task AddAsync(PhysicalFolderUser physicalFolderUser);
        void AddFolderUsers(Guid id, List<Guid> users);
        Task AddPhysicalFolderUsersChildsById(Guid id, List<Guid> users);
        Task AddRangeAsync(List<PhysicalFolderUser> physicalFolderUsers);
        void AssignPermission(Guid id, Guid folderId);
        Task DeleteAsync(PhysicalFolderUser physicalFolderUser);
        Task DeleteRangeAsync(List<PhysicalFolderUser> physicalFolderUsers);
        Task RemovedFolderUsers(List<HierarchyFolder> lstFolders, Guid userId);
        Task UpdateAsync(PhysicalFolderUser physicalFolderUser);
    }
}