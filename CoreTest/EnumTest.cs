using System;
using System.Text;
using System.ComponentModel;
using NUnit.Framework;
using SinGooCMS.Utility;
using DescriptionAttribute = System.ComponentModel.DescriptionAttribute;

namespace CoreTest
{
    public enum OperateType
    {
        [Description("增加")]
        Add,
        [Description("修改")]
        Modify,
        [Description("删除")]
        Delete
    }

    public class EnumTest
    {
        [Test]
        public void TestString2Enum()
        {
            Assert.AreEqual(OperateType.Add, EnumUtils.StringToEnum<OperateType>("Add"));
            Console.WriteLine($"描述：{EnumUtils.GetEnumDescription(OperateType.Add)}");
        }
    }
}
