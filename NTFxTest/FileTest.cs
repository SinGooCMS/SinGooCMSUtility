using System;
using System.Data;
using SinGooCMS.Utility.Extension;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SinGooCMS.Utility;

namespace NTFxTest
{
    [TestClass]
    public class FileTest
    {
        [TestMethod]
        public void ReadAllFiles()
        {
            var lst = FileUtils.GetAllFiles(@"F:\jsonlee\study");
            Console.WriteLine(lst.ToJson());
        }

        [TestMethod]
        public void CreateQRCode()
        {
            var img = QRCodeUtils.GenerateQrCode("http://www.baidu.com", 30);
            img.Save(@"f:\qrcode.png",System.Drawing.Imaging.ImageFormat.Png);
            Assert.AreEqual(true, System.IO.File.Exists(@"f:\qrcode.png"));
        }
    }
}
