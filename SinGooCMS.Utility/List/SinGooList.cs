using System;
using System.Collections;
using System.Collections.Generic;

namespace SinGooCMS.Utility
{
    /// <summary>
    /// List扩展，实现取范围数据
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class SinGooList<T> : List<T>
    {
        /// <summary>
        /// 获取一段数据，返回一个新列表
        /// </summary>
        /// <param name="indexBeg">起始下标，下标从0开始</param>
        /// <param name="indexEnd">截至下标，下标从0开始</param>
        /// <returns></returns>
        public SinGooList<T> this[int indexBeg, int indexEnd]
        {
            get
            {
                if (indexBeg < 0 || indexEnd < indexBeg || indexEnd > this.Count - 1)
                    return null;

                var lst = new SinGooList<T>();
                for (var i = 0; i < this.Count; i++)
                {
                    if (i < indexBeg || i > indexEnd)
                        continue;

                    lst.Add(this[i]);
                }

                return lst;
            }
        }
    }

    /// <summary>
    /// ArrayList扩展，实现取范围数据
    /// </summary>
    public class SinGooArray : ArrayList
    {
        /// <summary>
        /// 获取一段数据，返回一个新数组
        /// </summary>
        /// <param name="indexBeg">起始下标，下标从0开始</param>
        /// <param name="indexEnd">截至下标，下标从0开始</param>
        /// <returns></returns>
        public object this[int indexBeg, int indexEnd]
        {
            get
            {
                if (indexBeg < 0 || indexEnd < indexBeg || indexEnd > this.Count - 1)
                    return null;

                var arr = new ArrayList();
                for (var i = 0; i < this.Count; i++)
                {
                    if (i < indexBeg || i > indexEnd)
                        continue;

                    arr.Add(this[i]);
                }

                return arr;
            }
        }

    }

    /// <summary>
    /// Dictionary扩展，实现从下标取值
    /// </summary>
    /// <typeparam name="T1"></typeparam>
    /// <typeparam name="T2"></typeparam>
    public class SinGooDictionary<T1, T2> : Dictionary<T1, T2>
    {
        /// <summary>
        /// 获取一段数据，返回一个新字典
        /// </summary>
        /// <param name="indexBeg">起始下标，下标从0开始</param>
        /// <returns></returns>
        public object this[int indexBeg]
        {
            get
            {
                if (indexBeg < 0 || indexBeg > this.Count - 1)
                    return null;

                int counter = 0;
                var dict = new Dictionary<T1, T2>();
                foreach (KeyValuePair<T1, T2> item in this)
                {
                    if (indexBeg == counter)
                    {
                        dict.Add(item.Key,item.Value);
                        break;
                    }
                }

                return dict;
            }
        }

        /// <summary>
        /// 获取一段数据，返回一个新字典
        /// </summary>
        /// <param name="indexBeg">起始下标，下标从0开始</param>
        /// <param name="indexEnd">截至下标，下标从0开始</param>
        /// <returns></returns>
        public object this[int indexBeg, int indexEnd]
        {
            get
            {
                if (indexBeg < 0 || indexEnd < indexBeg || indexEnd > this.Count - 1)
                    return null;

                int counter = 0;
                var dict = new Dictionary<T1, T2>();
                foreach (KeyValuePair<T1, T2> item in this)
                {
                    if (counter < indexBeg || counter > indexEnd)
                    {
                        counter++;
                        continue;
                    }
                    else
                    {
                        dict.Add(item.Key, item.Value);
                        counter++;
                    }
                }

                return dict;
            }
        }
    }
}
