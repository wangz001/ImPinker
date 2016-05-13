
namespace EasyNet.Solr.Commons
{
    using System.Runtime.InteropServices;

    /// <summary>
    /// 
    /// </summary>
    [StructLayout(LayoutKind.Explicit)]
    public struct FloatConverter
    {
        [FieldOffset(0)]
        private float floatValue;
        [FieldOffset(0)]
        private int intValue;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <param name="converter"></param>
        /// <returns></returns>
        public static float ToFloat(int value, ref FloatConverter converter)
        {
            converter.intValue = value;

            return converter.floatValue;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <param name="converter"></param>
        /// <returns></returns>
        public static int ToInt(float value, ref FloatConverter converter)
        {
            converter.floatValue = value;

            return converter.intValue;
        }

    }
}
