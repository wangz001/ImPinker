
namespace EasyNet.Solr.Impl
{
    using System.Collections.Generic;

    /// <summary>
    /// Solr更新操作
    /// </summary>
    /// <typeparam name="IT">输入数据类型</typeparam>
    /// <typeparam name="OT">输出数据类型</typeparam>
    public class SolrUpdateOperations<IT, OT> : ISolrUpdateOperations<OT>
    {
        private readonly ISolrUpdateConnection<IT, OT> connection;
        private readonly IUpdateParametersConvert<IT> updateParametersConvert;

        /// <summary>
        /// Solr的wt参数
        /// </summary>
        public string ResponseWriter { get; set; }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="connection">Solr更新连接</param>
        /// <param name="updateParametersConvert">更新操作参数转换器</param>
        public SolrUpdateOperations(ISolrUpdateConnection<IT, OT> connection, IUpdateParametersConvert<IT> updateParametersConvert)
        {
            this.connection = connection;
            this.updateParametersConvert = updateParametersConvert;
        }

        /// <summary>
        /// 更新操作
        /// </summary>
        /// <param name="collection">Solr collection</param>
        /// <param name="handler">更新地址</param>
        /// <param name="updateOptions">更新选项</param>
        /// <returns>OT类型返回数据</returns>
        public OT Update(string collection, string handler, UpdateOptions updateOptions)
        {
            var options = new Dictionary<string, ICollection<string>>();

            if (!string.IsNullOrEmpty(ResponseWriter))
            {
                options["wt"] = new string[] { ResponseWriter };
            }

            if (updateOptions.CommitOptions.HasValue)
            {
                options["commit"] = new string[] { "true" };

                if (updateOptions.CommitOptions.Value.WaitFlush.HasValue)
                {
                    options["waitFlush"] = new string[] { updateOptions.CommitOptions.Value.WaitFlush.Value ? "true" : "false" };
                }

                if (updateOptions.CommitOptions.Value.WaitSearcher.HasValue)
                {
                    options["waitSearcher"] = new string[] { updateOptions.CommitOptions.Value.WaitSearcher.Value ? "true" : "false" };
                }
            }

            if (updateOptions.OptimizeOptions.HasValue)
            {
                options["optimize"] = new string[] { "true" };

                if (updateOptions.OptimizeOptions.Value.WaitFlush.HasValue)
                {
                    options["waitFlush"] = new string[] { updateOptions.OptimizeOptions.Value.WaitFlush.Value ? "true" : "false" };
                }

                if (updateOptions.OptimizeOptions.Value.WaitSearcher.HasValue)
                {
                    options["waitSearcher"] = new string[] { updateOptions.OptimizeOptions.Value.WaitSearcher.Value ? "true" : "false" };
                }
            }

            return connection.Post(collection + handler, updateParametersConvert.ConvertUpdateParameters(updateOptions), options);
        }

        /// <summary>
        /// 回退
        /// </summary>
        /// <param name="collection">Solr collection</param>
        /// <param name="handler">更新操作</param>
        /// <returns>OT类型数据</returns>
        public OT Rollback(string collection, string handler)
        {
            var options = new Dictionary<string, ICollection<string>>();

            options["rollback"] = new string[] { "true" };

            return connection.Post(handler, updateParametersConvert.ConvertRollbackParameters(), options);
        }
    }
}
