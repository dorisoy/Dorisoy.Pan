using Dorisoy.Pan.Services.API;

namespace Dorisoy.Pan.Services;
public interface IFolderService
{
    Task<ServiceResponse<DocumentDto>> UploadDocuments(FileInfo file, string fullFileName, Guid fid, Guid uid, Guid pid, CancellationToken calToken = default);
    Task<List<VirtualFolderInfoDto>> CreateFolders(AddChildFoldersCommand command, CancellationToken calToken = default);
}
