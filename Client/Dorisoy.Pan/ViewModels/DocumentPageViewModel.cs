using Spinner = Dorisoy.PanClient.Views.Spinner;

namespace Dorisoy.PanClient.ViewModels;


public class DocumentPageViewModel : MainPageViewModelBase
{
    private readonly IUsersService _usersService;
    private readonly IPhysicalFolderService _physicalFolderService;
    private readonly IVirtualFolderService _virtualFolderService;
    private readonly IPhysicalFolderUserService _physicalFolderUserService;
    private readonly IVirtualFolderUserUserService _virtualFolderUserUserService;
    private readonly IPatientService _patientService;
    private readonly IFolderService _folderService;
    private readonly IRemoteVirtualFolderService _remoteVirtualFolderService;
    private readonly IRemoteDocumentService _remoteDocumentService;
    private readonly IDocumentService _documentService;
    private readonly IMapper _mapper;

    public ReactiveCommand<Unit, Unit> UploadDocument { get; }
    public ReactiveCommand<Unit, Unit> UploadDocuments { get; }
    public ReactiveCommand<Unit, Unit> CreateFolders { get; }
    public ReactiveCommand<Unit, Unit> PreviousFolder { get; }

    public ReactiveCommand<DocumentFolderModel, Unit> ReviewDoc { get; }
    public ReactiveCommand<DocumentFolderModel, Unit> DeleteDoc { get; }
    public ReactiveCommand<DocumentFolderModel, Unit> DoubleClick { get; }
    public ReactiveCommand<Unit, Unit> DeleteCommand { get; }

    public ReactiveCommand<Unit, Unit> PreviousPageCommand { get; }
    public ReactiveCommand<Unit, Unit> NextPageCommand { get; }
    public ReactiveCommand<Unit, Unit> FirstPageCommand { get; }
    public ReactiveCommand<Unit, Unit> LastPageCommand { get; }

    private ReadOnlyObservableCollection<DocumentFolderModel> _items;
    public ReadOnlyObservableCollection<DocumentFolderModel> Items => _items;

    [Reactive] public DocumentFolderModel SelectedItem { get; set; }

    private const int PageSize = 20;
    private const int PageIndex = 1;
    private readonly ISubject<PageRequest> _pager;

    [Reactive] public int TotalItems { get; set; }
    [Reactive] public int CurrentPage { get; set; }
    [Reactive] public int TotalPages { get; set; }

    [Reactive] public string Title { get; set; }
    [Reactive] public bool IsViewGrid { get; set; }
    [Reactive] public Symbol ViewGridIcon { get; set; } = Symbol.ListFilled;

    [Reactive] public bool IsLoading { get; set; } = true;

    /// <summary>
    /// XamlRoot
    /// </summary>
    public Visual VisualRoot { get; set; }

    /// <summary>
    /// 当前文件夹
    /// </summary>
    [Reactive] public VirtualFolderModel RootFolder { get; set; }
    [Reactive] public List<HierarchyFolder> FolderPaths { get; set; }


    public PatientModel Patient { get; set; }

    private CancellationTokenSource _cts;

    public DocumentPageViewModel(IUsersService usersService) : base()
    {
        _usersService = usersService;
        _physicalFolderService = Locator.Current.GetService<IPhysicalFolderService>();
        _virtualFolderService = Locator.Current.GetService<IVirtualFolderService>();
        _physicalFolderUserService = Locator.Current.GetService<IPhysicalFolderUserService>();
        _virtualFolderUserUserService = Locator.Current.GetService<IVirtualFolderUserUserService>();
        _documentService = Locator.Current.GetService<IDocumentService>();
        _mapper = Locator.Current.GetService<IMapper>();
        _patientService = Locator.Current.GetService<IPatientService>();
        _folderService = Locator.Current.GetService<IFolderService>();
        _remoteVirtualFolderService = Locator.Current.GetService<IRemoteVirtualFolderService>();
        _remoteDocumentService = Locator.Current.GetService<IRemoteDocumentService>();

        //打印页面
        PrintPage = ReactiveCommand.Create(() => Printing(this.PageName));

        //上传文件
        UploadDocument = ReactiveCommand.CreateFromTask(async () =>
        {
            try
            {
                var storageProvider = Locator.Current.GetService<IStorageProvider>();
                var option = new FilePickerOpenOptions()
                {
                    Title = "上传文件",
                    AllowMultiple = true,
                    FileTypeFilter = new FilePickerFileType[]
                    {
                      new FilePickerFileType("All files (.*)")
                      {
                        Patterns = new List<string> { "mp4", "jepg", "*" }
                      }
                    }
                };
                var spder = await storageProvider.OpenFilePickerAsync(option);

                var filePaths = new List<UploadFile>();
                if (spder != null && spder.Any())
                {
                    foreach (var sd in spder)
                    {
                        filePaths.Add(new UploadFile
                        {
                            Path = sd.Path.ToString().Replace("file:///", ""),
                            IsThumbnail = false
                        });
                    }
                }

                if (filePaths != null && filePaths.Any())
                {
                    var dialog = new ContentDialog() { Title = "上传文件", CloseButtonText = "取消" };
                    dialog.Content = new AddFilesView()
                    {
                        DataContext = new AddFilesViewModel(Patient,
                        dialog,
                        RootFolder,
                        filePaths,
                        autoClose: true,
                        complete: () =>
                        {
                            Refresh();
                        })
                    };
                    await dialog.ShowAsync();
                }
            }
            catch (Exception ex)
            {
                MessageBox(ex.Message);
            }
        });

        //上传文件夹
        UploadDocuments = ReactiveCommand.CreateFromTask(async () =>
        {
            try
            {
                var folder = await OpenFolder("上传文件夹", _lastDirectorySelected);
                if (!string.IsNullOrEmpty(folder))
                {
                    var user = Globals.CurrentUser;
                    var model = Traversing(folder);

                    //注意：（重要）
                    var virtualFolderId = RootFolder.Id;
                    var physicalFolderId = RootFolder.PhysicalFolderId;

                    var cmd = new AddChildFoldersCommand
                    {
                        Paths = model.FolderPath,
                        PhysicalFolderId = physicalFolderId,
                        VirtualFolderId = virtualFolderId
                    };

                    //远程创建文件夹
                    var loading = new LoadingDialog(new Spinner());
                    await loading.ShowAsync(async (close) =>
                    {
                        var result = await _folderService.CreateFolders(cmd);
                        if (result != null)
                        {
                            //更新GridView
                            Refresh();
                        }
                        close();
                    });

                    #region 文件上传业务

                    var filePaths = new List<UploadFile>();
                    if (model.FilePath != null && model.FilePath.Any())
                    {
                        foreach (var s in model.FilePath)
                        {
                            filePaths.Add(new UploadFile { Path = s, IsThumbnail = false });
                        }
                    }

                    if (filePaths != null && filePaths.Any())
                    {
                        var dialog = new ContentDialog() { Title = "上传文件夹", CloseButtonText = "取消" };
                        dialog.Content = new AddFilesView()
                        {
                            DataContext = new AddFilesViewModel(Patient,
                            dialog,
                            RootFolder,
                            filePaths.ToList(),
                            model.RootPath,
                            autoClose: true)
                        };
                        var ok = await dialog.ShowAsync();
                        if (ok == ContentDialogResult.Primary)
                        {
                            Refresh();
                        }
                    }

                    #endregion
                }
            }
            catch (Exception ex)
            {
                MessageBox(ex.Message);
            }
        });

        //新建文件夹
        CreateFolders = ReactiveCommand.CreateFromTask(async () =>
        {
            var dialog = new ContentDialog()
            {
                Title = "新建文件夹",
                PrimaryButtonText = "创建",
                CloseButtonText = "取消"
            };
            dialog.Content = new AddFolderView()
            {
                DataContext = new AddFolderViewModel(dialog, RootFolder)
            };
            var ok = await dialog.ShowAsync();
            if (ok == ContentDialogResult.Primary)
            {
                Refresh();
            }
        });

        //上一目录
        PreviousFolder = ReactiveCommand.CreateFromTask(async () =>
        {
            if (RootFolder != null)
            {
                var user = Globals.CurrentUser;
                var item = RootFolder;
                if (user != null && item != null)
                {
                    var virtualFolder = await _virtualFolderService
                    .GetVirtualFolderDetailById(item.ParentId.Value, user.Id);
                    if (virtualFolder != null)
                    {
                        RootFolder = new VirtualFolderModel()
                        {
                            Id = virtualFolder.Id,
                            Name = virtualFolder.Name,
                            ParentId = virtualFolder.ParentId,
                            PhysicalFolderId = virtualFolder.PhysicalFolderId
                        };

                        LoadFolderPaths(virtualFolder.Id);
                    }
                }
            }
        });

        //双击上一目录
        DoubleClick = ReactiveCommand.Create<DocumentFolderModel>((item) =>
        {
            if (item != null)
            {
                if (item.DocType == DocType.Folder)
                {
                    MessageBox("双击上一目录");
                }
            }
        });

        //删除项目
        DeleteDoc = ReactiveCommand.Create<DocumentFolderModel>(async (item) =>
        {
            if (item != null)
            {
                var ok = await ConfirmBox("你确定要删除项目吗？");
                if (ok == DialogResult.Primary)
                {

                    var loading = new LoadingDialog(new Spinner());
                    await loading.ShowAsync(async (close) =>
                    {
                        var user = Globals.CurrentUser;
                        if (item.DocType == DocType.File)
                        {
                            var result = await _remoteDocumentService.DeleteDocument(item.Id);
                            if (result)
                            {
                                close();
                                MessageBox("文件删除成功！");
                                Refresh();
                            }
                        }
                        else if (item.DocType == DocType.Folder)
                        {
                            var result = await _remoteVirtualFolderService.DeleteFolder(item.Id);
                            if (result == null)
                            {
                                close();
                                MessageBox("文件夹删除成功！");
                                Refresh();
                            }
                        }
                    });
                }
            }
        });

        //批量删除项目
        DeleteCommand = ReactiveCommand.Create(() =>
        {
            var selections = Items.Where(s => s.Selected).ToList();
            if (!selections.Any())
            {
                MessageBox("请选择项目！");
                return;
            }


            foreach (var item in selections)
            {
                if (item.DocType == DocType.File)
                {
                    //删除文件
                }
                else if (item.DocType == DocType.Folder)
                {
                    //删除文件夹
                }
            }
        });

        //预览项目
        ReviewDoc = ReactiveCommand.Create<DocumentFolderModel>((item) =>
        {
            if (item != null && item.DocType == DocType.File)
            {
                if (!item.Extension.Contains(".bmp")
                && !item.Extension.Contains(".jpeg")
                && !item.Extension.Contains(".png")
                && !item.Extension.Contains(".mp4")
                && !item.Extension.Contains(".avi"))
                {
                    MessageBox("文件格式不支持预览！");
                    return;
                }

                if (Host is MainView mianView)
                {
                    if (item.Extension.Contains(".mp4") || item.Extension.Contains(".avi"))
                        mianView.NavigateTo(typeof(VideoManagePageViewModel));
                    else
                        mianView.NavigateTo(typeof(ImagePageViewModel));
                }
            }
            else
            {
                MessageBox("文件格式不支持预览！");
            }
        });

        //上一页
        PreviousPageCommand = ReactiveCommand.Create(() =>
        {
            if (CurrentPage > PageIndex)
                _pager.OnNext(new PageRequest(CurrentPage - 1, PageSize));
        });

        //下一页
        NextPageCommand = ReactiveCommand.Create(() =>
        {
            if (CurrentPage < TotalPages)
                _pager.OnNext(new PageRequest(CurrentPage + 1, PageSize));
        });

        //首页
        FirstPageCommand = ReactiveCommand.Create(() =>
        {
            if (CurrentPage > PageIndex)
                _pager.OnNext(new PageRequest(PageIndex, PageSize));
        });

        //尾页
        LastPageCommand = ReactiveCommand.Create(() =>
        {
            if (CurrentPage < TotalPages)
                _pager.OnNext(new PageRequest(TotalPages, PageSize));
        });

        this.WhenAnyValue(x => x.RootFolder)
            .WhereNotNull()
            .Where(s => s.PhysicalFolderId != Guid.Empty)
            .Subscribe(x =>
            {
                Refresh();
            });

        //分页
        _pager = new BehaviorSubject<PageRequest>(new PageRequest(PageIndex, PageSize));


        this.WhenActivated((CompositeDisposable disposables) =>
        {
            _cts = new CancellationTokenSource();

            //订阅文档信息
            _documentService
            .Connect()
            .Sort(SortExpressionComparer<DocumentFolderModel>
            .Descending(s => s.DocType).ThenByDescending(s => s.CreatedDate))
            .Page(_pager)
            .Do(change => PagingUpdate(change.Response))
            .ObserveOn(RxApp.MainThreadScheduler)
            .Bind(out _items)
            .Subscribe(s =>
            {
                IsLoading = s.Count <= 0;
            })
            .DisposeWith(disposables);

            IsLoading = true;

            LoadDataCommand.Execute(_cts.Token)
            .Subscribe()
            .DisposeWith(disposables);

            Observable.Timer(TimeSpan.FromSeconds(5))
            .Subscribe(_ =>
            {
                IsLoading = false;

            }).DisposeWith(disposables);

            System.Reactive.Disposables.Disposable.Create(() => _cts.Cancel()).DisposeWith(disposables);
        });
    }


    /// <summary>
    /// 载入数据
    /// </summary>
    /// <returns></returns>
    protected override void LoadDataAsync(CancellationToken token)
    {
        Task.Run(async () =>
        {
            //获取虚拟根目录
            var rootFolder = await _virtualFolderService.GetRootFolder();
            var folderPaths = new List<HierarchyFolder>();

            //获取层次结构文件夹
            if (rootFolder.Id != Guid.Empty)
                folderPaths = await _virtualFolderService.GetParentsHierarchyById(rootFolder.Id);

            await Dispatcher.UIThread.InvokeAsync(() =>
            {
                RootFolder = _mapper.Map<VirtualFolderModel>(rootFolder);
                FolderPaths = folderPaths;
                
            });

        }, token);
    }

    /// <summary>
    /// 当前选择目录
    /// </summary>
    /// <param name="item"></param>
    public async void Execute(DocumentFolderModel item)
    {
        await Dispatcher.UIThread.InvokeAsync(async () =>
        {
            if (item != null)
            {
                if (item.DocType == DocType.Folder)
                {
                    ///item.Id 这里的ID== VirtualFolder.Id
                    var user = Globals.CurrentUser;
                    var virtualFolder = await _virtualFolderService.GetVirtualFolderDetailById(item.Id, user.Id);
                    if (virtualFolder != null)
                    {
                        RootFolder = new VirtualFolderModel()
                        {
                            Id = virtualFolder.Id,
                            Name = virtualFolder.Name,
                            ParentId = virtualFolder.ParentId,
                            PhysicalFolderId = virtualFolder.PhysicalFolderId
                        };
                        //获取层次结构文件夹
                        LoadFolderPaths(virtualFolder.Id);
                    }

                    if (item.PatienterId != Guid.Empty)
                    {
                        Patient = await _patientService.GetPatient(item.PatienterId);
                    }
                }
            }

        }, DispatcherPriority.Background);
    }


    /// <summary>
    /// 刷新
    /// </summary>
    public async void Refresh()
    {
        IsLoading = true;
        if (RootFolder != null)
            await _documentService.GetAllDocuments(RootFolder.Id);
        IsLoading = false;
    }

    private void PagingUpdate(IPageResponse response)
    {
        TotalItems = response.TotalSize;
        CurrentPage = response.Page;
        TotalPages = response.Pages;
    }

    /// <summary>
    /// 最近一次选择目录
    /// </summary>
    private string? _lastDirectorySelected;


    /// <summary>
    /// 启动系统文件夹对话框，以便用户可以在磁盘上选择系统文件夹.
    /// </summary>
    /// <param name="title">标题</param>
    /// <param name="startDirectory">要浏览的根目录</param>
    /// <returns>选定的文件夹路径，如果对话框被取消则为null</returns>
    public async Task<string?> OpenFolder(string? title, string? startDirectory = null)
    {
        var fd = new OpenFolderDialog
        {
            Directory = startDirectory ?? _lastDirectorySelected,
            Title = title
        };

        var path = await fd.ShowAsync(GetActiveWindowOrMainWindow());

        _lastDirectorySelected = path!;

        return path;
    }


    private Window GetActiveWindowOrMainWindow()
    {
        if (Application.Current?.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            return desktop.Windows.SingleOrDefault(x => x.IsActive) ?? desktop.MainWindow;
        }

        throw new InvalidOperationException("Cannot find a Window when ApplicationLifetime is not ClassicDesktopStyleApplicationLifetime");
    }

    /// <summary>
    /// 遍历文件夹非递归实现（采用队列的广度优先算法）
    /// </summary>
    /// <param name="sPathName"></param>
    private List<string> Traversing(string sPathName, bool onlyFolder = false)
    {
        var paths = new List<string>();
        //创建一个队列用于保存子目录
        Queue<string> pathQueue = new Queue<string>();
        //首先把根目录排入队中
        pathQueue.Enqueue(sPathName);

        //"C:/Users/Administrator/Desktop"
        var root = string.Join("\\", sPathName.Split("\\").SkipLast(1));

        //开始循环查找文件，直到队列中无任何子目录
        while (pathQueue.Count > 0)
        {
            //从队列中取出一个目录，把该目录下的所有子目录排入队中
            DirectoryInfo diParent = new DirectoryInfo(pathQueue.Dequeue());
            foreach (DirectoryInfo diChild in diParent.GetDirectories())
            {
                pathQueue.Enqueue(diChild.FullName);
            }

            //查找该目录下的所有文件，依次处理
            foreach (FileInfo fi in diParent.GetFiles())
            {
                if (onlyFolder)
                {
                    var path = fi.FullName.Replace(root, "");
                    if (path.StartsWith("\\"))
                        path = path.Substring(1);

                    paths.Add(path);
                }
                else
                    paths.Add(fi.FullName);
            }
        }

        return paths;
    }

    private TraversingModel Traversing(string sPathName)
    {
        var model = new TraversingModel();
        //创建一个队列用于保存子目录
        Queue<string> pathQueue = new Queue<string>();
        //首先把根目录排入队中
        pathQueue.Enqueue(sPathName);

        //"C:/Users/Administrator/Desktop"
        var root = string.Join("\\", sPathName.Split("\\").SkipLast(1));
        model.RootPath = root;
        //开始循环查找文件，直到队列中无任何子目录
        while (pathQueue.Count > 0)
        {
            //从队列中取出一个目录，把该目录下的所有子目录排入队中
            DirectoryInfo diParent = new DirectoryInfo(pathQueue.Dequeue());
            foreach (DirectoryInfo diChild in diParent.GetDirectories())
            {
                pathQueue.Enqueue(diChild.FullName);
            }

            //查找该目录下的所有文件，依次处理
            foreach (FileInfo fi in diParent.GetFiles())
            {
                var path = fi.FullName.Replace(root, "");
                if (path.StartsWith("\\"))
                    path = path.Substring(1);

                if (!model.FolderPath.Contains(path))
                    model.FolderPath.Add(path);

                if (!model.FilePath.Contains(fi.FullName))
                    model.FilePath.Add(fi.FullName);
            }
        }

        return model;
    }


    /// <summary>
    /// 获取层次结构文件夹
    /// </summary>
    /// <param name="id"></param>
    private async void LoadFolderPaths(Guid id)
    {
        try
        {
            await Task.Run(async () =>
            {
                if (id != Guid.Empty)
                {
                    //获取层次结构文件夹
                    var folderPaths = await _virtualFolderService.GetParentsHierarchyById(id);
                    await Dispatcher.UIThread.InvokeAsync(() =>
                    {
                        FolderPaths = folderPaths;
                    }, DispatcherPriority.Background);
                }
            });
        }
        catch (Exception ex)
        {
            MessageBox(ex.Message);
        };
    }

}
