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

        //[Test]
        //public void CreateQRCode()
        //{
        //    var img = QRCodeUtils.GenerateQrCode("http://www.singoo.top", 30, SystemUtils.GetMapPath("/TestSource/logo.png"));
        //    img.Save(FileUtils.Combine(SystemUtils.GetMapPath("/TestSource/ImageTest"),"qrcode.png"), System.Drawing.Imaging.ImageFormat.Png);
        //}

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

        [Test]
        public void TestFileContent_Type()
        {
            var filePath = SystemUtils.GetMapPath("TestSource/通讯录.xlsx");
            Console.WriteLine(FileUtils.GetContentType(filePath));
            Assert.AreEqual("application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", FileUtils.GetContentType(filePath));
        }

        [Test]
        public void TestBase64File()
        {
            //读文件到base64
            var base64Str = Base64FileUtils.ReadFileToString(SystemUtils.GetMapPath("TestSource/通讯录.xlsx"), true);
            Console.WriteLine(base64Str);

            //写入文件
            var filePath = SystemUtils.GetMapPath("TestSource/base64test/通讯录_base64test.xlsx");
            Base64FileUtils.SaveStringToFile(base64Str, filePath);
            Assert.IsTrue(System.IO.File.Exists(filePath));
        }

    }
}
