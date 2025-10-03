using Refit;
using Dorisoy.PanClient.Services.API;

namespace Dorisoy.PanClient.Services;

[Headers("Authorization: Bearer")]
public interface IVirtualFolderApi
{
    [Post("/virtualFolder")]
    Task<ServiceResponse<List<VirtualFolderInfoDto>>> CreateFolder(AddFolderCommand command, CancellationToken calToken = default);

    [Delete("/virtualFolder/{id}")]
    Task<ServiceResponse<VirtualFolderDto>> DeleteFolder(Guid id, CancellationToken calToken = default);

    [Put("/virtualFolder/{id}/rename")]
    Task<bool> RenameFolder(Guid id, RenameFolderCommand command, CancellationToken calToken = default);
}


[Headers("Authorization: Bearer")]
public interface IDocumentApi
{
    [Get("/document/{id}/token")]
    Task<string> GetDocumentToken(Guid id, bool isVersion, CancellationToken calToken = default);

    [Delete("/document/token/{token}")]
    Task<bool> DeleteDocumentToken(Guid token, CancellationToken calToken = default);

    [Get("/document/folder/{id}")]
    Task<ServiceResponse<List<DocumentDto>>> GetDocumentByFolderId(Guid id, CancellationToken calToken = default);

    [Delete("/document/{id}")]
    Task<bool> DeleteDocument(Guid id, CancellationToken calToken = default);

    [Get("/document/{id}/viewer")]
    Task<ServiceResponse<DocumentSource>> DocumentViewer(Guid id, CancellationToken calToken = default);

    [Put("/document/{id}/rename")]
    Task<bool> RenameDocument(Guid id, RenameDocumentCommand renameDocumentCommand, CancellationToken calToken = default);
}


