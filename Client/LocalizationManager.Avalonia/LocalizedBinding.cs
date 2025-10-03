using LocalizationManager.Avalonia.Reactive;
using System.Reactive;
using System;
using Avalonia.Data;

namespace LocalizationManager.Avalonia;

public class LocalizedBinding : SubjectedObject<string>, IBinding
{
    static LocalizedBinding()
    {
        TokenProperty.Changed.AddClassHandler<LocalizedBinding, string>((s, e) =>
        {
            if (s is null)
                return;

            var localizationManager = s._localizationManager;
            if (localizationManager is null)
                return;

            s.OnNext(localizationManager[e.NewValue.Value]);
        });
    }

    public LocalizedBinding() : base(string.Empty)
    {
      
    }

    public LocalizedBinding(IBinding binding) : this()
    {
        var subscription = this.Bind(TokenProperty, binding);
    }

    public static readonly StyledProperty<string> TokenProperty =
            AvaloniaProperty.Register<LocalizedBinding, string>(nameof(Token));

    public static readonly StyledProperty<string?> CategoryProperty =
           AvaloniaProperty.Register<LocalizedBinding, string?>(nameof(Category));

    public static readonly StyledProperty<object[]?> ArgumentsProperty =
            AvaloniaProperty.Register<LocalizedBinding, object[]?>(nameof(Arguments));

    [Content]
    [MarkupExtensionDefaultOption]
    public string Token
    {
        get => GetValue(TokenProperty);
        set => SetValue(TokenProperty, value);
    }

    public string? Category
    {
        get => GetValue(CategoryProperty);
        set => SetValue(CategoryProperty, value);
    }

    public object[]? Arguments
    {
        get => GetValue(ArgumentsProperty);
        set => SetValue(ArgumentsProperty, value);
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
