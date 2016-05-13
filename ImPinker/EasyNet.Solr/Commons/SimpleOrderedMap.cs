
namespace EasyNet.Solr.Commons
{
    using System;

    /// <summary>
    /// 简单排序集合
    /// </summary>
    [Serializable]
    public class SimpleOrderedMap : NamedList
    { 
    
    }

    /// <summary>
    /// 简单排序泛型集合
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [Serializable]
    public class SimpleOrderedMap<T> : NamedList<T>
    {

    }
}
