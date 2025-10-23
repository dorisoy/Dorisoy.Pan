
using Ellipse = Avalonia.Controls.Shapes.Ellipse;
using Path = Avalonia.Controls.Shapes.Path;
using Point = Avalonia.Point;


namespace Dorisoy.Pan.ViewModels;

/// <summary>
/// 图片预览
/// </summary>
//[View(typeof(ImagePage))]
public class ImagePageViewModel : MainPageViewModelBase
{
    private readonly IAppState _appState;
    private readonly IUsersService _usersService;
    private readonly IPatientService _patientService;
    private readonly ISettingsProvider<AppSettings> _settingsProvider;
    private readonly IDocumentService _documentService;
    private readonly IVirtualFolderService _virtualFolderService;
    private readonly BrushSettings _brushSettings;


    [Reactive] public virtual ObservableCollection<DocumentModel> Images { get; set; } = new();

    /// <summary>
    /// 媒体地址
    /// </summary>
    [Reactive] public Bitmap ImageSource { get; set; }
    [Reactive] public string ImageURL { get; set; }


    [Reactive] public bool IsScreenCutter { get; set; }
    [Reactive] public DocumentModel SelectDocument { get; set; }
    /// <summary>
    /// 当前文件夹
    /// </summary>
    [Reactive] public VirtualFolderModel RootFolder { get; set; }

    public Canvas canvas { get; set; }
    private readonly Queue<Point> _linePointsQueue = new();
    private RenderTargetBitmap RenderTarget;


    /// <summary>
    /// 添加项目
    /// </summary>
    public ReactiveCommand<Unit, Unit> AddPatient { get; }

    /// <summary>
    /// 保存图片
    /// </summary>
    public ReactiveCommand<System.Drawing.Bitmap, Unit> SaveCommand { get; }

    /// <summary>
    /// 剪切屏幕
    /// </summary>
    public ReactiveCommand<Unit, Unit> ScreenCutCommand { get; }

    /// <summary>
    /// 播放图片
    /// </summary>
    public ReactiveCommand<Unit, Unit> NextCommand { get; }
    public ReactiveCommand<Unit, Unit> PrevCommand { get; }
    public int CurrentIndex { get; set; }


    /// <summary>
    /// 颜色枚举器
    /// </summary>
    private ColorsEnum _brushColor;
    public ColorsEnum BrushColor
    {
        get => _brushColor;
        set
        {
            this.RaiseAndSetIfChanged(ref _brushColor, value);
            _brushSettings.BrushColor = value;

            if (value == ColorsEnum.Eraser)
            {
                BrushThickness = ThicknessEnum.Eraser;
            }
            else
            {
                BrushThickness = _previousBrushThickness;
            }
        }
    }

    /// <summary>
    /// 画笔枚举器
    /// </summary>
    private ThicknessEnum _previousBrushThickness;
    private ThicknessEnum _brushThickness;
    public ThicknessEnum BrushThickness
    {
        get => _brushThickness;
        set
        {
            this.RaiseAndSetIfChanged(ref _brushThickness, value);
            _brushSettings.BrushThickness = value;

            if (_brushColor != ColorsEnum.Eraser || value != ThicknessEnum.Eraser)
            {
                _previousBrushThickness = _brushThickness;
            }
        }
    }


    public ImagePageViewModel(
        IUsersService usersService,
        IPatientService patientService) : base()
    {
        _usersService = usersService;
        _patientService = patientService;
        _settingsProvider = Locator.Current.GetService<ISettingsProvider<AppSettings>>();
        _documentService = Locator.Current.GetService<IDocumentService>();
        _appState = Locator.Current.GetRequiredService<IAppState>();
        _virtualFolderService = Locator.Current.GetService<IVirtualFolderService>();


        //画笔设置
        _brushSettings = _appState.BrushSettings;
        _brushColor = _brushSettings.BrushColor;
        _previousBrushThickness = _brushSettings.BrushThickness;
        _brushThickness = _brushSettings.BrushThickness;

        //添加项目
        AddPatient = ReactiveCommand.CreateFromTask(async () =>
        {
            try
            {
                var user = Globals.CurrentUser;
                var dialog = new ContentDialog()
                {
                    FullSizeDesired = true,
                    Title = "选择项目",
                    PrimaryButtonText = "确定",
                    CloseButtonText = "取消"
                };

                dialog.Content = new AddPatientView()
                {
                    DataContext = new AddPatientViewModel(dialog, showAdd: false)
                };

                var ok = await dialog.ShowAsync();
                if (ok == ContentDialogResult.Primary)
                {
                    CurrentPatient = Globals.CurrentPatient;
                    LoadImages(CurrentUser.Id, CurrentPatient.Id);
                }
            }
            catch (Exception ex)
            {
                MessageBox(ex.Message);
            }
        });


        //上下播放图片
        NextCommand = ReactiveCommand.Create(() =>
        {
            if (Images != null && Images.Any())
            {
                CurrentIndex++;
                if (CurrentIndex < Images.Count)
                {
                    var doc = Images[CurrentIndex];
                    UpdateUI(() =>
                    {
                        try
                        {
                            SelectDocument = doc;
                            //ImageSource = doc.Cover;
                            ImageURL = doc.Path;
                            foreach (var img in Images)
                            {
                                img.Selected = img.Id == doc.Id;
                            }
                        }
                        catch (Exception ex)
                        {
                            MessageBox(ex.Message);
                        }
                    });
                }
                else
                {
                    CurrentIndex = -1;
                }
            }
            else
            {
                MessageBox("未加载数据！");
            }
        });

        PrevCommand = ReactiveCommand.Create(() =>
        {
            if (Images != null && Images.Any())
            {
                CurrentIndex--;
                if (0 <= CurrentIndex && CurrentIndex < Images.Count)
                {
                    var doc = Images[CurrentIndex];
                    UpdateUI(() =>
                    {
                        try
                        {
                            SelectDocument = doc;
                            ImageURL = doc.Path;
                            foreach (var img in Images)
                            {
                                img.Selected = img.Id == doc.Id;
                            }
                        }
                        catch (Exception ex)
                        {
                            MessageBox(ex.Message);
                        }
                    });
                }
                else
                {
                    CurrentIndex = Images.Count;
                }
            }
            else
            {
                MessageBox("未加载数据！");
            }
        });


        //剪切屏幕
        ScreenCutCommand = ReactiveCommand.Create(() =>
        {
            try
            {
                IsScreenCutter = !IsScreenCutter;
            }
            catch (Exception ex)
            {
                MessageBox(ex.Message);
            }
        });

        this.WhenActivated((CompositeDisposable disposables) =>
        {
            RxApp.MainThreadScheduler.Schedule(LoadData)
            .DisposeWith(disposables);
        });
    }

    /// <summary>
    /// 保存屏幕区域并上传
    /// </summary>
    /// <param name="bitmap"></param>
    public async void Execute(System.Drawing.Bitmap bitmap)
    {
        try
        {
            //var path = @"C:\Users\Administrator\Desktop\Dorisoy\test.bmp";
            //RenderToFile(canvas, path);

            if (SelectDocument == null)
            {
                MessageBox("请选择文档图片！");
                return;
            }

            await Dispatcher.UIThread.InvokeAsync(async () =>
            {
                CheckPatient();

                var user = Globals.CurrentUser;
                var patient = Globals.CurrentPatient;

                var basePath = Environment.ExpandEnvironmentVariables(_settingsProvider.Settings.TempFolder);
                var path = System.IO.Path.Combine(basePath, $"{user.Id}", $"{patient.Id}");
                if (!Directory.Exists(path))
                    Directory.CreateDirectory(path);

                var fileName = System.IO.Path.Combine(path, $"{DateTime.Now:yyyyMMddHHmmss}.bmp");
                var extension = System.IO.Path.GetExtension(fileName);

                //保存图片到本地
                bitmap.Save(fileName);

                //上传文件
                var file = new FileInfo(fileName);
                var filePaths = new List<UploadFile>() { new UploadFile { Path = fileName, IsThumbnail = false } };
                //上传至项目文件夹
                RootFolder.PhysicalFolderId = patient.PhysicalFolderId;
                RootFolder.Id = patient.VirtualFolderId;

                var dialog = new ContentDialog() { Title = "上传文件", CloseButtonText = "取消" };
                dialog.Content = new AddFilesView()
                {
                    DataContext = new AddFilesViewModel(
                        patient,
                        dialog,
                        RootFolder,
                        filePaths,
                        autoClose: true,
                        complete: () =>
                        {
                            //删除本地
                            file.Delete();
                            //刷新
                            LoadImages(CurrentUser.Id, CurrentPatient.Id);
                        })
                };
                await dialog.ShowAsync();
            });
        }
        catch (Exception ex)
        {
            MessageBox(ex.Message);
        }

    }


    /// <summary>
    ///  检查项目
    /// </summary>
    private async void CheckPatient()
    {
        if (Globals.CurrentPatient == null)
        {
            MessageBox("请添加项目信息！");
            //提示添加项目
            await AddPatient.Execute();
            return;
        }
    }


    private async void UpdateUI(Action action)
    {
        await Dispatcher.UIThread.InvokeAsync(action, DispatcherPriority.Background);
    }

    private async void LoadData()
    {
        await Dispatcher.UIThread.InvokeAsync(async () =>
        {
            //获取虚拟根目录
            var _mapper = Locator.Current.GetService<IMapper>();
            var rootFolder = await _virtualFolderService.GetRootFolder();
            RootFolder = _mapper.Map<VirtualFolderModel>(rootFolder);

            if (CurrentUser.Id != Guid.Empty && CurrentPatient.Id != Guid.Empty)
                LoadImages(CurrentUser.Id, CurrentPatient.Id);

        }, DispatcherPriority.Background);
    }

    /// <summary>
    /// 刷新项目图片
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="patientId"></param>
    private async void LoadImages(Guid userId, Guid patientId)
    {
        var setting = _settingsProvider.Settings;
        await Dispatcher.UIThread.InvokeAsync(async () =>
        {
            try
            {
                Images.Clear();
                var _pathHelper = new PathHelper();
                //获取位图文档
                var docs = await _documentService.GetPatienterDocuments(userId, patientId);
                if (docs != null && docs.Any())
                {
                    foreach (var doc in docs)
                    {
                        if (doc.Extension.Contains(".bmp"))
                        {
                            //api/document/390952c2-3195-4e85-8958-0aff45a90ab7/download
                            doc.Path = setting.GetHost() + $"/document/{doc.Id}/download";
                            doc.FileType = FileType.Image;
                            doc.Selected = false;

                            //选择预览
                            doc.PlayCommand = ReactiveCommand.Create<DocumentModel>((item) =>
                            {
                                UpdateUI(() =>
                                {
                                    ImageURL = doc.Path;
                                    SelectDocument = item;
                                    foreach (var doc2 in docs)
                                    {
                                        doc2.Selected = doc2.Id == item.Id;
                                    }
                                });
                            });

                            Images.Add(doc);
                        }
                    }
                }
            }
            catch (Exception) { }
        });
    }

    public void EnqueueLineSegment(Point point1, Point point2)
    {
        _linePointsQueue.Enqueue(point1);
        _linePointsQueue.Enqueue(point2);
    }


    /// <summary>
    /// 渲染线条
    /// </summary>
    /// <returns></returns>
    public List<Point> RenderLine()
    {
        if (_linePointsQueue.Count == 0)
        {
            return new List<Point>();
        }

        var myPointCollection = new Points();

        var result = _linePointsQueue.ToList();
        var firstPoint = _linePointsQueue.Dequeue();

        while (_linePointsQueue.Count > 0)
        {
            var point = _linePointsQueue.Dequeue();
            myPointCollection.Add(point);
        }

        var pathGeometry = new PathGeometry();
        var pathFigure = new PathFigure
        {
            Segments = new PathSegments
            {
                new PolyLineSegment
                {
                    Points = myPointCollection
                }
            },
            StartPoint = firstPoint,
            IsClosed = false
        };
        pathGeometry.Figures.Add(pathFigure);

        var path = new Avalonia.Controls.Shapes.Path
        {
            Stroke = _brushSettings.ColorBrush,
            StrokeThickness = _brushSettings.Thickness,
            Data = pathGeometry
        };
        canvas.Children.Add(path);

        var ellipse = new Ellipse
        {
            Margin = new Thickness(firstPoint.X - _brushSettings.HalfThickness, firstPoint.Y - _brushSettings.HalfThickness, 0, 0),
            Fill = _brushSettings.ColorBrush,
            Width = _brushSettings.Thickness,
            Height = _brushSettings.Thickness
        };
        canvas.Children.Add(ellipse);

        return result;
    }
    /// <summary>
    /// 渲染线条
    /// </summary>
    /// <param name="linePointsQueue"></param>
    /// <param name="thickness"></param>
    /// <param name="colorBrush"></param>
    public void RenderLine(Queue<Point> linePointsQueue, double thickness, SolidColorBrush colorBrush)
    {
        if (linePointsQueue.Count == 0)
        {
            return;
        }

        var myPointCollection = new Points();

        var firstPoint = linePointsQueue.Dequeue();

        while (linePointsQueue.Count > 0)
        {
            var point = linePointsQueue.Dequeue();
            myPointCollection.Add(point);
        }

        var pathGeometry = new PathGeometry();
        var pathFigure = new PathFigure
        {
            Segments = new PathSegments
            {
                new PolyLineSegment
                {
                    Points = myPointCollection
                }
            },
            StartPoint = firstPoint,
            IsClosed = false
        };
        pathGeometry.Figures.Add(pathFigure);

        var path = new Path
        {
            Stroke = colorBrush,
            StrokeThickness = thickness,
            Data = pathGeometry
        };
        canvas.Children.Add(path);

        var ellipse = new Avalonia.Controls.Shapes.Ellipse
        {
            Margin = new Thickness(firstPoint.X - thickness / 2, firstPoint.Y - thickness / 2, 0, 0),
            Fill = colorBrush,
            Width = thickness,
            Height = thickness
        };
        canvas.Children.Add(ellipse);
    }

    public Point RestrictPointToCanvas(double x, double y)
    {
        if (x > _brushSettings.MaxBrushPointX)
        {
            x = _brushSettings.MaxBrushPointX;
        }
        else if (x < _brushSettings.MinBrushPoint)
        {
            x = _brushSettings.MinBrushPoint;
        }

        if (y > _brushSettings.MaxBrushPointY)
        {
            y = _brushSettings.MaxBrushPointY;
        }
        else if (y < _brushSettings.MinBrushPoint)
        {
            y = _brushSettings.MinBrushPoint;
        }
        return new Point(x, y);
    }

    public Task<bool> SaveAsync(string path)
    {
        return Task.Run(() =>
        {
            try
            {
                RenderTarget.Save(path);
            }
            catch (Exception)
            {
                return false;
            }

            return true;
        });
    }

    /// <summary>
    /// 绘制坐标点
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    public void DrawPoint(double x, double y)
    {
        var ellipse = new Avalonia.Controls.Shapes.Ellipse
        {
            Margin = new Thickness(x - _brushSettings.HalfThickness, y - _brushSettings.HalfThickness, 0, 0),
            Fill = _brushSettings.ColorBrush,
            Width = _brushSettings.Thickness,
            Height = _brushSettings.Thickness
        };
        canvas.Children.Add(ellipse);
    }

    /// <summary>
    /// 异步绘制坐标点
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <returns></returns>
    public async Task DrawPointAsync(double x, double y)
    {
        var data = PayloadConverter.ToBytes(x, y, _appState.BrushSettings.BrushThickness, _appState.BrushSettings.BrushColor);
        var point = PayloadConverter.ToPoint(data);
        await Dispatcher.UIThread.InvokeAsync(() =>
        {
            canvas.Children.Add(point);
        });
    }

    /// <summary>
    /// 异步绘制线条
    /// </summary>
    /// <param name="cpoints"></param>
    /// <returns></returns>
    public async Task DrawLineAsync(List<Point> cpoints)
    {
        var data = PayloadConverter.ToBytes(cpoints, _appState.BrushSettings.BrushThickness, _appState.BrushSettings.BrushColor);
        var (points, thickness, colorBrush) = PayloadConverter.ToLine(data);
        await Dispatcher.UIThread.InvokeAsync(() =>
        {
            RenderLine(points, thickness, colorBrush);
        });
    }
}

