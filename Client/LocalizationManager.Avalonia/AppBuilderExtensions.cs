namespace LocalizationManager.Avalonia;
using System;

public static class AppBuilderExtensions
{
    public static AppBuilder UseLocalizationManager(this AppBuilder builder, Func<ILocalizationProvider> configDelegate)
    {
        builder.AfterPlatformServicesSetup(app =>
        {
            LocalizationManagerExtensions.Default = LocalizationManagerExtensions.Make(configDelegate);
        });

        return builder;
    }
}
