using System.Globalization;
using System.Runtime.InteropServices;
using System.Text;
using Avalonia.Data.Converters;


namespace Dorisoy.PanClient.Converters
{

    public class FormatKbSizeConverter : IValueConverter
    {
        [DllImport("shlwapi.dll", CharSet = CharSet.Unicode)]
        private static extern long StrFormatByteSizeW(long qdw, [MarshalAs(UnmanagedType.LPWStr)] StringBuilder pszBuf,
            int cchBuf);

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var number = System.Convert.ToInt64(value);
            var sb = new StringBuilder(32);
            StrFormatByteSizeW(number, sb, sb.Capacity);
            return sb.ToString();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }


        /*
        string[] sizes = { "B", "KB", "MB", "GB", "TB" };
        double len = number;
        int order = 0;
        while (len >= 1024 && order < sizes.Length - 1) {
            order++;
            len = len / 1024;
        }
        string result = $"{len:0.##} {sizes[order]}";


        string result;
        if (number >= 1024 * 1024 * 1024) {
            result = (number / 1024.0 / 1024 / 1024).ToString("F1") + " GB";
        } else if (number >= 1024 * 1024) {
            result = (number / 1024.0 / 1024).ToString("F1") + " MB";
        } else if (number >= 1024) {
            result = (number / 1024.0).ToString("F1") + " KB";
        } else {
            result = number + " Bytes";
        }
         */
    }
}
