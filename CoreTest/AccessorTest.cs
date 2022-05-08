using System;
using System.Text;
using NUnit.Framework;
using System.Data;
using SinGooCMS.Utility;

namespace CoreTest
{
    public class AccessorTest
    {
        [Test]
        public void TestAccessor()
        {
            var student = new StudentInfo() { UserName = "jasonlee", Age = 18 };
            var assor = Accessor.Build(student);
            Assert.AreEqual("jasonlee", assor[student, "UserName"]);

            assor[student, "UserName"] = "刘备";
            Assert.AreEqual("刘备", assor[student, "UserName"]);
        }
    }
}
