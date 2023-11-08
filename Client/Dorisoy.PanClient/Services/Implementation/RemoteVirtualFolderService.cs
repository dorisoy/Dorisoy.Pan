using AutoMapper;
using Dorisoy.PanClient.Commands;
using Dorisoy.PanClient.Services.API;

namespace Dorisoy.PanClient.Services;

public class RemoteVirtualFolderService : IRemoteVirtualFolderService
{
    private readonly MakeRequest _makeRequest;
    private static string _url;
    private IMapper _mapper;
    private readonly AppSettings _appSettings;

    public RemoteVirtualFolderService(MakeRequest makeRequest, IMapper mapper, AppSettings appSettings)
    {
        _makeRequest = makeRequest;
        _mapper = mapper;
        _appSettings = appSettings;
        _url = _appSettings.HostUrl.EndsWith("/") ? (_appSettings.HostUrl + "api") : (_appSettings.HostUrl + "/api");
    }

    /// <summary>
    /// 创建文件夹
    /// </summary>
    /// <param name="command"></param>
    /// <param name="calToken"></param>
    /// <returns></returns>
    public async Task<ServiceResponse<List<VirtualFolderInfoDto>>> CreateFolder(AddFolderCommand command, CancellationToken calToken = default)
    {
        try
        {
            var api = RefitServiceBuilder.Build<IVirtualFolderApi>(_url, true);
            var result = await _makeRequest.Start(api.CreateFolder(command, calToken), calToken);
            return result;
        }
        catch (Exception e)
        {
            return null;
        }
    }

    /// <summary>
    /// 删除文件夹
    /// </summary>
    /// <param name="fid"></param>
    /// <param name="calToken"></param>
    /// <returns></returns>
    public async Task<ServiceResponse<VirtualFolderDto>> DeleteFolder(Guid fid, CancellationToken calToken = default)
    {
        try
        {
            var api = RefitServiceBuilder.Build<IVirtualFolderApi>(_url, true);
            var result = await _makeRequest.Start(api.DeleteFolder(fid, calToken), calToken);
            return result;
        }
        catch (Exception e)
        {
            return null;
        }
    }

    /// <summary>
    /// 重命名文件夹
    /// </summary>
    /// <param name="fid"></param>
    /// <param name="command"></param>
    /// <param name="calToken"></param>
    /// <returns></returns>
    public async Task<bool> RenameFolder(Guid fid, RenameFolderCommand command, CancellationToken calToken = default)
    {
        try
        {
            var api = RefitServiceBuilder.Build<IVirtualFolderApi>(_url, true);
            var result = await _makeRequest.Start(api.RenameFolder(fid, command, calToken), calToken);
            return result;
        }
        catch (Exception ex)
        {
            return false;
        }
    }

}

