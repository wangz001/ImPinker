using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Web;
using System.Web.Mvc;
using ImPinker.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace ImPinker.Common
{
    /// <summary>
    /// ajax请求统一返回方法。标识请求状态，返回数据等
    /// </summary>
    public class JsonResultHelper
    {
        public static HttpResponseMessage GetJson(object obj)
        {
            var dtConverter = new IsoDateTimeConverter
            {
                DateTimeFormat = "yyyy-MM-dd HH:mm:ss"
            };
            var jsonStr = JsonConvert.SerializeObject(obj, dtConverter);
            var request = HttpContext.Current.Request;
            if (!string.IsNullOrEmpty(request.QueryString["callback"]))
            {
                var callback = request.QueryString["callback"];
                jsonStr = string.Format("{0}({1});", callback, jsonStr);
            }
            var result = new HttpResponseMessage
            {
                Content = new StringContent(jsonStr, Encoding.GetEncoding("UTF-8"), "application/json")
            };
            return result;
        }

    }
}