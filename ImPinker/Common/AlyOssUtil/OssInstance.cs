
using Aliyun.OSS;

namespace Common.AlyOssUtil
{
    public class OssInstance
    {
        private const string AccessKeyId = "LTAILXbDypp1sHaO";

        private const string AccessKeySecret = "V754vCu6cAmjnrCIAqMxGvEADxd7NE";

        private const string Endpoint = "oss-cn-beijing.aliyuncs.com";


        static OssClient _client=null;

        /// <summary>
        /// 获取ossclient 实例
        /// </summary>
        /// <returns></returns>
        public static OssClient GetInstance()
        {
            if (_client==null)
            {
                _client = new OssClient(Endpoint, AccessKeyId, AccessKeySecret);
            }
            return _client;
        }
    }
}