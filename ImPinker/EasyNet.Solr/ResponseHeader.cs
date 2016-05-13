
namespace EasyNet.Solr
{
    using System.Text;

    /// <summary>
    /// 反回头信息
    /// </summary>
    public class ResponseHeader
    {
        /// <summary>
        /// 状态
        /// </summary>
        public int Status { get; set; }

        /// <summary>
        /// 花费时间
        /// </summary>
        public int QTime { get; set; }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();

            sb.Append("{\"ResponseHeader\":{\"Status\":\"");
            sb.Append(Status.ToString());
            sb.Append("\",\"QTime\":\"");
            sb.Append(QTime.ToString());
            sb.Append("\"}}");

            return sb.ToString();
        }

    }
}
