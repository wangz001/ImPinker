using System;
using System.Collections.Generic;
using AhDal;
using AhModel;

namespace AhBll
{
    public class BasicMakeBll
    {
        static readonly BasicMakeDal _basicMakeDal = new BasicMakeDal();
        static readonly BasicMasterBrandJoinMakeDal basicMasterBrandJoinMakeDal = new BasicMasterBrandJoinMakeDal();

        public static void AddMake(BasicMake model)
        {
            if (_basicMakeDal.IsExit(model))
            {
                if (_basicMakeDal.NeedUpdate(model))
                {
                    _basicMakeDal.Update(model);
                }
            }
            else
            {
                _basicMakeDal.Insert(model);
            }
        }

        public static void AddMasterBrandJoinMake(BasicMasterBrandJoinMake model)
        {
            if (!basicMasterBrandJoinMakeDal.IsExit(model))
            {
                basicMasterBrandJoinMakeDal.Insert(model);
            }
        }

        /// <summary>
        /// 获取搜狐汽车的品牌id。
        /// </summary>
        /// <param name="makeName"></param>
        /// <param name="companyId"></param>
        /// <returns></returns>
        public static int GetShMakeId(string makeName, int companyId)
        {

            var id = _basicMakeDal.GetMakeId(makeName, companyId);
            if (id == 0) //表示不存在
            {
                var makeID = -100; // 从-100开始，递减
                var minId = _basicMakeDal.GetMinId();
                if (minId != null && (Int32.Parse(minId.ToString()) <= -100))
                {
                    makeID = Int32.Parse(minId.ToString()) - 1;
                }
                return makeID;
            }
            else
            {
                return id;
            }

        }

        public static List<BasicMake> GetMakesByMasterBrandId(int companyId, int id)
        {
            return _basicMakeDal.GetListsByMasterBrandId(companyId,id);
        }

        public static BasicMake GetMakeById(int companyId, int makeId)
        {
            return _basicMakeDal.GetMakeById(companyId,makeId);
        }
    }
}
