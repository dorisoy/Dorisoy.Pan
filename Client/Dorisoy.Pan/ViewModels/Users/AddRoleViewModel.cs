using ValidationContext = ReactiveUI.Validation.Contexts.ValidationContext;

namespace Dorisoy.PanClient.ViewModels;

//[View(typeof(AddRoleView))]
public class AddRoleViewModel : ViewModelBase, IValidatableViewModel
{
    private readonly IUsersService _usersService;
    private readonly IDepartmentService _departmentService;
    private readonly IRoleService _roleService;

    ValidationContext IValidatableViewModel.ValidationContext { get; } = new ValidationContext();
    private readonly ContentDialog _dialog;

    [Reactive] public RoleModel Model { get; set; }

    [Reactive] public string RoleNameValidation { get; set; }

    public record NormalizedName(string Name = "");


    [Reactive]
    public List<NormalizedName> NormalizedNames { get; set; } = new List<NormalizedName>()
    {
        new NormalizedName(RoleConstants.AdministratorRole),
        new NormalizedName(RoleConstants.DoctorRole),
        new NormalizedName(RoleConstants.BasicRole)
    };
    [Reactive] public NormalizedName SelectName { get; set; }

    public AddRoleViewModel(ContentDialog dialog, RoleModel role = null) : base()
    {
        if (dialog is null)
            throw new ArgumentNullException(nameof(dialog));

        _departmentService = Locator.Current.GetService<IDepartmentService>();
        _usersService = Locator.Current.GetService<IUsersService>();
        _roleService = Locator.Current.GetService<IRoleService>();

        this._dialog = dialog;
        dialog.Closed += DialogOnClosed;
        dialog.PrimaryButtonClick += Dialog_PrimaryButtonClick;

        this.ValidationRule(vm => vm.Model.Name, name => !string.IsNullOrEmpty(name), "名称不能为空");
        this.ValidationRule(vm => vm.Model.Name, name => _roleService.RoleNameIsFree(role?.Id ?? Guid.Empty, name), "名称其他占用");

        if (role != null)
        {
            Model = role;
            SelectName = new NormalizedName(role.NormalizedName);
        }
        else
        {
            Model = new RoleModel() { NormalizedName = "Other" };
        }
    }



    /// <summary>
    /// 确认
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="args"></param>
    private void Dialog_PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
    {

        if (string.IsNullOrEmpty(Model.Name))
        {
            MessageBox("名称不能为空");
            return;
        }

        if (_roleService.RoleNameIsFree(Model?.Id ?? Guid.Empty, Model.Name))
        {
            MessageBox("名称已被其他占用");
            return;
        }


        this.IsValid().Subscribe(async s =>
        {
            if (s)
            {
                var user = Globals.CurrentUser;

                Model.NormalizedName = SelectName?.Name ?? "Other";
                Model.CreatedBy = user.Id;
                Model.ModifiedBy = user.Id;

                if (Model.Id != Guid.Empty)
                {
                    var result = await _roleService.UpdateRoleAsync(Model);
                    if (result.Succeeded)
                    {
                        MessageBox("编辑成功！");
                        _dialog.Hide(ContentDialogResult.Primary);
                    }
                }
                else
                {
                    var result = await _roleService.AddRoleAsync(Model);
                    if (result.Succeeded)
                    {
                        MessageBox("添加成功！");
                        _dialog.Hide(ContentDialogResult.Primary);
                    }
                }
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
