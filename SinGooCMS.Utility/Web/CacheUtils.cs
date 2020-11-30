using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
#if NETSTANDARD2_1
using Microsoft.Extensions.Caching.Memory;
#else
using System.Web;
using System.Web.Caching;
#endif

namespace SinGooCMS.Utility
{
    /// <summary>
    /// 缓存管理
    /// </summary>
    public class CacheUtils
    {
#if NETSTANDARD2_1
        private static IMemoryCache cache;
        static CacheUtils()
        {
            cache = new MemoryCache(new MemoryCacheOptions() { });
        }
#else
        private static System.Web.Caching.Cache cache;
        static CacheUtils()
        {
            cache = System.Web.HttpRuntime.Cache;
        }
#endif

        #region Get

        /// <summary>
        /// 读取缓存对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <returns></returns>
        public T Get<T>(string key)
        {
#if NETSTANDARD2_1
            return cache.Get<T>(key);
#else
            return (T)cache[key];
#endif
        }

        /// <summary>
        /// 是否包含key
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public bool ContainKey(string key) =>
            GetAllKeys().Contains(key);

        /// <summary>
        /// 读取所有缓存键
        /// </summary>
        /// <returns></returns>
        public IList<string> GetAllKeys()
        {
            var keys = new List<string>();
#if NETSTANDARD2_1
            const BindingFlags flags = BindingFlags.Instance | BindingFlags.NonPublic;
            var entries = cache.GetType().GetField("_entries", flags).GetValue(cache);
            var cacheItems = entries as IDictionary;
            
            if (cacheItems == null) return keys;
            foreach (DictionaryEntry cacheItem in cacheItems)
            {
                keys.Add(cacheItem.Key.ToString());
            }            
#else
            IDictionaryEnumerator CacheEnum = cache.GetEnumerator();
            while (CacheEnum.MoveNext())
            {
                keys.Add(CacheEnum.Key.ToString());
            }
#endif
            return keys;
        }

        #endregion

        #region Insert

        /// <summary>
        /// 加入到缓存
        /// </summary>
        /// <typeparam name="T">泛型</typeparam>
        /// <param name="key">键</param>
        /// <param name="t">对象</param>
        /// <param name="expireSeconds">超时时间（秒）</param>
        /// <param name="priority">优化级</param>
        /// <param name="isSliding">是否相对过期</param>
        public void Insert<T>(string key, T t, int expireSeconds, CacheItemPriority priority = CacheItemPriority.Normal, bool isSliding = true)
        {
            TimeSpan offset = new TimeSpan(0, 0, expireSeconds);

#if NETSTANDARD2_1

            var option = new MemoryCacheEntryOptions();
            option.Priority=priority;            

            cache.Set(key, t,
                isSliding
                    ? option.SetSlidingExpiration(offset)
                    : option.SetAbsoluteExpiration(offset));
#else

            if (t != null)
                cache.Insert(key, t, null, DateTime.Now.Add(offset), System.Web.Caching.Cache.NoSlidingExpiration, priority, null);
#endif
        }

        #endregion

        #region Delete

        /// <summary>
        /// 删除缓存
        /// </summary>
        /// <param name="key"></param>
        public void Del(string key) =>
            cache.Remove(key);

        /// <summary>
        /// 删除匹配的缓存
        /// </summary>
        /// <param name="pattern"></param>
        public void DelByPattern(string pattern)
        {
            var cacheNum = GetAllKeys();
            Regex regex = new Regex(pattern, RegexOptions.Singleline | RegexOptions.Compiled | RegexOptions.IgnoreCase);
            List<string> list = new List<string>();
            foreach (var item in cacheNum)
            {
                if (regex.IsMatch(item))
                    list.Add(item);
            }

            foreach (string str in list)
                cache.Remove(str);
        }

        /// <summary>
        /// 清除所有缓存
        /// </summary>
        /// <returns></returns>
        public int ClearAll()
        {
#if NETSTANDARD2_1
            var cacheNum = GetAllKeys();
            foreach (var item in cacheNum)
                cache.Remove(item);

            return cacheNum.Count;
#else
            IDictionaryEnumerator CacheEnum = cache.GetEnumerator();
            var al = new ArrayList();
            while (CacheEnum.MoveNext())
                al.Add(CacheEnum.Key);

            foreach (string key in al)
                cache.Remove(key);

            //返回清空的缓存数量
            return al.Count;
#endif
        }

        #endregion
    }
}
