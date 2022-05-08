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

        //[TestMethod]
        //public void CreateQRCode()
        //{
        //    var img = QRCodeUtils.GenerateQrCode("http://www.singoo.top", 30, SystemUtils.GetMapPath("/TestSource/logo.png"));
        //    img.Save(FileUtils.Combine(SystemUtils.GetMapPath("/TestSource/ImageTest"), "qrcode.png"), System.Drawing.Imaging.ImageFormat.Png);
        //}

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

        [TestMethod]
        public void TestFileContent_Type()
        {
            var filePath = SystemUtils.GetMapPath("TestSource/通讯录.xlsx");
            //Console.WriteLine(FileUtils.GetContentType(filePath));
            Assert.AreEqual("application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", FileUtils.GetContentType(filePath));
        }

        [TestMethod]
        public async Task TestBase64File()
        {
            //读文件到base64
            var base64Str = Base64FileUtils.ReadFileToString(SystemUtils.GetMapPath("TestSource/通讯录.xlsx"),true);
            Console.WriteLine(base64Str);

            //写入文件
            var filePath = SystemUtils.GetMapPath("TestSource/base64test/通讯录_base64test.xlsx");
            Base64FileUtils.SaveStringToFile(base64Str,filePath);
            Assert.IsTrue(System.IO.File.Exists(filePath));

            //写入文件
            var filePath2 = SystemUtils.GetMapPath("TestSource/base64test/通讯录_base64test2.xlsx");
            await Base64FileUtils.SaveStringToFileAsync(base64Str, filePath2);
            Assert.IsTrue(System.IO.File.Exists(filePath2));
        }
    }
}
