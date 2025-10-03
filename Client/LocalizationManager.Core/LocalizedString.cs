using LocalizationManager.Core.Mvvm;
using System;

namespace LocalizationManager;

public class LocalizedString : BindableBase
{
    public LocalizedString(Func<string> generator)
        : this(LocalizationManagerExtensions.Default!, generator)
    {

    }

    public LocalizedString(ILocalizationChanged localizationManager, Func<string> generator)
    {
        _generator = generator;
        localizationManager.PropertyChanged += (sender, e) => RaisePropertyChanged(nameof(Localized));
    }

    private readonly Func<string> _generator;

    public string Localized => _generator.Invoke();

    public static implicit operator LocalizedString(Func<string> generator) => new(generator);
}
