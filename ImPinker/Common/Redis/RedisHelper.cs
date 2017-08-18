using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Redis
{
    public class RedisHelper
    {
        private static string _conn = ConfigurationManager.AppSettings["SERedis"] ?? "127.0.0.1:6379";

        //http://www.cnblogs.com/bnbqian/p/4962855.html

    }
}
