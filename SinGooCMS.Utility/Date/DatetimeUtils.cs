using System;
using System.Collections.Generic;
using System.Text;
using SinGooCMS.Utility.Extension;

namespace SinGooCMS.Utility
{
    /// <summary>
    /// 日期工具
    /// </summary>
    public sealed class DatetimeUtils
    {
        /// <summary>
        /// 转unix时间
        /// </summary>
        /// <param name="time"></param>
        /// <returns></returns>
        public static double GetUnixTime(DateTime time)
        {
            return time.GetUnixTimeMilliseconds();
        }

        /// <summary>
        /// unix时间戳转c#日期
        /// </summary>
        /// <param name="unixSeconds"></param>
        /// <returns></returns>
        public static DateTime GetDTFromUnixTime(double unixSeconds)
        {
            return TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1))
                .Add(new TimeSpan((unixSeconds * 10000).ToLong()));
        }
    }
}
