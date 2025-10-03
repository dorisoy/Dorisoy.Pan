namespace Dorisoy.PanClient.Services;
public interface IDocumentCommentService
{
    Task<ServiceResult<DocumentCommentModel>> AddAsync(DocumentCommentModel model);
    IObservable<IChangeSet<DocumentCommentModel, Guid>> Connect();
    Task DeleteAsync(DocumentComment dc);
    Task<Result<int>> DeleteAsync(Guid id);
    Task<DocumentComment> FindByIdAsync(Guid id);
    Task<Result<List<DocumentCommentModel>>> GetAllAsync();
    Task<Result<List<DocumentCommentModel>>> GetAllDocumentComment(Guid documentId);
    Task<Result<DocumentCommentModel>> GetByIdAsync(Guid id);
}