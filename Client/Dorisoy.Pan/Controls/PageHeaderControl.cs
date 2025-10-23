using Avalonia.Controls.Primitives;

namespace Dorisoy.Pan.Controls;

public class PageHeaderControl : TemplatedControl
{
    private TextBlock _text1;
    private string _title;

    public PageHeaderControl()
    {
        SizeChanged += OnSizeChanged;
        ActualThemeVariantChanged += OnActualThemeVariantChanged;
    }

    public static readonly DirectProperty<PageHeaderControl, string> TitleProperty =
     AvaloniaProperty.RegisterDirect<PageHeaderControl, string>(nameof(Title),
         x => x.Title, (x, v) => x.Title = v);

    public string Title
    {
        get => _title;
        set => SetAndRaise(TitleProperty, ref _title, value);
    }

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        base.OnApplyTemplate(e);

        _text1 = e.NameScope.Get<TextBlock>("TitleTextHost");
        UpdateTitleText();
    }

    private void OnSizeChanged(object sender, SizeChangedEventArgs e)
    {
        var wid = e.NewSize.Width;
        if (wid < 630)
        {
            var delta = 630 - wid;

            _text1.Width = 400 - delta;
        }
        else
        {
            _text1.Width = double.NaN;
        }

        PseudoClasses.Set(":small", wid < 450);
    }

    private void UpdateTitleText()
    {
        if (_text1 == null)
            return;

        _text1.Text = Title;
    }

    private void OnActualThemeVariantChanged(object sender, EventArgs e)
    {
        UpdateTitleText();
    }
}
