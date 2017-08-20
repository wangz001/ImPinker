using Newtonsoft.Json;
using StackExchange.Redis;
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
        private static ConnectionMultiplexer _redis;
        private static readonly object _locker = new object();
        //http://www.cnblogs.com/bnbqian/p/4962855.html

        /// <summary>
        /// 获取redis实例
        /// </summary>
        public static ConnectionMultiplexer Manager
        {
            get
            {
                if (_redis == null)
                {
                    lock (_locker)
                    {
                        if (_redis != null) return _redis;

                        _redis = GetManager();
                        return _redis;
                    }
                }

                return _redis;
            }
        }

        private static ConnectionMultiplexer GetManager(string connectionString = null)
        {
            if (string.IsNullOrEmpty(connectionString))
            {
                connectionString = _conn;
            }
            return ConnectionMultiplexer.Connect(connectionString);
        }


        /// <summary>
        /// 根据Key获取值
        /// </summary>
        /// <param name="key">键值</param>
        /// <returns>System.String.</returns>
        public static string StringGet(string key)
        {
            try
            {
                var client = RedisHelper.Manager;
                return client.GetDatabase().StringGet(key);
            }
            catch (Exception)
            {
                return null;
            }
        }
        /// <summary>
        /// 单条存值
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="expireMinutes"></param>
        /// <returns></returns>
        public static bool Set(string key, string value, int expireMinutes = 0)
        {
            var db = RedisHelper.Manager.GetDatabase();

            if (expireMinutes > 0)
            {
                return db.StringSet(key, value, TimeSpan.FromMinutes(expireMinutes));
            }
            else
            {
                return db.StringSet(key, value);
            }
        }

        #region 泛型
        /// <summary>
        /// 存值并设置过期时间
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key">key</param>
        /// <param name="t">实体</param>
        /// <param name="ts">过期时间间隔</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        public static bool Set<T>(string key, T t, TimeSpan ts)
        {
            var str = JsonConvert.SerializeObject(t);
            var db = RedisHelper.Manager.GetDatabase();
            return db.StringSet(key, str, ts);
        }

        /// <summary>
        /// 
        /// 根据Key获取值
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key">The key.</param>
        /// <returns>T.</returns>
        public static T Get<T>(string key) where T : class
        {
            var db = RedisHelper.Manager.GetDatabase();
            var strValue = db.StringGet(key);
            return string.IsNullOrEmpty(strValue) ? null : JsonConvert.DeserializeObject<T>(strValue);
        }
        #endregion

    }
}
