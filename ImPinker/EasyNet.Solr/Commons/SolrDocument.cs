
namespace EasyNet.Solr.Commons
{
    using System;
    using System.Collections;
    using System.Collections.Generic;

    /// <summary>
    /// Solr返回文档
    /// </summary>
    public class SolrDocument : IDictionary<string, object>
    {
        private readonly IDictionary<string, object> fields = new LinkedHashMap<string, object>();

        /// <summary>
        /// 所有字段名称集合
        /// </summary>
        /// <returns></returns>
        public ICollection<String> GetFieldNames()
        {
            return fields.Keys;
        }

        /// <summary>
        /// 根据字段名称移除字段
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public bool RemoveFields(string name)
        {
            return fields.Remove(name);
        }

        /// <summary>
        /// 设置字段值
        /// </summary>
        /// <param name="name">字段名称</param>
        /// <param name="value">字段值</param>
        public void SetField(string name, object value)
        {
            if (value is object[])
            {
                value = new ArrayList((object[])value);
            }
            else if (value is string || value is ICollection || value is NamedList)
            {

            }
            else if (value is IEnumerable)
            {
                IList<object> lst = new List<object>();

                foreach (object o in (IEnumerable)value)
                {
                    lst.Add(o);
                }

                value = lst;
            }

            fields[name] = value;
        }

        #region IDictionary<string,object> Members

        public void Add(string key, object value)
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

        public bool TryGetValue(string key, out object value)
        {
            return fields.TryGetValue(key, out value);
        }

        public ICollection<object> Values
        {
            get { return fields.Values; }
        }

        public object this[string key]
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

        #region ICollection<KeyValuePair<string,object>> Members

        public void Add(KeyValuePair<string, object> item)
        {
            fields.Add(item);
        }

        public void Clear()
        {
            fields.Clear();
        }

        public bool Contains(KeyValuePair<string, object> item)
        {
            return fields.Contains(item);
        }

        public void CopyTo(KeyValuePair<string, object>[] array, int arrayIndex)
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

        public bool Remove(KeyValuePair<string, object> item)
        {
            return fields.Remove(item);
        }

        #endregion

        #region IEnumerable<KeyValuePair<string,object>> Members

        public IEnumerator<KeyValuePair<string, object>> GetEnumerator()
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
    }
}
