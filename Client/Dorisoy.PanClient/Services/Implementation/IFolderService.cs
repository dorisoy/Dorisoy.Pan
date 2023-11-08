using Dorisoy.PanClient.Commands;
using Dorisoy.PanClient.Services.API;

namespace Dorisoy.PanClient.Services;
public interface IFolderService
{
    Task<ServiceResponse<DocumentDto>> UploadDocuments(FileInfo file, string fullFileName, Guid fid, Guid uid, Guid pid, CancellationToken calToken = default);
    Task<List<VirtualFolderInfoDto>> CreateFolders(AddChildFoldersCommand command, CancellationToken calToken = default);
}
