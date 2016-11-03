using System;

namespace Common.Extensions
{
    public static class DateTimeExtension
    {
        /// <summary>
        /// 日期转换成unix时间戳
        /// </summary>
        /// <param name="dateTime"></param>
        /// <returns></returns>
        public static long DateTimeToUnixTimestamp(DateTime dateTime)
        {
            //var start = new DateTime(1970, 1, 1, 0, 0, 0, dateTime.Kind);
            //return Convert.ToInt64((dateTime - start).TotalSeconds);
            return (dateTime.ToUniversalTime().Ticks - 621355968000000000)/10000000;
        }

        /// <summary>
        /// unix时间戳转换成日期
        /// </summary>
        /// <param name="unixTimeStamp">时间戳（秒）</param>
        /// <returns></returns>
        public static DateTime UnixTimestampToDateTime(this DateTime target, long timestamp)
        {
            var start = new DateTime(1970, 1, 1, 0, 0, 0, target.Kind);
            return start.AddSeconds(timestamp);
        }
    }
}