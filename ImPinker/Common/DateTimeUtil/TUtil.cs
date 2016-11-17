using System;

namespace Common.DateTimeUtil
{
    public class TUtil
    {
        /// <summary>
        /// 获取跟当前时间差，返回 一天前，1小时前等等
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        public static string DateFormatToString(DateTime dt)
        {
            TimeSpan span = (DateTime.Now - dt).Duration();
            // if (span.TotalDays > 60)
            if (span.TotalDays > 2)
            {
                return dt.ToString("yyyy-MM-dd");
            }
            else if (span.TotalDays > 30)
            {
                return "1个月前";
            }
            else if (span.TotalDays > 14)
            {
                return "2周前";
            }
            else if (span.TotalDays > 7)
            {
                return "1周前";
            }
            else if (span.TotalDays > 1)
            {
                return string.Format("{0}天前", (int)Math.Floor(span.TotalDays));
            }
            else if (span.TotalHours > 1)
            {
                return string.Format("{0}小时前", (int)Math.Floor(span.TotalHours));
            }
            else if (span.TotalMinutes > 1)
            {
                return string.Format("{0}分钟前", (int)Math.Floor(span.TotalMinutes));
            }
            else if (span.TotalSeconds >= 1)
            {
                return string.Format("{0}秒前", (int)Math.Floor(span.TotalSeconds));
            }
            else
            {
                return "1秒前";
            }
        }
    }
}
