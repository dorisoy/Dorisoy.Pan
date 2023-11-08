using Dorisoy.PanClient.Commands;
using Dorisoy.PanClient.Services.API;

namespace Dorisoy.PanClient.Services;
public interface IRemoteVirtualFolderService
{
    Task<ServiceResponse<List<VirtualFolderInfoDto>>> CreateFolder(AddFolderCommand command, CancellationToken calToken = default);
    Task<ServiceResponse<VirtualFolderDto>> DeleteFolder(Guid fid, CancellationToken calToken = default);
    Task<bool> RenameFolder(Guid fid, RenameFolderCommand command, CancellationToken calToken = default);
}
