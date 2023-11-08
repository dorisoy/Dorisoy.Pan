namespace Dorisoy.PanClient.ViewModels;

[View(typeof(UserPage))]
public class UserPageViewModel : MainPageViewModelBase, IValidatableViewModel, IRoutableViewModel
{
    public string[] Columns = new string[] { "PhoneNumber", "RaleName", "Sex", "UserName", "Email" };
    [Reactive] public ObservableCollection<UserModel> Items { get; set; }
    [Reactive] public ObservableCollection<TreeResultModel<Guid, DepartmentModel>> Departments { get; set; }
    [Reactive] public TreeResultModel<Guid, DepartmentModel> SelectDepartment { get; set; }
    [Reactive] public int GridHeight { get; set; }
    [Reactive] public UserModel SelectedUser { get; set; }

    private readonly IUsersService _employeesService;
    private readonly IDepartmentService _departmentService;
    private readonly IRoleService _roleService;

    public ReactiveCommand<Unit, Unit> AddUser { get; }
    public ReactiveCommand<UserModel, Unit> EditUser { get; }
    public ReactiveCommand<UserModel, Unit> DeleteUser { get; }
    public ReactiveCommand<UserModel, Unit> ManageRoles { get; }
    public ReactiveCommand<UserModel, Unit> SendMessage { get; }
    public ReactiveCommand<Unit, Unit> ExportUser { get; }

    public ReactiveCommand<Unit, Unit> AddDeptment { get; }
    public ReactiveCommand<DepartmentModel, Unit> DeleteDeptment { get; }
    public string UrlPathSegment { get; set; }

    public UserPageViewModel() : this(Locator.Current.GetService<IUsersService>(),
           Locator.Current.GetService<IDepartmentService>(),
           Locator.Current.GetService<IRoleService>())
    { }

    public UserPageViewModel(IUsersService employeesService,
        IDepartmentService departmentService,
        IRoleService roleService) : base()
    {
        _employeesService = employeesService;
        _departmentService = departmentService;
        _roleService = roleService;


        //添加用户
        AddUser = ReactiveCommand.CreateFromTask(async () =>
        {
            var auth = CheckAuthorize(Permissions.Users.Create);
            if (auth)
            {
                var dept = SelectDepartment;
                if (dept == null || dept.Id == Guid.Empty)
                {
                    MessageBox("请选择部门组织节点");
                    return;
                }

                var dialog = new ContentDialog()
                {
                    Title = "添加用户",
                    PrimaryButtonText = "确定",
                    CloseButtonText = "取消"
                };

                dialog.Content = new AddUserView()
                {
                    DataContext = new AddUserViewModel(dialog, dept.Item)
                };

                var ok = await dialog.ShowAsync();
                if (ok == ContentDialogResult.Primary)
                {
                    LoadData();
                }
            }
        });

        //编辑用户
        EditUser = ReactiveCommand.Create<UserModel>(async (item) =>
        {
            if (item.IsAdmin)
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
            dialog.Content = new AddUserView()
            {
                DataContext = new AddUserViewModel(dialog, item.Department, item)
            };

            var ok = await dialog.ShowAsync();
            if (ok == ContentDialogResult.Primary)
            {
                LoadData();
            }
        });

        //删除用户
        DeleteUser = ReactiveCommand.Create<UserModel>(async (item) =>
        {
            if (item.IsAdmin)
            {
                MessageBox("无操作权限！");
                return;
            }

            var ok = await ConfirmBox("你确定要删除用户吗？");
            if (ok == DialogResult.Primary)
            {
                await _employeesService.DeleteAsync(item);
                MessageBox("删除成功！");
                LoadData();
            }
        });

        //管理角色
        ManageRoles = ReactiveCommand.Create<UserModel>(async (item) =>
        {
            try
            {
                var isAdmin = Globals.CurrentUser.IsAdmin;
                if (item.IsAdmin)
                {
                    MessageBox("无操作权限！");
                    return;
                }

                var dialog = new ContentDialog()
                {
                    Title = "管理角色",
                    PrimaryButtonText = "确定",
                    CloseButtonText = "取消",
                    FullSizeDesired = true,
                };

                dialog.Content = new AddUserRolesView()
                {
                    DataContext = new AddUserRolesViewModel(dialog, item)
                };

                var ok = await dialog.ShowAsync();
                if (ok == ContentDialogResult.Primary)
                {
                    //LoadData();
                }
            }
            catch (Exception ex)
            {
                ex.Handle();
            }
        });

        //发送消息
        SendMessage = ReactiveCommand.Create<UserModel>((item) =>
        {
            try
            {
                //await _connection.SendAsync("SendDirectMessage", "SendDirectMessage", item.Id);
                //_echoClient.SendAsync($"SendDirectMessage:{item.Id}");
                //var endpoint = new IPEndPoint(IPAddress.Parse("192.168.0.137"), 3333);
                //_echoClient.SendAsync(endpoint, $"SendDirectMessage:{item.Id}");
            }
            catch (Exception ex)
            {
                ex.Handle();
            }
        });

        //添加部门
        AddDeptment = ReactiveCommand.CreateFromTask(async () =>
        {
            var department = new DepartmentModel()
            {
                Id = Guid.Empty,
                Name = "root",
                Parent = new(),
                ParentId = Guid.Empty,
            };

            if (Departments.Any())
            {
                var dept = SelectDepartment;
                if (dept == null || dept.Id == Guid.Empty)
                {
                    MessageBox("请选择组织机构");
                    return;
                }

                department = dept.Item;
            }

            var dialog = new ContentDialog()
            {
                Title = "添加部门",
                PrimaryButtonText = "确定",
                CloseButtonText = "取消"
            };

            dialog.Content = new AddDeptmentView()
            {
                DataContext = new AddDeptmentViewModel(dialog, department)
            };

            var ok = await dialog.ShowAsync();
            if (ok == ContentDialogResult.Primary)
            {
                LoadData();
            }
        });

        //删除部门
        DeleteDeptment = ReactiveCommand.Create<DepartmentModel>(async (item) =>
        {
            var dept = SelectDepartment;
            if (dept == null || dept.Id == Guid.Empty)
            {
                MessageBox("请选择组织机构");
                return;
            }

            var ok = await ConfirmBox("你确定要删除该部门吗？");
            if (ok == DialogResult.Primary)
            {
                await _departmentService.DeleteAsync(dept.Item);
                MessageBox("删除成功！");
                LoadData();
            }
        });

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

        //WhenActivated允许您注册要在ViewModel的视图被激活时调用的Func
        this.WhenActivated((CompositeDisposable disposables) =>
        {
            RxApp.MainThreadScheduler.Schedule(LoadData)
            .DisposeWith(disposables);
        });
    }


    /// <summary>
    ///加载数据
    /// </summary>
    private async void LoadData()
    {
        Departments = new ObservableCollection<TreeResultModel<Guid, DepartmentModel>>();
        Items = new ObservableCollection<UserModel>();

        await Task.Run(async () =>
        {
            var root = new TreeResultModel<DepartmentModel> { Name = "root", ParentId = Guid.Empty };
            var depts = await _departmentService.GetDepartments();
            if (depts != null && depts.Any())
                ResolveTree(depts, root);

            var users = await _employeesService.GetUsers();
            //var users = await _employeesService.GetAllUsers();
            await Dispatcher.UIThread.InvokeAsync(() =>
            {
                Departments.Add(root.Children);

                if (users != null && users.Any())
                    Items.Add(users);

                GridHeight = 10 + (users.Count * 50);
            });
        });
    }

    /// <summary>
    /// 解析组织树
    /// </summary>
    /// <param name="all"></param>
    /// <param name="parent"></param>
    private void ResolveTree(IList<DepartmentModel> all, TreeResultModel<DepartmentModel> parent)
    {
        foreach (var dept in all.Where(m => m.ParentId == parent.Id).OrderBy(m => m.Sort))
        {
            var child = new TreeResultModel<DepartmentModel>
            {
                Id = dept.Id,
                Name = dept.Name,
                IsExpanded = true,
                Item = dept
            };
            ResolveTree(all, child);
            parent.Children.Add(child);
        }
    }

    protected override void DisposeManaged()
    {
    }

    protected override void DisposeUnmanaged()
    {
    }
}

