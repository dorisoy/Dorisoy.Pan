using ValidationContext = ReactiveUI.Validation.Contexts.ValidationContext;

namespace Dorisoy.Pan.ViewModels;


public class AddDeptmentViewModel : ViewModelBase, IValidatableViewModel
{
    ValidationContext IValidatableViewModel.ValidationContext { get; } = new ValidationContext();
    private CancellationTokenSource _cts;
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
        Departments = [new DepartmentModel() { Id = Guid.Empty, Name = "root" }];

        if (department != null)
        {
            Model.Parent = department;
            Model.ParentId = department.Id;
            SelectDepartment = department;
        }

        this.ValidationRule(vm => vm.Model.Name, s => !string.IsNullOrEmpty(s), "部门名称不能为空");
        //this.ValidationRule(vm => vm.Model.Name, s => _departmentService.DepartmentNameIsFree(s), "部门名称被其他用户占用");


        this.WhenActivated((CompositeDisposable disposables) =>
        {
            _cts = new CancellationTokenSource();

            LoadDataCommand.Execute(_cts.Token).Subscribe()
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
            var depts = await _departmentService.GetDepartments();
            await Dispatcher.UIThread.InvokeAsync(() =>
            {
                if (depts != null && depts.Count > 0)
                    Departments = depts;
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

        if (string.IsNullOrEmpty(Model.Name))
        {
            MessageBox("名称不能为空");
            return;
        }

        //if (_departmentService.DepartmentNameIsFree(Model.Name))
        //{
        //    MessageBox("部门名称被其他用户占用");
        //    return;
        //}

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

   

    private void DialogOnClosed(ContentDialog sender, ContentDialogClosedEventArgs args)
    {
        dialog.Closed -= DialogOnClosed;
        dialog.PrimaryButtonClick -= Dialog_PrimaryButtonClick;
    }
}
