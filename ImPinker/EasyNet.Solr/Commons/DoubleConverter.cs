
namespace EasyNet.Solr.Commons
{
    using System.Runtime.InteropServices;

    /// <summary>
    /// 
    /// </summary>
    [StructLayout(LayoutKind.Explicit)]
    public struct DoubleConverter
    {
        [FieldOffset(0)]
        private double doubleValue;
        [FieldOffset(0)]
        private long longValue;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <param name="converter"></param>
        /// <returns></returns>
        public static double ToDouble(long value, ref DoubleConverter converter)
        {
            converter.longValue = value;

            return converter.doubleValue;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <param name="converter"></param>
        /// <returns></returns>
        public static long ToLong(double value, ref DoubleConverter converter)
        {
            converter.doubleValue = value;

            return converter.longValue;
        }
    }
}
