using LocalizationManager.Avalonia.Reactive;
using System.Reactive;
using System;


namespace LocalizationManager.Avalonia;

public class LxaString : Subjected<string>, IBinding /*: MarkupExtension*/
{
    public LxaString() : base("")
    {
    }

    public LxaString(string token) : this()
    {
        Token = token;
    }

    public LxaString(string token, object[] arguments) : this(token)
    {
        Arguments = arguments;
    }

    public LxaString(string token, string category, object[] arguments) : this(token, arguments)
    {
        Category = category;
    }

    ~LxaString()
    {

    }

    private ILocalizationManager? _localizationManager;
    protected ILocalizationManager? LocalizationManager
    {
        get => _localizationManager;
        set
        {
            if (_localizationManager is not null)
                _localizationManager.PropertyChanged -= LanguageChanged;

            _localizationManager = value;

            if (_localizationManager is not null)
                _localizationManager.PropertyChanged += LanguageChanged;
        }
    }

    [Content]
    [MarkupExtensionDefaultOption]
    public string Token { get; set; } = string.Empty;

    public string? Category { get; set; }

    public object[]? Arguments { get; set; }
 
    public IBinding? ProvideValue(IServiceProvider serviceProvider)
    {
        var localizationManager = LocalizationManagerExtensions.Default;
        if (localizationManager is null)
            return default;

        LocalizationManager = localizationManager;
        SetLanguageValue(localizationManager);
        return this;
    }

    public InstancedBinding? Initiate(AvaloniaObject target, AvaloniaProperty? targetProperty, object? anchor = null, bool enableDataValidation = false)
    {
        var observer = Observer.Create<object?>(t =>
        {

        });

        return InstancedBinding.TwoWay(this, observer); ;
    }

    void LanguageChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (sender is not ILocalizationManager localizationManager)
            return;

        SetLanguageValue(localizationManager);
    }

    void SetLanguageValue(ILocalizationManager localizationManager)
    {
        if (localizationManager is null)
            return;

        if (Arguments is not null && !string.IsNullOrWhiteSpace(Category))
            OnNext(localizationManager[Token, Category!, Arguments]);
        else if (Arguments is not null)
            OnNext(localizationManager[Token, Arguments]);
        else if (!string.IsNullOrWhiteSpace(Category))
            OnNext(localizationManager[Token, Category!]);
        else
            OnNext(localizationManager[Token]);
    }
}
