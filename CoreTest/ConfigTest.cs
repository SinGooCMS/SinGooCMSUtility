using System;
using System.Text;
using NUnit.Framework;
using System.Data;
using SinGooCMS.Utility;
using SinGooCMS.Utility.Extension;
using System.Collections.Generic;

namespace CoreTest
{
    public class ConfigTest
    {
        [Test]
        public void TestCfg()
        {
            //连接字符串
            Console.WriteLine($"ProviderName1：{ConfigUtils.ProviderName}\r\n");
            Console.WriteLine($"ProviderName2：{ConfigUtils.GetByPath<string>("ConnectionStrings:ProviderName")}\r\n");
            Console.WriteLine($"SQLConnSTR：{ConfigUtils.DefConnStr}\r\n");
            //appsetting
            Console.WriteLine($"EnableCache：{ConfigUtils.GetAppSetting<bool>("EnableCache")}\r\n");

            //core
            Console.WriteLine("配置值："+ConfigUtils.GetByPath<string>("Logging:LogLevel:Default"));
        }
    }
}
