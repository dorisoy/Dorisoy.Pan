using ReactiveUI.Validation.Extensions;
using ValidationContext = ReactiveUI.Validation.Contexts.ValidationContext;

namespace Dorisoy.PanClient.ViewModels;

[View(typeof(AddDeptmentView))]
public class AddDeptmentViewModel : ViewModelBase, IValidatableViewModel
{
    ValidationContext IValidatableViewModel.ValidationContext { get; } = new ValidationContext();

    private readonly ContentDialog dialog;
    private readonly IDepartmentService _departmentService;

    [Reactive] public DepartmentModel Model { get; set; }


    [Reactive] public string NameValidation { get; set; }
    [Reactive] public Guid ParentValidation { get; set; }
    [Reactive] public List<DepartmentModel> Departments { get; set; }
    [Reactive] public DepartmentModel SelectDepartment { get; set; }

    public AddDeptmentViewModel(ContentDialog dialog, DepartmentModel department) : base()
    {
        if (dialog is null)
            throw new ArgumentNullException(nameof(dialog));

        _departmentService = Locator.Current.GetService<IDepartmentService>();

        this.dialog = dialog;
        dialog.Closed += DialogOnClosed;
        dialog.PrimaryButtonClick += Dialog_PrimaryButtonClick;

        Model = new DepartmentModel();

        if (department != null)
        {
            Model.Parent = department;
            Model.ParentId = department.Id;
            SelectDepartment = department;
        }

        this.ValidationRule(vm => vm.Model.Name, s => !string.IsNullOrEmpty(s), "部门名称不能为空");
        this.ValidationRule(vm => vm.Model.Name, s => _departmentService.DepartmentNameIsFree(s), "部门名称被其他用户占用");



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

        if (string.IsNullOrEmpty(Model.Name))
        {
            MessageBox("名称不能为空");
            return;
        }

        if (_departmentService.DepartmentNameIsFree(Model.Name))
        {
            MessageBox("部门名称被其他用户占用");
            return;
        }

        this.IsValid().Subscribe(async s =>
        {
            if (s)
            {
                var user = Globals.CurrentUser;
                Model.Code = "";
                Model.FullPath = "";
                Model.Parent = SelectDepartment;
                Model.ParentId = SelectDepartment.Id;
                Model.CreatedDate = DateTime.Now;
                Model.ModifiedDate = DateTime.Now;
                Model.CreatedBy = user.Id;
                Model.ModifiedBy = user.Id;
                var result = await _departmentService.AddAsync(Model);
                if (result.Succeeded)
                {
                    MessageBox("操作成功！");
                    dialog.Hide(ContentDialogResult.Primary);
                }
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
