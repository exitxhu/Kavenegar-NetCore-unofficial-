using System.Globalization;

namespace Kavenegar_NetCore_unofficial_
{
    internal static class Helpers
    {
        public static long DateTimeToUnixTimestamp(this DateTime? dateTime)
        {
            if (!dateTime.HasValue) return 0;
            try
            {
                var idateTime = dateTime.Value;
                idateTime = new DateTime(idateTime.Year, idateTime.Month, idateTime.Day, idateTime.Hour, idateTime.Minute, idateTime.Second);
                TimeSpan unixTimeSpan = (idateTime - new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Local).ToLocalTime());
                return long.Parse(unixTimeSpan.TotalSeconds.ToString(CultureInfo.InvariantCulture));
            }
            catch
            {
                return 0;
            }
        }
        public static DateTime UnixTimestampToDateTime(this long unixTimeStamp)
        {
            try
            {
                return (new DateTime(1970, 1, 1, 0, 0, 0)).AddSeconds(unixTimeStamp);
            }
            catch
            {
                return DateTime.MaxValue;
            }
        }
    }
 
}