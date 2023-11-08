using Avalonia;
using Avalonia.Reactive;
using ReactiveUI.Validation.Extensions;
using ValidationContext = ReactiveUI.Validation.Contexts.ValidationContext;


namespace Dorisoy.PanClient.ViewModels;

[View(typeof(AddUserRolesView))]
public class AddUserRolesViewModel : ViewModelBase, IValidatableViewModel
{
    private readonly IUsersService _userService;
    private readonly IDepartmentService _departmentService;
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
        _roleService = Locator.Current.GetService<IRoleService>();

        _userRoles = new SourceCache<UserRoleModel, string>(e => e.RoleName);

        this._dialog = dialog;
        dialog.Closed += DialogOnClosed;
        dialog.PrimaryButtonClick += Dialog_PrimaryButtonClick;

        this.Title = $"为[{user.RaleName}]分配角色";

        Model = user;

        this.WhenActivated((CompositeDisposable disposables) =>
        {
            UserRoleConnect()
            .ObserveOn(RxApp.MainThreadScheduler)
            .Bind(out _items)
            .Subscribe(s =>
            {
                System.Diagnostics.Debug.Print($"{s.Count}");
            })
            .DisposeWith(disposables);

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

    //private async void LoadData()
    //{
    //    //Items = new ObservableCollection<PatientModel>();
    //    //var users = await _patientService.GetPatients();
    //    //await Dispatcher.UIThread.InvokeAsync(() =>
    //    //{
    //    //    if (users != null && users.Any())
    //    //        Items.Add(users);
    //    //}, DispatcherPriority.Background);
    //}

    private void DialogOnClosed(ContentDialog sender, ContentDialogClosedEventArgs args)
    {
        _dialog.Closed -= DialogOnClosed;
        _dialog.PrimaryButtonClick -= Dialog_PrimaryButtonClick;
    }

    protected override void DisposeManaged()
    {
    }

    protected override void DisposeUnmanaged()
    {

    }
}
