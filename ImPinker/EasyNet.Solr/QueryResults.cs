
namespace EasyNet.Solr
{
    using System.Collections;
    using System.Collections.Generic;

    /// <summary>
    /// 查询结果泛型类
    /// </summary>
    /// <typeparam name="T">查询结果数据类型</typeparam>
    public class QueryResults<T> : IList<T>
    {
        private readonly IList<T> list = new List<T>();

        /// <summary>
        /// 符合条件的结果数量
        /// </summary>
        public long NumFound { get; set; }

        /// <summary>
        /// 最大评分
        /// </summary>
        public float? MaxScore { get; set; }

        #region List<T>接口实现

        public int IndexOf(T item)
        {
            return list.IndexOf(item);
        }

        public void Insert(int index, T item)
        {
            list.Insert(index, item);
        }

        public void RemoveAt(int index)
        {
            list.RemoveAt(index);
        }

        public T this[int index]
        {
            get
            {
                return list[index];
            }
            set
            {
                list[index] = value;
            }
        }

        public void Add(T item)
        {
            list.Add(item);
        }

        public void Clear()
        {
            list.Clear();
        }

        public bool Contains(T item)
        {
            return list.Contains(item);
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            list.CopyTo(array, arrayIndex);
        }

        public int Count
        {
            get { return list.Count; }
        }

        public bool IsReadOnly
        {
            get { return list.IsReadOnly; }
        }

        public bool Remove(T item)
        {
            return list.Remove(item);
        }

        public IEnumerator<T> GetEnumerator()
        {
            return list.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        #endregion
    }
}
