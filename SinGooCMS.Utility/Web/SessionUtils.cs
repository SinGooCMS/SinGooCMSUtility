using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SinGooCMS.Utility.Extension;

#if NETSTANDARD2_1
using Microsoft.AspNetCore.Http;
#else
using System.Web;
#endif

namespace SinGooCMS.Utility
{
    /// <summary>
    /// session会话工具
    /// </summary>
    public class SessionUtils
    {
        /// <summary>
        /// 是否包含key
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static bool ContainKey(string key)
        {
#if NETSTANDARD2_1
            return UtilsBase.HttpContext.Session.Keys.Contains(key);
#else
            return UtilsBase.HttpContext.Session[key] == null;
#endif
        }

        #region 写入Session

        /// <summary>
        /// 写入Session值
        /// </summary>
        /// <param name="key">键</param>
        /// <param name="value">值</param>
        /// <param name="time">超时时间(分钟)</param>
        public static void SetSession(string key, string value, int time = 60 * 24)
        {
#if NETSTANDARD2_1
            UtilsBase.HttpContext.Session.SetString(key, value); //net core的session过期时间在startup里面设置
#else
            HttpContext.Current.Session[key] = value;
            HttpContext.Current.Session.Timeout = time;
#endif
        }

        /// <summary>
        /// 写入Session值
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="time"></param>
        public static void SetSession<T>(string key, T value, int time = 60 * 24)
        {
#if NETSTANDARD2_1
            SetSession(key, value.ToJson());
#else
            HttpContext.Current.Session[key] = value;
            HttpContext.Current.Session.Timeout = time;
#endif
        }

        #endregion

        #region 读取Session

#if NETSTANDARD2_1
        /// <summary>
        /// 读Session值
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static string GetSession(string key)
        {
            return UtilsBase.HttpContext.Session.GetString(key);
        }
#endif

        /// <summary>
        /// 读Session值
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <returns></returns>
        public static T GetSession<T>(string key) where T : class
        {
#if NETSTANDARD2_1
            var value = GetSession(key);
            return value != null ? value.JsonToObject<T>() : default;
#else
            if (UtilsBase.HttpContext.Session != null && UtilsBase.HttpContext.Session[key] != null)
                return (T)UtilsBase.HttpContext.Session[key];
            else
                return null;
#endif
        }

        #endregion

        #region 删除Session

        /// <summary>
        /// 删除Session
        /// </summary>
        /// <param name="key"></param>
        public static void DelSession(string key)
        {
            UtilsBase.HttpContext.Session.Remove(key);
        }

        #endregion
    }
}
