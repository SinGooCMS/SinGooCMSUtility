using System;
using System.Collections.Generic;
using System.Text;

namespace SinGooCMS.Utility
{
    /// <summary>
    /// 日期工具
    /// </summary>
    public class DatetimeUtils
    {
        /// <summary>
        /// 转unix时间
        /// </summary>
        /// <param name="time"></param>
        /// <returns></returns>
        public static long GetUnixTime(DateTime time)
        {
            System.DateTime startTime = TimeZone.CurrentTimeZone.ToLocalTime(new System.DateTime(1970, 1, 1));
            return (long)(time - startTime).TotalSeconds;
        }

        /// <summary>
        /// unix时间戳转c#日期
        /// </summary>
        /// <param name="unixSecond"></param>
        /// <returns></returns>
        public static DateTime GetDTFromUnixTime(long unixSecond)
        {
            TimeSpan span = new TimeSpan(unixSecond * 10000000);
            DateTime baseTime = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1));
            DateTime resultTime = baseTime.Add(span);
            return resultTime;
        }
    }
}
