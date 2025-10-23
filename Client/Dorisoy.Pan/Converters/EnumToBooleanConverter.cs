using Avalonia.Data;
using Avalonia.Data.Converters;

namespace Dorisoy.Pan.Converters;


public class StringConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        return value.ToString();
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotSupportedException();
    }
}


public class EnumToBooleanConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (targetType == typeof(bool) || targetType == typeof(bool?))
            return value.ToString() == parameter.ToString();
        return null;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value == null)
            return BindingNotification.UnsetValue;
        var s = Enum.Parse(targetType, parameter.ToString(), true);
        return value.Equals(true) ? s : BindingOperations.DoNothing;
    }
}


//public class EnumToBooleanConverter : IValueConverter
//{
//    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
//    {
//        return value?.Equals(parameter);
//    }

//    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
//    {
//        return value?.Equals(true) == true ? parameter : BindingOperations.DoNothing;
//    }
//}

//public class EnumDescriptionTypeConverter : EnumConverter
//{
//    public EnumDescriptionTypeConverter(Type type)
//        : base(type)
//    {
//    }
//    public override object ConvertTo(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value, Type destinationType)
//    {
//        if (destinationType == typeof(string))
//        {
//            if (value != null)
//            {
//                FieldInfo fi = value.GetType().GetField(value.ToString());
//                if (fi != null)
//                {
//                    var attributes = (DescriptionAttribute[])fi.GetCustomAttributes(typeof(DescriptionAttribute), false);
//                    return ((attributes.Length > 0) && (!String.IsNullOrEmpty(attributes[0].Description))) ? attributes[0].Description : value.ToString();
//                }
//            }

//            return string.Empty;
//        }

//        return base.ConvertTo(context, culture, value, destinationType);
//    }
//}

public class StretchToStringConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        string EnumString = "";
        try
        {
            var stretch = (Stretch)value;
            switch (stretch)
            {
                case Stretch.None:
                    EnumString = "原始大小";
                    break;
                case Stretch.Fill:
                    EnumString = "填充目标";
                    break;
                case Stretch.Uniform:
                    EnumString = "纵横比大小自适应";
                    break;
                case Stretch.UniformToFill:
                    EnumString = "纵横比完全填充";
                    break;
            }
            return EnumString;
        }
        catch
        {
            return string.Empty;
        }
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}

public class StretchDirectionToStringConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        string EnumString = "";
        try
        {
            var stretch = (StretchDirection)value;
            switch (stretch)
            {
                //始终根据拉伸模式进行拉伸以适应可用空间
                case StretchDirection.Both:
                    EnumString = "自适应拉伸";
                    break;
                //只有当内容小于可用空间时，才向上缩放内容。如果内容较大，则不会向下缩放
                case StretchDirection.UpOnly:
                    EnumString = "小于可用空间时向上缩放";
                    break;
                //只有当内容大于可用空间时，才向下缩放内容。如果内容较小，则不进行向上扩展
                case StretchDirection.DownOnly:
                    EnumString = "大于可用空间时向下缩放";
                    break;
            }
            return EnumString;
        }
        catch
        {
            return string.Empty;
        }
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}

public class EnumDescriptionConverter : IValueConverter
{
    private string GetEnumDescription(Enum enumObj)
    {
        FieldInfo fieldInfo = enumObj.GetType().GetField(enumObj.ToString());
        object[] attribArray = fieldInfo.GetCustomAttributes(false);

        if (attribArray.Length == 0)
            return enumObj.ToString();
        else
        {
            DescriptionAttribute attrib = null;

            foreach (var att in attribArray)
            {
                if (att is DescriptionAttribute)
                    attrib = att as DescriptionAttribute;
            }

            if (attrib != null)
                return attrib.Description;

            return enumObj.ToString();
        }
    }

    object IValueConverter.Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        Enum myEnum = (Enum)value;
        string description = GetEnumDescription(myEnum);
        return description;
    }

    object IValueConverter.ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return string.Empty;
    }
}


//public class EnumToStringConverter : IValueConverter
//{
//    public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
//    {
//        string parameterString = parameter as string;
//        if (parameterString == null)
//            return DependencyProperty.UnsetValue;

//        if (Enum.IsDefined(value.GetType(), value) == false)
//            return DependencyProperty.UnsetValue;

//        var desc = (value.GetType().GetField(parameterString).GetCustomAttributes(typeof(DescriptionAttribute), false).FirstOrDefault() as DescriptionAttribute);
//        if (desc != null)
//            return desc.Description;
//        else
//            return parameter.ToString();
//    }

//    public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
//    {
//        throw new NotImplementedException();
//    }
//}
