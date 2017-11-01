
using System.Net.Http;
using System.Web.Helpers;
using System.Web.Http;
using System.Web.UI;
using ImBLL;
using ImpinkerApi.Models;

namespace ImpinkerApi.Controllers
{
    public class SearchController : BaseApiController
    {
        //
        // GET: /Search/
        [HttpGet]
        public HttpResponseMessage GetWeiboByGeo(int userid,int weiboId,string lat,string lng,int distance,int pagenum,int pagesize)
        {
            pagenum = pagenum > 0 ? pagenum : 1;
            pagesize = pagesize > 0 ? pagesize : 10;
            double dLat;
            double dLng;
            double.TryParse(lat, out dLat);
            double.TryParse(lng, out dLng);
            if (!string.IsNullOrEmpty(lat)&&!string.IsNullOrEmpty(lng)&&dLat>0&&dLng>0)
            {
                var searchVm = SolrNetSearchBll.QueryWeiboByGeo(dLat, dLng, distance, userid>0? userid:0,pagenum,pagesize);
                return GetJson(new JsonResultViewModel
                {
                    IsSuccess = 1,
                    Data = searchVm,
                    Description = "search"
                });
            }
            //http://api.impinker.com/api/Search/GetWeiboByGeo?weiboId=1209&lat=36.702109&lng=119.033300&distance=10&pagenum=1&pagesize=10&userid=1
            
            return GetJson(new JsonResultViewModel
            {
                IsSuccess = 0,
                Data = null,
                Description = "参数错误"
            });
        }

    }
}
