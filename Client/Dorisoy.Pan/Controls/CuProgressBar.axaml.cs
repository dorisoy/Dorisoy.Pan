namespace Dorisoy.Pan.Controls;

public partial class CuProgressBar : UserControl
{
    private Avalonia.Controls.Shapes.Arc _progressBar;
    private TextBlock _progressBarText;

    /// <summary>
    /// ֱ�����ԣ������ⲿ�ؼ��Ϳ��԰󶨵��������ǵĿؼ���UI�����߼����ڹ���������µ����ԡ�
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
            // ����ֱ�Ӹ��½��ȶ����������
            UpdateProgress();
        }
    }

    public CuProgressBar()
    {
        InitializeComponent();

        // �󶨿ؼ�
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

        // �˴�����Arc��SweepAngle�Ա�ʾ�ٷֱ�
        _progressBar.SweepAngle = 360.0 * (percentage / 100.0);
        _progressBar.StartAngle = 0;
        // �����ı���ʾ����
        _progressBarText.Text = $"{percentage}";
    }
}
