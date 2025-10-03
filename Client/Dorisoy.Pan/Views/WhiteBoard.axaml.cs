using Avalonia.Controls.PanAndZoom;
using Dorisoy.PanClient.ViewModels;
using Point = Avalonia.Point;

namespace Dorisoy.PanClient.Views;

public partial class WhiteBoard : ReactiveCoreWindow<WhiteBoardViewModel>
{
    private readonly IAppState _appState;
    private readonly IRenderer _renderer;
    private readonly DispatcherTimer _lineRenderingTimer = new();
    private Point currentPoint = new();
    private bool pressed;
    private Action closeAdditionalAction = () => { };
    private bool isClosing;


    public WhiteBoard()
    {
        this.InitializeComponent();

        _appState = Locator.Current.GetRequiredService<IAppState>();
        _renderer = new Renderer(_appState, canvas, true);

        _lineRenderingTimer.Tick += LineRenderingTimer_Tick;
        _lineRenderingTimer.Interval = TimeSpan.FromMilliseconds(Globals.RenderingIntervalMs);
        _lineRenderingTimer.Start();

        canvas.Cursor = _appState.BrushSettings.Cursor;
        canvas.PointerMoved += Canvas_PointerMoved;
        //canvas.PointerWheelChanged += Canvas_PointerWheelChanged;
        _appState.BrushSettings.BrushChanged += BrushSettings_BrushChanged;

        ZoomBorder1 = this.Find<ZoomBorder>("ZoomBorder1");
        if (ZoomBorder1 != null)
        {
            ZoomBorder1.KeyDown += ZoomBorder_KeyDown;
            ZoomBorder1.ZoomChanged += ZoomBorder_ZoomChanged;
        }

        this.WhenActivated(disposables => { });
    }


    private void ZoomBorder_KeyDown(object? sender, KeyEventArgs e)
    {
        var zoomBorder = this.DataContext as ZoomBorder;

        switch (e.Key)
        {
            case Key.F:
                zoomBorder?.Fill();
                break;
            case Key.U:
                zoomBorder?.Uniform();
                break;
            case Key.R:
                zoomBorder?.ResetMatrix();
                break;
            case Key.T:
                zoomBorder?.ToggleStretchMode();
                zoomBorder?.AutoFit();
                break;
        }
    }


    private void ZoomBorder_ZoomChanged(object sender, ZoomChangedEventArgs e)
    {
        Debug.WriteLine($"[ZoomChanged] {e.ZoomX} {e.ZoomY} {e.OffsetX} {e.OffsetY}");
    }


    ///// <summary>
    ///// 鼠标滚轮缩放画布 MouseWheel
    ///// </summary>
    ///// <param name="sender"></param>
    ///// <param name="e"></param>
    ///// <exception cref="NotImplementedException"></exception>
    //private void Canvas_PointerWheelChanged(object sender, PointerWheelEventArgs e)
    //{
    //    //滚轮滚动时控制 放大的倍数,没有固定的值，可以根据需要修改。
    //    //double scale = e.Delta.WithX * 0.002
    //    var x = e.Delta.X;
    //    var y = e.Delta.Y
    //}

    protected override void OnDataContextChanged(EventArgs e)
    {
        var vm = DataContext as WhiteBoardViewModel;

        //vm._whiteBoardService.Connection.On<string, byte[]>("DrewPoint", Connection_ParticipantDrewPoint);

        //vm._whiteBoardService.Connection.On<string, byte[]>("DrewLine", Connection_ParticipantDrewLine);

        //vm._whiteBoardService.Connection.Closed += Connection_Closed;

        base.OnDataContextChanged(e);
    }

    private void LineRenderingTimer_Tick(object sender, EventArgs e)
    {
        Dispatcher.UIThread.InvokeAsync(async () =>
        {
            var points = _renderer.RenderLine();
            if (!points.Any())
                return;

            //var vm = DataContext as WhiteBoardViewModel;
            //_ = vm.SignalRService.DrawLineAsync(points);

            //绘制
            var vm = this.ViewModel;

            //await vm._whiteBoardService.DrawLineAsync(points);
        });
    }

    private void BrushSettings_BrushChanged(object sender, BrushChangedEventArgs e)
    {
        canvas.Cursor = e.Cursor;
    }

    private void Connection_ParticipantDrewPoint(string participant, byte[] data)
    {

        Dispatcher.UIThread.InvokeAsync(() =>
        {
            var point = PayloadConverter.ToPoint(data);
            canvas.Children.Add(point);
            IndicateDrawing(participant);
        });
    }

    private void Connection_ParticipantDrewLine(string participant, byte[] data)
    {

        Dispatcher.UIThread.InvokeAsync(() =>
        {
            var (points, thickness, colorBrush) = PayloadConverter.ToLine(data);
            _renderer.RenderLine(points, thickness, colorBrush);
            IndicateDrawing(participant);
        });

    }

    private void Canvas_PointerPressed(object sender, PointerPressedEventArgs e)
    {
        currentPoint = e.GetPosition(canvas);
        pressed = true;
        //更新画笔区域范围
        _appState.BrushSettings.UpdateBrushPoint();
    }

    private void Canvas_PointerReleased(object sender, PointerReleasedEventArgs e)
    {
        Dispatcher.UIThread.InvokeAsync(() =>
        {
            pressed = false;

            var newPoint = _renderer.RestrictPointToCanvas(currentPoint.X, currentPoint.Y);
            _renderer.DrawPoint(newPoint.X, newPoint.Y);

            var vm = DataContext as WhiteBoardViewModel;
            //_ = vm._whiteBoardService.DrawPointAsync(newPoint.X, newPoint.Y);

            IndicateDrawing(_appState.Nickname);
        });
    }

    /// <summary>
    /// 光标移动后
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void Canvas_PointerMoved(object sender, PointerEventArgs e)
    {
        if (!pressed)
        {
            return;
        }

        Dispatcher.UIThread.InvokeAsync(() =>
        {
            Point newPosition = e.GetPosition(canvas);
            var newPoint = _renderer.RestrictPointToCanvas(newPosition.X, newPosition.Y);

            _renderer.EnqueueLineSegment(currentPoint, newPoint);

            currentPoint = newPoint;

            IndicateDrawing(_appState.Nickname);
        });

    }

    private void IndicateDrawing(string nickname)
    {
        //var vm1 = ViewModel;
        var vm = DataContext as WhiteBoardViewModel;
        vm.IndicateDrawing(nickname);
    }


    /// <summary>
    /// 连接关闭时
    /// </summary>
    /// <param name="arg"></param>
    /// <returns></returns>
    private Task Connection_Closed(Exception arg)
    {
        if (isClosing)
            return Task.CompletedTask;

        closeAdditionalAction = () =>
        {
            var vm = DataContext as WhiteBoardViewModel;
            //if (vm._whiteBoardService.Connection.State == HubConnectionState.Disconnected)
            //{
            //    vm.MessageBox("您已断开连接：（请检查您的互联网连接，或稍后再试)");
            //}
        };

        Dispatcher.UIThread.InvokeAsync(Close);

        return Task.CompletedTask;
    }

    /// <summary>
    /// 关闭窗体时
    /// </summary>
    /// <param name="e"></param>
    protected override void OnClosing(WindowClosingEventArgs e)
    {
        isClosing = true;

        //var vm = DataContext as WhiteBoardViewModel;
        //if (vm..Connection.State == HubConnectionState.Connected)
        //{
        //    _ = vm.LeaveRoomAsync(Globals.CurrentUser.Id);
        //    _ = vm.Connection.StopAsync();
        //}
        //_ = vm.SignalRService.Connection.DisposeAsync();

        closeAdditionalAction();
    }
}
