using System;
using System.Data;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SinGooCMS.Utility.Extension;
using SinGooCMS.Utility;
using System.ComponentModel.DataAnnotations.Schema;
using SinGooCMS.Ado.Interface;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace NTFxTest
{
    [TestClass]
    public class AdoTest
    {
        [TestMethod]
        public void TestConnected()
        {            
            var connStr = @"server=.\SQLEXPRESS;database=www.singoo.top;uid=sa;pwd=Zontec2018;";
            var isConnected=AdoUtils.IsConnected(connStr);
            Assert.IsTrue(isConnected);
        }

        [TestMethod]
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
