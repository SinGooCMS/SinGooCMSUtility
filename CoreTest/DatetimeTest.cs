using System;
using System.Text;
using NUnit.Framework;
using System.Data;
using SinGooCMS.Utility;
using System.Collections.Generic;

namespace CoreTest
{
    public class DatetimeTest
    {
        [Test]
        public void TestUnixTime()
        {
            var dt = DateTime.Now;
            //unix 时间
            var unixTime = DatetimeUtils.GetUnixTime(dt);
            //c#时间
            var csharpTime = DatetimeUtils.GetDTFromUnixTime(unixTime);
            //Assert.AreEqual(dt,csharpTime); //注意这2个不相等是因为dt还有毫秒数，而unixTime只计算了秒数
            Assert.AreEqual(unixTime, DatetimeUtils.GetUnixTime(csharpTime));
        }
    }
}
