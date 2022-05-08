﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Data;
using SinGooCMS.Utility;
using System.Text;

namespace NTFxTest
{
    [TestClass]
    public class SystemTest
    {
        [TestMethod]
        public void TestMapPath()
        {
            string path = "/";
            Console.WriteLine(SystemUtils.GetMapPath()); //当前绝对路径
            Console.WriteLine(SystemUtils.GetMapPath("/upload")); //当前绝对路径
            Console.WriteLine("桌面：" + SystemUtils.Desktop);
            Console.WriteLine("收藏夹：" + SystemUtils.Favorites);
            Console.WriteLine("我的文档：" + SystemUtils.MyDocuments);
        }

        [TestMethod]
        public void TestProcess()
        {
            //打开百度
            ProcessUtils.Execute(@"C:\Program Files (x86)\Google\Chrome\Application\chrome.exe", "http://www.baidu.com");

            //ProcessUtils.Shutdown();
        }

        [TestMethod]
        public void SetStartItem()
        {
            //软件设置开机自启动
            RegistryUtils.SetRunValue("CTTools", @"D:\MyDev\CTTool\CTTool.v1.2\CTTools.exe");

            //这个键是否添加了
            var regKey = RegistryUtils.GetRunValue("CTTools");
            Assert.IsTrue(regKey != null);

            //取消开机自启动
            RegistryUtils.DeleteRunValue("CTTools");
            var regKey2 = RegistryUtils.GetRunValue("CTTools");
            Assert.IsTrue(regKey2 == null);
        }
    }
}
