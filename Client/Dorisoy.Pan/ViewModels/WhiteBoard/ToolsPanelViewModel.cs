namespace Dorisoy.PanClient.ViewModels;

public class ToolsPanelViewModel : ViewModelBase
{
    private readonly BrushSettings _brushSettings;
    public ToolsPanelViewModel(BrushSettings brushSettings) : base()
    {
        _brushSettings = brushSettings;
        _brushSettings.IsTeam = true;
        _brushColor = _brushSettings.BrushColor;
        _previousBrushThickness = _brushSettings.BrushThickness;
        _brushThickness = _brushSettings.BrushThickness;

        this.WhenActivated((CompositeDisposable disposables) =>
        {

        });
    }

    /// <summary>
    /// 颜色枚举器
    /// </summary>
    private ColorsEnum _brushColor;
    public ColorsEnum BrushColor
    {
        get => _brushColor;
        set
        {
            this.RaiseAndSetIfChanged(ref _brushColor, value);
            _brushSettings.BrushColor = value;

            if (value == ColorsEnum.Eraser)
            {
                BrushThickness = ThicknessEnum.Eraser;
            }
            else
            {
                BrushThickness = _previousBrushThickness;
            }
        }
    }

    /// <summary>
    /// 画笔枚举器
    /// </summary>
    private ThicknessEnum _previousBrushThickness;
    private ThicknessEnum _brushThickness;

    public ThicknessEnum BrushThickness
    {
        get => _brushThickness;
        set
        {
            this.RaiseAndSetIfChanged(ref _brushThickness, value);
            _brushSettings.BrushThickness = value;

            if (_brushColor != ColorsEnum.Eraser || value != ThicknessEnum.Eraser)
            {
                _previousBrushThickness = _brushThickness;
            }
        }
    }

}
