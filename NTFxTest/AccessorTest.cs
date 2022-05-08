using System;
using System.Data;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SinGooCMS.Utility.Extension;
using SinGooCMS.Utility;

namespace NTFxTest
{
    [TestClass]
    public class AccessorTest
    {
        [TestMethod]
        public void TestAccessor()
        {
            var student=new StudentInfo() { UserName="jasonlee",Age=18 };
            var assor=Accessor.Build(student);
            Assert.AreEqual("jasonlee", assor[student, "UserName"]);

            assor[student, "UserName"] = "刘备";
            Assert.AreEqual("刘备", assor[student, "UserName"]);

            //var sex = IdCardUtils.Parse("36213319810901061X").Sex;
        }
    }
}
