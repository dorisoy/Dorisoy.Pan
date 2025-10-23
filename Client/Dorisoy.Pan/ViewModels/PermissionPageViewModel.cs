using Spinner = Dorisoy.Pan.Views.Spinner;

namespace Dorisoy.Pan.ViewModels;


//[View(typeof(PermissionPage))]
public class PermissionPageViewModel : MainPageViewModelBase, IValidatableViewModel
{
    private ReadOnlyObservableCollection<RoleModel> _items;
    public ReadOnlyObservableCollection<RoleModel> RolesItems => _items;
    [Reactive] public RoleModel SelectedItem { get; set; }



    private ReadOnlyObservableCollection<PermissionRoleClaim> _groupedRoleClaims;
    public ReadOnlyObservableCollection<PermissionRoleClaim> GroupedRoleClaims => _groupedRoleClaims;

    private readonly SourceCache<PermissionRoleClaim, Guid> _roleClaims;
    private IObservable<IChangeSet<PermissionRoleClaim, Guid>> RoleClaimsConnect() => _roleClaims.Connect();
    [Reactive] public PermissionRoleClaim SelectedRoleClaimsItem { get; set; }
    [Reactive] public int SelectedIndex { get; set; }

    private readonly IUsersService _usersService;
    private readonly IDepartmentService _departmentService;
    private readonly IPatientService _patientService;
    private readonly IRoleService _roleService;

    public ReactiveCommand<Unit, Unit> UpdatePermissionCommand { get; }
    private PermissionResponse _model;

    public PermissionPageViewModel(
      IUsersService usersService,
      IDepartmentService departmentService,
      IPatientService patientService,
      IRoleService roleService) : base()
    {
        _usersService = usersService;
        _departmentService = departmentService;
        _patientService = patientService;
        _roleService = roleService;

        _roleClaims = new SourceCache<PermissionRoleClaim, Guid>(e => e.Id);

        //打印页面
        PrintPage = ReactiveCommand.Create(() => Printing(this.PageName));

        //更新权限
        UpdatePermissionCommand = ReactiveCommand.CreateFromTask(async () =>
        {
            if (SelectedItem == null || SelectedItem.Id == Guid.Empty)
            {
                MessageBox("请选择要绑定的角色！");
                return;
            }

            if (SelectedRoleClaimsItem != null)
            {
                var alls = new List<RoleClaimResponseModel>();
                foreach (var gr in GroupedRoleClaims)
                {
                    alls.AddRange(gr.AllRoleClaimsInGroup);
                }

                var selectRoleClaims = alls.Where(s => s.Selected).ToList();

                var request = new PermissionRequest()
                {
                    RoleId = SelectedItem.Id,
                    RoleClaims = selectRoleClaims
                };

                var loading = new LoadingDialog(new Spinner());
                await loading.ShowAsync(async (close) =>
                {
                    var result = await _roleService.UpdatePermissionsAsync(request);
                    if (result.Succeeded)
                    {
                        MessageBox("权限已经更新...");
                    }
                    else

                    {
                        MessageBox("权限更新失败！");
                    }

                    close.Invoke();
                });
            }
        });

        //选择角色
        this.WhenAnyValue(s => s.SelectedItem)
            .WhereNotNull()
            .Subscribe(s =>
            {
                LoadData();
            });


        this.WhenActivated((CompositeDisposable disposables) =>
        {
            //角色
            _roleService
             .Connect()
             .ObserveOn(RxApp.MainThreadScheduler)
             .Bind(out _items)
             .Subscribe(s => { })
             .DisposeWith(disposables);

            //权限
            RoleClaimsConnect()
            .ObserveOn(RxApp.MainThreadScheduler)
            .Bind(out _groupedRoleClaims)
            .Subscribe(s => { })
            .DisposeWith(disposables);

            //
            RxApp.MainThreadScheduler.Schedule(LoadData)
            .DisposeWith(disposables);
        });



        // this.MyList = new ObservableCollection<string>();
        // var backgroundScheduler = new EventLoopScheduler();

        // 这个问题解决了在这里有合适的调度程序
        // var currentScheduler = RxApp.MainThreadScheduler; 

        // Observable
        //    .Interval(TimeSpan.FromMilliseconds(250), backgroundScheduler)
        //    .Take(20)
        //    .Select(o => string.Format("Title {0}", o))
        //    .ObserveOn(currentScheduler)
        //    .Subscribe(o => { 
        //        this.MyList.Add(o); 
        //    });

    }

    // public ObservableCollection<string> MyList { get; set; }

    /// <summary>
    /// 加载角色权限
    /// </summary>
    private async void LoadData()
    {
        await Task.Run(async () =>
        {
            await Dispatcher.UIThread.InvokeAsync(async () =>
            {
                _roleClaims.Clear();

                var roleId = Guid.Empty;
                if (SelectedItem != null)
                    roleId = SelectedItem.Id;

                var result = await _roleService.GetAllPermissionsAsync(roleId);
                if (result.Succeeded)
                {
                    _model = result.Data;

                    //分組
                    var grcs = new Dictionary<string, List<RoleClaimResponseModel>>
                    {
                        { "全部", _model.RoleClaims }
                    };

                    foreach (var claim in _model.RoleClaims)
                    {
                        if (grcs.ContainsKey(claim.Group))
                        {
                            grcs[claim.Group].Add(claim);
                        }
                        else
                        {
                            grcs.Add(claim.Group, new List<RoleClaimResponseModel> { claim });
                        }
                    }

                    if (_model != null)
                    {
                        var roleClaims = new List<PermissionRoleClaim>();
                        foreach (var group in grcs.Keys)
                        {
                            var selectedRoleClaimsInGroup = grcs[group].Where(c => c.Selected).ToList();
                            var allRoleClaimsInGroup = grcs[group].ToList();

                            roleClaims.Add(new PermissionRoleClaim()
                            {
                                Id = Guid.NewGuid(),
                                Group = group,
                                SelectCount = allRoleClaimsInGroup.Count,
                                Count = allRoleClaimsInGroup.Count,
                                AllRoleClaimsInGroup = allRoleClaimsInGroup,
                                BadgeData = $"{group} {selectedRoleClaimsInGroup.Count}/{allRoleClaimsInGroup.Count}"
                            });
                        }

                        _roleClaims.AddOrUpdate(roleClaims);
                    }
                }

                SelectedIndex = 0;

            }, DispatcherPriority.Background);
        });
    }
}
