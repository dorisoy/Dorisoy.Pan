namespace Dorisoy.PanClient.Services
{
    public interface IVirtualFolderService
    {
        Task AddRangeAsync(List<VirtualFolder> virtualFolders);
        Task<List<VirtualFolder>> VirtualFoldersToCreate(Guid physicalFolderId);
        Task<VirtualFolder> FindAsync(Guid folderId);
        Task<ServiceResult<VirtualFolder>> AddAsync(VirtualFolder entity);
        Task<ServiceResult<VirtualFolderModel>> AddAsync(VirtualFolderModel model);
        IObservable<IChangeSet<VirtualFolderModel, Guid>> Connect();
        IObservable<IChangeSet<VirtualFolderInfoModel, Guid>> FolderConnect();
        Task DeleteAsync(VirtualFolderModel model);
        Task<List<HierarchyFolder>> GetChildsHierarchyById(Guid id);
        Task<string> GetFolderName(string name, Guid Id, int index, Guid userId);
        Task<List<Guid>> GetFolderUserByPhysicalFolderId(Guid id);
        Task<string> GetParentFolderPath(Guid childId);
        Task<List<HierarchyFolder>> GetParentsHierarchyById(Guid id);
        Task<VirtualFolder> GetRootFolder();
        Task<VirtualFolder> GetVirtualFolder(string name, Guid? parentId = null);
        Task<List<VirtualFolderModel>> GetVirtualFolders();
        Task<List<VirtualFolder>> GetVirualFoldersByPhysicalId(Guid PhysicalFolderId);
        Task<ServiceResult<VirtualFolderModel>> UpdateAsync(VirtualFolderModel model);
        bool VirtualFoldernameIsFree(Guid id, string vrtualFoldername);
        Task<List<Guid>> GetFolderUserIds(string folderName, Guid parentPhysicalFolderId, Guid parentVirtualFolderId, Guid userId);
        Task<List<VirtualFolderInfoModel>> GetVirtualFolderInfos(List<Guid> folderIdsToReturn);
        Task<List<VirtualFolderInfoModel>> GetVirtualFolders(Guid parentId, Guid userid);
        Task<List<DocumentFolderModel>> GetDocumentVirtualFolders(Guid parentId, Guid userid);
        Task<ServiceResult> DeleteDeletedFolder(Guid virtualFolderId, Guid userid);
        Task<ServiceResult> DeleteVirtualFolder(Guid virtualFolderId, Guid userid);
        Task<VirtualFolderInfoModel> GetVirtualFolderDetailById(Guid virtualFolderId, Guid userId);
    }
}
