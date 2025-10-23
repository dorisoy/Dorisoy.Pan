using Color = Avalonia.Media.Color;
using Point = Avalonia.Point;

namespace Dorisoy.Pan.Controls;


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
            // ����ֱ�Ӹ��½��ȶ����������
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

    //���뾶����
    private const double MaxRadiusFactor = 0.8;


    public Radar()
    {
        InitializeComponent();
        _myRadar = this.FindControl<TextBlock>("myRadar");

        // �趨����Ч����ʱ����������һ��������ʱ����
        Wacth();

    }


    public override void Render(DrawingContext context)
    {
        base.Render(context);

        double maxRadius = Math.Min(Bounds.Width, Bounds.Height) / 2 * MaxRadiusFactor;

        //���¶����ĵ�ǰ�뾶
        _currentRadius += 1.0;

        if (_currentRadius > maxRadius)
        {
            _currentRadius -= maxRadius;
        }

        // Բ�����ĵ�
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
            //����Բ
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
        // �ӿ��ӻ�����ɾ���ؼ�ʱ������ʱ�����ģ��Է�ֹ�ڴ�й©
        _timerSubscription.Dispose();
        _disposed = true;
        base.OnDetachedFromVisualTree(e);
    }

    /*
    private void StartAnimation()
    {
        for (int i = 1; i <= 5; i++)
        {
            // ָ��Բ�ĳ�ʼ��С��͸����
            double initialScale = 0.1;
            double initialOpacity = 1.0;

            // �Զ�����ʽ������Щֵ
            double finalScale = 1.0;
            double finalOpacity = 0.0;

            // ����Բ�λ�ˢ
            var brush = new SolidColorBrush(Color.FromRgb(255, 255, 255));

            // ����Բ��·��
            var geometry = new EllipseGeometry(new Rect(0, 0, 10, 10));

            // ����ÿ��Բ��
            this.RenderTransform = new ScaleTransform(initialScale, initialScale);


            //context.DrawGeometry(brush, null, geometry);

            // �������Ŷ���
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

            // ��ʼ���Ŷ���
            scaleAnimation.RunAsync(this, new CancellationToken());



            // ����͸���ȶ���
            var opacityAnimation = new Animation
            {
                Duration = TimeSpan.FromSeconds(2),
                IterationCount = IterationCount.Infinite,
                Easing = new LinearEasing(),
                FillMode = FillMode.Both
            };

            // ��ʼ�ؼ��� (opacity from 1)
            opacityAnimation.Children.Add(new KeyFrame
            {
                Setters = { new Setter(Visual.OpacityProperty, initialOpacity) },
                Cue = new Cue(0d)
            });

            // Final �ؼ��� (opacity to 0)
            opacityAnimation.Children.Add(new KeyFrame
            {
                Setters = { new Setter(Visual.OpacityProperty, finalOpacity) },
                Cue = new Cue(1d)
            });

            // ��ʼ͸���ȶ���
            opacityAnimation.RunAsync(this, new CancellationToken());
        }
    }
    */
}
