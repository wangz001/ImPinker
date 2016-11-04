using System.Collections.Generic;
using AhDal;
using AhModel;

namespace AhBll
{
    public class BasicPropertyMapBll
    {
        static BasicPropertyMapDal basicPropertyMapDal=new BasicPropertyMapDal();


        public static BasicPropertyMap GetMatchingBasicProperty(int companyId, int bitPropertyId)
        {
            var entity = basicPropertyMapDal.GetPropertyMap(companyId, bitPropertyId);
            
            return entity;
        }
        /// <summary>
        /// 获取全部的参配对应关系
        /// </summary>
        /// <returns></returns>
        public static List<BasicPropertyMap> GetMatchingBasicProperty()
        {
            var lists = basicPropertyMapDal.GetPropertyMap();
            return lists;
        }

        public static void Add(BasicPropertyMap map)
        {
            var entity = basicPropertyMapDal.GetPropertyMap(map.CompanyId, map.BitPropertyId);
            if (entity!=null)
            {
                basicPropertyMapDal.Update(map);
            }
            else
            {
                basicPropertyMapDal.Insert(map);
            }
        
        }
    }
}
