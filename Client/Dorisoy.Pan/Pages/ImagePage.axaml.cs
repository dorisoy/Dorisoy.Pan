using System.Drawing;
using Dorisoy.Pan.ViewModels;
using Bitmap = System.Drawing.Bitmap;
using Color = Avalonia.Media.Color;
using Point = Avalonia.Point;

namespace Dorisoy.Pan.Pages;

public partial class ImagePage : ReactiveUserControl<ImagePageViewModel>
{
    private readonly IAppState _appState;
    private readonly DispatcherTimer _lineRenderingTimer = new();
    private Point currentPoint = new();
    private bool pressed;
    private bool canvaReady;
    private bool areaMoveing;


    private Point currentPoint2 = new();
    private bool pressed2;

    public ImagePage()
    {
        this.InitializeComponent();

        _appState = Locator.Current.GetRequiredService<IAppState>();

        //����Timer���ڼ�����Ⱦ
        _lineRenderingTimer.Tick += LineRenderingTimer_Tick;
        _lineRenderingTimer.Interval = TimeSpan.FromMilliseconds(Globals.RenderingIntervalMs);
        _lineRenderingTimer.Start();

        //��갴��ʱ
        canvasBorder.PointerPressed += AreaCanvas_PointerPressed;
        //����ɿ�ʱ
        canvasBorder.PointerReleased += AreaCanvas_PointerReleased;
        //����ƶ�ʱ
        canvasBorder.PointerMoved += AreaCanvas_PointerMoved;


        //������
        canvas.Cursor = _appState.BrushSettings.Cursor;
        //��갴��ʱ
        canvas.PointerPressed += Canvas_PointerPressed;
        //����ɿ�ʱ
        canvas.PointerReleased += Canvas_PointerReleased;
        //����ƶ�ʱ
        canvas.PointerMoved += Canvas_PointerMoved;

        //���ʸ���ʱ
        _appState.BrushSettings.BrushChanged += BrushSettings_BrushChanged;


        this.WhenActivated(disposable =>
        {
            ViewModel.canvas = canvas;
        });
    }

    /// <summary>
    /// ������Ļ��ͼ
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void SaveShotArea_Click(object sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        try
        {
            var position = shotArea.PointToScreen(new Point(1d, 1d));
            var w = this.shotArea.GetValue(Canvas.WidthProperty);
            var h = this.shotArea.GetValue(Canvas.HeightProperty);

            var width = (int)shotArea.Width - 2;
            var height = (int)shotArea.Height - 2;

            var screenshot = TakeScreenshot((int)position.X, (int)position.Y, width, height);
            //var path = @"C:\Users\Administrator\Desktop\Dorisoy\screenshot.bmp";
            //screenshot.Save(path);
            ViewModel.Execute(screenshot);

            //
            shotArea.SetValue(Canvas.LeftProperty, 0);
            shotArea.SetValue(Canvas.TopProperty, 0);
            shotArea.Width = 0;
            shotArea.Height = 0;

            canvaReady = false;
            saveShotArea.IsVisible = false;
            closeShotArea.IsVisible = false;
            ViewModel.IsScreenCutter = false;

            canvas.Children.Clear();
        }
        catch (Exception ex)
        {
            ViewModel?.MessageBox(ex.Message);
        }
    }

    /// <summary>
    /// �ر���Ļ��ͼ
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void CloseShotArea_Click(object sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        try
        {
            shotArea.SetValue(Canvas.LeftProperty, 0);
            shotArea.SetValue(Canvas.TopProperty, 0);
            shotArea.Width = 0;
            shotArea.Height = 0;

            canvaReady = false;
            saveShotArea.IsVisible = false;
            closeShotArea.IsVisible = false;
            ViewModel.IsScreenCutter = false;

            canvas.Children.Clear();
        }
        catch (Exception ex)
        {
            ViewModel?.MessageBox(ex.Message);
        }
    }

    private Bitmap TakeScreenshot(int fromX, int fromY, int width, int height)
    {
        var screenshot = new Bitmap(width, height, System.Drawing.Imaging.PixelFormat.Format32bppRgb);
        using (Graphics g = Graphics.FromImage(screenshot))
        {
            g.CopyFromScreen(fromX, fromY, 0, 0, screenshot.Size);
        }
        return screenshot;
    }


    /// <summary>
    /// ��갴��ʱ
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void AreaCanvas_PointerPressed(object sender, PointerEventArgs e)
    {
        if (!pressed)
        {
            if (!canvaReady)
            {
                currentPoint = e.GetPosition(canvasBorder);
                pressed = true;
                canvaReady = false;
                var pos = e.GetPosition(this);
                shotArea.SetValue(Canvas.LeftProperty, pos.X);
                shotArea.SetValue(Canvas.TopProperty, pos.Y);
                shotArea.Width = 0;
                shotArea.Height = 0;
                shotArea.BorderBrush = new SolidColorBrush(Color.FromRgb(255, 127, 39));

                saveShotArea.IsVisible = false;
                closeShotArea.IsVisible = false;
                ViewModel.IsScreenCutter = false;
            }
        }
    }
    public bool IsDrawed => shotArea.Width > 0 && shotArea.Height > 0;

    /// <summary>
    /// ����ɿ�ʱ
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void AreaCanvas_PointerReleased(object sender, PointerEventArgs e)
    {
        pressed = false;
        areaMoveing = false;

        if (IsDrawed)
        {
            saveShotArea.IsVisible = true;
            closeShotArea.IsVisible = true;
            ViewModel.IsScreenCutter = true;
        }
    }

    /// <summary>
    /// ����ƶ�ʱ���ƾ���
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void AreaCanvas_PointerMoved(object sender, PointerEventArgs e)
    {
        if (!pressed)
            return;

        var pos = e.GetPosition(canvasBorder);

        areaMoveing = true;

        // ���þ��ε�λ��
        var x = Math.Min(pos.X, currentPoint.X);
        var y = Math.Min(pos.Y, currentPoint.Y);

        // ���þ��εĳߴ�
        var w = Math.Max(pos.X, currentPoint.X) - x;
        var h = Math.Max(pos.Y, currentPoint.Y) - y;

        this.shotArea.Width = w;
        this.shotArea.Height = h;

        Canvas.SetLeft(shotArea, x);
        Canvas.SetTop(shotArea, y);

        var cx = this.shotArea.GetValue(Canvas.LeftProperty);
        var cy = this.shotArea.GetValue(Canvas.TopProperty);

        canvaReady = true;

        //���水ť�������
        Canvas.SetLeft(saveShotArea, cx + (int)w - 40);
        Canvas.SetTop(saveShotArea, cy + (int)h + 5);

        Canvas.SetLeft(closeShotArea, cx + (int)w - 85);
        Canvas.SetTop(closeShotArea, cy + (int)h + 5);


        //��̬����߿�
        Globals.CanvasWidth = (int)w;
        Globals.CanvasHeight = (int)h;

        //���»�������Χ
        _appState.BrushSettings.UpdateBrushPoint();
    }

    //=============================
    //Canvas
    //=============================


    /// <summary>
    /// ��갴��ʱ
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void Canvas_PointerPressed(object sender, PointerEventArgs e)
    {
        currentPoint2 = e.GetPosition(canvas);
        pressed2 = true;
    }

    /// <summary>
    /// ����ɿ�ʱ
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private async void Canvas_PointerReleased(object sender, PointerEventArgs e)
    {
        pressed2 = false;

        var newPoint = ViewModel.RestrictPointToCanvas(currentPoint2.X, currentPoint2.Y);
        ViewModel.DrawPoint(newPoint.X, newPoint.Y);
        //����
        await ViewModel.DrawPointAsync(newPoint.X, newPoint.Y);
    }


    /// <summary>
    /// ����ƶ�ʱ
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void Canvas_PointerMoved(object sender, PointerEventArgs e)
    {
        if (!pressed2 || areaMoveing)
        {
            return;
        }

        var newPosition = e.GetPosition(canvas);
        var newPoint = ViewModel.RestrictPointToCanvas(newPosition.X, newPosition.Y);
        ViewModel.EnqueueLineSegment(currentPoint2, newPoint);
        currentPoint2 = newPoint;
    }


    private void LineRenderingTimer_Tick(object sender, EventArgs e)
    {
        Dispatcher.UIThread.InvokeAsync(async () =>
        {
            var points = ViewModel.RenderLine();
            if (!points.Any())
                return;
            //����
            await ViewModel.DrawLineAsync(points);
        });
    }

    /// <summary>
    /// ���ʸ���ʱ
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void BrushSettings_BrushChanged(object sender, BrushChangedEventArgs e)
    {
        canvas.Cursor = e.Cursor;
    }
}
