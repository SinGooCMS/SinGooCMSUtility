using System;
using System.Text;
using NUnit.Framework;
using System.Data;
using SinGooCMS.Utility;
using SinGooCMS.Utility.Extension;
using System.Collections.Generic;

namespace CoreTest
{
    public class AdoTest
    {
        [Test]
        public void TestConnected()
        {
            var connStr = @"server=.\SQLEXPRESS;database=www.singoo.top;uid=sa;pwd=Zontec2018;";
            var isConnected = AdoUtils.IsConnected(connStr);
            Assert.IsTrue(isConnected);
        }

        [Test]
        public void TestParams()
        {
            var model = new StudentInfo()
            {
                AutoID = 1,
                UserName = "jsonlee",
                Age = 18
            };

            var dict = new Dictionary<string, (string, bool)>();
            dict.Add("UserName", ("UserName", false));
            dict.Add("Age", ("Age", true));
            dict.Add("IsAdmin", ("isadmin", false));

            var param = model.ToSqlParams<StudentInfo>(dict);
            Assert.AreEqual(true, param.Length > 0);
        }
    }
}
