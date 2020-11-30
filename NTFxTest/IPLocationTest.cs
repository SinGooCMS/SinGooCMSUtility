using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Data;
using SinGooCMS.Utility.Extension;
using System.Linq;
using SinGooCMS.Utility;

namespace NTFxTest
{
    [TestClass]
    public class IPLocationTest
    {
        [TestMethod]
        public void TestIPArea()
        {
            var ip = "218.95.66.68";
            Console.WriteLine("IP定位：" + new IPScanner().IPLocation(ip));
        }
    }
}
