using System;
using System.Text;
using NUnit.Framework;
using System.Data;
using SinGooCMS.Utility;
using SinGooCMS.Utility.Extension;
using System.Collections.Generic;

namespace CoreTest
{
    public class CNDateTest
    {
        [Test]
        public void TestCNDate()
        {
            //CNDate date = ChinaDate.GetChinaDate();
            //Console.WriteLine("农历信息："+ date.ToString()+"\r\n");

            ChineseCalendar c = new ChineseCalendar();
            StringBuilder dayInfo = new StringBuilder();
            //dayInfo.Append("阳历：" + c.DateString + "\r\n");
            dayInfo.Append("农历：" + c.ChineseDateString + "\r\n");
            //dayInfo.Append("星期：" + c.WeekDayStr);
            //dayInfo.Append("时辰：" + c.ChineseHour + "\r\n");
            //dayInfo.Append("属相：" + c.AnimalString + "\r\n");
            //dayInfo.Append("节气：" + c.ChineseTwentyFourDay + "\r\n");
            //dayInfo.Append("前一个节气：" + c.ChineseTwentyFourPrevDay + "\r\n");
            //dayInfo.Append("下一个节气：" + c.ChineseTwentyFourNextDay + "\r\n");
            //dayInfo.Append("节日：" + c.DateHoliday + "\r\n");
            //dayInfo.Append("干支：" + c.GanZhiDateString + "\r\n");
            //dayInfo.Append("星宿：" + c.ChineseConstellation + "\r\n");
            //dayInfo.Append("星座：" + c.Constellation + "\r\n");
            Console.WriteLine("中国日历信息：" + dayInfo.ToString());

        }
    }
}
