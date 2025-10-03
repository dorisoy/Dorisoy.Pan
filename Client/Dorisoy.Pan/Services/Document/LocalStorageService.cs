using Dorisoy.Pan.Data.Contexts;

namespace Dorisoy.Pan.Services;

public class LocalStorageService
{
    private readonly IMapper _mapper;
    private readonly IDbContextFactory<CaptureManagerContext> _contextFactory;

    private readonly IUsersService _usersService;
    private readonly IVirtualFolderService _virtualFolderService;

    private readonly SourceCache<UploadFile, Guid> _items;
    public IObservable<IChangeSet<UploadFile, Guid>> Connect() => _items.Connect();

    private readonly IObservable<bool> _isLoading;
    public IObservable<bool> IsLoading => _isLoading;


    public LocalStorageService(IDbContextFactory<CaptureManagerContext> contextFactory, IMapper mapper)
    {
        _mapper = mapper;
        _contextFactory = contextFactory;
        _usersService = Locator.Current.GetService<IUsersService>();
        _virtualFolderService = Locator.Current.GetService<IVirtualFolderService>();

        _items = new SourceCache<UploadFile, Guid>(e => e.Id);
    }

    public async Task AddUpload(Guid folderId)
    {
        await Task.Run(() =>
        {
            var upload = new UploadFile();
            _items.AddOrUpdate(upload);
        });
    }

}
