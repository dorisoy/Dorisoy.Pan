namespace Dorisoy.PanClient.ViewModels;

//[View(typeof(RolePage))]
public class RolePageViewModel : MainPageViewModelBase, IValidatableViewModel
{
    private readonly IUsersService _usersService;
    private readonly IDepartmentService _departmentService;
    private readonly IPatientService _patientService;
    private readonly IRoleService _roleService;

    public string[] Columns = new string[] { "ID", "Name", "NormalizedName", "Description" };
    private ReadOnlyObservableCollection<RoleModel> _items;
    public ReadOnlyObservableCollection<RoleModel> Items => _items;
    public ReactiveCommand<RoleModel, Unit> ManagePermissions { get; }

    [Reactive] public RoleModel SelectedRole { get; set; }

    public ReactiveCommand<Unit, Unit> AddRole { get; }
    public ReactiveCommand<RoleModel, Unit> EditRole { get; }
    public ReactiveCommand<RoleModel, Unit> DeleteRole { get; }
    public ReactiveCommand<Unit, Unit> ExportRole { get; }
    public ReactiveCommand<Unit, Unit> PrintRole { get; }


    public RolePageViewModel(
        IUsersService usersService,
        IDepartmentService departmentService,
        IPatientService patientService,
        IRoleService roleService) : base()
    {
        _usersService = usersService;
        _departmentService = departmentService;
        _patientService = patientService;
        _roleService = roleService;

        //打印页面
        PrintPage = ReactiveCommand.Create(() => Printing(this.PageName));

        //导出CVS
        ExportCvs = ReactiveCommand.Create(() =>
        {
            Export(Items.ToList(), Columns, ExportType.CSV);
        });

        //导出Excel
        ExportExcel = ReactiveCommand.Create(() =>
        {
            Export(Items.ToList(), Columns, ExportType.Excel);
        });

        //管理权限 
        ManagePermissions = ReactiveCommand.Create<RoleModel>((item) =>
        {
            var isAdmin = Globals.CurrentUser.IsAdmin;
            if (item.NormalizedName == RoleConstants.AdministratorRole)
            {
                MessageBox("系统权限不能编辑！");
                return;
            }

            var vitem = new NavigationViewItem
            {
                Content = "权限",
                Tag = typeof(PermissionPage),
                Classes = { "DorisoyAppNav" }
            };

            //导航到权限管理
            if (Host is MainView mianView)
            {
                mianView.NavigateTo(typeof(PermissionPageViewModel));
            }
        });

        //添加角色
        AddRole = ReactiveCommand.CreateFromTask(async () =>
        {
            try
            {
                var dialog = new ContentDialog()
                {
                    Title = "添加角色",
                    PrimaryButtonText = "确定",
                    CloseButtonText = "取消"
                };
                dialog.Content = new AddRoleView()
                {
                    DataContext = new AddRoleViewModel(dialog)
                };

                var ok = await dialog.ShowAsync();
                if (ok == ContentDialogResult.Primary)
                {
                    //LoadData();
                }
            }
            catch (Exception ex)
            {
                MessageBox(ex.Message);
            }
        });

        //编辑用户
        EditRole = ReactiveCommand.Create<RoleModel>(async (item) =>
        {
            if (item.IsSystem)
            {
                MessageBox("无操作权限！");
                return;
            }

            var dialog = new ContentDialog()
            {
                Title = "编辑用户",
                PrimaryButtonText = "确定",
                CloseButtonText = "取消"
            };

            dialog.Content = new AddRoleView()
            {
                DataContext = new AddRoleViewModel(dialog, item)
            };

            var ok = await dialog.ShowAsync();
            if (ok == ContentDialogResult.Primary)
            {
                //LoadData();
            }
        });

        //删除角色
        DeleteRole = ReactiveCommand.Create<RoleModel>(async (item) =>
        {
            if (item.IsSystem)
            {
                MessageBox("无操作权限！");
                return;
            }

            var ok = await ConfirmBox("你确定要删除角色吗？");
            if (ok == DialogResult.Primary)
            {
                await _roleService.DeleteRoleAsync(item);
                MessageBox("删除成功！");
            }
        });

        this.WhenActivated((CompositeDisposable disposables) =>
        {
            _roleService
                       .Connect()
                       .ObserveOn(RxApp.MainThreadScheduler)
                       .Bind(out _items)
                       .Subscribe(s =>
                       {
                           var sss = s;
                       })
                       .DisposeWith(disposables);
        });

    }
}
