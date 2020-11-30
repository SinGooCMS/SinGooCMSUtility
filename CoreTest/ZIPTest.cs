using System;
using System.Text;
using NUnit.Framework;
using System.Data;
using SinGooCMS.Utility;
using SinGooCMS.Utility.Extension;
using System.Collections.Generic;

namespace CoreTest
{
    public class ZIPTest
    {
        [Test]
        public void TestZIP()
        {
            //当前程序的目录
            string baseDir = System.Environment.CurrentDirectory;
            string path = ZipUtils.Zip(baseDir);

            Console.WriteLine(path);
        }

        [Test]
        public void TestUnZip()
        {
            ZipUtils.UnZip(@"F:\jsonlee\W3Cschool-v2.1.0-win32-x64.zip", @"F:\ABC");
        }
    }
}
