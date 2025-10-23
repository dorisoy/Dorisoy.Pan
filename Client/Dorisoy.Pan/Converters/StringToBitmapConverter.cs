using Avalonia.Data.Converters;

namespace Dorisoy.Pan.Converters;

public class StringToBitmapConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        try
        {
            using var s = AssetLoader.Open(new Uri(value.ToString()));
            return new Bitmap(s);
        }
        catch
        {
            return null;
        }
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
