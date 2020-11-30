using System;
using System.Diagnostics;
using System.Globalization;

namespace SinGooCMS.Utility.Extension
{
    /// <summary>
    /// 日期扩展
    /// </summary>
    public static class DateTimeExtension
    {
        #region 周一、周日

        /// <summary>
        /// 计算周一的日期
        /// </summary>
        /// <param name="someDate"></param>
        /// <returns></returns>
        public static DateTime GetMonday(this DateTime someDate)
        {
            var date = GetShortDate(someDate);
            return date.AddDays(1 - Convert.ToInt32(date.DayOfWeek.ToString("d")));
        }

        /// <summary>
        /// 计算周日的日期
        /// </summary>
        /// <param name="someDate"></param>
        /// <returns></returns>
        public static DateTime GetSunday(this DateTime someDate)
        {
            return someDate.GetMonday().AddDays(6);
        }

        #endregion

        #region 月初、月尾

        /// <summary>
        /// 计算月初的日期
        /// </summary>
        /// <param name="someDate"></param>
        /// <returns></returns>
        public static DateTime GetMonthFirstDay(this DateTime someDate)
        {
            var date = GetShortDate(someDate);
            return DateTime.Parse(date.ToString("yyyy-MM") + "-01");
        }

        /// <summary>
        /// 计算月尾的日期
        /// </summary>
        /// <param name="someDate"></param>
        /// <returns></returns>
        public static DateTime GetMonthLastDay(this DateTime someDate)
        {
            return GetMonthFirstDay(someDate).AddMonths(1).AddDays(-1);
        }

        #endregion

        #region 季初、季尾

        /// <summary>
        /// 计算月初的日期
        /// </summary>
        /// <param name="someDate"></param>
        /// <returns></returns>
        public static DateTime GetQuarterFirstDay(this DateTime someDate)
        {
            var date = GetShortDate(someDate);
            return date.AddMonths(0 - (date.Month - 1) % 3).AddDays(1 - date.Day);
        }

        /// <summary>
        /// 计算月尾的日期
        /// </summary>
        /// <param name="someDate"></param>
        /// <returns></returns>
        public static DateTime GetQuarterLastDay(this DateTime someDate)
        {
            return GetQuarterFirstDay(someDate).AddMonths(3).AddDays(-1);
        }

        #endregion

        #region 年初、年尾

        /// <summary>
        /// 计算年初的日期
        /// </summary>
        /// <param name="someDate"></param>
        /// <returns></returns>
        public static DateTime GetYearFirstDay(this DateTime someDate)
        {
            var date = GetShortDate(someDate);
            return DateTime.Parse(date.ToString("yyyy") + "-01-01");
        }

        /// <summary>
        /// 计算年尾的日期
        /// </summary>
        /// <param name="someDate"></param>
        /// <returns></returns>
        public static DateTime GetYearLastDay(this DateTime someDate)
        {
            return GetYearFirstDay(someDate).AddYears(1).AddDays(-1);
        }

        #endregion

        #region 本年第几周

        /// <summary>
        /// 本年第几周
        /// </summary>
        /// <param name="someDate"></param>
        /// <returns></returns>
        public static int WeekOfYear(this DateTime someDate)
        {
            GregorianCalendar gc = new GregorianCalendar(GregorianCalendarTypes.USEnglish);
            int weekOfYear = gc.GetWeekOfYear(someDate, CalendarWeekRule.FirstDay, DayOfWeek.Monday);
            return weekOfYear;
        }

        #endregion

        #region 日期格式化

        /// <summary>
        /// 日期格式化
        /// </summary>
        /// <param name="dtime"></param>
        /// <param name="format"></param>
        /// <returns></returns>
        public static string ToFormatString(this DateTime dtime, string format = "yyyy-MM-dd HH:mm:ss") =>
            dtime.ToString(format);

        /// <summary>
        /// 格式为化日期格式字符串
        /// </summary>
        /// <param name="dtime"></param>
        /// <param name="format"></param>
        /// <returns></returns>
        public static string ToFormatDate(this DateTime dtime, string format = "yyyy-MM-dd") =>
            dtime.ToString(format);

        /// <summary>
        /// 格式化为时间字符串
        /// </summary>
        /// <param name="dtime"></param>
        /// <param name="format"></param>
        /// <returns></returns>
        public static string ToFormatTime(this DateTime dtime, string format = "HH:mm:ss") =>
            dtime.ToString(format);

        /// <summary>
        /// 格式化为日期格式字符串
        /// </summary>
        /// <param name="strInput"></param>
        /// <param name="format"></param>
        /// <returns></returns>
        public static string ToFormatDateString(this string strInput, string format = "yyyy-MM-dd")
        {
            var date = new DateTime();
            if (DateTime.TryParse(strInput, out date))
                return date.ToString(format);

            return new DateTime(1900, 1, 1).ToString(format);
        }

        #endregion

        #region 相对于1970-01-01

        /// <summary>
        /// 获取该时间相对于1970-01-01 00:00:00的秒数
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        public static double GetTotalSeconds(this in DateTime dt) => new DateTimeOffset(dt).ToUnixTimeSeconds();

        /// <summary>
        /// 获取该时间相对于1970-01-01 00:00:00的毫秒数
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        public static double GetTotalMilliseconds(this in DateTime dt) => new DateTimeOffset(dt).ToUnixTimeMilliseconds();

        /// <summary>
        /// 获取该时间相对于1970-01-01 00:00:00的微秒时间戳
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        public static long GetTotalMicroseconds(this in DateTime dt) => new DateTimeOffset(dt).Ticks / 10;

        /// <summary>
        /// 获取该时间相对于1970-01-01 00:00:00的纳秒时间戳
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        public static long GetTotalNanoseconds(this in DateTime dt) => new DateTimeOffset(dt).Ticks * 100 + Stopwatch.GetTimestamp() % 100;

        /// <summary>
        /// 获取该时间相对于1970-01-01 00:00:00的分钟数
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        public static double GetTotalMinutes(this in DateTime dt) => new DateTimeOffset(dt).Offset.TotalMinutes;

        /// <summary>
        /// 获取该时间相对于1970-01-01 00:00:00的小时数
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        public static double GetTotalHours(this in DateTime dt) => new DateTimeOffset(dt).Offset.TotalHours;

        /// <summary>
        /// 获取该时间相对于1970-01-01 00:00:00的天数
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        public static double GetTotalDays(this in DateTime dt) => new DateTimeOffset(dt).Offset.TotalDays;

        #endregion

        #region 时间差

        /// <summary>
        /// 返回时间差
        /// </summary>
        /// <param name="dateTime1">时间1</param>
        /// <param name="dateTime2">时间2</param>
        /// <returns>时间差</returns>
        public static string DateDiff(this in DateTime dateTime1, in DateTime dateTime2)
        {
            string dateDiff;
            var ts = dateTime2 - dateTime1;
            if (ts.Days >= 1)
            {
                dateDiff = dateTime1.Month + "月" + dateTime1.Day + "日";
            }
            else
            {
                dateDiff = ts.Hours > 1 ? ts.Hours + "小时前" : ts.Minutes + "分钟前";
            }

            return dateDiff;
        }

        #endregion

        private static DateTime GetShortDate(DateTime date)
        {
            return DateTime.Parse(date.ToString("yyyy-MM-dd"));
        }
    }
}
