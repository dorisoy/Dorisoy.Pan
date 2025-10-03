using ValidationContext = ReactiveUI.Validation.Contexts.ValidationContext;


namespace Dorisoy.PanClient.ViewModels;

//[View(typeof(AddUserRolesView))]
public class AddUserRolesViewModel : ViewModelBase, IValidatableViewModel
{
    private readonly IUsersService _userService;
    private readonly IDepartmentService _departmentService;
    private readonly IPatientService _patientService;
    private readonly IRoleService _roleService;

    ValidationContext IValidatableViewModel.ValidationContext { get; } = new ValidationContext();

    private readonly ContentDialog _dialog;

    /// <summary>
    /// 选选择角色
    /// </summary>
    [Reactive] public RoleModel SelectedItem { get; set; }
    [Reactive] public string Title { get; set; }

    public UserModel Model { get; set; }


    private ReadOnlyObservableCollection<UserRoleModel> _items;
    public ReadOnlyObservableCollection<UserRoleModel> Items => _items;

    private SourceCache<UserRoleModel, string> _userRoles;
    private IObservable<IChangeSet<UserRoleModel, string>> UserRoleConnect() => _userRoles.Connect();

    public AddUserRolesViewModel(ContentDialog dialog, UserModel user = null) : base()
    {
        if (dialog is null)
            throw new ArgumentNullException(nameof(dialog));

        _departmentService = Locator.Current.GetService<IDepartmentService>();
        _userService = Locator.Current.GetService<IUsersService>();
        _patientService = Locator.Current.GetService<IPatientService>();
        _roleService = Locator.Current.GetService<IRoleService>();

        _userRoles = new SourceCache<UserRoleModel, string>(e => e.RoleName);

        this._dialog = dialog;
        dialog.Closed += DialogOnClosed;
        dialog.PrimaryButtonClick += Dialog_PrimaryButtonClick;

        this.Title = $"为[{user.RaleName}]分配角色";

        Model = user;

        this.WhenActivated((CompositeDisposable disposables) =>
        {
            //使用 WhenAnyValue 来监视属性的变化
            UserRoleConnect()
            .ObserveOn(RxApp.MainThreadScheduler) // 切换到主线程
            .Bind(out _items)
            .Subscribe(s =>
            {
                // 在主线程中更新 UI
                System.Diagnostics.Debug.Print($"{s.Count}");
            })
            .DisposeWith(disposables);

            //通过 Schedule 方法来执行更新操作
            RxApp.MainThreadScheduler.Schedule(LoadData)
            .DisposeWith(disposables);
        });
    }

    /// <summary>
    /// 加载用户角色
    /// </summary>
    private async void LoadData()
    {
        await Dispatcher.UIThread.InvokeAsync(async () =>
        {
            var response = await _userService.GetRolesAsync(Model.Id);
            var userRolesList = response.Data.UserRoles;
            _userRoles.AddOrUpdate(userRolesList);

        }, DispatcherPriority.Background);
    }

    /// <summary>
    /// 确认
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="args"></param>
    private void Dialog_PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
    {

        this.IsValid().Subscribe(async s =>
        {
            var request = new UpdateUserRolesRequest()
            {
                UserId = Model.Id,
                UserRoles = Items
            };
            var result = await _userService.UpdateRolesAsync(request);
            if (result.Succeeded)
            {
                MessageBox("添加成功！");
                _dialog.Hide(ContentDialogResult.Primary);
            }
            else
            {
                MessageBox(result.Messages[0]);
                _dialog.Hide(ContentDialogResult.Primary);
            }
        });

        args.Cancel = true;
    }

    private void DialogOnClosed(ContentDialog sender, ContentDialogClosedEventArgs args)
    {
        _dialog.Closed -= DialogOnClosed;
        _dialog.PrimaryButtonClick -= Dialog_PrimaryButtonClick;
    }
}
