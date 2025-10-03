using System;
namespace LocalizationManager;
public interface ILocalizationProvider : ILocalizationLanguageMap, IDisposable
{
    string GetString(string token, CultureInfo culture);
    string GetString(string token, CultureInfo culture, params object[] arguments);

    string GetString(string token, string? category, CultureInfo culture);
    string GetString(string token, string? category, CultureInfo culture, params object[] arguments);
}
