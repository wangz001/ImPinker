
namespace EasyNet.Solr.Commons
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Text.RegularExpressions;
    using System.Web;

    public static class Extension
    {
        private static readonly DateTime utcDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);

        public static string BuildParams(this IDictionary<string, ICollection<string>> parameters)
        {
            if (parameters == null)
            {
                return null;
            }

            var sb = new StringBuilder();
            var i = 0;

            foreach (var pair in parameters)
            {
                if (i != 0)
                {
                    sb.Append('&');
                }

                if (pair.Value != null)
                {
                    if (pair.Value.Count == 1)
                    {
                        sb.AppendFormat("{0}={1}", HttpUtility.UrlEncode(pair.Key), HttpUtility.UrlEncode(pair.Value.Take(1).Single()));
                    }
                    else
                    {
                        var j = 0;

                        foreach (var value in pair.Value)
                        {
                            if (j > 0)
                            {
                                sb.Append('&');
                            }

                            sb.AppendFormat("{0}={1}", HttpUtility.UrlEncode(pair.Key), HttpUtility.UrlEncode(value));

                            j++;
                        }
                    }
                }

                i++;
            }

            return sb.ToString();
        }

        public static string HTMLStrip(this string text)
        {
            text = Regex.Replace(text, @"<script[^>]*?>.*?</script>", "", RegexOptions.IgnoreCase);
            text = Regex.Replace(text, @"<script[\s\S]+</script *>", "", RegexOptions.IgnoreCase);
            text = Regex.Replace(text, @" href *= *[\s\S]*script *:", "", RegexOptions.IgnoreCase);
            text = Regex.Replace(text, @"([\r\n])[\s]+", "", RegexOptions.IgnoreCase);
            text = Regex.Replace(text, @" no[\s\S]*=", "", RegexOptions.IgnoreCase);
            text = Regex.Replace(text, @"<iframe[\s\S]+</iframe *>", "", RegexOptions.IgnoreCase);
            text = Regex.Replace(text, @"<frameset[\s\S]+</frameset *>", "", RegexOptions.IgnoreCase);
            text = Regex.Replace(text, @"\<img[^\>]+\>", "", RegexOptions.IgnoreCase);
            text = Regex.Replace(text, @"</p>", "", RegexOptions.IgnoreCase);
            text = Regex.Replace(text, @"<p>", "", RegexOptions.IgnoreCase);
            text = Regex.Replace(text, @"<[^>]*>", "", RegexOptions.IgnoreCase);
            text = Regex.Replace(text, @"<(.[^>]*)>", "", RegexOptions.IgnoreCase);
            text = Regex.Replace(text, @"([\r\n])[\s]+", "", RegexOptions.IgnoreCase);
            text = Regex.Replace(text, @"-->", "", RegexOptions.IgnoreCase);
            text = Regex.Replace(text, @"<!--.*", "", RegexOptions.IgnoreCase);
            text = Regex.Replace(text, @"&(quot|#34);", "\"", RegexOptions.IgnoreCase);
            text = Regex.Replace(text, @"&(amp|#38);", "&", RegexOptions.IgnoreCase);
            text = Regex.Replace(text, @"&(lt|#60);", "<", RegexOptions.IgnoreCase);
            text = Regex.Replace(text, @"&(gt|#62);", ">", RegexOptions.IgnoreCase);
            text = Regex.Replace(text, @"&(nbsp|#160);", "   ", RegexOptions.IgnoreCase);
            text = Regex.Replace(text, @"&(iexcl|#161);", "\xa1", RegexOptions.IgnoreCase);
            text = Regex.Replace(text, @"&(cent|#162);", "\xa2", RegexOptions.IgnoreCase);
            text = Regex.Replace(text, @"&(pound|#163);", "\xa3", RegexOptions.IgnoreCase);
            text = Regex.Replace(text, @"&(copy|#169);", "\xa9", RegexOptions.IgnoreCase);
            text = Regex.Replace(text, @"&#(\d+);", "", RegexOptions.IgnoreCase);
            text.Replace("<", "");
            text.Replace(">", "");
            text.Replace("\r\n", "");

            text = HttpUtility.HtmlEncode(text).Trim();

            return text;
        }

        public static object ConvertToNumberType(this string text)
        {
            var intNumber = 0;

            if (int.TryParse(text, out intNumber))
            {
                return intNumber;
            }

            long longNumber = 0;

            if (long.TryParse(text, out longNumber))
            {
                return longNumber;
            }

            float floatNumber = 0;

            if (float.TryParse(text, out floatNumber))
            {
                return floatNumber;
            }

            double doubleNumber = 0;

            if (double.TryParse(text, out doubleNumber))
            {
                return doubleNumber;
            }

            return text;
        }

        public static object ConvertToObject(this string text, string type)
        {
            if (type.Equals("number", StringComparison.OrdinalIgnoreCase))
            {
                return text.ConvertToNumberType();
            }
            else if (type.Equals("boolean", StringComparison.OrdinalIgnoreCase))
            {
                return bool.Parse(text);
            }
            else
            {
                return text;
            }
        }

        public static long ConvertToLong(this DateTime dateTime)
        {
            return (long)(dateTime).ToUniversalTime().Subtract(utcDateTime).TotalMilliseconds;
        }

        public static string ConvertToString(this DateTime dateTime)
        {
            return dateTime.ToString("yyyy-MM-ddThh:mm:ssZ");
        }

        public static DateTime ConvertToDateTime(this long value)
        {
            try
            {
                return utcDateTime.AddMilliseconds(value).ToLocalTime();
            }
            catch
            {
                return new DateTime();
            }
        }
    }
}
