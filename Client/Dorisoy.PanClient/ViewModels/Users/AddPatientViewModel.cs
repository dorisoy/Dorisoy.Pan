using AutoMapper;
using ReactiveUI.Validation.Extensions;
using ValidationContext = ReactiveUI.Validation.Contexts.ValidationContext;

namespace Dorisoy.PanClient.ViewModels;

[View(typeof(AddPatientView))]
public class AddPatientViewModel : ViewModelBase, IValidatableViewModel
{
    private readonly IUsersService _employeesService;
    private readonly IDepartmentService _departmentService;
    private readonly IVirtualFolderService _virtualFolderService;
    private readonly IPhysicalFolderService _physicalFolderService;

    ValidationContext IValidatableViewModel.ValidationContext { get; } = new ValidationContext();
    private readonly ContentDialog dialog;

    [Reactive] public PatientModel Model { get; set; }
    [Reactive] public string UsernameValidation { get; set; }
    [Reactive] public ObservableCollection<PatientModel> Items { get; set; }
    [Reactive] public PatientModel SelectedPatientItem { get; set; }

    [Reactive] public List<DepartmentModel> Departments { get; set; }
    [Reactive] public DepartmentModel SelectDepartment { get; set; }
    [Reactive] public VirtualFolderModel RootFolder { get; set; }
    [Reactive] public bool ShowAddFrom { get; set; }

    public AddPatientViewModel(ContentDialog dialog, PatientModel user = null, bool showAdd = true) : base()
    {
        if (dialog is null)
            throw new ArgumentNullException(nameof(dialog));

        _departmentService = Locator.Current.GetService<IDepartmentService>();
        _employeesService = Locator.Current.GetService<IUsersService>();
        _virtualFolderService = Locator.Current.GetService<IVirtualFolderService>();
        _physicalFolderService = Locator.Current.GetService<IPhysicalFolderService>();

        this.dialog = dialog;
        dialog.Closed += DialogOnClosed;
        dialog.PrimaryButtonClick += Dialog_PrimaryButtonClick;

        Items = new ObservableCollection<PatientModel>();
        ShowAddFrom = showAdd;

        this.ValidationRule(vm => vm.Model.RaleName, name => !string.IsNullOrEmpty(name), "用户名不能为空");

        //this.ValidationRule(vm => vm.Model.RaleName, name => _employeesService.UsernameIsFree(user?.Id ?? Guid.Empty, name), "用户名被其他用户占用");

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
            RxApp.MainThreadScheduler.Schedule(LoadData)
            .DisposeWith(disposables);
        });
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

        //if (_employeesService.UsernameIsFree(Model?.Id ?? Guid.Empty, Model.RaleName))
        //{
        //    MessageBox("名称已被其他占用");
        //    return;
        //}

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

                            }
                            else
                            {
                                MessageBox("目录创建失败！");
                            }
                        }
                        else
                        {
                            Model = SelectedPatientItem;
                            folderName = $"{Model.Code}_{Model.RaleName}";
                            //项目目录已经存在时
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

    private async void LoadData()
    {
        await Dispatcher.UIThread.InvokeAsync(async () =>
        {
            //获取虚拟根目录
            var _mapper = Locator.Current.GetService<IMapper>();
            var rootFolder = await _virtualFolderService.GetRootFolder();
            RootFolder = _mapper.Map<VirtualFolderModel>(rootFolder);


        }, DispatcherPriority.Background);
    }

    private void DialogOnClosed(ContentDialog sender, ContentDialogClosedEventArgs args)
    {
        dialog.Closed -= DialogOnClosed;
        dialog.PrimaryButtonClick -= Dialog_PrimaryButtonClick;
    }

    protected override void DisposeManaged()
    {
    }

    protected override void DisposeUnmanaged()
    {

    }
}
