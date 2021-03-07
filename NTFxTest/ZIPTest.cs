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
            string destZipFile = SystemUtils.GetMapPath("/TestSource/TestSource.zip");
            if (System.IO.File.Exists(destZipFile))
                System.IO.File.Delete(destZipFile);

            string path = ZipUtils.Zip(SystemUtils.GetMapPath("/TestSource"), destZipFile);
            Console.WriteLine(path);
        }

        [TestMethod]
        public void TestUnZip()
        {
            string zipFile = SystemUtils.GetMapPath("/TestSource/TestSource.zip");
            if (System.IO.File.Exists(zipFile))
                ZipUtils.UnZip(zipFile, SystemUtils.GetMapPath("/TestSource/ZipTest/"));
        }
    }
}
