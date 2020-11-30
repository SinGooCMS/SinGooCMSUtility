using System;
using System.Data;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SinGooCMS.Utility.Extension;
using SinGooCMS.Utility;

namespace NTFxTest
{
    [TestClass]
    public class ConfigTest
    {
        [TestMethod]
        public void TestCfg()
        {
            //连接字符串
            Console.WriteLine($"ProviderName：{ConfigUtils.ProviderName}\r\n");
            Console.WriteLine($"SQLConnSTR：{ConfigUtils.DefConnStr}\r\n");
            //appsetting
            Console.WriteLine($"EnableCache：{ConfigUtils.GetAppSetting<string>("EnableCache")}\r\n");
        }
    }
}
