#if NETSTANDARD2_1
using Microsoft.AspNetCore.Http;
#else
using System.Web;
#endif
using System;
using System.Linq;


namespace SinGooCMS.Utility
{
    /// <summary>
    /// cookie工具
    /// </summary>
    public sealed class CookieUtils
    {
        /// <summary>
        /// 是否包含key
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static bool ContainKey(string key)
        {
#if NETSTANDARD2_1
            return UtilsBase.HttpContext.Request.Cookies.ContainsKey(key);
#else
            return UtilsBase.HttpContext.Request.Cookies.AllKeys.Contains(key);
#endif
        }

        /// <summary>
        /// 写入cookie(有效期一年)
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public static void SetCookie(string key, string value)
        {
            SetCookie(key, value, 3600 * 24 * 365);
        }

        /// <summary>
        /// 写入cookie
        /// </summary>
        /// <param name="key">cookie键</param>
        /// <param name="value">cookie值</param>
        /// <param name="expires">过期时长（秒），默认30分钟</param>
        /// <param name="isHttpOnly"></param>
        public static void SetCookie(string key, string value, int expires = 30 * 60, bool isHttpOnly = true)
        {
#if NETSTANDARD2_1
            if (!UtilsBase.HttpContext.Response.Headers.IsReadOnly)
            {
                UtilsBase.HttpContext.Response.Cookies.Append(key, value, new CookieOptions
                {
                    Expires = DateTime.Now.AddSeconds(expires),
                    HttpOnly = isHttpOnly
                });
            }
#else
            if (UtilsBase.HttpContext != null)
            {
                var cookie = UtilsBase.HttpContext.Request.Cookies[key];
                if (cookie == null)
                    cookie = new HttpCookie(key);

                cookie.Value = value;
                if (expires > 0)
                    cookie.Expires = DateTime.Now.AddSeconds(expires);

                //cookie.Path = strPath;
                cookie.HttpOnly = isHttpOnly;

                if (cookie == null)
                    UtilsBase.HttpContext.Response.SetCookie(cookie);
                else
                    UtilsBase.HttpContext.Response.AppendCookie(cookie);
            }
#endif
        }

        /// <summary>
        /// 读cookie值
        /// </summary>
        /// <param name="key">名称</param>
        /// <returns>cookie值</returns>
        public static string GetCookie(string key)
        {
#if NETSTANDARD2_1
            UtilsBase.HttpContext.Request.Cookies.TryGetValue(key, out string value);
            return string.IsNullOrEmpty(value) ? string.Empty : value;
#else
            if (UtilsBase.HttpContext != null 
                && UtilsBase.HttpContext.Request.Cookies != null 
                && UtilsBase.HttpContext.Request.Cookies[key] != null)
            {
                return HttpContext.Current.Request.Cookies[key].Value;
            }

            return string.Empty;
#endif
        }

        /// <summary>
        /// 清除cookie
        /// </summary>
        /// <param name="key">key of cookie</param>
        public static void RemoveCookie(string key)
        {
#if NETSTANDARD2_1
            UtilsBase.HttpContext.Response.Cookies.Delete(key);
#else
            if (UtilsBase.HttpContext != null)
            {
                HttpCookie cookie = UtilsBase.HttpContext.Request.Cookies[key];
                if (cookie != null)
                {
                    UtilsBase.HttpContext.Request.Cookies.Remove(key);
                }
            }
#endif
        }
    }
}