using System;
using System.Data;
using SinGooCMS.Utility.Extension;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SinGooCMS.Utility;
using System.Threading.Tasks;

namespace NTFxTest
{
    [TestClass]
    public class FileTest
    {
        readonly string savePath = SystemUtils.GetMapPath("/TestSource/ImageTest");
        public FileTest()
        {
            FileUtils.CreateDirectory(savePath);
        }

        [TestMethod]
        public void ReadAllFiles()
        {
            var lst = FileUtils.GetAllFiles(SystemUtils.GetMapPath("/TestSource/"));
        }

        [TestMethod]
        public void CreateQRCode()
        {
            var img = QRCodeUtils.GenerateQrCode("http://www.singoo.top", 30, SystemUtils.GetMapPath("/TestSource/logo.png"));
            img.Save(FileUtils.Combine(SystemUtils.GetMapPath("/TestSource/ImageTest"), "qrcode.png"), System.Drawing.Imaging.ImageFormat.Png);
        }

        [TestMethod]
        public async Task CreateCaptchaCode()
        {
            var captcha = CaptchaUtils.Create();
            await captcha.CheckCodeImg.WriteToFileAsync(FileUtils.Combine(SystemUtils.GetMapPath("/TestSource/ImageTest"), "Captcha.png"));
            Console.WriteLine("验证码字符串：" + captcha.CheckCodeString);
        }

        /// <summary>
        /// 计算文件哈希值
        /// </summary>
        [TestMethod]
        public void CalFileHashVal()
        {
            var fs = FileUtils.ReadFileToStream(SystemUtils.GetMapPath("/TestSource/logo.png"));
            Console.WriteLine("哈希值：" + fs.GetFileMD5());
        }

        [TestMethod]
        public async Task SaveFileAsync()
        {
            var fs = FileUtils.ReadFileToStream(SystemUtils.GetMapPath("/TestSource/logo.png"));
            await fs.SaveFileAsync(SystemUtils.GetMapPath("/TestSource/logo2.png"));
            Assert.AreEqual(true, System.IO.File.Exists(SystemUtils.GetMapPath("/TestSource/logo2.png")));
        }
    }
}
