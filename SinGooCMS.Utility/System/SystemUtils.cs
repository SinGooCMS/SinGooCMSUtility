using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Web;

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
        public static string Favorites=>
            Environment.GetFolderPath(Environment.SpecialFolder.Favorites);

        /// <summary>
        /// 我的文档
        /// </summary>
        public static string MyDocuments=>
            Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);

        #endregion

        /// <summary>
        /// 读取绝对路径
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static string GetMapPath(string path = "/")
        {
#if NET461
            if (System.Web.HttpContext.Current != null)
                return System.Web.HttpContext.Current.Server.MapPath(path);
            else
            {
                path = path.Replace("~/", "/").Replace("\\", "/");
                if (path.StartsWith("/"))
                    path = path.TrimStart('/');

                return Path.Combine(AppDomain.CurrentDomain.BaseDirectory, path);
            }
#endif
#if NETSTANDARD2_1
            path = path.Replace("~/", "/").Replace("\\", "/");
            if (path.StartsWith("/"))
                path = path.TrimStart('/');

            return Path.Combine(AppDomain.CurrentDomain.BaseDirectory, path);
#endif           
        }
    }
}
