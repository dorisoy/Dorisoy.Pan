using LocalizationManager.Core.Imps;
using System;
using System.IO;
using System.Collections.Generic;


namespace LocalizationManager;

public class LocalizationProviderExtensions
{
    public static ILocalizationProvider MakeXmlFileProvider(string baseDirectory, string baseName)
    {
        if (string.IsNullOrWhiteSpace(baseName)) throw new ArgumentNullException(nameof(baseName));
        if (string.IsNullOrWhiteSpace(baseDirectory)) throw new ArgumentNullException(nameof(baseDirectory));

        if (!Directory.Exists(baseDirectory)) throw new ArgumentNullException(nameof(baseDirectory), $"The baseDirectory:{baseDirectory} is not exists!");
        return new LocalizationXmlFileProvider(baseDirectory,baseName);
    }

    public static ILocalizationProvider MakeXmlFilesProvider(string baseDirectory)
    {
        if (string.IsNullOrWhiteSpace(baseDirectory)) throw new ArgumentNullException(nameof(baseDirectory));
        if (!Directory.Exists(baseDirectory)) throw new ArgumentNullException(nameof(baseDirectory), $"The baseDirectory:{baseDirectory} is not exists!");
        return new LocalizationXmlFilesProvider(baseDirectory);
    }

    public static ILocalizationProvider MakeResourceProvider(ResourceManager resourceManager)
    {
        if (resourceManager is null) throw new ArgumentNullException(nameof(resourceManager));
        resourceManager.GetString(string.Empty);
        return new LocalizationResourceProvider(resourceManager);
    }

    public static ILocalizationProvider MakeResourceProvider(IEnumerable<ResourceManager> resourceManagers)
    {
        if (resourceManagers is null) throw new ArgumentNullException(nameof(resourceManagers));
        return new LocalizationResourceProvider(resourceManagers);
    }

    public static ILocalizationProvider MakeResourceProvider(string resourceDirectory, string baseName, Type? usingResourceSet = null)
    {
        if (string.IsNullOrWhiteSpace(baseName)) throw new ArgumentNullException(nameof(baseName));
        if (string.IsNullOrWhiteSpace(resourceDirectory)) throw new ArgumentNullException(nameof(resourceDirectory));

        if (!Directory.Exists(resourceDirectory)) throw new ArgumentNullException(nameof(resourceDirectory), $"The resourceDirectory:{resourceDirectory} is not exists!");
        return new LocalizationResourceProvider(resourceDirectory, baseName, usingResourceSet);
    }

}
