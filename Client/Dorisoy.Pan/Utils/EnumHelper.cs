namespace Dorisoy.PanClient
{

    public class DescriptiveEnum<T> where T : Enum
    {
        public DescriptiveEnum(T value)
        {
            Value = value;

            FieldInfo fi = value.GetType().GetField(value.ToString());
            object[] attributes = fi?.GetCustomAttributes(typeof(DescriptionAttribute), false);
            Description = (attributes != null && attributes.Length != 0) ?
                                                                ((DescriptionAttribute)attributes[0]).Description :
                                                                value.ToString();
        }

        public string Description { get; set; }
        public T Value { get; set; }
    }

    public static class EnumHelper
    {
        public static IEnumerable<T> GetAllEnumValues<T>() where T : Enum
        {
            return Enum.GetValues(typeof(T)).Cast<T>();
        }

        public static IEnumerable<T> GetEnumValues<T>(params T[] exclude) where T : Enum
        {
            foreach (T item in Enum.GetValues(typeof(T)))
            {
                if (exclude != null && !exclude.Contains(item))
                {
                    yield return item;
                }
            }
        }

        public static IEnumerable<DescriptiveEnum<T>> GetDescriptiveEnums<T>(params T[] exclude) where T : Enum
        {
            foreach (T item in Enum.GetValues(typeof(T)))
            {
                if (exclude != null && !exclude.Contains(item))
                {
                    yield return new DescriptiveEnum<T>(item);
                }
            }
        }
    }
}
