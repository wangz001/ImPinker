using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Web;

namespace ImpinkerApi.Models
{
    /// <summary>
    /// 创建一个 Provider 用于重命名接收到的文件 
    /// </summary>
    public class CustomMultipartFormDataStreamProvider : MultipartFormDataStreamProvider
    {
        public CustomMultipartFormDataStreamProvider(string path) : base(path) { }
        public override string GetLocalFileName(HttpContentHeaders headers)
        {
            //var fileName = "weiboimage/" + DateTime.Now.ToString("yyyyMMdd") + "/" + headers.ContentDisposition.FileName.Replace("\"", string.Empty);
            var type = Path.GetExtension(headers.ContentDisposition.FileName.Replace("\"", string.Empty));
            var sb = new StringBuilder((DateTime.Now.Ticks.ToString(CultureInfo.InvariantCulture)).Replace("\"", "").Trim().Replace(" ", "_") + type);
            Array.ForEach(Path.GetInvalidFileNameChars(), invalidChar => sb.Replace(invalidChar, '-'));
            return sb.ToString();

        }
    }
}