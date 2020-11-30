using System;
using System.Text;
using NUnit.Framework;
using System.Data;
using SinGooCMS.Utility;
using SinGooCMS.Utility.Extension;
using System.Collections.Generic;

namespace CoreTest
{
    public class ImageTest
    {
        private readonly string filePath = @"F:\jsonlee\study\test\abc.png";

        [Test]
        public void TestTrans()
        {
            //文件转图像类型
            var image = ImageUtils.ReadFileToImage(filePath);
            var bitmap = ImageUtils.ReadFileBitmap(filePath);
            var stream = FileUtils.ReadFileToStream(filePath);
            Console.WriteLine("image宽高：" + image.Width + " x " + image.Height);
            Console.WriteLine("bitmap宽高：" + bitmap.Width + " x " + bitmap.Height);
            Console.WriteLine("stream-length：" + stream.Length);

            //base64和图片互转
            var base64Str = image.ToBase64();
            Console.WriteLine("base64string:"+base64Str);
            ImageUtils.Base64StrToImage(base64Str).Save(@"f:\123.png");
        }

        [Test]
        public void TestProcess()
        {
            var image = ImageUtils.ReadFileToImage(filePath);
            //转黑白图片
            image.ToBWPic().Save(@"f:\黑白.png");
            //调整光暗
            image.LDPic(50).Save(@"f:\光暗.png");
            //反色
            image.RePic().Save(@"f:\反色.png");
            //浮雕
            image.Relief().Save(@"f:\浮雕.png");
            //拉伸
            image.ResizeImage(500, 300).Save(@"f:\拉伸.png");
            //滤色
            image.ColorFilter().Save(@"f:\滤色.png");
            //马赛克
            image.Mosaic(10).Save(@"f:\马赛克.png");
            //上下翻转
            image.UpDownRev().Save(@"f:\上下翻转.png");
            //左右翻转
            image.LeftRightRev().Save(@"f:\左右翻转.png");
            //压缩图片
            image.Compress(1000, 600).Save(@"f:\压缩.png");
            //裁切
            image.CutImage(new System.Drawing.Rectangle(0, 0, 300, 200)).Save(@"f:\裁切.png"); ;

        }

        [Test]
        public void TestCut()
        {
            var image = ImageUtils.ReadFileToImage(filePath);
            //缩略图
            image.ThumbnailImage(300,150).Save(@"f:\缩略图.png");
            ImageUtils.MakeThumbnail(filePath, 500, 450);

            //水印
            //文字水印
            //ImageUtils.AddTextWatermark(filePath, "这是一个测试");
            //图片水印
            string watermarkPic = @"F:\jsonlee\study\test\logo.png";
            ImageUtils.AddImageWatermark(filePath, watermarkPic);
        }
    }
}
