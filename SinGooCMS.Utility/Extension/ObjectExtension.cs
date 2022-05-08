using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using AutoMapper;

namespace SinGooCMS.Utility.Extension
{
    /// <summary>
    /// 对象扩展
    /// </summary>
    public static class ObjectExtension
    {
        #region AutoMapper

        /// <summary>
        /// 单个对象映射
        /// </summary>
        /// <typeparam name="TSource">源对象</typeparam>
        /// <typeparam name="TDestination">目标对象</typeparam>
        /// <param name="source"></param>
        /// <param name="thenDeal">自动匹配后的处理</param>
        /// <returns></returns>
        public static TDestination MapTo<TSource, TDestination>(this TSource source, Func<TDestination, TDestination> thenDeal = null)
            where TDestination : class
            where TSource : class
        {
            if (source == null) return default(TDestination);

            var config = new MapperConfiguration(cfg => cfg.CreateMap(typeof(TSource), typeof(TDestination)));
            var mapper = config.CreateMapper();

            var result = mapper.Map<TDestination>(source);
            if (thenDeal != null)
                result = thenDeal(result);

            return result;
        }

        /// <summary>
        ///  集合列表类型映射  
        /// </summary>
        /// <typeparam name="TSource">源对象</typeparam>
        /// <typeparam name="TDestination">目标对象</typeparam>
        /// <param name="source"></param>
        /// <param name="thenDeal">自动匹配后的处理</param>
        /// <returns></returns>
        public static IEnumerable<TDestination> MapToList<TSource, TDestination>(this IEnumerable<TSource> source, Func<IEnumerable<TDestination>, IEnumerable<TDestination>> thenDeal = null)
            where TDestination : class
            where TSource : class
        {
            if (source == null) return default(IEnumerable<TDestination>);

            Type sourceType = source.GetType().GetGenericArguments()[0];  //获取枚举的成员类型
            var config = new MapperConfiguration(cfg => cfg.CreateMap(sourceType, typeof(TDestination)));
            var mapper = config.CreateMapper();

            var result = mapper.Map<IEnumerable<TDestination>>(source);
            if (thenDeal != null)
                result = thenDeal(result);

            return result;
        }

        #endregion

        /// <summary>
        /// 深度克隆，注意T必须是 Serializable
        /// </summary>
        /// <typeparam name="T">泛型</typeparam>
        /// <param name="obj">原对象</param>
        /// <returns></returns>
        public static T DeepClone<T>(this T obj)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                BinaryFormatter bf = new BinaryFormatter();
                bf.Serialize(ms, obj);
                ms.Position = 0;
                return (T)bf.Deserialize(ms);
            }
        }

        /// <summary>
        /// 严格比较两个对象是否是同一对象(判断引用)
        /// </summary>
        /// <param name="obj">自己</param>
        /// <param name="o">需要比较的对象</param>
        /// <returns>是否同一对象</returns>
        public new static bool ReferenceEquals(this object obj, object o)
        {
            return object.ReferenceEquals(obj, o);
        }
    }
}