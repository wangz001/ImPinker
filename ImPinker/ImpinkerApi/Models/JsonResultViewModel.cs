using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ImpinkerApi.Models
{
    public class JsonResultViewModel
    {
        /// <summary>
        /// 请求状态 0：失败；1：成功
        /// </summary>
        public int IsSuccess { get; set; }
        /// <summary>
        /// 描述
        /// </summary>
        public string Description { get; set; }
        /// <summary>
        /// 业务数据
        /// </summary>
        public object Data { get; set; }
    }
}