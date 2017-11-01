using System.Configuration;
using System.Data;
using System.IO;
using Aliyun.Api;
using Common.AlyOssUtil;
using Common.Utils;
using ImDal;
using System;
using System.Collections.Generic;
using System.Linq;
using ImModel;
using ImModel.Enum;
using ImModel.ViewModel;

namespace ImBLL
{
    public class WeiBoBll
    {
        private readonly WeiBoDal _weiBoDal = new WeiBoDal();

        /// <summary>
        /// 上传微博图片到oss(大图和缩略图 _s.jpg),返回oss图片路径
        /// </summary>
        /// <returns></returns>
        public string UploadWeiBoimgToOss(string bucketName, int userid, string localFileName)
        {
            string imgUrlformat = ConfigurationManager.AppSettings["WeiboImage"];
            var imgUrl = string.Format(imgUrlformat, DateTime.Now.ToString("yyyyMMdd"), userid, DateTime.Now.Ticks);
            //上传到oss
            var flag1 = ObjectOperate.UploadImage(bucketName, localFileName, imgUrl, 1024);
            return flag1 ? imgUrl : "";

            /*
             * 不再上传缩略图。由阿里云oss动态生成缩略图。
             //上传缩略图到oss
           var extention = Path.GetExtension(localFileName);
           if (extention != null)
           {
               var sLocalPath = localFileName.Replace(extention, "_s.jpg");
               ImageUtils.GetReduceImgFromCenter(300, 200, localFileName, sLocalPath, 85);
               var sImgUrl = imgUrl.Replace(".jpg", "_s.jpg");
               var flag2 = ObjectOperate.UploadImage(bucketName, sLocalPath, sImgUrl,100);
               return (flag1 && flag2) ? imgUrl : "";
           }
           return "";
             */
        }


        public bool AddWeiBo(WeiBo model)
        {
            var flag = _weiBoDal.AddWeiBo(model);
            return flag;
        }
        /// <summary>
        /// 分页获取数据
        /// </summary>
        /// <param name="pageindex"></param>
        /// <param name="pagesize"></param>
        /// <returns></returns>
        public List<WeiboVm> GetListByPage(int pageindex, int pagesize)
        {
            var resultList = new List<WeiboVm>();
            var ds = _weiBoDal.GetListByPage(pageindex, pagesize);
            if (ds != null && ds.Tables[0].Rows.Count > 0)
            {
                resultList.AddRange(from DataRow dataRow in ds.Tables[0].Rows select DataRowToModel(dataRow));
            }
            return resultList;
        }

        /// <summary>
        /// 分页获取数据,获取用户微博列表
        /// </summary>
        /// <param name="userid"></param>
        /// <param name="pageindex"></param>
        /// <param name="pagesize"></param>
        /// <returns></returns>
        public List<WeiboVm> GetListByPage(int userid, int pageindex, int pagesize)
        {
            var resultList = new List<WeiboVm>();
            var ds = _weiBoDal.GetListByPage(userid, pageindex, pagesize);
            if (ds != null && ds.Tables[0].Rows.Count > 0)
            {
                resultList.AddRange(from DataRow dataRow in ds.Tables[0].Rows select DataRowToModel(dataRow));
            }
            return resultList;
        }

        /// <summary>
        /// 得到一个对象实体
        /// </summary>
        public WeiboVm DataRowToModel(DataRow row)
        {
            var model = new WeiboVm();
            if (row != null)
            {
                if (row.Table.Columns.Contains("Id") && row["Id"] != null && row["Id"].ToString() != "")
                {
                    model.Id = long.Parse(row["Id"].ToString());
                }
                if (row.Table.Columns.Contains("UserId") && row["UserId"] != null && row["UserId"].ToString() != "")
                {
                    model.UserId = Int32.Parse(row["UserId"].ToString());
                }
                if (row.Table.Columns.Contains("Description") && row["Description"] != null)
                {
                    model.Description = row["Description"].ToString();
                }
                if (row.Table.Columns.Contains("ContentValue") && row["ContentValue"] != null)
                {
                    model.ContentValue = row["ContentValue"].ToString();
                }
                if (row.Table.Columns.Contains("ContentType") && row["ContentType"] != null && row["ContentType"].ToString() != "")
                {
                    model.ContentType = (WeiBoContentTypeEnum)int.Parse(row["ContentType"].ToString());
                }
                if (row.Table.Columns.Contains("LocationText") && row["LocationText"] != null)
                {
                    model.LocationText = row["LocationText"].ToString();
                }
                if (row.Table.Columns.Contains("Longitude") && row["Longitude"] != null)
                {
                    model.Longitude = decimal.Parse(row["Longitude"].ToString());
                }
                if (row.Table.Columns.Contains("Latitude") && row["Latitude"] != null)
                {
                    model.Lantitude = decimal.Parse(row["Latitude"].ToString());
                }
                if (row.Table.Columns.Contains("Height") && row["Height"] != null)
                {
                    model.Height = decimal.Parse(row["Height"].ToString());
                }
                if (row.Table.Columns.Contains("State") && row["State"] != null && row["State"].ToString() != "")
                {
                    model.State = (WeiBoStateEnum)int.Parse(row["State"].ToString());
                }
                if (row.Table.Columns.Contains("HardWareType") && row["HardWareType"] != null)
                {
                    model.HardWareType = row["HardWareType"].ToString();
                }
                if (row.Table.Columns.Contains("IsRePost") && row["IsRePost"] != null)
                {
                    model.IsRePost = Boolean.Parse(row["IsRePost"].ToString());
                }
                if (row.Table.Columns.Contains("CreateTime") && row["CreateTime"] != null && row["CreateTime"].ToString() != "")
                {
                    model.CreateTime = DateTime.Parse(row["CreateTime"].ToString());
                }
                if (row.Table.Columns.Contains("UpdateTime") && row["UpdateTime"] != null && row["UpdateTime"].ToString() != "")
                {
                    model.UpdateTime = DateTime.Parse(row["UpdateTime"].ToString());
                }
                if (row.Table.Columns.Contains("VoteCount") && row["VoteCount"] != null && row["VoteCount"].ToString() != "")
                {
                    model.VoteCount = Int32.Parse(row["VoteCount"].ToString());
                }
                if (row.Table.Columns.Contains("CommentCount") && row["CommentCount"] != null && row["CommentCount"].ToString() != "")
                {
                    model.CommentCount = Int32.Parse(row["CommentCount"].ToString());
                }
            }
            return model;
        }

        /// <summary>
        /// 根据weiboid 获取实体
        /// </summary>
        /// <param name="weiboid"></param>
        /// <returns></returns>
        public WeiboVm GetById(long weiboid)
        {
            var ds = _weiBoDal.GetById(weiboid);
            if (ds!=null&&ds.Tables[0].Rows.Count>0)
            {
                var item = DataRowToModel(ds.Tables[0].Rows[0]);
                return item;
            }
            return null;
        }
    }
}
