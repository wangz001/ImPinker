using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using Aliyun.OSS;
using Aliyun.OSS.Common;

namespace Common.AlyOssUtil
{
    public class BucketOperate
    {
        private static readonly OssClient Client = OssInstance.GetInstance();
        /// <summary>
        /// 获得所有bucket
        /// </summary>
        /// <returns></returns>
        public IEnumerable<Bucket> GetAllBuckets()
        {
            IEnumerable<Bucket> buckets = null;
            try
            {
                buckets = Client.ListBuckets();
            }
            catch (OssException ex)
            {
                Console.WriteLine("Failed with error code: {0}; Error info: {1}. \nRequestID:{2}\tHostID:{3}",
                                  ex.ErrorCode, ex.Message, ex.RequestId, ex.HostId);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Failed with error info: {0}", ex.Message);
            }
            return buckets;
        }



        /// <summary>
        /// 添加bucket
        /// </summary>
        /// <param name="bucketName"></param>
        public static bool CreateBucket(string bucketName)
        {
            try
            {
                if (Client.DoesBucketExist(bucketName))
                {
                    Console.WriteLine("existed bucket name:{0}  ", bucketName);
                    return false;
                }
                Client.CreateBucket(bucketName);
                Console.WriteLine("Created bucket name:{0} succeeded ", bucketName);
                return true;
            }
            catch (OssException ex)
            {
                Console.WriteLine("Failed with error info: {0}; Error info: {1}. \nRequestID:{2}\tHostID:{3}",
                                  ex.ErrorCode, ex.Message, ex.RequestId, ex.HostId);
            }
            return false;

        }
    }
}
