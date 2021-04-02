using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Web;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;

namespace SinGooCMS.Utility
{
    /// <summary>
    /// 系统工具类
    /// </summary>
    public sealed class SystemUtils
    {
        #region 系统目录

        /// <summary>
        /// 桌面文件夹
        /// </summary>
        public static string Desktop =>
            Environment.GetFolderPath(Environment.SpecialFolder.Desktop);

        /// <summary>
        /// 收藏夹
        /// </summary>
        public static string Favorites =>
            Environment.GetFolderPath(Environment.SpecialFolder.Favorites);

        /// <summary>
        /// 我的文档
        /// </summary>
        public static string MyDocuments =>
            Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);

        #endregion

        #region 判断浏览器客户端来源

        /// <summary>
        /// user agent是指用户代理，简称 UA。
        /// 作用：使服务器能够识别客户使用的操作系统及版本、CPU 类型、浏览器及版本、浏览器渲染引擎、浏览器语言、浏览器插件等。
        /// </summary>
        public static Lazy<string> UserAgent => new Lazy<string>(() =>
        {
#if NETSTANDARD2_1
            return UtilsBase.Request.Headers["User-Agent"].ToString();
#else
            return UtilsBase.Request.UserAgent;
#endif
        });

        /// <summary>
        /// 是否移动端
        /// </summary>
        /// <returns></returns>
        public static Lazy<bool> IsMobileClient => new Lazy<bool>(() =>
        {
            string mobileUserAgent = "iphone|android|nokia|zte|huawei|lenovo|samsung|motorola|sonyericsson|lg|philips|gionee|htc|coolpad|symbian|sony|ericsson|mot|cmcc|iemobile|sgh|panasonic|alcatel|cldc|midp|wap|mobile|blackberry|windows ce|mqqbrowser|ucweb";
            Regex MOBILE_REGEX = new Regex(mobileUserAgent, RegexOptions.IgnoreCase | RegexOptions.Compiled);
            if (!string.IsNullOrEmpty(UserAgent.Value) && MOBILE_REGEX.IsMatch(UserAgent.Value.ToLower()))
                return true;

            return false;
        });

        /// <summary>
        /// 是否微信端
        /// </summary>
        /// <returns></returns>
        public static Lazy<bool> IsWeixinClient => new Lazy<bool>(() => UserAgent.Value.ToLower().IndexOf("micromessenger") != -1);

        /// <summary>
        /// 是否苹果端应用
        /// </summary>
        /// <returns></returns>
        public static Lazy<bool> IsMacClient => new Lazy<bool>(() => UserAgent.Value.ToLower().IndexOf("macintosh") != -1);

        /// <summary>
        /// 是否iphone
        /// </summary>
        /// <returns></returns>
        public static Lazy<bool> IsIphone => new Lazy<bool>(() => UserAgent.Value.ToLower().IndexOf("iphone") != -1);

        /// <summary>
        /// 是否安卓
        /// </summary>
        /// <returns></returns>
        public static Lazy<bool> IsAndroid => new Lazy<bool>(() => UserAgent.Value.ToLower().IndexOf("android") != -1);


        #endregion

#if NETSTANDARD2_1
        /// <summary>
        /// 是否winidows系统
        /// </summary>
        public static Lazy<bool> IsWindows => new Lazy<bool>(() => RuntimeInformation.IsOSPlatform(OSPlatform.Windows));
        /// <summary>
        /// 是否linux系统
        /// </summary>
        public static Lazy<bool> IsLinux => new Lazy<bool>(() => RuntimeInformation.IsOSPlatform(OSPlatform.Linux));
        /// <summary>
        /// 是否苹果系统
        /// </summary>
        public static Lazy<bool> IsMacOs => new Lazy<bool>(() => RuntimeInformation.IsOSPlatform(OSPlatform.OSX));
#endif

        /// <summary>
        /// 读取绝对路径
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static string GetMapPath(string path = "/")
        {
            /*
             * 1）windows系统支持 /和\两种路径，最终是\，如f:\abc.txt
             * 2）linux系统只支持 /一种路径，认为\路径不合法，如/etc/home/wwwroot，因此路径都转成/
             * 3）net framework不支持跨平台，不用考虑linux
             */

#if NETSTANDARD2_1
            if (IsLinux.Value)
                return FileUtils.Combine(AppDomain.CurrentDomain.BaseDirectory, path).Replace(@"\", "/");

            return FileUtils.Combine(AppDomain.CurrentDomain.BaseDirectory, path).Replace("/", @"\");
#else
            return System.Web.HttpContext.Current != null
                ? System.Web.HttpContext.Current.Server.MapPath(path).Replace("/",@"\")
                : FileUtils.Combine(AppDomain.CurrentDomain.BaseDirectory, path).Replace("/",@"\");
#endif
        }
    }
}
