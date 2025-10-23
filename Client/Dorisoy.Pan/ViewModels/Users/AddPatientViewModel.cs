using ValidationContext = ReactiveUI.Validation.Contexts.ValidationContext;

namespace Dorisoy.Pan.ViewModels;

public class AddPatientViewModel : ViewModelBase, IValidatableViewModel
{
    private readonly IUsersService _employeesService;
    private readonly IDepartmentService _departmentService;
    private readonly IPatientService _patientService;
    private readonly IVirtualFolderService _virtualFolderService;
    private readonly IPhysicalFolderService _physicalFolderService;
    ValidationContext IValidatableViewModel.ValidationContext { get; } = new ValidationContext();
    private readonly ContentDialog dialog;


    [Reactive] public PatientModel Model { get; set; }
    [Reactive] public string UsernameValidation { get; set; }
    [Reactive] public List<PatientModel> Items { get; set; }
    [Reactive] public PatientModel SelectedPatientItem { get; set; }

    [Reactive] public List<DepartmentModel> Departments { get; set; }
    [Reactive] public DepartmentModel SelectDepartment { get; set; }
    [Reactive] public VirtualFolderModel RootFolder { get; set; }
    [Reactive] public bool ShowAddFrom { get; set; }


    [Reactive] public string Tets { get; set; } = "sdadads";


    private CancellationTokenSource _cts;

    public AddPatientViewModel(ContentDialog dialog, PatientModel user = null, bool showAdd = true) : base()
    {
        if (dialog is null)
            throw new ArgumentNullException(nameof(dialog));

        _departmentService = Locator.Current.GetService<IDepartmentService>();
        _employeesService = Locator.Current.GetService<IUsersService>();
        _patientService = Locator.Current.GetService<IPatientService>();
        _virtualFolderService = Locator.Current.GetService<IVirtualFolderService>();
        _physicalFolderService = Locator.Current.GetService<IPhysicalFolderService>();

        this.dialog = dialog;
        dialog.Closed += DialogOnClosed;
        dialog.PrimaryButtonClick += Dialog_PrimaryButtonClick;
        dialog.DataContext = this;

        Items = new List<PatientModel>();
        ShowAddFrom = showAdd;

        this.ValidationRule(vm => vm.Model.RaleName, name => !string.IsNullOrEmpty(name), "用户名不能为空");
        this.ValidationRule(vm => vm.Model.PhoneNumber, name => !string.IsNullOrEmpty(name), "手机号不能为空");


        if (user != null)
        {
            Model = user;
        }
        else
        {
            Model = new PatientModel()
            {
                Code = CommonHelper.GetBillNumber("S", 0),
                RaleName = CommonHelper.GenerateStr(8),
                PhoneNumber = $"1300292{CommonHelper.Number(4, false)}"
            };
        }

        this.WhenActivated((CompositeDisposable disposables) =>
        {
            _cts = new CancellationTokenSource();
            //加载数据
            LoadDataCommand.Execute(_cts.Token)
                      .Subscribe()
                      .DisposeWith(disposables);

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
            Items.Clear();

            var rootFolder = await _virtualFolderService.GetRootFolder();
            var users = await _patientService.GetPatients();
            await Dispatcher.UIThread.InvokeAsync(() =>
            {
                //获取虚拟根目录
                var _mapper = Locator.Current.GetService<IMapper>();
                RootFolder = _mapper.Map<VirtualFolderModel>(rootFolder);

                if (users != null && users.Count > 0)
                    Items = new List<PatientModel>(users);

            });
        }, token);
    }

    /// <summary>
    /// 确认
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="args"></param>
    private void Dialog_PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
    {
        if (string.IsNullOrEmpty(Model.RaleName))
        {
            MessageBox("用户名不能为空");
            return;
        }

        if (string.IsNullOrEmpty(Model.PhoneNumber))
        {
            MessageBox("手机号不能为空");
            return;
        }

        this.IsValid().Subscribe(async s =>
        {
            if (s)
            {
                await Dispatcher.UIThread.InvokeAsync(async () =>
                {
                    try
                    {
                        var user = Globals.CurrentUser;
                        Model.CreatedBy = user.Id;
                        Model.ModifiedBy = user.Id;

                        var folderName = $"{Model.Code}_{Model.RaleName}";
                        if (SelectedPatientItem == null)
                        {
                            //创建项目文件夹
                            Model.StorePath = folderName;

                            var create = await _physicalFolderService
                            .AddFolder(folderName,
                            RootFolder.Id,
                            RootFolder.PhysicalFolderId,
                            user.Id);

                            if (create != null)
                            {
                                if (create.Succeeded)
                                {

                                    //虚拟目录信息
                                    var vfim = create.Data;
                                    if (vfim != null)
                                    {
                                        Model.VirtualFolderId = vfim.Id;
                                        Model.PhysicalFolderId = vfim.PhysicalFolderId;

                                        //创建缩略目录
                                        var thumbnail = await _physicalFolderService
                                        .AddFolder("Thumbnails",
                                        vfim.Id,
                                        vfim.PhysicalFolderId,
                                        user.Id);

                                        if (thumbnail.Succeeded)
                                        {
                                            Model.VirtualThumbnailFolderId = thumbnail.Data?.Id ?? Guid.Empty;
                                            Model.PhysicalThumbnailFolderId = thumbnail.Data?.PhysicalFolderId ?? Guid.Empty;
                                        }
                                    }

                                    //添加项目
                                    var result = await _patientService.AddAsync(Model);
                                    if (result.Succeeded)
                                    {
                                        Globals.CurrentPatient = Model;
                                        MessageBox("添加成功！");
                                        dialog.Hide(ContentDialogResult.Primary);
                                    }
                                }
                                else
                                {
                                    MessageBox("目录创建失败！");
                                }
                            }
                        }
                        else
                        {
                            Model = SelectedPatientItem;
                            folderName = $"{Model.Code}_{Model.RaleName}";

                            //如果项目目录已经存在时
                            var vf = await _virtualFolderService.GetVirtualFolder(folderName);
                            if (vf != null)
                            {
                                Model.VirtualFolderId = vf.Id;
                                Model.PhysicalFolderId = vf.PhysicalFolderId;

                                //缩略目录是否存在
                                var vtf = await _virtualFolderService.GetVirtualFolder("Thumbnails", vf.Id);
                                if (vtf == null)
                                {
                                    //创建缩略目录
                                    var thumbnail = await _physicalFolderService
                                    .AddFolder("Thumbnails",
                                    vf.Id,
                                    vf.PhysicalFolderId,
                                    user.Id);

                                    if (thumbnail.Succeeded)
                                    {
                                        Model.VirtualThumbnailFolderId = thumbnail.Data?.Id ?? Guid.Empty;
                                        Model.PhysicalThumbnailFolderId = thumbnail.Data?.PhysicalFolderId ?? Guid.Empty;
                                    }
                                }
                                else
                                {
                                    Model.VirtualThumbnailFolderId = vtf.Id;
                                    Model.PhysicalThumbnailFolderId = vtf.PhysicalFolderId;
                                }
                            }
                            //如果患者的物理目录和虚拟目录不存在时（因为重建目录，如果存在物理删除目录，且没有级联更新患者信息时，资料的保存上传就会受影响）
                            else
                            {
                                var pdir = Model.PhysicalFolderId;
                                var vdir = Model.VirtualFolderId;

                                //重新重建
                                var create = await _physicalFolderService
                                .AddFolder(folderName,
                                RootFolder.Id,
                                RootFolder.PhysicalFolderId,
                                user.Id);

                                if (create != null && create.Succeeded)
                                {
                                    //虚拟目录信息
                                    var vfim = create.Data;
                                    if (vfim != null)
                                    {
                                        Model.VirtualFolderId = vfim.Id;
                                        Model.PhysicalFolderId = vfim.PhysicalFolderId;

                                        //创建缩略目录
                                        var thumbnail = await _physicalFolderService
                                        .AddFolder("Thumbnails",
                                        vfim.Id,
                                        vfim.PhysicalFolderId,
                                        user.Id);

                                        if (thumbnail != null)
                                        {
                                            if (thumbnail.Succeeded)
                                            {
                                                Model.VirtualThumbnailFolderId = thumbnail.Data?.Id ?? Guid.Empty;
                                                Model.PhysicalThumbnailFolderId = thumbnail.Data?.PhysicalFolderId ?? Guid.Empty;
                                            }

                                            //更新项目
                                            var result = await _patientService.UpdateAsync(Model);
                                            if (result.Succeeded)
                                            {
                                                Globals.CurrentPatient = Model;
                                            }
                                        }
                                    }
                                }
                            }

                            SelectedPatientItem.StorePath = folderName;
                            SelectedPatientItem.VirtualThumbnailFolderId = Model.VirtualThumbnailFolderId;
                            SelectedPatientItem.PhysicalThumbnailFolderId = Model.PhysicalThumbnailFolderId;
                            Globals.CurrentPatient = SelectedPatientItem;
                            dialog.Hide(ContentDialogResult.Primary);
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox(ex.ToString());
                    }
                }, DispatcherPriority.Background);
            }
        });

        args.Cancel = true;
    }

    private void DialogOnClosed(ContentDialog sender, ContentDialogClosedEventArgs args)
    {
        dialog.Closed -= DialogOnClosed;
        dialog.PrimaryButtonClick -= Dialog_PrimaryButtonClick;
    }
}
