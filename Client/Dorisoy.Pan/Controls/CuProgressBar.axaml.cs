namespace Dorisoy.PanClient.Controls;

public partial class CuProgressBar : UserControl
{
    private Avalonia.Controls.Shapes.Arc _progressBar;
    private TextBlock _progressBarText;

    /// <summary>
    /// 直接属性，这样外部控件就可以绑定到它，我们的控件的UI更新逻辑现在关联到这个新的属性。
    /// </summary>
    public static readonly DirectProperty<CuProgressBar, double> ProgressValueProperty =
           AvaloniaProperty.RegisterDirect<CuProgressBar, double>(
               nameof(ProgressValue),
               o => o.ProgressValue,
               (o, v) => o.ProgressValue = v);


    private double _progressValue;
    public double ProgressValue
    {
        get => _progressValue;
        set
        {
            SetAndRaise(ProgressValueProperty, ref _progressValue, value);
            // 现在直接更新进度而不传入参数
            UpdateProgress();
        }
    }

    public CuProgressBar()
    {
        InitializeComponent();

        // 绑定控件
        _progressBar = this.FindControl<Avalonia.Controls.Shapes.Arc>("ProgressBar");
        _progressBarText = this.FindControl<TextBlock>("ProgressBarText");

        //this.WhenActivated(disposables =>
        //{
        //    this.OneWayBind(this.ViewModel, vm => vm.ProgressValue, v => v.ProgressBarText.Text)
        //    .DisposeWith(disposables);

        //    this.WhenAnyValue(x => x.ViewModel.ProgressValue)
        //        .Subscribe(progress =>
        //        {
        //            UpdateProgress(progress);
        //        });
        //});
    }

    private void UpdateProgress()
    {
        var percentage = ProgressValue;

        _progressBar.Width = this.Width;
        _progressBar.Height = this.Height;
        _progressBarText.FontSize = this.FontSize;

        // 此处更新Arc的SweepAngle以表示百分比
        _progressBar.SweepAngle = 360.0 * (percentage / 100.0);
        _progressBar.StartAngle = 0;
        // 更新文本显示进度
        _progressBarText.Text = $"{percentage}";
    }
}
