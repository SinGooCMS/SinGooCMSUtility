using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Data;
using SinGooCMS.Utility.Extension;
using System.Linq;
using SinGooCMS.Utility;

namespace NTFxTest
{
    [TestClass]
    public class FtpTest
    {
        FtpClient ftp = FtpClient.GetClient("39.108.247.193", "fbac3245", "fc156f7fdd");

        [TestMethod]
        public void TestDownFile()
        {            
            ftp.Download("/web/web.config", "f:\\web.config",true);

            var lst = ftp.GetFiles("/web/Upload/pdf/");
            Console.WriteLine(lst.ToJson());

            var files = ftp.GetFilesDetails("/web/Upload");
            Console.WriteLine(files.ToJson());

            Assert.AreEqual(true, System.IO.File.Exists("f:\\web.config"));
        }
    }
}
