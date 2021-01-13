using System;
using System.Data;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SinGooCMS.Utility.Extension;
using SinGooCMS.Utility;

namespace NTFxTest
{
    [TestClass]
    public class ZIPTest
    {
        [TestMethod]
        public void TestZIP()
        {
            //当前程序的目录
            string baseDir = System.Environment.CurrentDirectory;
            string path = ZipUtils.Zip(baseDir);

            Console.WriteLine(path);
        }

        [TestMethod]
        public void TestUnZip()
        {
            ZipUtils.UnZip(@"F:\jsonlee\W3Cschool-v2.1.0-win32-x64.zip", @"F:\ABC");
        }
    }
}
