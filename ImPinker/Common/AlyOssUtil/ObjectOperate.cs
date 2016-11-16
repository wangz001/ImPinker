using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Aliyun.OSS;
using Aliyun.OSS.Common;

namespace Common.AlyOssUtil
{

    public class ObjectOperate
    {
        private static readonly OssClient Client = OssInstance.GetInstance();

        /// <summary>
        /// 上传图片
        /// </summary>
        /// <param name="bucketName">bucket名称</param>
        /// <param name="sourcePath">本地图片地址</param>
        /// <param name="imgUrl">保存的图片url</param>
        /// <returns></returns>
        public static bool UploadImage(string bucketName, string sourcePath, string imgUrl)
        {
            var fileInfo = new FileInfo(sourcePath);
            if (!fileInfo.Exists)
            {
                return false;
            }
            if (!Client.DoesBucketExist(bucketName))
            {
                return false;
            }
            try
            {
                using (var content = File.Open(sourcePath, FileMode.Open))
                {
                    
                    var meta = new ObjectMetadata
                    {
                        ContentLength = content.Length, 
                        ContentType = "image/jpeg"
                    };

                    var result=Client.PutObject(bucketName, imgUrl, content, meta);
                    Console.WriteLine("Put object:{0} succeeded", imgUrl);
                    return true;
                }

            }
            catch (OssException ex)
            {
                Console.WriteLine("Failed with error code: {0}; Error info: {1}. \nRequestID:{2}\tHostID:{3}",
                    ex.ErrorCode, ex.Message, ex.RequestId, ex.HostId);
                return false;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Failed with error info: {0}", ex.Message);
                return false;
            }
        }


    }
}
