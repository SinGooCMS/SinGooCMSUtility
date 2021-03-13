using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using SinGooCMS.Utility.Extension;

namespace SinGooCMS.Utility
{
    /// <summary>
    /// web工具
    /// </summary>
    public sealed class WebUtils
    {
        #region query

        /// <summary>
        /// 读取Query值，默认只有一个值
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="defaultVal"></param>
        /// <returns></returns>
        public static T GetQueryVal<T>(string key, T defaultVal = default(T))
        {
            var cols = GetQueryVals<T>(key);
            return cols == null
                ? defaultVal
                : cols.First();
        }

        /// <summary>
        /// 读取Query值
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <returns></returns>
        public static IEnumerable<T> GetQueryVals<T>(string key)
        {
            var lst = new List<T>();
#if NETSTANDARD2_1
            if (UtilsBase.HttpContext != null
                && UtilsBase.HttpContext.Request != null
                && UtilsBase.HttpContext.Request.Query.ContainsKey(key))
            {
                var cols = UtilsBase.HttpContext.Request.Query[key];
                for (var i = 0; i < cols.Count; i++)
                    lst.Add(HttpUtility.UrlDecode(cols[i]).Trim().To<T>()); //中文需要URL解码

                return lst;
            }
#else
            if (UtilsBase.HttpContext != null && UtilsBase.HttpContext.Request.QueryString[key] != null)
            {
                //中文需要URL解码
                string queryVal = HttpUtility.UrlDecode(UtilsBase.HttpContext.Request.QueryString[key]).Trim();
                queryVal.Split(',').ForEach(item =>
                {
                    lst.Add(item.To<T>());
                });

                return lst;
            }
#endif
            return null;
        }

        /// <summary>
        /// 读取query值
        /// </summary>
        /// <param name="key"></param>
        /// <param name="defalutValue"></param>
        /// <returns></returns>        
        public static string GetQueryString(string key, string defalutValue = "")
        {
#if NETSTANDARD2_1
            if (UtilsBase.HttpContext != null
                && UtilsBase.HttpContext.Request != null
                && UtilsBase.HttpContext.Request.Query.ContainsKey(key)
                && !string.IsNullOrEmpty(UtilsBase.HttpContext.Request.Query[key].ToString()))
            {
                //这里中文需要URL解码
                return HttpUtility.UrlDecode(UtilsBase.HttpContext.Request.Query[key]).Trim();
            }
#else
            if (UtilsBase.HttpContext != null && UtilsBase.HttpContext.Request.QueryString[key] != null)
            {
                //这里中文需要URL解码
                return HttpUtility.UrlDecode(UtilsBase.HttpContext.Request.QueryString[key]).Trim();
            }
#endif

            return defalutValue;
        }

        /// <summary>
        /// 读取query值
        /// </summary>
        /// <param name="key"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public static int GetQueryInt(string key, int defaultValue = 0)
        {
            return GetQueryString(key).ToInt(defaultValue);
        }

        /// <summary>
        /// 读取query值
        /// </summary>
        /// <param name="key"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public static bool GetQueryBool(string key, bool defaultValue = false)
        {
            return GetQueryString(key).ToBool(defaultValue);
        }

        /// <summary>
        /// 读取query值
        /// </summary>
        /// <param name="key"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public static long GetQueryLong(string key, long defaultValue = 0)
        {
            return GetQueryString(key).ToLong(defaultValue);
        }

        /// <summary>
        /// 读取query值
        /// </summary>
        /// <param name="key"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public static float GetQueryFloat(string key, float defaultValue = 0.0f)
        {
            return GetQueryString(key).ToFloat(defaultValue);
        }

        /// <summary>
        /// 读取query值
        /// </summary>
        /// <param name="key"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public static double GetQueryDouble(string key, double defaultValue = 0.0d)
        {
            return GetQueryString(key).ToDouble(defaultValue);
        }

        /// <summary>
        /// 读取query值
        /// </summary>
        /// <param name="key"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public static decimal GetQueryDecimal(string key, decimal defaultValue = 0.0m)
        {
            return GetQueryString(key).ToDecimal(defaultValue);
        }

        /// <summary>
        /// 读取query值并转化为Datetime类型，默认是1900-1-1
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static DateTime GetQueryDatetime(string key)
        {
            return GetQueryString(key).ToDateTime();
        }

        /// <summary>
        /// 读取query值
        /// </summary>
        /// <param name="key"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public static DateTime GetQueryDatetime(string key, DateTime defaultValue)
        {
            return GetQueryString(key).ToDateTime(defaultValue);
        }

        #endregion

        #region form

        /// <summary>
        /// 读取Form值，默认只有一个值
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="defaultVal"></param>
        /// <returns></returns>
        public static T GetFormVal<T>(string key, T defaultVal = default(T))
        {
            var cols = GetFormVals<T>(key);
            return cols == null
                ? defaultVal
                : cols.First();
        }

        /// <summary>
        /// 读取Form值
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <returns></returns>
        public static IEnumerable<T> GetFormVals<T>(string key)
        {
            var lst = new List<T>();
#if NETSTANDARD2_1
            //直接引用UtilsBase.HttpContext.Request.Form会抛出异常，首先要判断 HasFormContentType
            if (UtilsBase.HttpContext != null
                && UtilsBase.HttpContext.Request != null
                && UtilsBase.HttpContext.Request.HasFormContentType
                && UtilsBase.HttpContext.Request.Form.ContainsKey(key))
            {
                var cols = UtilsBase.HttpContext.Request.Form[key];
                //Post数据有多个同名键，值以逗号分隔 如111,2222，也可以用下标读取 Form[key][0]
                for (var i = 0; i < cols.Count; i++)
                    lst.Add(HttpUtility.UrlDecode(cols[i]).Trim().To<T>()); //中文需要URL解码

                return lst;
            }
#else
            if (UtilsBase.HttpContext != null && UtilsBase.HttpContext.Request.Form[key] != null)
            {
                //中文需要URL解码
                string formVal = HttpUtility.UrlDecode(UtilsBase.HttpContext.Request.Form[key]).Trim();
                formVal.Split(',').ForEach(item =>
                {
                    lst.Add(item.To<T>());
                });

                return lst;
            }
#endif
            return null;
        }

        /// <summary>
        /// 读取form值
        /// </summary>
        /// <param name="key"></param>
        /// <param name="defalutValue"></param>
        /// <returns></returns>
        public static string GetFormString(string key, string defalutValue = "")
        {
#if NETSTANDARD2_1
            if (UtilsBase.HttpContext != null
                && UtilsBase.HttpContext.Request != null
                && UtilsBase.HttpContext.Request.HasFormContentType
                && UtilsBase.HttpContext.Request.Form.ContainsKey(key)
                && !string.IsNullOrEmpty(UtilsBase.HttpContext.Request.Form[key]))
            {
                return HttpUtility.UrlDecode(UtilsBase.HttpContext.Request.Form[key]).Trim();
            }
#else
            if (UtilsBase.HttpContext != null && UtilsBase.HttpContext.Request.Form[key] != null)
            {
                //这里中文需要URL解码
                return HttpUtility.UrlDecode(UtilsBase.HttpContext.Request.Form[key]).Trim();
            }
#endif

            return defalutValue;
        }

        /// <summary>
        /// 读取form值
        /// </summary>
        /// <param name="key"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public static int GetFormInt(string key, int defaultValue = 0)
        {
            return GetFormString(key).ToInt(defaultValue);
        }

        /// <summary>
        /// 读取form值
        /// </summary>
        /// <param name="key"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public static bool GetFormBool(string key, bool defaultValue = false)
        {
            return GetFormString(key).ToBool(defaultValue);
        }

        /// <summary>
        /// 读取form值
        /// </summary>
        /// <param name="key"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public static float GetFormFloat(string key, float defaultValue = 0.0f)
        {
            return GetFormString(key).ToFloat(defaultValue);
        }

        /// <summary>
        /// 读取form值
        /// </summary>
        /// <param name="key"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public static double GetFormDouble(string key, double defaultValue = 0.0d)
        {
            return GetFormString(key).ToDouble(defaultValue);
        }

        /// <summary>
        /// 读取form值
        /// </summary>
        /// <param name="key"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public static decimal GetFormDecimal(string key, decimal defaultValue = 0.0m)
        {
            return GetFormString(key).ToDecimal(defaultValue);
        }

        /// <summary>
        /// 读取form值并转化为Datetime类型，默认是1900-1-1
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static DateTime GetFormDatetime(string key)
        {
            return GetFormString(key).ToDateTime();
        }

        /// <summary>
        /// 读取form值
        /// </summary>
        /// <param name="key"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public static DateTime GetFormDatetime(string key, DateTime defaultValue)
        {
            return GetFormString(key).ToDateTime(defaultValue);
        }

        #endregion

        #region 其它

        /// <summary>
        /// 获取url地址
        /// </summary>
        /// <returns></returns>
        public static string GetAbsoluteUri()
        {
#if NETSTANDARD2_1
            var request = UtilsBase.Request;
            return new StringBuilder()
                .Append(request.Scheme)
                .Append("://")
                .Append(request.Host)
                .Append(request.PathBase)
                .Append(request.Path)
                .Append(request.QueryString)
                .ToString();
#else
            return UtilsBase.Request.Url.ToString();
#endif
        }

        #endregion
    }
}

