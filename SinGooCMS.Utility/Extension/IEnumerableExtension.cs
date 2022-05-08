using System;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Collections;

namespace SinGooCMS.Utility.Extension
{
    /// <summary>
    /// 遍历扩展
    /// </summary>
    public static partial class IEnumerableExtension
    {
        #region SyncForEach

        /// <summary>
        /// 遍历IEnumerable(这个挺好的，类似于jquery的foreach)
        /// </summary>
        /// <param name="objs"></param>
        /// <param name="action">回调方法</param>
        /// <typeparam name="T"></typeparam>
        public static void ForEach<T>(this IEnumerable<T> objs, Action<T> action)
        {
            foreach (var o in objs)
            {
                action(o);
            }
        }

        #endregion SyncForEach

        #region AsyncForEach

        /// <summary>
        /// 遍历IEnumerable
        /// </summary>
        /// <param name="objs"></param>
        /// <param name="action">回调方法</param>
        /// <typeparam name="T"></typeparam>
        public static async void ForEachAsync<T>(this IEnumerable<T> objs, Action<T> action)
        {
            await Task.Run(() => Parallel.ForEach(objs, action));
        }

        #endregion AsyncForEach        

        /// <summary>
        /// 指定实体集合转DataTable
        /// <para>custColHeader 第一参数是key，第二个参数是要显示的名称</para>
        /// </summary>
        /// <typeparam name="T">实体</typeparam>
        /// <param name="list">实体集合</param>
        /// <param name="custColHeader">自定义表头</param>
        /// <returns></returns>
        public static DataTable ToDataTable<T>(this IEnumerable<T> list, IDictionary<string, string> custColHeader = null)
        {
            var table = new DataTable();
            //创建列头
            var propertys = typeof(T).GetProperties();
            foreach (var pi in propertys)
            {
                var pt = pi.PropertyType;
                if (pt.IsGenericType && (pt.GetGenericTypeDefinition() == typeof(Nullable<>)))
                {
                    pt = pt.GetGenericArguments()[0];
                }

                if (custColHeader != null && custColHeader.Count > 0)
                {
                    if (custColHeader.ContainsKey(pi.Name))
                        table.Columns.Add(new DataColumn(custColHeader[pi.Name], pt)); //自定义表头
                }
                else
                    table.Columns.Add(new DataColumn(pi.Name, pt));
            }
            //创建数据行
            if (list.Count() <= 0) return table;
            {
                foreach (var item in list)
                {
                    var tempList = new ArrayList();
                    foreach (var pi in propertys)
                    {
                        if (custColHeader != null && custColHeader.Count > 0)
                        {
                            if (custColHeader.ContainsKey(pi.Name))
                                tempList.Add(pi.GetValue(item, null));
                        }
                        else
                        {
                            tempList.Add(pi.GetValue(item, null));
                        }
                    }

                    var array = tempList.ToArray();
                    table.LoadDataRow(array, true);
                }
            }
            return table;
        }

        /// <summary>
        /// 一维数组转字符串
        /// </summary>
        /// <param name="array">数组</param>
        /// <param name="separator">分割符</param>
        /// <returns></returns>
        public static string ToSplitterString(this object[] array, string separator = ",")
        {
            return string.Join(separator, array);
        }

        /// <summary>
        /// 将当前集合中的元素根据指定的分隔符(<paramref name="separator"/>)拼接成字符串
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sequence"></param>
        /// <param name="separator"></param>
        /// <returns></returns>
        public static string ToSplitterString<T>(this IEnumerable<T> sequence, string separator)
        {
            if (!sequence.Any()) { return string.Empty; }
            return string.Join(separator, sequence);
        }

        /// <summary>
        /// 打乱集合元素顺序
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="arr"></param>
        /// <param name="multiple">对于元素很少的集合，可以通过升级倍数来提升随机率，默认1倍</param>
        /// <returns></returns>
        public static IEnumerable<T> RandomArray<T>(this IEnumerable<T> arr, int multiple = 1)
        {
            List<T> temp = new T[arr.Count() * multiple].ToList();
            foreach (var l in arr)
            {
                var rd = new Random(Guid.NewGuid().GetHashCode());
                var index = rd.Next(0, arr.Count() * multiple);
                temp.Insert(index, l);
            }
            temp.RemoveAll(new Predicate<T>((t) => { return !arr.Contains(t); }));
            return temp;
        }

        /// <summary>
		/// 从一个列表中随机选择对象
		/// </summary>
		/// <typeparam name="T">队列数据类型</typeparam>
		/// <param name="list">列表</param>
		/// <param name="random">使用的随机种子。如果使用null，则会新建一个</param>
		/// <returns>获得的结果</returns>
		public static T RandomTake<T>(this List<T> list, Random random = null)
        {
            if (list == null || list.Count == 0)
                return default(T);

            return list[(random ?? new Random()).Next(list.Count)];
        }

        /// <summary>
        /// 按字段去重
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <typeparam name="TKey"></typeparam>
        /// <param name="source"></param>
        /// <param name="keySelector"></param>
        /// <returns></returns>
        public static IEnumerable<TSource> DistinctBy<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector)
        {
            var hash = new HashSet<TKey>();
            return source.Where(p => hash.Add(keySelector(p)));
        }

        /// <summary>
        /// 添加多个元素
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="this"></param>
        /// <param name="values"></param>
        public static void AddRange<T>(this ICollection<T> @this, params T[] values)
        {
            foreach (var obj in values)
            {
                @this.Add(obj);
            }
        }

        /// <summary>
        /// 添加符合条件的多个元素
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="this"></param>
        /// <param name="predicate"></param>
        /// <param name="values"></param>
        public static void AddRangeIf<T>(this ICollection<T> @this, Func<T, bool> predicate, params T[] values)
        {
            foreach (var obj in values)
            {
                if (predicate(obj))
                {
                    @this.Add(obj);
                }
            }
        }

        /// <summary>
        /// 添加不重复的元素
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="this"></param>
        /// <param name="values"></param>
        public static void AddRangeIfNotContains<T>(this ICollection<T> @this, params T[] values)
        {
            foreach (T obj in values)
            {
                if (!@this.Contains(obj))
                {
                    @this.Add(obj);
                }
            }
        }

        /// <summary>
        /// 移除符合条件的元素
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="this"></param>
        /// <param name="where"></param>
        public static void RemoveWhere<T>(this ICollection<T> @this, Func<T, bool> @where)
        {
            foreach (var obj in @this.Where(where).ToList())
            {
                @this.Remove(obj);
            }
        }

        /// <summary>
        /// 在元素之后添加元素
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <param name="condition">条件</param>
        /// <param name="value">值</param>
        public static void InsertAfter<T>(this IList<T> list, Func<T, bool> condition, T value)
        {
            foreach (var item in list.Select((item, index) => new { item, index }).Where(p => condition(p.item)).OrderByDescending(p => p.index))
            {
                if (item.index + 1 == list.Count)
                {
                    list.Add(value);
                }
                else
                {
                    list.Insert(item.index + 1, value);
                }
            }
        }

        /// <summary>
        /// 在元素之后添加元素
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <param name="index">索引位置</param>
        /// <param name="value">值</param>
        public static void InsertAfter<T>(this IList<T> list, int index, T value)
        {
            foreach (var item in list.Select((v, i) => new { Value = v, Index = i }).Where(p => p.Index == index).OrderByDescending(p => p.Index))
            {
                if (item.Index + 1 == list.Count)
                {
                    list.Add(value);
                }
                else
                {
                    list.Insert(item.Index + 1, value);
                }
            }
        }

        /// <summary>
        /// 转HashSet,不重复的集合
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="source"></param>
        /// <param name="selector"></param>
        /// <returns></returns>
        public static HashSet<TResult> ToHashSet<T, TResult>(this IEnumerable<T> source, Func<T, TResult> selector)
        {
            var set = new HashSet<TResult>();
            set.UnionWith(source.Select(selector));
            return set;
        }
    }
}