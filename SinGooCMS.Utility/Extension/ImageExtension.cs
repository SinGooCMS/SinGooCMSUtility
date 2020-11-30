using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace SinGooCMS.Utility.Extension
{
    /// <summary>
    /// image图像扩展
    /// </summary>
    public static class ImageExtension
    {
        #region image转base64字符串

        /// <summary>
        /// Image转成base64字符串
        /// </summary>
        /// <param name="image"></param>
        public static string ToBase64(this Image image)
        {
            using (MemoryStream memoryStream = new MemoryStream())
            {
                image.Save(memoryStream, image.RawFormat);
                byte[] imageBytes = memoryStream.ToArray();
                return Convert.ToBase64String(imageBytes);
            }
        }

        #endregion

        #region 像素色阶处理

        #region 调整光暗

        /// <summary>
        /// 调整光暗
        /// </summary>
        /// <param name="mybm">原始图片</param>
        /// <param name="val">增加或减少的光暗值</param>
        public static Image LDPic(this Image mybm, int val)
        {
            int width = mybm.Width;
            int height = mybm.Height;
            Bitmap bm = new Bitmap(width, height); //初始化一个记录经过处理后的图片对象
            int x, y; //x、y是循环次数，后面三个是记录红绿蓝三个值的
            for (x = 0; x < width; x++)
            {
                for (y = 0; y < height; y++)
                {
                    var pixel = ((Bitmap)mybm).GetPixel(x, y);
                    var resultR = pixel.R + val; //x、y是循环次数，后面三个是记录红绿蓝三个值的
                    var resultG = pixel.G + val; //x、y是循环次数，后面三个是记录红绿蓝三个值的
                    var resultB = pixel.B + val; //x、y是循环次数，后面三个是记录红绿蓝三个值的
                    bm.SetPixel(x, y, Color.FromArgb(resultR > 255 ? 255 : resultR, resultG > 255 ? 255 : resultG, resultB > 255 ? 255 : resultB)); //绘图
                }
            }

            return bm;
        }

        #endregion

        #region 反色处理

        /// <summary>
        /// 反色处理
        /// </summary>
        /// <param name="mybm">原始图片</param>
        public static Image RePic(this Image mybm)
        {
            int width = mybm.Width;
            int height = mybm.Height;
            var bm = new Bitmap(width, height); //初始化一个记录处理后的图片的对象

            for (var x = 0; x < width; x++)
            {
                for (var y = 0; y < height; y++)
                {
                    var pixel = ((Bitmap)mybm).GetPixel(x, y);
                    var resultR = 255 - pixel.R;
                    var resultG = 255 - pixel.G;
                    var resultB = 255 - pixel.B;
                    bm.SetPixel(x, y, Color.FromArgb(resultR, resultG, resultB)); //绘图
                }
            }

            return bm;
        }

        #endregion

        #region 浮雕处理

        /// <summary>
        /// 浮雕处理
        /// </summary>
        /// <param name="oldBitmap">原始图片</param>
        public static Image Relief(this Image oldBitmap)
        {
            int width = oldBitmap.Width;
            int height = oldBitmap.Height;
            var newBitmap = new Bitmap(width, height);

            for (int x = 0; x < width - 1; x++)
            {
                for (int y = 0; y < height - 1; y++)
                {
                    var color1 = ((Bitmap)oldBitmap).GetPixel(x, y);
                    var color2 = ((Bitmap)oldBitmap).GetPixel(x + 1, y + 1);
                    var r = Math.Abs(color1.R - color2.R + 128);
                    var g = Math.Abs(color1.G - color2.G + 128);
                    var b = Math.Abs(color1.B - color2.B + 128);
                    if (r > 255) r = 255;
                    if (r < 0) r = 0;
                    if (g > 255) g = 255;
                    if (g < 0) g = 0;
                    if (b > 255) b = 255;
                    if (b < 0) b = 0;
                    newBitmap.SetPixel(x, y, Color.FromArgb(r, g, b));
                }
            }

            return newBitmap;
        }

        #endregion

        #region 拉伸图片

        /// <summary>
        /// 拉伸图片
        /// </summary>
        /// <param name="bmp">原始图片</param>
        /// <param name="newW">新的宽度</param>
        /// <param name="newH">新的高度</param>
        public static Image ResizeImage(this Image bmp, int newW, int newH)
        {
            Bitmap bap = new Bitmap(newW, newH);
            using (Graphics g = Graphics.FromImage(bap))
            {
                g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                g.DrawImage(bmp, new Rectangle(0, 0, newW, newH), new Rectangle(0, 0, bmp.Width, bmp.Height), GraphicsUnit.Pixel);
                return bap;
            }
        }

        #endregion

        #region 滤色处理

        /// <summary>
        /// 滤色处理
        /// </summary>
        /// <param name="mybm">原始图片</param>
        public static Image ColorFilter(this Image mybm)
        {
            int width = mybm.Width;
            int height = mybm.Height;
            var bm = new Bitmap(width, height);

            for (var x = 0; x < width; x++)
            {
                int y;
                for (y = 0; y < height; y++)
                {
                    var pixel = ((Bitmap)mybm).GetPixel(x, y);
                    bm.SetPixel(x, y, Color.FromArgb(0, pixel.G, pixel.B)); //绘图
                }
            }

            return bm;
        }

        #endregion

        #region 转换为黑白图片

        /// <summary>
        /// 转换为黑白图片
        /// </summary>
        /// <param name="mybm">要进行处理的图片</param>
        public static Image ToBWPic(this Image mybm)
        {
            int width = mybm.Width;
            int height = mybm.Height;
            var bm = new Bitmap(width, height);
            for (var x = 0; x < width; x++)
            {
                for (var y = 0; y < height; y++)
                {
                    var pixel = ((Bitmap)mybm).GetPixel(x, y);
                    var result = (pixel.R + pixel.G + pixel.B) / 3; //记录处理后的像素值
                    bm.SetPixel(x, y, Color.FromArgb(result, result, result));
                }
            }

            return bm;
        }

        #endregion

        #region 马赛克

        /// <summary>
        /// 图片马赛克
        /// </summary>
        /// <param name="bitmap"></param>
        /// <param name="effectWidth">影响范围</param>
        /// <returns></returns>
        public static Image Mosaic(this Image bitmap, int effectWidth = 10)
        {
            // 差异最多的就是以照一定范围取样 之后直接去下一个范围
            for (int heightOfffset = 0; heightOfffset < bitmap.Height; heightOfffset += effectWidth)
            {
                for (int widthOffset = 0; widthOffset < bitmap.Width; widthOffset += effectWidth)
                {
                    int avgR = 0, avgG = 0, avgB = 0;
                    int blurPixelCount = 0;

                    for (int x = widthOffset; (x < widthOffset + effectWidth && x < bitmap.Width); x++)
                    {
                        for (int y = heightOfffset; (y < heightOfffset + effectWidth && y < bitmap.Height); y++)
                        {
                            System.Drawing.Color pixel = ((Bitmap)bitmap).GetPixel(x, y);

                            avgR += pixel.R;
                            avgG += pixel.G;
                            avgB += pixel.B;

                            blurPixelCount++;
                        }
                    }

                    // 计算范围平均
                    avgR = avgR / blurPixelCount;
                    avgG = avgG / blurPixelCount;
                    avgB = avgB / blurPixelCount;


                    // 所有范围内都设定此值
                    for (int x = widthOffset; (x < widthOffset + effectWidth && x < bitmap.Width); x++)
                    {
                        for (int y = heightOfffset; (y < heightOfffset + effectWidth && y < bitmap.Height); y++)
                        {

                            Color newColor = Color.FromArgb(avgR, avgG, avgB);
                            ((Bitmap)bitmap).SetPixel(x, y, newColor);
                        }
                    }
                }
            }

            return bitmap;
        }

        #endregion

        #endregion

        #region 左右翻转

        /// <summary>
        /// 左右翻转
        /// </summary>
        /// <param name="mybm">原始图片</param>
        public static Image LeftRightRev(this Image mybm)
        {
            int width = mybm.Width;
            int height = mybm.Height;
            var bm = new Bitmap(width, height);

            //x,y是循环次数,z是用来记录像素点的x坐标的变化的
            for (var y = height - 1; y >= 0; y--)
            {
                int x; //x,y是循环次数,z是用来记录像素点的x坐标的变化的
                int z; //x,y是循环次数,z是用来记录像素点的x坐标的变化的
                for (x = width - 1, z = 0; x >= 0; x--)
                {
                    var pixel = ((Bitmap)mybm).GetPixel(x, y);
                    bm.SetPixel(z++, y, Color.FromArgb(pixel.R, pixel.G, pixel.B)); //绘图
                }
            }

            return bm;
        }

        #endregion

        #region 上下翻转

        /// <summary>
        /// 上下翻转
        /// </summary>
        /// <param name="mybm">原始图片</param>
        public static Image UpDownRev(this Image mybm)
        {
            int width = mybm.Width;
            int height = mybm.Height;
            var bm = new Bitmap(width, height);

            for (var x = 0; x < width; x++)
            {
                int y;
                int z;
                for (y = height - 1, z = 0; y >= 0; y--)
                {
                    var pixel = ((Bitmap)mybm).GetPixel(x, y);
                    bm.SetPixel(x, z++, Color.FromArgb(pixel.R, pixel.G, pixel.B)); //绘图
                }
            }

            return bm;
        }

        #endregion        

        #region 裁剪图片

        /// <summary>
        /// 裁剪图片 -- 用GDI+   
        /// </summary>
        /// <param name="b">原始Bitmap</param>
        /// <param name="rec">裁剪区域</param>
        /// <returns>剪裁后的Bitmap</returns>
        public static Image CutImage(this Image b, Rectangle rec)
        {
            int w = b.Width;
            int h = b.Height;
            if (rec.X >= w || rec.Y >= h)
            {
                return null;
            }

            if (rec.X + rec.Width > w)
            {
                rec.Width = w - rec.X;
            }

            if (rec.Y + rec.Height > h)
            {
                rec.Height = h - rec.Y;
            }

            try
            {
                var bmpOut = new Bitmap(rec.Width, rec.Height, PixelFormat.Format24bppRgb);
                using (var g = Graphics.FromImage(bmpOut))
                {
                    g.DrawImage(b, new Rectangle(0, 0, rec.Width, rec.Height), new Rectangle(rec.X, rec.Y, rec.Width, rec.Height), GraphicsUnit.Pixel);
                    return bmpOut;
                }
            }
            catch (Exception)
            {
                return null;
            }
        }

        #endregion

        #region 裁剪并缩放

        /// <summary>
        /// 裁剪并缩放
        /// </summary>
        /// <param name="bmp">原始图片</param>
        /// <param name="rec">裁剪的矩形区域</param>
        /// <param name="newWidth">新的宽度</param>  
        /// <param name="newHeight">新的高度</param>  
        /// <returns>处理以后的图片</returns>
        public static Image CutAndResize(this Image bmp, Rectangle rec, int newWidth, int newHeight) => bmp.CutImage(rec).Compress(newWidth, newHeight);

        #endregion

        #region 压缩图片

        /// <summary>  
        ///  Resize图片   
        /// </summary>  
        /// <param name="bmp">原始Bitmap </param>  
        /// <param name="newWidth">新的宽度</param>  
        /// <param name="newHeight">新的高度</param>  
        /// <returns>处理以后的图片</returns>  
        public static Image Compress(this Image bmp, int newWidth, int newHeight)
        {
            if (newWidth > bmp.Width || newHeight > bmp.Height)
                return bmp;

            var b = new Bitmap(newWidth, newHeight);
            using (var g = Graphics.FromImage(b))
            {
                g.CompositingQuality = CompositingQuality.HighQuality;
                g.SmoothingMode = SmoothingMode.HighQuality;
                g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                g.DrawImage(bmp, new Rectangle(0, 0, newWidth, newHeight), new Rectangle(0, 0, bmp.Width, bmp.Height), GraphicsUnit.Pixel);
                return b;
            }
        }

        #endregion                

        #region 生成缩略图

        /// <summary>
        /// 生成缩略图
        /// </summary>
        /// <param name="originalImage">原图</param>
        /// <param name="width">缩略图宽度</param>
        /// <param name="height">缩略图高度</param>
        /// <param name="mode">生成缩略图的方式</param>    
        public static Image ThumbnailImage(this Image originalImage, int width, int height, ThumbnailCutMode mode = ThumbnailCutMode.Cut)
        {
            int towidth = width;
            int toheight = height;

            int x = 0;
            int y = 0;
            int ow = originalImage.Width;
            int oh = originalImage.Height;

            switch (mode)
            {
                case ThumbnailCutMode.Fixed:  //指定高宽缩放（可能变形）                
                    break;
                case ThumbnailCutMode.LockWidth:   //指定宽，高按比例                    
                    toheight = originalImage.Height * width / originalImage.Width;
                    break;
                case ThumbnailCutMode.LockHeight:   //指定高，宽按比例
                    towidth = originalImage.Width * height / originalImage.Height;
                    break;
                case ThumbnailCutMode.Cut: //指定高宽裁减（不变形）                
                    if (originalImage.Width / (double)originalImage.Height > towidth / (double)toheight)
                    {
                        oh = originalImage.Height;
                        ow = originalImage.Height * towidth / toheight;
                        y = 0;
                        x = (originalImage.Width - ow) / 2;
                    }
                    else
                    {
                        ow = originalImage.Width;
                        oh = originalImage.Width * height / towidth;
                        x = 0;
                        y = (originalImage.Height - oh) / 2;
                    }
                    break;
            }

            //新建一个bmp图片
            Image bitmap = new Bitmap(towidth, toheight);

            //新建一个画板
            Graphics g = Graphics.FromImage(bitmap);

            //设置高质量插值法
            g.InterpolationMode = InterpolationMode.High;

            //设置高质量,低速度呈现平滑程度
            g.SmoothingMode = SmoothingMode.HighQuality;

            //清空画布并以透明背景色填充
            g.Clear(Color.Transparent);

            //在指定位置并且按指定大小绘制原图片的指定部分
            g.DrawImage(originalImage, new Rectangle(0, 0, towidth, toheight), new Rectangle(x, y, ow, oh), GraphicsUnit.Pixel);

            try
            {
                return bitmap;
            }
            catch (Exception e)
            {
                throw e;
            }
            finally
            {
                originalImage.Dispose();
                g.Dispose();
            }
        }

        #endregion

        #region 获取图片中的各帧

        /// <summary>
        /// 获取图片中的各帧
        /// </summary>
        /// <param name="gif">源gif</param>
        /// <param name="pSavedPath">保存路径</param>
        public static void GetFrames(this Image gif, string pSavedPath)
        {
            var fd = new FrameDimension(gif.FrameDimensionsList[0]);
            int count = gif.GetFrameCount(fd); //获取帧数(gif图片可能包含多帧，其它格式图片一般仅一帧)
            for (int i = 0; i < count; i++) //以Jpeg格式保存各帧
            {
                gif.SelectActiveFrame(fd, i);
                gif.Save(pSavedPath + "\\frame_" + i + ".jpg", ImageFormat.Jpeg);
            }
        }

        #endregion
    }
}
