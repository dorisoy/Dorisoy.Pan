using LocalizationManager.Core.Imps;
using System;

namespace LocalizationManager;

public class LocalizationManagerExtensions
{
    public static ILocalizationManager? Default { get; internal set; }

    public static ILocalizationManager Make(Func<ILocalizationProvider> providerDelegate) => new LocalizationManagerImp(providerDelegate); 
    public static ILocalizationManager Make(ILocalizationProvider localizationProvider) => new LocalizationManagerImp(localizationProvider);
}
