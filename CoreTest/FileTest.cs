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
            var img = QRCodeUtils.GenerateQrCode("http://www.baidu.com", 30, @"f:\img.ico");
            img.Save(@"f:\qrcode.png", System.Drawing.Imaging.ImageFormat.Png);
        }

        [Test]
        public void CreateCaptchaCode()
        {
            var captcha = CaptchaUtils.Create();
            captcha.CheckCodeImg.WriteToFile(@"f:\Captcha.png");
            Console.WriteLine("验证码字符串：" + captcha.CheckCodeString);
        }

        /// <summary>
        /// 计算文件哈希值
        /// </summary>
        [Test]
        public void CalFileHashVal()
        {
            var hashVal = FileUtils.ReadFileToStream(@"F:\worklog.txt");
            //Console.WriteLine("哈希值："+hashVal.GetFileMD5());
            Assert.AreEqual("c8c08e7e31479e701f9afa7cd8970c74", hashVal.GetFileMD5());
        }

        [Test]
        public void SaveFileAsync()
        {
            var fs = FileUtils.ReadFileToStream(@"F:\worklog.txt");
            fs.SaveFileAsync(@"F:\worklog222.txt");
            Assert.AreEqual(true, System.IO.File.Exists(@"F:\worklog222.txt"));
        }

    }
}
