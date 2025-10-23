using Refit;
using Dorisoy.Pan.Services.API;

namespace Dorisoy.Pan.Services;

public class FolderService : IFolderService
{
    private readonly MakeRequest _makeRequest;
    private static string _url;
    private IMapper _mapper;
    private readonly AppSettings _appSettings;

    public FolderService(MakeRequest makeRequest, IMapper mapper, AppSettings appSettings)
    {
        _makeRequest = makeRequest;
        _mapper = mapper;
        _appSettings = appSettings;
        _url = _appSettings.GetHost();
    }

    /// <summary>
    /// 上传文档
    /// </summary>
    /// <param name="file"></param>
    /// <param name="fid"></param>
    /// <param name="uid"></param>
    /// <param name="pid"></param>
    /// <param name="calToken"></param>
    /// <returns></returns>
    public async Task<ServiceResponse<DocumentDto>> UploadDocuments(FileInfo file, string fullFileName, Guid fid, Guid uid, Guid pid, CancellationToken calToken = default)
    {
        try
        {
            using (var fs = file.OpenRead())
            {
                var api = RefitServiceBuilder.Build<IFolderApi>(_url, true);
                var stream = new StreamPart(fs, fullFileName);
                var result = await _makeRequest.Start(api.UploadDocuments(stream, fid, uid, pid, calToken), calToken);
                return result;
            }
        }
        catch (Exception e)
        {
            return null;
        }
    }

    /// <summary>
    /// 创建文件夹
    /// </summary>
    /// <param name="fid"></param>
    /// <param name="command"></param>
    /// <param name="calToken"></param>
    /// <returns></returns>
    public async Task<List<VirtualFolderInfoDto>> CreateFolders(AddChildFoldersCommand command, CancellationToken calToken = default)
    {
        try
        {
            var api = RefitServiceBuilder.Build<IFolderApi>(_url, true);
            var result = await _makeRequest.Start(api.CreateFolders(command.VirtualFolderId, command, calToken), calToken);
            return result;
        }
        catch (Exception e)
        {
            return null;
        }
    }
}

