using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Data;
using SinGooCMS.Utility.Extension;
using System.Linq;
using SinGooCMS.Utility;
using System.Threading.Tasks;

namespace NTFxTest
{
    [TestClass]
    public class ImageTest
    {
        //源图
        private readonly string filePath = SystemUtils.GetMapPath("/TestSource/original.png");
        //保存的目录
        private readonly string savePath = SystemUtils.GetMapPath("/TestSource/ImageTest/");

        [TestMethod]
        public async Task TestTrans()
        {
            FileUtils.CreateDirectory(savePath);

            //文件转图像类型
            var image = ImageUtils.ReadFileToImage(filePath);
            var bitmap = ImageUtils.ReadFileBitmap(filePath);
            var stream = FileUtils.ReadFileToStream(filePath);
            Console.WriteLine("image宽高：" + image.Width + " x " + image.Height);
            Console.WriteLine("bitmap宽高：" + bitmap.Width + " x " + bitmap.Height);
            Console.WriteLine("stream-length：" + stream.Length);

            //base64和图片互转
            var base64Str = image.ToBase64();
            await FileUtils.WriteFileContentAsync(FileUtils.Combine(savePath, "original_base64.txt"), base64Str);
            Console.WriteLine("base64string:" + base64Str);
            ImageUtils.Base64StrToImage(base64Str).Save(FileUtils.Combine(savePath, "original_base64.png"));
        }

        [TestMethod]
        public void TestProcess()
        {
            FileUtils.CreateDirectory(savePath);

            var image = ImageUtils.ReadFileToImage(filePath);
            //转黑白图片
            image.ToBWPic().Save(FileUtils.Combine(savePath, "黑白.png"));
            //调整光暗
            image.LDPic(50).Save(FileUtils.Combine(savePath, "光暗.png"));
            //反色
            image.RePic().Save(FileUtils.Combine(savePath, "反色.png"));
            //浮雕
            image.Relief().Save(FileUtils.Combine(savePath, "浮雕.png"));
            //拉伸
            image.ResizeImage(500, 300).Save(FileUtils.Combine(savePath, "拉伸.png"));
            //滤色
            image.ColorFilter().Save(FileUtils.Combine(savePath, "滤色.png"));
            //马赛克
            image.Mosaic(10).Save(FileUtils.Combine(savePath, "马赛克.png"));
            //上下翻转
            image.UpDownRev().Save(FileUtils.Combine(savePath, "上下翻转.png"));
            //左右翻转
            image.LeftRightRev().Save(FileUtils.Combine(savePath, "左右翻转.png"));
            //压缩图片
            image.Compress(600, 400).Save(FileUtils.Combine(savePath, "压缩.png"));
            //等比压缩图片
            image.CompressRate().Save(FileUtils.Combine(savePath, "等比压缩.png"));
            //等宽压缩图片
            image.CompressWidth(300).Save(FileUtils.Combine(savePath, "等宽压缩.png"));
            //等高压缩图片
            image.CompressHeight(200).Save(FileUtils.Combine(savePath, "等高压缩.png"));
            //裁切
            image.CutImage(new System.Drawing.Rectangle(0, 0, 300, 200)).Save(FileUtils.Combine(savePath, "裁切.png"));

        }

        [TestMethod]
        public void TestCut()
        {
            FileUtils.CreateDirectory(savePath);

            var image = ImageUtils.ReadFileToImage(filePath);
            //缩略图
            image.ThumbnailImage(300, 150).Save(FileUtils.Combine(savePath, "缩略图.png"));
            ImageUtils.MakeThumbnail(filePath, 500, 450, FileUtils.Combine(savePath, "thumb.png"));

            //水印
            //文字水印
            ImageUtils.AddTextWatermark(filePath, "这是一个测试", "#ffffff", "微软雅黑", WatermarkPosition.MiddleCenter, 0.6f, FileUtils.Combine(savePath, "文本水印.png"));
            //图片水印
            string watermarkPic = SystemUtils.GetMapPath("/TestSource/logo.png");
            ImageUtils.AddImageWatermark(filePath, watermarkPic, WatermarkPosition.MiddleCenter, 0.6f, FileUtils.Combine(savePath, "图片水印.png"));
        }
    }
}
