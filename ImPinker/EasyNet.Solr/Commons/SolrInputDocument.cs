
namespace EasyNet.Solr.Commons
{
    using System.Collections;
    using System.Collections.Generic;

    /// <summary>
    /// Solr输入文档
    /// </summary>
    public class SolrInputDocument : IDictionary<string, SolrInputField>, IEnumerable<SolrInputField>
    {
        private readonly IDictionary<string, SolrInputField> fields = new LinkedHashMap<string, SolrInputField>();

        /// <summary>
        /// 文档评分
        /// </summary>
        public float? Boost { get; set; }

        /// <summary>
        /// 构造函数
        /// </summary>
        public SolrInputDocument()
        {
            Boost = 1.0f;
        }

        #region IDictionary<string,SolrInputField> Members

        public void Add(string key, SolrInputField value)
        {
            fields.Add(key, value);
        }

        public bool ContainsKey(string key)
        {
            return fields.ContainsKey(key);
        }

        public ICollection<string> Keys
        {
            get { return fields.Keys; }
        }

        public bool Remove(string key)
        {
            return fields.Remove(key);
        }

        public bool TryGetValue(string key, out SolrInputField value)
        {
            return fields.TryGetValue(key, out value);
        }

        public ICollection<SolrInputField> Values
        {
            get { return fields.Values; }
        }

        public SolrInputField this[string key]
        {
            get
            {
                return fields[key];
            }
            set
            {
                fields[key] = value;
            }
        }

        #endregion

        #region ICollection<KeyValuePair<string,SolrInputField>> Members

        public void Add(KeyValuePair<string, SolrInputField> item)
        {
            fields.Add(item);
        }

        public void Clear()
        {
            fields.Clear();
        }

        public bool Contains(KeyValuePair<string, SolrInputField> item)
        {
            return fields.Contains(item);
        }

        public void CopyTo(KeyValuePair<string, SolrInputField>[] array, int arrayIndex)
        {
            fields.CopyTo(array, arrayIndex);
        }

        public int Count
        {
            get { return fields.Count; }
        }

        public bool IsReadOnly
        {
            get { return fields.IsReadOnly; }
        }

        public bool Remove(KeyValuePair<string, SolrInputField> item)
        {
            return fields.Remove(item);
        }

        #endregion

        #region IEnumerable<KeyValuePair<string,SolrInputField>> Members

        public IEnumerator<KeyValuePair<string, SolrInputField>> GetEnumerator()
        {
            return fields.GetEnumerator();
        }

        #endregion

        #region IEnumerable Members

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        #endregion

        #region IEnumerable<SolrInputField> Members

        IEnumerator<SolrInputField> IEnumerable<SolrInputField>.GetEnumerator()
        {
            return fields.Values.GetEnumerator();
        }

        #endregion
    }
}
