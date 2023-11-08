using ReactiveUI.Validation.Extensions;
using Dorisoy.PanClient.Common;
using ValidationContext = ReactiveUI.Validation.Contexts.ValidationContext;
using Dorisoy.PanClient.Commands;

namespace Dorisoy.PanClient.ViewModels;

[View(typeof(AddUserView))]
public class AddUserViewModel : ViewModelBase, IValidatableViewModel
{
    private readonly IUsersService _employeesService;
    private readonly IDepartmentService _departmentService;
    private readonly ISettingsProvider<AppSettings> _settingsProvider;
    ValidationContext IValidatableViewModel.ValidationContext { get; } = new ValidationContext();
    private readonly ContentDialog _dialog;

    [Reactive] public UserModel Model { get; set; }
    [Reactive] public string UsernameValidation { get; set; }
    [Reactive] public string PasswordValidation { get; set; }
    [Reactive] public List<DepartmentModel> Departments { get; set; }
    [Reactive] public DepartmentModel SelectDepartment { get; set; }


    public AddUserViewModel(ContentDialog dialog, DepartmentModel department, UserModel user = null) : base()
    {
        if (dialog is null)
            throw new ArgumentNullException(nameof(dialog));

        _departmentService = Locator.Current.GetService<IDepartmentService>();
        _employeesService = Locator.Current.GetService<IUsersService>();
        _settingsProvider = Locator.Current.GetService<ISettingsProvider<AppSettings>>();


        this._dialog = dialog;
        dialog.Closed += DialogOnClosed;
        dialog.PrimaryButtonClick += Dialog_PrimaryButtonClick;


        this.ValidationRule(vm => vm.Model.UserName, name => !string.IsNullOrEmpty(name), "用户名不能为空");
        this.ValidationRule(vm => vm.Model.UserName, name => _employeesService
        .UsernameIsFree(user?.Id ?? Guid.Empty, name), "用户名被其他用户占用");

        this.ValidationRule(vm => vm.Model.Password, password => !string.IsNullOrEmpty(password), "密码不能为空");
        this.ValidationRule(vm => vm.Model.PhoneNumber, name => !string.IsNullOrEmpty(name), "手机号不能为空");



        if (user != null)
        {
            Model = user;
        }
        else
        {
            Model = new UserModel()
            {
                UserName = CommonHelper.GenerateStr(8),
                Email = $"{CommonHelper.GenerateStr(5)}@sinol.com",
                PhoneNumber = $"1300292{CommonHelper.Number(4, false)}",
                Password = "123456",
            };
        }

        if (department != null)
        {
            Model.DepartmentId = department.Id;
            SelectDepartment = department;
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
        if (string.IsNullOrEmpty(Model.UserName))
        {
            MessageBox("用户名不能为空");
            args.Cancel = false;
        }

        if (Model.DepartmentId == Guid.Empty)
        {
            MessageBox("部门不能为空");
            args.Cancel = false;
        }

        if (string.IsNullOrEmpty(Model.RaleName))
        {
            MessageBox("姓名不能为空");
            args.Cancel = false;
        }

        if (string.IsNullOrEmpty(Model.Email))
        {
            MessageBox("邮箱不能为空");
            args.Cancel = false;
        }

        if (string.IsNullOrEmpty(Model.Password))
        {
            MessageBox("密码不能为空");
            args.Cancel = false;
        }

        if (_employeesService.UsernameIsFree(Model?.Id ?? Guid.Empty, Model.UserName))
        {
            MessageBox("用户名被其他用户占用");
            args.Cancel = false;
        }

        if (string.IsNullOrEmpty(Model.PhoneNumber))
        {
            MessageBox("手机号不能为空");
            args.Cancel = false;
        }

        this.IsValid().Subscribe(async s =>
        {
            var user = Globals.CurrentUser;

            Model.IsAdmin = false;
            Model.CreatedBy = user.Id;
            Model.ModifiedBy = user.Id;
            Model.DepartmentId = SelectDepartment.Id;

            if (Model.Id != Guid.Empty)
            {
                var cmd = new UpdateUserCommand()
                {
                    Id = Model.Id,
                    DepartmentId = Model.DepartmentId,
                    Email = Model.Email,
                    RaleName = Model.RaleName,
                    PhoneNumber = Model.PhoneNumber,
                    IsActive = true,
                    Address = Model.Address,
                    IsAdmin = false,
                    UserClaims = new UserClaimDto()
                };

                var loading = new LoadingDialog(new Spinner());
                await loading.ShowAsync(async (close) =>
                {
                    var result = await _employeesService.UpdateUser(Model.Id, cmd);
                    if (result != null)
                    {
                        MessageBox("编辑成功！");
                        _dialog.Hide(ContentDialogResult.Primary);
                    }
                    else
                    {
                        MessageBox("编辑失败！");
                        _dialog.Hide(ContentDialogResult.Primary);
                    }
                    close.Invoke();
                });
            }
            else
            {
                //var result = await _employeesService.AddAsync(Model);
                //if (result.Succeeded)
                //{
                //    MessageBox("添加成功！");
                //    _dialog.Hide(ContentDialogResult.Primary);
                //}

                var cmd = new AddUserCommand()
                {
                    UserName = Model.UserName,
                    DepartmentId = Model.DepartmentId,
                    RaleName = Model.RaleName,
                    Email = Model.Email,
                    Sex = Model.Sex.ToString(),
                    Password = Model.Password,
                    PhoneNumber = Model.PhoneNumber,
                    IsActive = true,
                    Address = Model.Address,
                    IsAdmin = false,
                    UserClaims = new UserClaimDto()
                };

                var loading = new LoadingDialog(new Spinner());
                await loading.ShowAsync(async (close) =>
                {
                    var result = await _employeesService.AddUser(cmd);
                    if (result != null)
                    {
                        MessageBox("添加成功！");
                        _dialog.Hide(ContentDialogResult.Primary);
                    }
                    else
                    {
                        MessageBox("添加失败！");
                        _dialog.Hide(ContentDialogResult.Primary);
                    }
                    close.Invoke();
                });
            }

            //创建用户目录
            if (Model.Id != Guid.Empty)
            {
                var folder = _settingsProvider.Settings.DocumentPath;
                var documentPath = Path.Combine(folder, user.Id.ToString());
                if (!Directory.Exists($"{documentPath}"))
                    Directory.CreateDirectory($"{documentPath}");
            }
        });

        args.Cancel = true;
    }

    private async void LoadData()
    {
        var depts = await _departmentService.GetDepartments();
        if (depts != null && depts.Any())
            Departments = depts;
        else
        {
            Departments = new List<DepartmentModel>() { new DepartmentModel() { Id = Guid.Empty, Name = "root" } };
        }
    }

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
