using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using System.Data;
using SinGooCMS.Utility;
using SinGooCMS.Utility.Extension;
using System.Threading.Tasks;

namespace CoreTest
{
    public class FileTest
    {
        [Test]
        public void ReadAllFiles()
        {
            var lst = FileUtils.GetAllFiles(SystemUtils.GetMapPath("/TestSource/"));
        }

        [Test]
        public void CreateQRCode()
        {
            var img = QRCodeUtils.GenerateQrCode("http://www.singoo.top", 30, SystemUtils.GetMapPath("/TestSource/logo.png"));
            img.Save(FileUtils.Combine(SystemUtils.GetMapPath("/TestSource/ImageTest"),"qrcode.png"), System.Drawing.Imaging.ImageFormat.Png);
        }

        [Test]
        public async Task CreateCaptchaCode()
        {
            var captcha = CaptchaUtils.Create();
            await captcha.CheckCodeImg.WriteToFileAsync(FileUtils.Combine(SystemUtils.GetMapPath("/TestSource/ImageTest"), "Captcha.png"));
            Console.WriteLine("验证码字符串：" + captcha.CheckCodeString);
        }

        /// <summary>
        /// 计算文件哈希值
        /// </summary>
        [Test]
        public void CalFileHashVal()
        {
            var fs = FileUtils.ReadFileToStream(SystemUtils.GetMapPath("/TestSource/logo.png"));
            Console.WriteLine("哈希值："+fs.GetFileMD5());
        }

        [Test]
        public async Task SaveFileAsync()
        {
            var fs = FileUtils.ReadFileToStream(SystemUtils.GetMapPath("/TestSource/logo.png"));
            await fs.SaveFileAsync(SystemUtils.GetMapPath("/TestSource/logo2.png"));
            Assert.AreEqual(true, System.IO.File.Exists(SystemUtils.GetMapPath("/TestSource/logo2.png")));
        }

    }
}
