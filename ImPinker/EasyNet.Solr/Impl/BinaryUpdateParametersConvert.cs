
namespace EasyNet.Solr.Impl
{
    using System.Collections.Generic;

    using Commons;

    /// <summary>
    /// 二进制Solr更新参数转换器
    /// </summary>
    public class BinaryUpdateParametersConvert : IUpdateParametersConvert<NamedList>
    {
        public NamedList ConvertUpdateParameters(UpdateOptions updateOptions)
        {
            var namedList = new NamedList();
            var parameters = new NamedList();

            if (updateOptions.AddOptions.HasValue)
            {
                if (updateOptions.AddOptions.Value.CommitWithin.HasValue)
                {
                    parameters.Add("commitWithin", updateOptions.AddOptions.Value.CommitWithin.Value);
                }

                if (updateOptions.AddOptions.Value.Overwrite.HasValue)
                {
                    parameters.Add("overwrite", new string[] { updateOptions.AddOptions.Value.Overwrite.Value ? "true" : "false" });
                }
            }

            if (updateOptions.CommitOptions.HasValue)
            {
                parameters.Add("commit", new string[] { "true" });

                if (updateOptions.CommitOptions.Value.ExpungeDeletes.HasValue)
                {
                    parameters.Add("expungeDeletes ", new string[] { updateOptions.CommitOptions.Value.ExpungeDeletes.Value ? "true" : "false" });
                }

                if (updateOptions.CommitOptions.Value.WaitFlush.HasValue && (!updateOptions.OptimizeOptions.HasValue || (updateOptions.OptimizeOptions.HasValue && updateOptions.OptimizeOptions.Value.WaitFlush.HasValue)))
                {
                    parameters.Add("waitFlush", new string[] { updateOptions.CommitOptions.Value.WaitFlush.Value ? "true" : "false" });
                }

                if (updateOptions.CommitOptions.Value.WaitSearcher.HasValue && (!updateOptions.OptimizeOptions.HasValue || (updateOptions.OptimizeOptions.HasValue && updateOptions.OptimizeOptions.Value.WaitSearcher.HasValue)))
                {
                    parameters.Add("waitSearcher", new string[] { updateOptions.CommitOptions.Value.WaitSearcher.Value ? "true" : "false" });
                }
            }


            if (updateOptions.OptimizeOptions.HasValue)
            {
                parameters.Add("optimize", new string[] { "true" });

                if (updateOptions.OptimizeOptions.Value.MaxSegments.HasValue)
                {
                    parameters.Add("maxSegments", new string[] { updateOptions.OptimizeOptions.Value.MaxSegments.Value.ToString() });
                }

                if (updateOptions.OptimizeOptions.Value.WaitFlush.HasValue)
                {
                    parameters.Add("waitFlush", new string[] { updateOptions.OptimizeOptions.Value.WaitFlush.Value ? "true" : "false" });
                }

                if (updateOptions.OptimizeOptions.Value.WaitSearcher.HasValue)
                {
                    parameters.Add("waitSearcher", new string[] { updateOptions.OptimizeOptions.Value.WaitSearcher.Value ? "true" : "false" });
                }
            }

            namedList.Add("params", parameters);
            namedList.Add("delById", updateOptions.DelById);
            namedList.Add("delByQ", updateOptions.DelByQ);

            if (updateOptions.Docs != null)
            {
                var docIter = new List<IList<NamedList>>();

                foreach (var doc in updateOptions.Docs)
                {
                    docIter.Add(SolrInputDocumentToList(doc));
                }

                namedList.Add("docs", docIter.GetEnumerator());
            }

            return namedList;
        }

        public NamedList ConvertRollbackParameters()
        {
            var namedList = new NamedList();
            var paramsNamedList = new NamedList();

            paramsNamedList.Add("rollback", "true");

            namedList.Add("params", paramsNamedList);

            return namedList;
        }

        private IList<NamedList> SolrInputDocumentToList(SolrInputDocument doc)
        {
            var l = new List<NamedList>();

            var nl = new NamedList();

            nl.Add("boost", doc.Boost == 1.0f ? null : doc.Boost);
            l.Add(nl);

            foreach (var field in doc.Values)
            {
                nl = new NamedList();
                nl.Add("name", field.Name);
                nl.Add("val", field.Value);
                nl.Add("boost", field.Boost);

                l.Add(nl);
            }

            return l;
        }
    }
}
