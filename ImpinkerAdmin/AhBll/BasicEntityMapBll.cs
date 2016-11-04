using System;
using System.Collections.Generic;
using System.Linq;
using AhDal;
using AhModel;

namespace AhBll
{

    public class BasicEntityMapBll
    {
        static readonly BasicEntityMapDal BasicEntityMapDal = new BasicEntityMapDal();

        public  static bool AddOrUpdateEntityMap(int bitId, int compareId
            , int companyId, int entityType, int isPeopleSet)
        {
            var result = false;
            var entity = new BasicEntityMap
            {
                BitEntityId = bitId,
                CompareEntityId = compareId,
                CompanyId = companyId,
                EntityType = entityType,
                IsPeopleSet = isPeopleSet,
                CreateTime = DateTime.Now,
                UpdateTime = DateTime.Now
            };
            var entityMap = BasicEntityMapDal.GetEntityMap(bitId, companyId, entityType);
            if (entityMap == null)
            {
                result=BasicEntityMapDal.Insert(entity);
            }
            if (entityMap != null && isPeopleSet == 0 && entityMap.IsPeopleSet == 0)
            {
                entity.Id = entityMap.Id;
                result=BasicEntityMapDal.Update(entity);
            }
            if (entityMap != null && isPeopleSet == 1)
            {
                entity.Id = entityMap.Id;
                result=BasicEntityMapDal.Update(entity);
            }
            return result;
        }

        /// <summary>
        /// 删除车系对应关系
        /// </summary>
        /// <param name="bitId">易车车系/车型id</param>
        /// <param name="companyId">竞品id</param>
        /// <param name="entityType">车系/车型</param>
        /// <returns></returns>
        public static bool DeleteEntityMap(int bitId, int companyId, int entityType)
        {
            return BasicEntityMapDal.Delete(bitId,companyId,entityType);
        }

        public static IEnumerable<BasicEntityMap> GetCompareStyles(int bitStyleId,int entityType)
        {
            return  BasicEntityMapDal.GetEntityMaps(bitStyleId,entityType);

        }

        /// <summary>
        /// 根据易车车系Id获取对应的其他品牌车系
        /// </summary>
        /// <param name="modelType"></param>
        /// <param name="bitModelIds"></param>
        /// <returns></returns>
        public static List<BasicEntityMap> GetBasicEntityMaps(int modelType, IEnumerable<int> bitModelIds)
        {
            var modelIds = bitModelIds as int[] ?? bitModelIds.ToArray();
            if (modelIds.Any())
            {
                var bitIds = string.Join(",", modelIds);
                return BasicEntityMapDal.GetEntityMaps(modelType,bitIds);
            }
            return null;
        }
    }
}
