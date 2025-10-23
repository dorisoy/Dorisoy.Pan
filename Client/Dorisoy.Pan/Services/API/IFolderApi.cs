using Refit;
using Dorisoy.Pan.Services.API;

namespace Dorisoy.Pan.Services;

[Headers("Authorization: Bearer")]
public interface IFolderApi
{
    [Post("/folder/upload/{id}/{uid}/{pid}")]
    [Multipart]
    Task<ServiceResponse<DocumentDto>> UploadDocuments([AliasAs("file")] StreamPart stream, Guid id, Guid uid, Guid pid, CancellationToken calToken = default);

    [Post("/folder/folder/{id}")]
    Task<List<VirtualFolderInfoDto>> CreateFolders(Guid id, AddChildFoldersCommand command, CancellationToken calToken = default);
}


