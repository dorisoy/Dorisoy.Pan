using AutoMapper;

namespace Dorisoy.PanClient.Services;

public class RemoteDocumentService : IRemoteDocumentService
{
    private readonly MakeRequest _makeRequest;
    private static string _url;
    private IMapper _mapper;
    private readonly AppSettings _appSettings;

    public RemoteDocumentService(MakeRequest makeRequest, IMapper mapper, AppSettings appSettings)
    {
        _makeRequest = makeRequest;
        _mapper = mapper;
        _appSettings = appSettings;
        _url = _appSettings.HostUrl.EndsWith("/") ? (_appSettings.HostUrl + "api") : (_appSettings.HostUrl + "/api");
    }


    public async Task<bool> DeleteDocument(Guid did, CancellationToken calToken = default)
    {
        try
        {
            var api = RefitServiceBuilder.Build<IDocumentApi>(_url, true);
            var result = await _makeRequest.Start(api.DeleteDocument(did, calToken), calToken);
            return result;
        }
        catch (Exception e)
        {
            return false;
        }
    }
}

