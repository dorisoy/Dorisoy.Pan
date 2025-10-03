﻿using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Text.Json;
using Avalonia;
using Avalonia.Platform;

namespace FluentAvalonia.UI;

/// <summary>
/// 用于存储FluentVallonia/WinUI控件的本地化字符串的帮助程序类
/// </summary>
/// <remarks>
/// 字符串资源取自WinUI repo。并非WinUI中的所有资源都可以在此处使用，只有已知在控件中使用的资源
/// </remarks>
public class FALocalizationHelper
{
    private FALocalizationHelper()
    {
        using var al = AssetLoader.Open(new Uri("avares://FluentAvalonia/Assets/ControlStrings.json"));

        KeepType<LocalizationMap>();
        KeepType<LocalizationEntry>();
        _mappings = JsonSerializer.Deserialize<LocalizationMap>(al);

        static void KeepType<
#if NET5_0_OR_GREATER
            [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)]
#endif
            T>() { }
    }

    static FALocalizationHelper()
    {
        Instance = new FALocalizationHelper();
    }

    public static FALocalizationHelper Instance { get; }

    /// <summary>
    /// Gets a string resource by the specified name using the CurrentCulture
    /// </summary>
    public string GetLocalizedStringResource(string resName) =>
        GetLocalizedStringResource(CultureInfo.CurrentCulture, resName);

    /// <summary>
    /// Gets a string resource by the specified name and using the specified culture
    /// </summary>
    /// <remarks>
    /// InvariantCulture is not supported here and will default to en-US
    /// </remarks>
    public string GetLocalizedStringResource(CultureInfo ci, string resName)
    {
        // Don't allow InvariantCulture - default to en-us in that case
        if (ci == CultureInfo.InvariantCulture)
            ci = new CultureInfo(s_enUS);

        if (_mappings.ContainsKey(resName))
        {
            if (_mappings[resName].ContainsKey(ci.Name))
            {
                return _mappings[resName][ci.Name];
            }
            else if (_mappings[resName].ContainsKey(s_enUS))
            {
                return _mappings[resName][s_enUS];
            }
        }

        return string.Empty;
    }

    // <ResourceName, Entries>
    private readonly LocalizationMap _mappings;
    private static readonly string s_enUS = "en-US";

    /// <summary>
    /// Dictionary of language entries for a resource name. &lt;language, value&gt; where
    /// language is the abbreviated name, e.g., en-US
    /// </summary>
    public class LocalizationEntry : Dictionary<string, string>
    {
        public LocalizationEntry()
            : base(StringComparer.InvariantCultureIgnoreCase)
        {

        }
    }

    private class LocalizationMap : Dictionary<string, LocalizationEntry>
    {
        public LocalizationMap()
            : base(StringComparer.InvariantCultureIgnoreCase)
        {

        }
    }
}
