using System;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SinGooCMS.Utility;

namespace NTFxTest
{
    public enum OperateType
    {
        Add,
        Modify,
        Delete
    }

    [TestClass]
    public class EnumTest
    {
        [TestMethod]
        public void TestString2Enum()
        {
            Assert.AreEqual(OperateType.Add, EnumUtils.StringToEnum<OperateType>("Add"));
        }
    }
}
