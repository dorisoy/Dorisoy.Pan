using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;

namespace LocalizationManager.Core.Imps;
internal class LocalizationXmlFilesProvider : ILocalizationProvider
{
    public LocalizationXmlFilesProvider(string baseDirectory)
    {
        _baseDirectory = baseDirectory;
        _mapResources = new();
        _languages = new();
        LoadResource();
    }

    ~LocalizationXmlFilesProvider()
    {
        Dispose(disposing: false);
    }

    readonly string _baseDirectory;
    readonly Dictionary<string, Dictionary<string, Dictionary<string, string>>> _mapResources;
    private bool _isDisposed;

    readonly List<CultureInfo> _languages;
    IEnumerable<CultureInfo>? ILocalizationLanguageMap.LanguageMaps => throw new NotImplementedException();

    void LoadResource()
    {
        var directory = new DirectoryInfo(_baseDirectory);
        var subdirectories = directory.GetDirectories();

        foreach (var subDirectory in subdirectories)
        {
            try
            {
                CultureInfo cultureInfo = new CultureInfo(subDirectory.Name);
                _languages.Add(cultureInfo);
            }
            catch (Exception)
            {
            }

            if (subDirectory.Name.Contains(CultureInfo.CurrentCulture.TwoLetterISOLanguageName))
                LoadLanguageResource(subDirectory);
        }
    }

    void LoadResource(CultureInfo locateCultureInfo)
    {
        var directory = new DirectoryInfo(_baseDirectory);
        var subdirectories = directory.GetDirectories();

        foreach (var subDirectory in subdirectories)
        {
            if (subDirectory.Name.Contains(locateCultureInfo.TwoLetterISOLanguageName))
                LoadLanguageResource(subDirectory);
        }
    }

    void LoadLanguageResource(DirectoryInfo directory)
    {
        var allFiles = directory.GetFiles();

        if (allFiles is null || allFiles.Length <= 0)
            return;

        if (!_mapResources.TryGetValue(directory.Name, out var mapResource))
        {
            mapResource = new Dictionary<string, Dictionary<string, string>>();
            _mapResources[directory.Name] = mapResource;
        }

        foreach (var file in allFiles)
        {
            using var readStream = file.OpenRead();
            if (readStream is null)
                continue;

            var document = XElement.Load(readStream);
            var mapValues = new Dictionary<string, string>();
            LoadLanguage(document, mapValues);

            mapResource[Path.GetFileNameWithoutExtension(file.Name)] = mapValues;
        }
    }

    void LoadLanguage(XElement node, Dictionary<string, string> mapValues)
    {
        if (node is null)
            return;

        if (mapValues is null)
            return;

        if (node.HasElements)
        {
            foreach (var item in node.Elements())
                LoadLanguage(item, mapValues);
        }
        else
        {
            var name = node.Name.LocalName;
            mapValues[name] = node.Value;
        }

        return;
    }

    string ILocalizationProvider.GetString(string token, CultureInfo culture) => ((ILocalizationProvider)this).GetString(token, default, culture);

    string ILocalizationProvider.GetString(string token, CultureInfo culture, params object[] arguments) => ((ILocalizationProvider)this).GetString(token, default, culture, arguments);

    string ILocalizationProvider.GetString(string token, string? category, CultureInfo culture)
    {
        if (string.IsNullOrWhiteSpace(token))
            return string.Empty;

        var mapResource = _mapResources.Where(kv => kv.Key.Contains(culture.TwoLetterISOLanguageName)).FirstOrDefault().Value;
        if (mapResource is null)
            LoadResource(culture);

        mapResource = _mapResources.Where(kv => kv.Key.Contains(culture.TwoLetterISOLanguageName)).FirstOrDefault().Value;
        if (mapResource is null)
            mapResource = _mapResources.FirstOrDefault().Value;

        if (!string.IsNullOrWhiteSpace(category))
        {
            var mapValues = mapResource.Where(kv => kv.Key.Contains(category)).FirstOrDefault().Value;
            if (mapValues is null)
                return string.Empty;

            mapValues.TryGetValue(token, out var value);
            return value;
        }
        else
        {
            foreach (var item in mapResource)
            {
                var mapValues = item.Value;
                if (mapValues is null)
                    continue;

                if (mapValues.TryGetValue(token, out var value))
                    return value;
            }
        }

        return string.Empty;
    }

    string ILocalizationProvider.GetString(string token, string? category, CultureInfo culture, params object[] arguments)
    {
        var value = ((ILocalizationProvider)this).GetString(token, category, culture);
        return string.Format(value, arguments);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!_isDisposed)
        {
            if (disposing)
            {

            }

            foreach (var item in _mapResources)
            {
                foreach (var subItem in item.Value)
                    subItem.Value.Clear();
                item.Value.Clear();
            }

            _isDisposed = true;
        }
    }

    void IDisposable.Dispose()
    {
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }
}
