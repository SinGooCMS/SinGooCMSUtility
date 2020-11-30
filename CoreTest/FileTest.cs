using System;
using System.Text;
using NUnit.Framework;
using System.Data;
using SinGooCMS.Utility;
using SinGooCMS.Utility.Extension;
using System.Collections.Generic;

namespace CoreTest
{
    public class FileTest
    {
        [Test]
        public void ReadAllFiles()
        {
            var lst = FileUtils.GetAllFiles(@"F:\jsonlee\study");
        }

        [Test]
        public void CreateQRCode()
        {
            var img = QRCodeUtils.GenerateQrCode("http://www.baidu.com", 30);
            img.Save(@"f:\qrcode.png",System.Drawing.Imaging.ImageFormat.Png);
        }
    }
}
