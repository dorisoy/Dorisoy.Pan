using Avalonia.Controls.Templates;

namespace Dorisoy.Pan;


public sealed class ViewLocator : IDataTemplate
{
    public bool SupportsRecycling => false;

    public Control? Build(object? data)
    {
        if (data is null)
        {
            return null;
        }

        var view = Locator.Current.GetService(typeof(IViewFor<>).MakeGenericType(data.GetType()));

        return view is Control control
            ? control
            : new TextBlock { Text = "Not Found: " + view?.GetType().FullName };
    }

    public bool Match(object? data) =>
        data is ReactiveObject;
}


/*
public class ViewLocator : IDataTemplate
{
    public bool SupportsRecycling => false;

    public Control Build(object? data)
    {
        if (data is null)
            throw new ArgumentNullException(nameof(data));

        var type = data.GetType();

        if (type is null)
            throw new InvalidOperationException("Type is null");

        if (Attribute.GetCustomAttribute(type, typeof(ViewAttribute)) is ViewAttribute viewAttr)
        {
            var viewType = viewAttr.GetViewType();

#pragma warning disable IL2072
            // 在设计模式中，只需创建视图的新实例
            if (Design.IsDesignMode)
            {
                return (Control)Activator.CreateInstance(viewType)!;
            }
#pragma warning restore IL2072
            // 否则从服务提供器Splat处获取
            if (Locator.Current.GetService(viewType) is Control view)
            {
                return view;
            }
        }

        return new TextBlock
        {
            Text = "Not Found: " + data.GetType().FullName
        };
    }

    public bool Match(object? data) => data is ViewModelBase;

}


*/
