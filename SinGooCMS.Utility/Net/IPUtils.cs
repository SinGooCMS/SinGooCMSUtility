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
    public sealed class IPUtils
    {
        /// <summary>
        /// 获得当前页面客户端的IP
        /// </summary>
        /// <returns>当前页面客户端的IP</returns>
        public static string GetIP()
        {
            var ip = "0.0.0.0";
#if NETSTANDARD2_1
            if (UtilsBase.HttpContext != null)
            {
                ip = UtilsBase.Request.Headers["X-Forwarded-For"].FirstOrDefault();
                if (ip.IsNullOrEmpty())
                {
                    ip = UtilsBase.HttpContext.Connection.RemoteIpAddress?.ToString();
                }
            }
            else
            {
                return Dns.GetHostEntry(Dns.GetHostName())
                    .AddressList.FirstOrDefault(p => p.AddressFamily.ToString() == "InterNetwork")?.ToString();
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

            return !ip.IsIP() ? "127.0.0.1" : ip;
        }

        /// <summary>
        /// IP定位信息
        /// </summary>
        /// <param name="ip">不传IP，默认是当前的IP</param>
        /// <returns></returns>
        public static string GetIPAreaStr(string ip = "")
        {
            if (ip.IsNullOrEmpty())
                ip = GetIP();

            return new IPScanner().IPLocation(ip); //从纯真数据库读取
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

    #region IP地理位置信息

    /// <summary>
    /// IP地理位置信息
    /// </summary>
    public class IPAreaInfo
    {
        /// <summary>
        /// 国家
        /// </summary>
        public string Country { get; set; }
        /// <summary>
        /// 省
        /// </summary>
        public string Province { get; set; }
        /// <summary>
        /// 市
        /// </summary>
        public string City { get; set; }
        /// <summary>
        /// 区
        /// </summary>
        public string County { get; set; }

        /// <summary>
        /// 重写ToString
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            StringBuilder builder = new StringBuilder();
            if (!Country.IsNullOrEmpty())
                builder.Append(Country);
            if (!Province.IsNullOrEmpty())
                builder.Append("," + Province);
            if (!City.IsNullOrEmpty())
                builder.Append("," + City);
            if (!County.IsNullOrEmpty())
                builder.Append("," + County);

            return builder.ToString();
        }
    }

    #endregion
}