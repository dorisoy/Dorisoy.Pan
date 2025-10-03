using Avalonia.Controls.Templates;
using Avalonia.Data.Converters;
using Avalonia.Layout;
using Avalonia.Markup.Xaml.Templates;

namespace Dorisoy.PanClient.Converters
{

    public class SumConverter : IMultiValueConverter
    {
        public object Convert(IList<object> values, Type targetType, object parameter, CultureInfo culture)
        {
            double _Sum = 0;

            if (values == null)
                return _Sum;

            foreach (var item in values)
            {
                double _Value;
                if (double.TryParse(item.ToString(), out _Value))
                    _Sum += _Value;
            }
            return _Sum;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class ViewTypePanelConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var template = new ItemsPanelTemplate()
            {
                Content = new Func<IServiceProvider, TemplateResult<object>>(_ =>
                {
                    var content =
                        value switch
                        {
                            true => new StackPanel { Orientation = Orientation.Vertical },
                            false => new StackPanel { Orientation = Orientation.Horizontal },
                            _ => throw new NotImplementedException(),
                        };
                    return new TemplateResult<object>(content, new NameScope());
                })
            };
            return template;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
