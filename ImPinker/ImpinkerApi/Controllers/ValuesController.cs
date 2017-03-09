using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using ImpinkerApi.Common;
using ImpinkerApi.Filters;

namespace ImpinkerApi.Controllers
{
    public class ValuesController : ApiController
    {
        // GET api/values
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/values/5
        [TokenCheck]
        public string Get(int id)
        {
            var userinfo = TokenHelper.GetUserInfoByHeader(Request.Headers);
            return "usrname:" + userinfo.UserName+"---"+userinfo.PassWord;
        }

        // POST api/values
        public string Post([FromBody]string value)
        {
            return "valuepost";
        }

        // PUT api/values/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/values/5
        public void Delete(int id)
        {
        }
    }
}