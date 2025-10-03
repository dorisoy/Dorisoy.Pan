using System;
using Avalonia.Controls;
using Avalonia.Markup.Xaml.XamlIl.Runtime;
using Avalonia.Styling;
using System.Linq;
using System.Collections.Generic;


namespace LocalizationManager.Avalonia.Extensions;
internal static class Extensions
{
    public static T? GetService<T>(this IServiceProvider sp) => (T?)sp?.GetService(typeof(T));

    public static T? GetFirstParent<T>(this IServiceProvider ctx) where T : class
           => ctx.GetService<IAvaloniaXamlIlParentStackProvider>()?.Parents.OfType<T>().FirstOrDefault();

    public static T? GetLastParent<T>(this IServiceProvider ctx) where T : class
    => ctx.GetService<IAvaloniaXamlIlParentStackProvider>()?.Parents.OfType<T>().LastOrDefault();

    public static IEnumerable<T>? GetParents<T>(this IServiceProvider sp)
    {
        return sp.GetService<IAvaloniaXamlIlParentStackProvider>()?.Parents.OfType<T>();
    }

    public static bool IsInControlTemplate(this IServiceProvider sp) => sp.GetService<IAvaloniaXamlIlControlTemplateProvider>() != null;

    public static Type ResolveType(this IServiceProvider ctx, string namespacePrefix, string type)
    {
        var tr = ctx.GetService<IXamlTypeResolver>(); 
        string name = string.IsNullOrEmpty(namespacePrefix) ? type : $"{namespacePrefix}:{type}";
        return (tr?.Resolve(name))!;
    }

    public static object? GetDefaultAnchor(this IServiceProvider provider)
    {
        if (provider is null)
            return default;

        object? anchor = provider.GetFirstParent<Control>();
        if (anchor is null)
            anchor = provider.GetFirstParent<IDataContextProvider>();

        return anchor ?? provider.GetService<IRootObjectProvider>()?.RootObject as IStyle ?? provider.GetLastParent<IStyle>();
    }
}
