using System.Collections.Generic;
using AhDal;
using AhModel;

namespace AhBll
{
    public class BasicMasterBrandBll
    {
        static readonly BasicMasterBrandDal _basicMasterBrandDal = new BasicMasterBrandDal();

        public static void AddBrand(BasicMasterBrand brand)
        {
            if (_basicMasterBrandDal.IsExit(brand))
            {
                if (_basicMasterBrandDal.NeedUpdate(brand))
                {
                    _basicMasterBrandDal.Update(brand);
                }
            }
            else
            {
                _basicMasterBrandDal.Insert(brand);
            }
        }


        public static List<BasicMasterBrand> GetAllMasterBrands(int companyId)
        {
            return _basicMasterBrandDal.GetAllMasterBrands(companyId);
        }

        
    }
}
