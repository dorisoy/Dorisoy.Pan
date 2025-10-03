using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;

using LocalizationManager.Core.Mvvm;

namespace LocalizationManager.Core.Imps;
internal class LocalizationManagerImp : BindableBase, ILocalizationManager
{
    public LocalizationManagerImp(ILocalizationProvider provider)
    {
        DefaultCulture = CultureInfo.CurrentCulture;
        _provider = provider;
    }

    public LocalizationManagerImp(Func<ILocalizationProvider> providerDelegate) 
        :this(providerDelegate.Invoke())
    {

    }

    private event EventHandler<EventArgs>? _LanguageChanged;

    event EventHandler<EventArgs>? ILocalizationChanged.LanguageChanged
    {
        add => _LanguageChanged += value;
        remove => _LanguageChanged -= value;
    }

    ILocalizationProvider? _provider;
    CultureInfo _currentCulture = CultureInfo.CurrentCulture;

    public bool SetProvider(ILocalizationProvider localizationProvider)
    {
        _provider = localizationProvider;
        return true;
    }

    public string this[string token] => GetValue(token);
    public string this[string token, string category] => GetValue(token, category);
    public string this[string token, params object[] arguments] => GetValue(token, arguments);
    public string this[string token, string category, params object[] arguments] => GetValue(token, category, arguments);

    public CultureInfo DefaultCulture { get; }

    public CultureInfo CurrentCulture
    {
        get => _currentCulture;
        set
        {
            if (SetProperty(ref _currentCulture, value, (oldValue, newValue) =>
            {
                CultureInfo.CurrentCulture = newValue;
                CultureInfo.CurrentUICulture = newValue;
                CultureInfo.DefaultThreadCurrentCulture = newValue;
                CultureInfo.DefaultThreadCurrentUICulture = newValue;
                return true;
            }, propertyName:null))
                _LanguageChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(CurrentCulture)));
        }
    }

    public IEnumerable<CultureInfo>? LanguageMaps => _provider?.LanguageMaps;

    public string GetValue(string token) => _provider?.GetString(token, CurrentCulture) ?? string.Empty;

    public string GetValue(string token, params object[] arguments) => _provider?.GetString(token, CurrentCulture, arguments) ?? string.Empty;

    public string GetValue(string token, string? category = null) => _provider?.GetString(token, category, CurrentCulture) ?? string.Empty;

    public string GetValue(string token, string? category, params object[] arguments) => _provider?.GetString(token, category, CurrentCulture, arguments) ?? string.Empty;

    public void Dispose()
    {
        _provider?.Dispose();
    }

    public ValueTask DisposeAsync()
    {
        _provider?.Dispose();
        return new ValueTask();
    }


}
