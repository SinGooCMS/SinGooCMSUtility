using System;
using System.Text;
using NUnit.Framework;
using System.Data;
using SinGooCMS.Utility;
using System.Collections.Generic;
using SinGooCMS.Utility.Extension;

namespace CoreTest
{
    public class HttpTest
    {
        [Test]
        public void TestNetwork()
        {
            var getStr = NetWorkUtils.HttpGet("http://www.singoo.top");
            Console.WriteLine("get返回数据：" + getStr);
            var postStr = NetWorkUtils.HttpPost("http://passports.singoo.top/passports/login", "_loginname=admin&_loginpwd=123");
            Console.WriteLine("post返回数据：" + postStr);
        }

        [Test]
        public void TestPing()
        {
            Assert.AreEqual(true, NetWorkUtils.Ping("www.baidu.com"));
            Assert.AreEqual(true, NetWorkUtils.Ping("103.235.46.39"));
        }
    }
}
