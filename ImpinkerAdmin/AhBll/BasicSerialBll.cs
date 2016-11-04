using System.Collections.Generic;
using AhDal;
using AhModel;

namespace AhBll
{
    public class BasicSerialBll
    {
        static BasicSerialDal _serialDal = new BasicSerialDal();
        static BasicMasterBrandJoinSerialDal brandJoinSerialDal=new BasicMasterBrandJoinSerialDal();

        public static void AddSerial(BasicSerial model)
        {
            if (_serialDal.IsExit(model))
            {
                if (_serialDal.NeedUpdate(model))
                {
                    _serialDal.Update(model);
                }
            }
            else
            {
                _serialDal.Insert(model);
            }
        }


        public static void AddBrandJoinSerial(BasicMasterBrandJoinSerial model)
        {
            if (!brandJoinSerialDal.IsExit(model))
            {
                brandJoinSerialDal.Insert(model);
            }
        }

        public static List<BasicSerial> GetSerialsByMasterBrandId(int companyId, int id)
        {
            return _serialDal.GetSerialsByMasterBrandId(companyId,id);
        }

        public static List<BasicSerial> GetSerialsByMakeId(int companyId, int makeid)
        {
            return _serialDal.GetSerialsByMakeId(companyId, makeid);
        }

        public static BasicSerial GetSerialById(int companyId, int serialId)
        {
            return _serialDal.GetSerialById(companyId,serialId);
        }

        public static List<BasicSerial> GetSerial(int companyId)
        {
            return _serialDal.GetSerial(companyId);
        }

        /// <summary>
        /// 获取车系所属的主品牌
        /// </summary>
        /// <param name="basicSerialId"></param>
        /// <param name="companyId"></param>
        /// <returns></returns>
        public static BasicMasterBrand GetMasterBrand(int basicSerialId,int companyId)
        {
            return _serialDal.GetMasterBrand(basicSerialId,companyId);
        }
    }
}
