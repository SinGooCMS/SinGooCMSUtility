using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using SinGooCMS.Utility.Extension;
using System.Net;

namespace SinGooCMS.Utility
{
    /// <summary>
    /// IP工具
    /// </summary>
    public static class IPUtils
    {
        /// <summary>
        /// 获得当前页面客户端的IP
        /// </summary>
        /// <returns>当前页面客户端的IP</returns>
        public static string GetIP()
        {
            var ip = "0.0.0.0";
#if NETSTANDARD2_1
            ip = UtilsBase.Request.Headers["X-Forwarded-For"].FirstOrDefault();
            if (ip.ToString().IsNullOrEmpty())
            {
                ip = UtilsBase.HttpContext.Connection.RemoteIpAddress.ToString();
            }
#else
            if (UtilsBase.HttpContext != null)
            {
                ip = UtilsBase.HttpContext.Request.ServerVariables["HTTP_X_FORWARDED_FOR"] ?? string.Empty;
                if (!ip.IsIP())
                    ip = UtilsBase.HttpContext.Request.ServerVariables["REMOTE_ADDR"] ?? string.Empty;

                if (!ip.IsIP())
                    ip = UtilsBase.HttpContext.Request.UserHostAddress;
            }
            else
            {
                return Dns.GetHostEntry(Dns.GetHostName()).AddressList.FirstOrDefault(p => p.AddressFamily.ToString() == "InterNetwork")?.ToString();
            }
#endif

            return ip;
        }

        /// <summary>
        /// IP定位信息
        /// </summary>
        /// <param name="ip"></param>
        /// <returns></returns>
        public static string GetIPAreaStr(string ip)
        {
            if (ip.IsNullOrEmpty())
                return string.Empty;

            IPScanner scanner = new IPScanner();
            return scanner.IPLocation(ip); //从纯真数据库读取
        }

        #region 检查IP是否在IP段中

        /// <summary>
        /// 检测当前ip是否在指定的ip段中
        /// </summary>
        /// <param name="strCurrIP">当前IP</param>
        /// <param name="strBeginIP">起始IP</param>
        /// <param name="strEndIP">终止IP</param>
        /// <returns></returns>
        public static bool IsInIPDuan(string strCurrIP, string strBeginIP, string strEndIP)
        {
            int[] inip, begipint, endipint = new int[4];
            inip = IP2IntArr(strCurrIP);
            begipint = IP2IntArr(strBeginIP);
            endipint = IP2IntArr(strEndIP);

            for (int i = 0; i < 4; i++)
            {
                if (inip[i] < begipint[i] || inip[i] > endipint[i])
                {
                    return false;
                }
                else if (inip[i] > begipint[i] || inip[i] < endipint[i])
                {
                    return true;
                }
            }
            return true;
        }

        /// <summary>
        /// 将ip地址转成整形数组
        /// </summary>
        /// <param name="strIP"></param>
        /// <returns></returns>
        public static int[] IP2IntArr(string strIP)
        {
            int[] retip = new int[4];
            int i, count;
            char c;
            for (i = 0, count = 0; i < strIP.Length; i++)
            {
                c = strIP[i];
                if (c != '.')
                {
                    retip[count] = retip[count] * 10 + int.Parse(c.ToString());
                }
                else
                {
                    count++;
                }
            }

            return retip;

        }

        #endregion
    }
}