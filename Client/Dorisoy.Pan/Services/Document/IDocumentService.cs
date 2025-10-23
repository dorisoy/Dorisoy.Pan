namespace Dorisoy.Pan.Services;

public interface IDocumentService
{
    Task<List<DocumentModel>> GetPatienterDocuments(Guid userId, Guid patienterId, string ext = "");
    Task<List<DocumentFolderModel>> GetAllDocuments(Guid folderId);
    Task<ServiceResult<DocumentModel>> AddAsync(DocumentModel model, bool update = false);
    IObservable<IChangeSet<DocumentFolderModel, Guid>> Connect();
    Task DeleteAsync(DocumentModel model);
    bool DocumentnameIsFree(Guid id, string documentname);
    Task<List<DocumentModel>> GetDocuments();
    Task<ServiceResult<DocumentModel>> UpdateAsync(DocumentModel model);
    Task<DocumentModel> GetDocument(Guid parentId, string fileName, Guid userId);
    Task<ServiceResult> DeleteDeletedDocument(Guid documentId, Guid userid);
    Task<ServiceResult> DeleteDocument(Guid documentId);
    Task<int> GetDocumentsCount(Guid userId, string ext = "");
}
