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
using Common.Utils;

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
        /// <param name="maxSize">最大图片。单位k</param>
        /// <returns></returns>
        public static bool UploadImage(string bucketName, string sourcePath, string imgUrl,int maxSize)
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
            if (fileInfo.Length/1024 > maxSize) //fileInfo.Length 单位是byte。 1024K
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
                LogHelper.Instance.Error(ex.ToString());
                return false;
            }
            catch (Exception ex)
            {
                LogHelper.Instance.Error(string.Format("Failed with error info: {0}", ex.Message));
                return false;
            }
        }


    }
}
