using System;

namespace SinGooCMS.Utility
{
    /// <summary>
    /// 时间工具
    /// </summary>
    public sealed class TimeUtils
    {
        /// <summary>
        /// 返回时间差
        /// </summary>
        /// <param name="time1">起始日期</param>
        /// <param name="time2">结束日期</param>
        /// <returns></returns>
        public static string GetDateDiff(DateTime time1, DateTime time2)
        {
            string dateDiff = null;
            try
            {
                TimeSpan ts = time2 - time1;
                if (ts.Days >= 1)
                {
                    dateDiff = time1.Month + "月" + time1.Day + "日";
                }
                else
                {
                    if (ts.Hours > 1)
                    {
                        dateDiff = ts.Hours + "小时前";
                    }
                    else
                    {
                        dateDiff = ts.Minutes + "分钟前";
                    }
                }
            }
            catch
            {
                // ignored
            }
            return dateDiff;
        }

        /// <summary>
        /// 获得两个日期的间隔
        /// </summary>
        /// <param name="time1">起始日期</param>
        /// <param name="time2">结束日期</param>
        /// <returns>日期间隔TimeSpan。</returns>
        public static TimeSpan GetTimeSpan(DateTime time1, DateTime time2)
        {
            TimeSpan ts1 = new TimeSpan(time1.Ticks);
            TimeSpan ts2 = new TimeSpan(time2.Ticks);
            TimeSpan ts = ts1.Subtract(ts2).Duration();
            return ts;
        }
    }
}
