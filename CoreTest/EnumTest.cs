using System;
using System.Text;
using NUnit.Framework;
using SinGooCMS.Utility;

namespace CoreTest
{
    public enum OperateType
    {
        Add,
        Modify,
        Delete
    }

    public class EnumTest
    {
        [Test]
        public void TestString2Enum()
        {
            Assert.AreEqual(OperateType.Add, EnumUtils.StringToEnum<OperateType>("Add"));
        }
    }
}
