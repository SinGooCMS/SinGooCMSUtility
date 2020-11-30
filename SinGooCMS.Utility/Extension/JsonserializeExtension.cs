using System.Data;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace SinGooCMS.Utility.Extension
{
    /// <summary>
    /// json扩展
    /// </summary>
    public static class JsonserializeExtension
    {
        #region 序列化

        /// <summary>
        /// 序列化
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="t"></param>
        /// <returns></returns>
        public static string ToJson<T>(this T t)
        {
            if (t == null)
                return "{}";

            return JsonConvert.SerializeObject(t, new IsoDateTimeConverter
            {
                DateTimeFormat = "yyyy-MM-dd HH:mm:ss"
            });
        }
        /// <summary>
        /// 序列化
        /// </summary>
        /// <param name="ds"></param>
        /// <returns></returns>
        public static string ToJson(this DataSet ds)
        {
            if (ds == null || (ds != null && ds.Tables.Count == 0))
                return "{}";

            return JsonConvert.SerializeObject(ds, new IsoDateTimeConverter
            {
                DateTimeFormat = "yyyy-MM-dd HH:mm:ss"
            }, new DataSetConverter());
        }
        /// <summary>
        /// 序列化
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        public static string ToJson(this DataTable dt)
        {
            if (dt == null || (dt != null && dt.Rows.Count == 0))
                return "{}";

            return JsonConvert.SerializeObject(dt, new IsoDateTimeConverter
            {
                DateTimeFormat = "yyyy-MM-dd HH:mm:ss"
            }, new DataTableConverter());
        }
        #endregion

        #region 反序列化
        /// <summary>
        /// 反序列化
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="strJson"></param>
        /// <returns></returns>
        public static T JsonToObject<T>(this string strJson) =>
            JsonConvert.DeserializeObject<T>(strJson);

        /// <summary>
        /// 反序列化(匿名类型),这个挺好的，懒人大法
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="strJson"></param>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static T JsonToAnonymousObject<T>(this string strJson, T obj) =>
            JsonConvert.DeserializeAnonymousType(strJson, obj);

        #endregion
    }
}
