using Color = Avalonia.Media.Color;
using Point = Avalonia.Point;

namespace Dorisoy.PanClient.Controls;


public partial class Radar : UserControl
{
    public static readonly DirectProperty<Radar, int> ValueProperty = AvaloniaProperty.RegisterDirect<Radar, int>(
       nameof(Value),
       o => o.Value,
       (o, v) => o.Value = v);


    private int _value;
    public int Value
    {
        get => _value;
        set
        {
            SetAndRaise(ValueProperty, ref _value, value);
            // 现在直接更新进度而不传入参数
            UpdateStatus(_value);
            InvalidateVisual();
        }
    }


    public void UpdateStatus(int value)
    {
        _myRadar.Text = $"{value}";
    }

    private TextBlock _myRadar;
    private IDisposable _timerSubscription;

    private double _currentRadius = 0;

    private const int MaxCircles = 5;

    //最大半径因子
    private const double MaxRadiusFactor = 0.8;


    public Radar()
    {
        InitializeComponent();
        _myRadar = this.FindControl<TextBlock>("myRadar");

        // 设定动画效果的时间间隔，订阅一个持续的时间流
        Wacth();

    }


    public override void Render(DrawingContext context)
    {
        base.Render(context);

        double maxRadius = Math.Min(Bounds.Width, Bounds.Height) / 2 * MaxRadiusFactor;

        //更新动画的当前半径
        _currentRadius += 1.0;

        if (_currentRadius > maxRadius)
        {
            _currentRadius -= maxRadius;
        }

        // 圆的中心点
        var center = new Point(Bounds.Width / 2, Bounds.Height / 2);
        for (int i = 0; i < MaxCircles; i++)
        {
            double radius = _currentRadius + (i * (maxRadius / MaxCircles));
            if (radius > maxRadius)
            {
                radius -= maxRadius;
            }

            double opacity = 1 - (radius / maxRadius);
            var brush = new SolidColorBrush(Color.Parse("#00aaeb"), opacity);
            //绘制圆
            context.DrawEllipse(brush, null, center, radius, radius);
        }

        if (_disposed)
        {
            Wacth();
        }

    }


    private void Wacth()
    {
        _timerSubscription = Observable.Interval(TimeSpan.FromMilliseconds(30))
               .ObserveOn(AvaloniaScheduler.Instance)
               .Subscribe(_ =>
               {
                   _disposed = false;
                   InvalidateVisual();
               });

    }

    private bool _disposed = false;
    protected override void OnDetachedFromVisualTree(VisualTreeAttachmentEventArgs e)
    {
        // 从可视化树中删除控件时处理计时器订阅，以防止内存泄漏
        _timerSubscription.Dispose();
        _disposed = true;
        base.OnDetachedFromVisualTree(e);
    }

    /*
    private void StartAnimation()
    {
        for (int i = 1; i <= 5; i++)
        {
            // 指定圆的初始大小和透明度
            double initialScale = 0.1;
            double initialOpacity = 1.0;

            // 以动画形式递增这些值
            double finalScale = 1.0;
            double finalOpacity = 0.0;

            // 创建圆形画刷
            var brush = new SolidColorBrush(Color.FromRgb(255, 255, 255));

            // 创建圆形路径
            var geometry = new EllipseGeometry(new Rect(0, 0, 10, 10));

            // 绘制每个圆形
            this.RenderTransform = new ScaleTransform(initialScale, initialScale);


            //context.DrawGeometry(brush, null, geometry);

            // 创建缩放动画
            var scaleAnimation = new Animation
            {
                Duration = TimeSpan.FromSeconds(2),
                IterationCount = IterationCount.Infinite,
                Easing = new QuadraticEaseOut(),
                FillMode = FillMode.Both
            };

            // Initial keyframe (scale from 0.1x)
            scaleAnimation.Children.Add(new KeyFrame
            {
                Setters = { new Setter(ScaleTransform.ScaleXProperty,initialScale),
                           new Setter(ScaleTransform.ScaleYProperty,initialScale) },
                Cue = new Cue(0d)
            });

            // Final keyframe (scale to 1x)
            scaleAnimation.Children.Add(new KeyFrame
            {
                Setters = { new Setter(ScaleTransform.ScaleXProperty,finalScale),
                           new Setter(ScaleTransform.ScaleYProperty,finalScale) },
                Cue = new Cue(1d)
            });

            // 开始缩放动画
            scaleAnimation.RunAsync(this, new CancellationToken());



            // 创建透明度动画
            var opacityAnimation = new Animation
            {
                Duration = TimeSpan.FromSeconds(2),
                IterationCount = IterationCount.Infinite,
                Easing = new LinearEasing(),
                FillMode = FillMode.Both
            };

            // 初始关键字 (opacity from 1)
            opacityAnimation.Children.Add(new KeyFrame
            {
                Setters = { new Setter(Visual.OpacityProperty, initialOpacity) },
                Cue = new Cue(0d)
            });

            // Final 关键字 (opacity to 0)
            opacityAnimation.Children.Add(new KeyFrame
            {
                Setters = { new Setter(Visual.OpacityProperty, finalOpacity) },
                Cue = new Cue(1d)
            });

            // 开始透明度动画
            opacityAnimation.RunAsync(this, new CancellationToken());
        }
    }
    */
}
