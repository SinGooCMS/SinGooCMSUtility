using System;
using System.Data;
using SinGooCMS.Utility.Extension;
using System.Linq;
using System.IO;
using System.Text;
using SinGooCMS.Utility;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace NTFxTest
{
    [TestClass]
    public class ShareFileTest
    {
        [TestMethod]
        public void TestShareFile()
        {
            ShareFileUtils.DisconnectAll();

            string path = @"\\172.18.20.101";
            string uid = @"luxshare\11000890";
            string pwd = @"abc";

            var client = ShareFileUtils.Connect(path, uid, pwd);
            if (client != null)
            {
                //var file = client.GetFile("KPI.xlsx");
                //client.DownFile(@"KPI.xlsx", "f:"); //下载
                //client.UpFile(@"f:\web.config", "ABC"); //上传

                //删除目录
                //var dir = client.GetDir("ABC");
                //dir.Delete(true);
            }
        }
    }
}
