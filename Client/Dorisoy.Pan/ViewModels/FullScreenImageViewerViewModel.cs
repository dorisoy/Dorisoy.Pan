namespace Dorisoy.Pan.ViewModels;

public class FullScreenImageViewerViewModel : ViewModelBase
{
    private int _target;

    /// <summary>
    /// ���л�����
    /// </summary>
    [Reactive] public WebcamStreamingPlayer PipWebcam { get; set; }
    [Reactive] public Avalonia.Media.Imaging.Bitmap PipWebcamViewSource { get; set; }

    /// <summary>
    /// �������
    /// </summary>
    [Reactive] public Stretch SelectetStretch { get; set; } = Stretch.UniformToFill;

    /// <summary>
    /// ������������ʱ����ʹ�õ���������
    /// </summary>
    [Reactive] public StretchDirection SelectetStretchDirection { get; set; } = StretchDirection.Both;

    public FullScreenImageViewerViewModel(int target, MonitorPageViewModel mpvm)
    {
        _target = target;
        CallOpenPlayer(mpvm);
    }

    private CancellationTokenSource _mainCaptureCts = new();
    /// <summary>
    /// ������Ƶ
    /// </summary>
    public async void CallOpenPlayer(MonitorPageViewModel mpvm)
    {
        var token = _mainCaptureCts.Token;
        await Task.Run(async () =>
        {
            while (!token.IsCancellationRequested)
            {
                await Dispatcher.UIThread.InvokeAsync(() =>
                {
                    if (_target == 1)
                        PipWebcamViewSource = mpvm.MainWebcamViewSource;
                    else
                        PipWebcamViewSource = mpvm.PipWebcamViewSource;
                });
            }
        }, _mainCaptureCts.Token);
    }

}

