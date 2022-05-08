using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace SinGooCMS.Utility.Extension
{
    /// <summary>
    /// 字典扩展类
    /// </summary>
    public static class DictionaryExtension
    {
        /// <summary>
        /// 按key排序
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="dict"></param>
        /// <returns></returns>
        public static IDictionary<TKey, TValue> SortByKey<TKey, TValue>(this IDictionary<TKey, TValue> dict)
        {
            return dict.OrderBy(p => p.Key).ToDictionary(p => p.Key, p => p.Value);
        }

        /// <summary>
        /// 按下标读取元素Key
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="dict"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        public static TKey GetKey<TKey, TValue>(this IDictionary<TKey, TValue> dict, int @index)
        {
            return dict.ElementAt(@index).Key;
        }

        /// <summary>
        /// 按下标读取元素Value
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="dict"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        public static TValue GetValue<TKey, TValue>(this IDictionary<TKey, TValue> dict, int @index)
        {
            return dict.ElementAt(@index).Value;
        }
    }
}
