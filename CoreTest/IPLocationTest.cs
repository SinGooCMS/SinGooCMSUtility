using System;
using System.Text;
using NUnit.Framework;
using System.Data;
using SinGooCMS.Utility;
using SinGooCMS.Utility.Extension;
using System.Collections.Generic;

namespace CoreTest
{
    public class IPLocationTest
    {
        [Test]
        public void TestIPArea()
        {
            /*
             * 1、在nuget包管理里面加入 System.Text.Encoding.CodePages
             * 2、注册 System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);
             * 这样就支持 gb2312 了
             */
            System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);
            
            var ip = "218.95.66.68";
            Console.WriteLine("IP定位："+new IPScanner().IPLocation(ip));
        }
    }
}
