using System.Net.Http;
using System.Text;
using System.Web;
using System.Web.Http;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace ImpinkerApi.Controllers
{
    public class BaseApiController : ApiController
    {
        protected HttpResponseMessage GetJson(object obj)
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
