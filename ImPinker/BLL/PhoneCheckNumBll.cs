using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Top.Api;
using Top.Api.Request;
using Top.Api.Response;

namespace ImBLL
{
    public class PhoneCheckNumBll
    {

        #region 验证码相关
        /// <summary>
        /// 存储电话号码和验证码。注册成功后调用移除方法，超过30分钟的记录都移除。
        /// </summary>
        static Dictionary<string, CheckNumModel> phoneNumDic = new Dictionary<string, CheckNumModel>();

        /// <summary>
        /// 发送验证码
        /// </summary>
        /// <param name="phoneNum"></param>
        /// <param name="operateEnum"></param>
        /// <returns></returns>
        public static bool Send(string phoneNum, SendCheckNumOperateEnum operateEnum)
        {
            RemoveFromPhoneNumDic();
            if (!IsSendAllow(phoneNum))
            {
                return false;
            }
            //根据操作类型选择验证码模板
            var checkNum = new Random().Next(100000, 999999);//验证码随机数生成
            string smsTemplateCode = ConfigurationManager.AppSettings["SmsTemplateCode"];
            const string url = "http://gw.api.taobao.com/router/rest";  //测试环境
            const string appkey = "23546735";
            const string secret = "5ccfde439d81c9ae0aeb2df33fa6421e";
            ITopClient client = new DefaultTopClient(url, appkey, secret);
            var req = new AlibabaAliqinFcSmsNumSendRequest();
            req.Extend = phoneNum;
            req.SmsType = "normal";
            req.SmsFreeSignName = "车酷网";
            req.SmsParam = string.Format("{{checknum:'{0}'}}", checkNum);
            req.RecNum = phoneNum;
            req.SmsTemplateCode = smsTemplateCode;
            AlibabaAliqinFcSmsNumSendResponse rsp = client.Execute(req);
            if (rsp.IsError)
            {
                return false;
            }
            AddDic(phoneNum, checkNum.ToString(), operateEnum);
            return true;
        }
        /// <summary>
        /// 判断验证码是否有效
        /// </summary>
        /// <param name="phoneNum"></param>
        /// <param name="checkNum"></param>
        /// <param name="operateEnum"></param>
        /// <returns></returns>
        public static bool CheckPhoneNum(string phoneNum, string checkNum, SendCheckNumOperateEnum operateEnum)
        {
            if (phoneNumDic.ContainsKey(phoneNum)
                    && (DateTime.Now - phoneNumDic[phoneNum].SendTime).TotalSeconds > 600)
            {
                //AddErrors(IdentityResult.Failed("手机验证码超时，10分钟内有效。请重新获取验证码"));
                return false;
            }
            if (!(phoneNumDic.ContainsKey(phoneNum)
                && phoneNumDic[phoneNum].CheckNum.Equals(checkNum)))
            {
                //AddErrors(IdentityResult.Failed("您输入的手机验证码有误"));
                return false;
            }
            if (phoneNumDic[phoneNum].OperateEnum!=operateEnum)
            {
                //验证码对应的操作不一致
                return false;
            }
            return true;
        }
         /// <summary>
        /// 是否1分钟内已发送过验证码等，后期要加上ip验证，防止恶意注册
        /// </summary>
        /// <param name="phoneNum"></param>
        /// <returns>ture,可以继续发送；false：不可继续发送</returns>
        private static bool IsSendAllow(string phoneNum)
        {
            if (phoneNumDic.ContainsKey(phoneNum))
            {
                var model = phoneNumDic[phoneNum];
                if ((DateTime.Now - model.SendTime).TotalSeconds < 80)//间隔时间小于80秒
                {
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// 移除过期的记录
        /// </summary>
        private static void RemoveFromPhoneNumDic()
        {
            if (phoneNumDic.Count > 0)
            {
                foreach (string key in phoneNumDic.Keys)
                {
                    var model = phoneNumDic[key];
                    if ((DateTime.Now - model.SendTime).TotalMinutes > 30)
                    {
                        phoneNumDic.Remove(key);
                    }
                }
            }
        }

        /// <summary>
        /// 向字典中加入数据
        /// </summary>
        /// <param name="phoneNum"></param>
        /// <param name="checkNum"></param>
        /// <param name="operativeEnum"></param>
        private static void AddDic(string phoneNum, string checkNum,SendCheckNumOperateEnum operativeEnum)
        {
            var model = new CheckNumModel
            {
                PhoneNum = phoneNum,
                CheckNum = checkNum,
                SendTime = DateTime.Now,
                OperateEnum = operativeEnum
            };
            if (phoneNumDic.ContainsKey(phoneNum))
            {
                phoneNumDic[phoneNum] = model;
            }
            else
            {
                phoneNumDic.Add(phoneNum, model);
            }
        }
       
        #endregion
    
    }

    /// <summary>
    /// 注册时手机验证码验证
    /// </summary>
    public class CheckNumModel
    {
        /// <summary>
        /// 手机号
        /// </summary>
        public string PhoneNum { get; set; }
        /// <summary>
        /// 验证码
        /// </summary>
        public string CheckNum { get; set; }
        /// <summary>
        /// 验证码发送时间
        /// </summary>
        public DateTime SendTime { get; set; }
        /// <summary>
        /// 用户请求的ip地址。防止恶意注册
        /// </summary>
        public string IpAddress { get; set; }
        /// <summary>
        /// 操作类型
        /// </summary>
        public SendCheckNumOperateEnum OperateEnum { get; set; }
    }
    /// <summary>
    /// 发送验证码的操作类型
    /// </summary>
    public enum SendCheckNumOperateEnum
    {
        /// <summary>
        /// 注册
        /// </summary>
        Regist=1,
        /// <summary>
        /// 手机登录
        /// </summary>
        LoginByPhone=2,
        /// <summary>
        /// 找回密码
        /// </summary>
        FindPassword=3
    }
}
