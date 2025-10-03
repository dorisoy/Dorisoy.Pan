using System;

namespace LocalizationManager;

public static class LocalizationManagerBuilder
{
    public static void Initialize(Func<ILocalizationProvider> configDelegate)
    {
        LocalizationManagerExtensions.Default = LocalizationManagerExtensions.Make(configDelegate);
    }
}
