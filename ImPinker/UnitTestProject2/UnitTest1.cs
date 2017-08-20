using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTestProject2
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestRedisGet()
        {
            var flag = Common.Redis.RedisHelper.Set("aaaaa", "奔驰G500", 0);

            var str=Common.Redis.RedisHelper.StringGet("aaaaa");

        }
    }
}
