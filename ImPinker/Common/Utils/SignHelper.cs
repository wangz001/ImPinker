using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace Common.Utils
{
    public sealed class SignHelper
    {
        public static string CreateLinkString(Dictionary<string, string> dic)
        {
            var prestr = new StringBuilder();
            foreach (var temp in dic)
            {
                prestr.Append(temp.Key.ToLower() + "=" + temp.Value + "&");
            }

            //去掉最後一個&字符
            var nLen = prestr.Length;
            prestr.Remove(nLen - 1, 1);
            return prestr.ToString();
        }

        public static string CreateSign(
            IDictionary<string, string> parameters, string secret)
        {
            parameters.Remove("sign");
            IDictionary<string, string> sortedParams
                = new SortedDictionary<string, string>(parameters);
            IEnumerator<KeyValuePair<string, string>> dem
                = sortedParams.GetEnumerator();
            StringBuilder query = new StringBuilder(secret);
            while (dem.MoveNext())
            {
                string key = dem.Current.Key;
                string value = dem.Current.Value;
                if (!string.IsNullOrEmpty(key)
                    && !string.IsNullOrEmpty(value))
                {
                    query.Append(key).Append(value);
                }
            }
            query.Append(secret);
            MD5 md5 = MD5.Create();
            byte[] bytes = md5.ComputeHash(
                Encoding.UTF8.GetBytes(query.ToString()));
            StringBuilder result = new StringBuilder();

            for (int i = 0; i < bytes.Length; i++)
            {
                string hex = bytes[i].ToString("X");
                if (hex.Length == 1)
                {
                    result.Append("0");
                }
                result.Append(hex);
            }

            return result.ToString();
        }
    }
}