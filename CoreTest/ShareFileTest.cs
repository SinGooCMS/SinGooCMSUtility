using System;
using System.Text;
using System.Linq;
using NUnit.Framework;
using SinGooCMS.Utility;
using SinGooCMS.Utility.Extension;

namespace CoreTest
{
    public class ShareFileTest
    {
        [Test]
        public void TestShareFile()
        {
            ShareFileUtils.DisconnectAll();

            string path = @"\\172.18.20.110";
            string uid = @"luxshare\11000890";
            string pwd = @"abc";

            var client = ShareFileUtils.Connect(path, uid, pwd);
            if (client != null)
            {
                //var file = client.GetFile("KPI.xlsx");
                //client.DownFile(@"KPI.xlsx", "f:"); //下载
                client.UpFile(@"f:\web.config", "ABC"); //上传
            }
        }
    }
}
