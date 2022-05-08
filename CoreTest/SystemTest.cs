using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using NUnit.Framework;
using SinGooCMS.Utility;
using System.Linq;

namespace CoreTest
{
    public class SystemTest
    {
        [Test]
        public void TestMapPath()
        {
            string path = "/";
            Console.WriteLine(SystemUtils.GetMapPath()); //当前绝对路径
            Console.WriteLine(SystemUtils.GetMapPath("/upload")); //当前绝对路径
            Console.WriteLine(SystemUtils.GetMapPath("/upload/1.txt")); //当前绝对路径
            Console.WriteLine("桌面：" + SystemUtils.Desktop);
            Console.WriteLine("收藏夹：" + SystemUtils.Favorites);
            Console.WriteLine("我的文档：" + SystemUtils.MyDocuments);
        }

        [Test]
        public void TestProcess()
        {
            //打开百度
            //ProcessUtils.Execute(@"C:\Program Files (x86)\Google\Chrome\Application\chrome.exe", "http://www.baidu.com");

            //ProcessUtils.Shutdown();

            string path = @"\\172.18.20.101";
            string uid = @"luxshare\11000890";
            string pwd = @"abc";
            string errMsg = "";

            var commandTexts = new List<string>() {
                $"net use {path} {pwd} /user:{uid}"
            };

            if(ProcessUtils.ExecuteCMD(commandTexts))
            {
                //FileInfo file=new FileInfo("");
                var lstFiles = new List<FileInfo>();
                var dir = new DirectoryInfo(path);
                foreach (var folder in dir.GetDirectories())
                {
                    foreach (var item in folder.GetFiles())
                    {
                        if (item.Name.Contains("KPI")) //文件名要包含KPI 3个字
                            lstFiles.Add(item);
                    }
                }
                var file = lstFiles.OrderByDescending(p => p.LastWriteTime).FirstOrDefault(); //日期最新的文件
                Console.WriteLine("文件名："+file.Name);

                //显示所有连接
                //var result = ShareFileUtils.ShowConnect();
                //Console.WriteLine("结果：" + result);
            }            
            
        }

        [Test]
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
