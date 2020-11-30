using System;
using System.Collections.Generic;
using System.Text;
using System.Web;
using SinGooCMS.Utility.Extension;

namespace SinGooCMS.Utility
{
    /// <summary>
    /// web工具
    /// </summary>
    public class WebUtils
    {
        #region query

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
                return HttpUtility.HtmlDecode(HttpUtility.UrlDecode(UtilsBase.HttpContext.Request.Query[key])).Trim();
            }
#else
            if (UtilsBase.HttpContext != null && UtilsBase.HttpContext.Request.QueryString[key] != null)
            {
                //这里中文需要URL解码
                return HttpUtility.HtmlDecode(HttpUtility.UrlDecode(UtilsBase.HttpContext.Request.QueryString[key])).Trim();
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
        /// 读取query值
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

        /// <summary>
        /// 获取查询字符串，并以“参数名=参数值”的形式存入字典
        /// </summary>
        /// <returns></returns>
        public static IDictionary<string, string> GetRequestGet()
        {
            var dict = new Dictionary<string, string>();
#if NETSTANDARD2_1
            var requestKeys = UtilsBase.HttpContext.Request.Query.Keys;
            foreach (var key in requestKeys)
                dict.Add(key, UtilsBase.HttpContext.Request.Query[key]);
#else
            var requestKeys = UtilsBase.HttpContext.Request.QueryString.Keys;
            foreach (string key in requestKeys)
                dict.Add(key, UtilsBase.HttpContext.Request.QueryString[key]);
#endif
            return dict;
        }

#endregion

        #region form

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
                && UtilsBase.HttpContext.Request.ContentType != null
                && UtilsBase.HttpContext.Request.Form.ContainsKey(key)
                && !string.IsNullOrEmpty(UtilsBase.HttpContext.Request.Form[key]))
            {
                return HttpUtility.HtmlDecode(UtilsBase.HttpContext.Request.Form[key]).Trim();
            }
#else
            if (UtilsBase.HttpContext != null && UtilsBase.HttpContext.Request.Form[key] == null)
            {
                //这里中文需要URL解码
                return HttpUtility.HtmlDecode(HttpUtility.UrlDecode(HttpContext.Current.Request.Form[key])).Trim();
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
        /// 读取form值
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

        /// <summary>
        /// 获取post数据，并以“参数名=参数值”的形式组成数组
        /// </summary>
        /// <returns></returns>
        public static IDictionary<string, string> GetRequestPost()
        {
            var dict = new Dictionary<string, string>();
#if NETSTANDARD2_1
            var requestKeys = UtilsBase.HttpContext.Request.Form.Keys;
            foreach (var key in requestKeys)
                dict.Add(key, UtilsBase.HttpContext.Request.Form[key]);
#else
            var requestKeys = UtilsBase.HttpContext.Request.Form.Keys;
            foreach (string key in requestKeys)
                dict.Add(key, UtilsBase.HttpContext.Request.Form[key]);
#endif
            return dict;
        }

        #endregion

        #region 其它

        /// <summary>
        /// 地址字符串的查询部分
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public static string GetUrlSearch(string url)
        {
            if (!string.IsNullOrEmpty(url))
            {
                string[] strs1 = url.Split(new char[] { '/' });
                string urlSearch = strs1[strs1.Length - 1];
                if (urlSearch.IndexOf("?") != -1)
                    return strs1[strs1.Length - 1].Split(new char[] { '?' })[1];
            }

            return "";
        }

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

