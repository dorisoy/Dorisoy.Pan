using System.Collections.Generic;

namespace LocalizationManager;
public interface ILocalizationLanguageMap
{
    IEnumerable<CultureInfo>? LanguageMaps { get; }
}
