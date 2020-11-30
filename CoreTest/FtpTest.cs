using System;
using System.Text;
using NUnit.Framework;
using System.Data;
using SinGooCMS.Utility;
using System.Collections.Generic;
using SinGooCMS.Utility.Extension;

namespace CoreTest
{
    public class FtpTest
    {
        FtpClient ftp = FtpClient.GetClient("39.108.247.193", "fbac32bc", "fc156f7f66");

        [Test]
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
