using System.Linq;

namespace AhBll
{
    /// <summary>
    /// 计算相似度
    /// </summary>
    public class SimilarCalculate
    {

        public static decimal GetSimilarityWith(string sourceString, string str)
        {

            decimal Kq = 2;
            decimal Kr = 1;
            decimal Ks = 1;

            char[] ss = sourceString.ToCharArray();
            char[] st = str.ToCharArray();

            //获取交集数量
            int q = ss.Intersect(st).Count();
            int s = ss.Length - q;
            int r = st.Length - q;

            return Kq * q / (Kq * q + Kr * r + Ks * s);
        }
    }
}
