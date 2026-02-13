using System;

namespace Dorisoy.Pan.Helper
{
    public static class UTCDateTimeExtension
    {
        public static DateTime UTCDateTime(this DateTime dateTime)
        {
            if (dateTime.Kind == DateTimeKind.Unspecified)
                dateTime = DateTime.SpecifyKind(dateTime, DateTimeKind.Utc);
            return dateTime.ToUniversalTime();
        }
        public static DateTime? UTCToISTDateTime(this DateTime? dt)
        {
            if (dt?.Kind == DateTimeKind.Unspecified)
                dt = DateTime.SpecifyKind(Convert.ToDateTime(dt), DateTimeKind.Local);
            return dt?.ToLocalTime();
        }
    }
}
