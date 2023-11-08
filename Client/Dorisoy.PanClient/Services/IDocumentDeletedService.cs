using Dorisoy.PanClient.Common;

namespace Dorisoy.PanClient.Services;
public interface IDocumentDeletedService
{
    IObservable<IChangeSet<DocumentFolderModel, Guid>> Connect();
    Task DeleteAsync(DeletedDocumentInfoModel model);
    Task<ServiceResult> DeleteDocument(Guid documentId);
    Task<List<DeletedDocumentInfoModel>> GetAllDeletedDocuments();
    Task<ServiceResult> RestoreDeletedDocument(Guid documentId);
}