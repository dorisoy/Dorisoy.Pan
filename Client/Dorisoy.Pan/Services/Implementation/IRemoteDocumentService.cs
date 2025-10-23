namespace Dorisoy.Pan.Services;
public interface IRemoteDocumentService
{
    Task<bool> DeleteDocument(Guid did, CancellationToken calToken = default);
}
