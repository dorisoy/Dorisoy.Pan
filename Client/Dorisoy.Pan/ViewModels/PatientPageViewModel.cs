namespace Dorisoy.PanClient.ViewModels;


//[View(typeof(PatientPage))]
public class PatientPageViewModel : MainPageViewModelBase
{
    private readonly IUsersService _usersService;
    private readonly IPatientService _patientService;
    private readonly ISettingsProvider<AppSettings> _settingsProvider;
    private readonly IDocumentService _documentService;
    private readonly IMapper _mapper;

    [Reactive] public ObservableCollection<PatientModel> PatientItems { get; set; } = new();
    [Reactive] public PatientModel SelectedPatientItem { get; set; }

    /// <summary>
    /// 当前项目信息
    /// </summary>
    [Reactive] public PatientModel Patient { get; set; }
    [Reactive] public DocumentModel SelectedDocumentItem { get; set; }

    protected ObservableAsPropertyHelper<bool> _isNull;
    public bool ISNull { get { return _isNull.Value; } }


    protected ObservableAsPropertyHelper<bool> _isNull2;
    public bool ISNull2 { get { return _isNull2.Value; } }


    public PatientPageViewModel(
        IUsersService usersService,
        IPatientService patientService) : base()
    {
        _usersService = usersService;
        _patientService = patientService;
        _settingsProvider = Locator.Current.GetService<ISettingsProvider<AppSettings>>();
        _documentService = Locator.Current.GetService<IDocumentService>();
        _mapper = Locator.Current.GetService<IMapper>();

        //打印页面
        PrintPage = ReactiveCommand.Create(() => Printing(this.PageName));

        this.WhenAnyValue(x => x.SelectedPatientItem)
             .WhereNotNull()
            .Subscribe(s =>
            {
                Patient = s;
                GetPatienterDocuments(CurrentUser.Id, s.Id);
            });


        this.WhenAnyValue(x => x.Patient.Videos).Select(s => s.Count > 0).ToProperty(this, x => x.ISNull, out _isNull);


        this.WhenAnyValue(x => x.Patient.Images).Select(s => s.Count > 0).ToProperty(this, x => x.ISNull2, out _isNull2);


        this.WhenActivated((CompositeDisposable disposables) =>
        {
            RxApp.MainThreadScheduler.Schedule(LoadData)
            .DisposeWith(disposables);
        });

    }

    private async void LoadData()
    {
        await Dispatcher.UIThread.InvokeAsync(async () =>
        {
            var users = await _patientService.GetPatients();
            if (users != null && users.Any())
                PatientItems.Add(users);

        }, DispatcherPriority.Background);
    }



    private async void GetPatienterDocuments(Guid userId, Guid patientId)
    {
        var setting = _settingsProvider.Settings;
        await Dispatcher.UIThread.InvokeAsync(async () =>
        {
            try
            {
                Patient.Images.Clear();
                Patient.Videos.Clear();

                var _pathHelper = new PathHelper();
                //获取位图文档
                var docs = await _documentService.GetPatienterDocuments(userId, patientId);
                if (docs != null && docs.Any())
                {
                    foreach (var doc in docs)
                    {
                        if (doc.Extension.Contains(".bmp") && doc.IsAttachment == false)
                        {
                            doc.PathURL = setting.GetHost() + $"/document/{doc.Id}/download";
                            doc.FileType = FileType.Image;
                            Patient.Images.Add(doc);
                        }
                        else if (doc.Extension.Contains(".avi"))
                        {
                            //获取视频缩略图
                            var name = doc.Name.Replace(".avi", ".bmp");
                            var tdoc = docs.Where(s => s.Name == name).FirstOrDefault();
                            if (tdoc != null)
                            {
                                doc.PathURL = setting.GetHost() + $"/document/{tdoc.Id}/download";
                                doc.FileType = FileType.Video;
                                Patient.Videos.Add(doc);
                            }
                        }
                    }

                }
            }
            catch (Exception) { }
        });
    }


    public async void Execute(DocumentModel item)
    {
        await Dispatcher.UIThread.InvokeAsync(() =>
        {
            if (item != null)
            {
                SelectedDocumentItem = item;
            }

        }, DispatcherPriority.Background);
    }
}
