namespace Dorisoy.Pan.Services;

public interface IPhysicalFolderService
{
    Task<ServiceResult<PhysicalFolder>> AddAsync(PhysicalFolder entity);
    Task AddRangeAsync(List<PhysicalFolder> physicalFolders);
    Task<ServiceResult<PhysicalFolderModel>> AddAsync(PhysicalFolderModel model);
    IObservable<IChangeSet<PhysicalFolderModel, Guid>> Connect();
    Task DeleteAsync(PhysicalFolderModel model);
    Task<string> GetParentFolderPath(Guid childId);
    Task<string> GetParentOriginalFolderPath(Guid childId);
    Task<List<HierarchyFolder>> GetParentsHierarchyById(Guid id);
    Task<List<PhysicalFolderModel>> GetPhysicalFolders();
    bool PhysicalFoldernameIsFree(Guid id, string physicalFoldername);
    Task<List<HierarchyFolder>> GetChildsHierarchyById(Guid id);
    Task<ServiceResult<PhysicalFolderModel>> UpdateAsync(PhysicalFolderModel model);
    Task<PhysicalFolderModel> ExistingPhysicalFolder(string name, Guid physicalFolderId, Guid userId);
    Task<ServiceResult<VirtualFolderInfoModel>> AddFolder(string name, Guid virtualParentId, Guid? physicalFolderId, Guid userId);
}
