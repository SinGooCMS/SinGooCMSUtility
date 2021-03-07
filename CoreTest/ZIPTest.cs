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
            string destZipFile = SystemUtils.GetMapPath("/TestSource/TestSource.zip");
            if (System.IO.File.Exists(destZipFile))
                System.IO.File.Delete(destZipFile);

            string path = ZipUtils.Zip(SystemUtils.GetMapPath("/TestSource"), destZipFile);
            Console.WriteLine(path);
        }

        [Test]
        public void TestUnZip()
        {
            string zipFile = SystemUtils.GetMapPath("/TestSource/TestSource.zip");
            if (System.IO.File.Exists(zipFile))
                ZipUtils.UnZip(zipFile, SystemUtils.GetMapPath("/TestSource/ZipTest/"));
        }
    }
}
