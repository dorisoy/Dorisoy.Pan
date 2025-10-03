using System;
using System.Collections.Generic;

namespace LocalizationManager.Avalonia.Providers;
internal class LocalizationAvaloniaResourceProvider : ILocalizationProvider
{
    IEnumerable<CultureInfo>? ILocalizationLanguageMap.LanguageMaps => throw new NotImplementedException();

    public void Dispose()
    {
        throw new NotImplementedException();
    }

    string ILocalizationProvider.GetString(string token, CultureInfo culture)
    {
        throw new NotImplementedException();
    }

    string ILocalizationProvider.GetString(string token, CultureInfo culture, params object[] arguments)
    {
        throw new NotImplementedException();
    }

    string ILocalizationProvider.GetString(string token, string? category, CultureInfo culture)
    {
        throw new NotImplementedException();
    }

    string ILocalizationProvider.GetString(string token, string? category, CultureInfo culture, params object[] arguments)
    {
        throw new NotImplementedException();
    }
}
