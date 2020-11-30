using Newtonsoft.Json;
using System.Threading.Tasks;

namespace SinGooCMS.Utility.Extension
{
    /// <summary>
    /// 对象扩展
    /// </summary>
    public static partial class ObjectExtension
    {
        #region Clone

        /// <summary>
        /// 浅克隆
        /// </summary>
        /// <param name="source">源</param>
        /// <typeparam name="TDestination">目标类型</typeparam>
        /// <returns>目标类型</returns>
        public static TDestination Clone<TDestination>(this object source)
            where TDestination : new()
        {
            TDestination dest = new TDestination();
            dest.GetType().GetProperties().ForEach(p =>
            {
                p.SetValue(dest, source.GetType().GetProperty(p.Name)?.GetValue(source));
            });
            return dest;
        }

        /// <summary>
        /// 深克隆
        /// </summary>
        /// <param name="source">源</param>
        /// <typeparam name="TDestination">目标类型</typeparam>
        /// <returns>目标类型</returns>
        public static TDestination DeepClone<TDestination>(this object source)
            where TDestination : new()
        {
            return JsonConvert.DeserializeObject<TDestination>(JsonConvert.SerializeObject(source));
        }

        /// <summary>
        /// 浅克隆(异步)
        /// </summary>
        /// <param name="source">源</param>
        /// <typeparam name="TDestination">目标类型</typeparam>
        /// <returns>目标类型</returns>
        public static Task<TDestination> CloneAsync<TDestination>(this object source)
            where TDestination : new()
        {
            return Task.Run(() =>
           {
               return source.Clone<TDestination>();
           });
        }

        /// <summary>
        /// 深克隆(异步)
        /// </summary>
        /// <param name="source">源</param>
        /// <typeparam name="TDestination">目标类型</typeparam>
        /// <returns>目标类型</returns>
        public static async Task<TDestination> DeepCloneAsync<TDestination>(this object source)
            where TDestination : new()
        {
            return await Task.Run(() => JsonConvert.DeserializeObject<TDestination>(JsonConvert.SerializeObject(source)));
        }

        #endregion Map

        /// <summary>
        /// 严格比较两个对象是否是同一对象(判断引用)
        /// </summary>
        /// <param name="this">自己</param>
        /// <param name="o">需要比较的对象</param>
        /// <returns>是否同一对象</returns>
        public new static bool ReferenceEquals(this object @this, object o)
        {
            return object.ReferenceEquals(@this, o);
        }
    }
}