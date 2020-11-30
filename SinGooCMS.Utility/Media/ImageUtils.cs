using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using SinGooCMS.Utility.Extension;

namespace SinGooCMS.Utility
{
    /// <summary>
    /// 图片处理
    /// </summary>
    public static class ImageUtils
    {
        #region 图片文件转Image、Bitmap

        /// <summary>
        /// 文件转Image
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public static Image ReadFileToImage(string filePath) =>
            new Bitmap(filePath);

        /// <summary>
        /// 文件转Bitmap对象
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public static Bitmap ReadFileBitmap(string filePath) =>
            new Bitmap(filePath);

        #endregion

        #region base64和图片互转

        /// <summary>
        /// 将Base64字符串转换为图片
        /// </summary>
        /// <param name="base64String"></param>
        /// <returns></returns>
        public static Image Base64StrToImage(string base64String)
        {
            byte[] imageBytes = Convert.FromBase64String(base64String);
            using (MemoryStream ms = new MemoryStream(imageBytes, 0, imageBytes.Length))
            {
                ms.Write(imageBytes, 0, imageBytes.Length);
                return Image.FromStream(ms, true);
            }
        }

        /// <summary>
        /// Image转成base64字符串
        /// </summary>
        /// <param name="fileFullName"></param>
        public static string ImageToBase64(string fileFullName)
        {
            Image file = new Bitmap(fileFullName);
            using (MemoryStream memoryStream = new MemoryStream())
            {
                file.Save(memoryStream, file.RawFormat);
                byte[] imageBytes = memoryStream.ToArray();
                return Convert.ToBase64String(imageBytes);
            }
        }

        #endregion        

        #region 无损压缩图片

        /// <summary>
        /// 无损压缩图片
        /// </summary>
        /// <param name="sFile">原图片地址</param>
        /// <param name="dFile">压缩后保存图片地址</param>
        /// <param name="quality">压缩质量（数字越小压缩率越高）1-100</param>
        /// <param name="size">压缩后图片的最大大小</param>
        /// <param name="sfsc">是否是第一次调用</param>
        /// <returns></returns>
        public static bool CompressImage(string sFile, string dFile, byte quality = 90, int size = 1024, bool sfsc = true)
        {
            //如果是第一次调用，原始图像的大小小于要压缩的大小，则直接复制文件，并且返回true
            var firstFileInfo = new FileInfo(sFile);
            if (sfsc && firstFileInfo.Length < size * 1024)
            {
                firstFileInfo.CopyTo(dFile);
                return true;
            }

            using (Image iSource = Image.FromFile(sFile))
            {
                ImageFormat tFormat = iSource.RawFormat;
                int dHeight = iSource.Height;
                int dWidth = iSource.Width;
                int sW, sH;

                //按比例缩放
                Size temSize = new Size(iSource.Width, iSource.Height);
                if (temSize.Width > dHeight || temSize.Width > dWidth)
                {
                    if (temSize.Width * dHeight > temSize.Width * dWidth)
                    {
                        sW = dWidth;
                        sH = dWidth * temSize.Height / temSize.Width;
                    }
                    else
                    {
                        sH = dHeight;
                        sW = temSize.Width * dHeight / temSize.Height;
                    }
                }
                else
                {
                    sW = temSize.Width;
                    sH = temSize.Height;
                }

                using (Bitmap bmp = new Bitmap(dWidth, dHeight))
                {
                    using (Graphics g = Graphics.FromImage(bmp))
                    {
                        g.Clear(Color.WhiteSmoke);
                        g.CompositingQuality = CompositingQuality.HighQuality;
                        g.SmoothingMode = SmoothingMode.HighQuality;
                        g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                        g.DrawImage(iSource, new Rectangle((dWidth - sW) / 2, (dHeight - sH) / 2, sW, sH), 0, 0, iSource.Width, iSource.Height, GraphicsUnit.Pixel);

                        //以下代码为保存图片时，设置压缩质量
                        using (var ep = new EncoderParameters())
                        {
                            using (var eParam = new EncoderParameter(Encoder.Quality, new long[] { quality }))
                            {
                                ep.Param[0] = eParam;
                                try
                                {
                                    ImageCodecInfo[] arrayIci = ImageCodecInfo.GetImageEncoders();
                                    ImageCodecInfo jpegIcIinfo = arrayIci.FirstOrDefault(t => t.FormatDescription.Equals("JPEG"));
                                    if (jpegIcIinfo != null)
                                    {
                                        bmp.Save(dFile, jpegIcIinfo, ep);//dFile是压缩后的新路径
                                        FileInfo fi = new FileInfo(dFile);
                                        if (fi.Length > 1024 * size && quality > 10)
                                        {
                                            quality -= 10;
                                            CompressImage(sFile, dFile, quality, size, false);
                                        }
                                    }
                                    else
                                    {
                                        bmp.Save(dFile, tFormat);
                                    }
                                    return true;
                                }
                                catch
                                {
                                    return false;
                                }
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 无损压缩图片
        /// </summary>
        /// <param name="src">原图片文件流</param>
        /// <param name="dest">压缩后图片文件流</param>
        /// <param name="quality">压缩质量（数字越小压缩率越高）1-100</param>
        /// <param name="size">压缩后图片的最大大小</param>
        /// <param name="sfsc">是否是第一次调用</param>
        /// <returns></returns>
        public static bool CompressImage(Stream src, Stream dest, byte quality = 90, int size = 1024, bool sfsc = true)
        {
            //如果是第一次调用，原始图像的大小小于要压缩的大小，则直接复制文件，并且返回true
            if (sfsc && src.Length < size * 1024)
            {
                src.CopyTo(dest);
                return true;
            }

            using (Image imgSource = Image.FromStream(src))
            {
                ImageFormat tFormat = imgSource.RawFormat;
                int dHeight = imgSource.Height;
                int dWidth = imgSource.Width;
                int sW, sH;
                //按比例缩放
                Size temSize = new Size(imgSource.Width, imgSource.Height);
                if (temSize.Width > dHeight || temSize.Width > dWidth)
                {
                    if ((temSize.Width * dHeight) > (temSize.Width * dWidth))
                    {
                        sW = dWidth;
                        sH = (dWidth * temSize.Height) / temSize.Width;
                    }
                    else
                    {
                        sH = dHeight;
                        sW = (temSize.Width * dHeight) / temSize.Height;
                    }
                }
                else
                {
                    sW = temSize.Width;
                    sH = temSize.Height;
                }

                using (Bitmap bmp = new Bitmap(dWidth, dHeight))
                {
                    using (Graphics g = Graphics.FromImage(bmp))
                    {
                        g.Clear(Color.WhiteSmoke);
                        g.CompositingQuality = CompositingQuality.HighQuality;
                        g.SmoothingMode = SmoothingMode.HighQuality;
                        g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                        g.DrawImage(imgSource, new Rectangle((dWidth - sW) / 2, (dHeight - sH) / 2, sW, sH), 0, 0, imgSource.Width, imgSource.Height, GraphicsUnit.Pixel);

                        //以下代码为保存图片时，设置压缩质量
                        using (var ep = new EncoderParameters())
                        {
                            using (var eParam = new EncoderParameter(Encoder.Quality, new long[] { quality }))
                            {
                                ep.Param[0] = eParam;
                                try
                                {
                                    ImageCodecInfo[] arrayIci = ImageCodecInfo.GetImageEncoders();
                                    ImageCodecInfo jpegIcIinfo = arrayIci.FirstOrDefault(t => t.FormatDescription.Equals("JPEG"));
                                    if (jpegIcIinfo != null)
                                    {
                                        bmp.Save(dest, jpegIcIinfo, ep);//dFile是压缩后的新路径
                                        if (dest.Length > 1024 * size && quality > 10)
                                        {
                                            quality -= 10;
                                            CompressImage(src, dest, quality, size, false);
                                        }
                                    }
                                    else
                                    {
                                        bmp.Save(dest, tFormat);
                                    }
                                    return true;
                                }
                                catch
                                {
                                    return false;
                                }
                            }
                        }
                    }
                }
            }
        }

        #endregion

        #region 生成缩略图

        /// <summary>
        /// 生成缩略图
        /// </summary>
        /// <param name="originalFileName">原图文件</param>        
        /// <param name="width">缩略图宽度</param>
        /// <param name="height">缩略图高度</param>
        /// <param name="thumbnailFileName">缩略图文件（物理路径）</param>
        /// <param name="mode">生成缩略图的方式</param>    
        public static void MakeThumbnail(string originalFileName, int width, int height, string thumbnailFileName = "", ThumbnailCutMode mode = ThumbnailCutMode.Cut)
        {
            Image originalImage = Image.FromFile(originalFileName);

            string ext = Path.GetExtension(originalFileName);
            if (thumbnailFileName == "")
                thumbnailFileName = originalFileName.Replace(ext, "_thumb" + ext); //没有指定缩略图文件名，原文件名后面跟上_thumb

            originalImage.ThumbnailImage(width, height, mode).Save(thumbnailFileName);
        }

        #endregion

        #region 生成水印图
        /// <summary>
        /// 生成图片水印
        /// </summary>
        /// <param name="originalImagePath">原图文件路径</param>        
        /// <param name="watermarkImage">水印图文件路径</param>
        /// <param name="watermarkPosition">水印位置，默认正中间</param>
        /// <param name="alpha">透明度，默认0.5</param>
        /// <returns></returns>
        public static string AddImageWatermark(string originalImagePath, string watermarkImage, WatermarkPosition watermarkPosition = WatermarkPosition.MiddleCenter, float alpha = 0.5f) =>
            AddWatermark(originalImagePath, "图片水印", watermarkImage, string.Empty, 0, string.Empty, string.Empty, watermarkPosition, alpha);

        /// <summary>
        /// 生成文字水印
        /// </summary>
        /// <param name="originalImagePath"></param>
        /// <param name="watermarkText"></param>
        /// <param name="fontColor"></param>
        /// <param name="fontFamily"></param>
        /// <param name="watermarkPosition"></param>
        /// <param name="alpha"></param>
        /// <returns></returns>
        public static string AddTextWatermark(string originalImagePath, string watermarkText, string fontColor = "#ff0000", string fontFamily = "黑体", WatermarkPosition watermarkPosition = WatermarkPosition.MiddleCenter, float alpha = 0.2f)
        {
            //文字大小 按原图的1/10
            var img = Image.FromFile(originalImagePath);
            if (img != null)
            {
                int fontSize = img.Width;
                if (img.Height < img.Width)
                    fontSize = img.Height;

                fontSize = (int)(fontSize * 0.1);

                return AddWatermark(originalImagePath, "文字水印", string.Empty, watermarkText, fontSize, fontColor, fontFamily, watermarkPosition, alpha);
            }

            return string.Empty;
        }

        /// <summary>
        /// 生成文字水印
        /// </summary>
        /// <param name="originalImagePath"></param>        
        /// <param name="watermarkText"></param>
        /// <param name="fontSize"></param>
        /// <param name="fontColor"></param>
        /// <param name="fontFamily"></param>
        /// <param name="watermarkPosition"></param>
        /// <param name="alpha"></param>
        /// <returns></returns>
        public static string AddTextWatermark(string originalImagePath, string watermarkText, int fontSize, string fontColor, string fontFamily, WatermarkPosition watermarkPosition = WatermarkPosition.MiddleCenter, float alpha = 0.5f) =>
            AddWatermark(originalImagePath, "文字水印", string.Empty, watermarkText, fontSize, fontColor, fontFamily, watermarkPosition, alpha);

        /// <summary>
        /// 生成水印图
        /// </summary>
        /// <param name="originalImagePath">源图绝对路径</param>
        /// <param name="watermarkType">水印类型:文字水印,图片水印</param>        
        /// <param name="watermarkImage">水印图绝对路径</param>
        /// <param name="watermarkText">水印文字</param>
        /// <param name="fontSize">字体大小</param>
        /// <param name="fontColor">字体颜色</param>
        /// <param name="fontFamily">字体</param>
        /// <param name="watermarkPosition">水印位置</param>
        /// <param name="alpha">透明度</param>
        /// <returns></returns>
        public static string AddWatermark(string originalImagePath, string watermarkType, string watermarkImage
            , string watermarkText, int fontSize, string fontColor, string fontFamily, WatermarkPosition watermarkPosition, float alpha)
        {
            Image img = Image.FromFile(originalImagePath);
            // 封装 GDI+ 位图，此位图由图形图像及其属性的像素数据组成。   
            Bitmap bmPhoto = new Bitmap(img.Width, img.Height, PixelFormat.Format32bppRgb);
            // 设定分辨率
            bmPhoto.SetResolution(72, 72);
            System.Drawing.Graphics g = Graphics.FromImage(bmPhoto);
            //设置高质量插值法
            g.InterpolationMode = InterpolationMode.High;
            //消除锯齿
            g.SmoothingMode = SmoothingMode.AntiAlias;
            //g.SmoothingMode = SmoothingMode.HighQuality;
            g.DrawImage(img, new Rectangle(0, 0, img.Width, img.Height), 0, 0, img.Width, img.Height, GraphicsUnit.Pixel);

            //文件扩展名
            string strExt = Path.GetExtension(originalImagePath);
            //生成的水印图文件名
            string strWatermarkFile = originalImagePath.Replace(strExt, "_watermark" + strExt);

            ImageAttributes imageAttributes = new ImageAttributes();
            ColorMap colorMap = new ColorMap();
            colorMap.OldColor = Color.FromArgb(255, 0, 255, 0);
            colorMap.NewColor = Color.FromArgb(0, 0, 0, 0);
            ColorMap[] remapTable = { colorMap };
            imageAttributes.SetRemapTable(remapTable, ColorAdjustType.Bitmap);

            float[][] colorMatrixElements = {
                                                 new float[] {1.0f,  0.0f,  0.0f,  0.0f, 0.0f},
                                                 new float[] {0.0f,  1.0f,  0.0f,  0.0f, 0.0f},
                                                 new float[] {0.0f,  0.0f,  1.0f,  0.0f, 0.0f},
                                                 new float[] {0.0f,  0.0f,  0.0f,  alpha, 0.0f},  //水印透明度    
                                                 new float[] {0.0f,  0.0f,  0.0f,  0.0f, 1.0f}
                                            };
            ColorMatrix colorMatrix = new ColorMatrix(colorMatrixElements);
            imageAttributes.SetColorMatrix(colorMatrix, ColorMatrixFlag.Default, ColorAdjustType.Bitmap);

            //水印所在位置
            int xpos = 0;
            int ypos = 0;

            int intWatermarkWidth = 0;
            int intWatermarkHeight = 0;

            Image watermark = null;
            if (watermarkType.Equals("图片水印") && File.Exists(watermarkImage))
            {
                //加载水印图片
                watermark = new Bitmap(watermarkImage);
                intWatermarkWidth = watermark.Width;
                intWatermarkHeight = watermark.Height;
            }
            else if (watermarkType.Equals("文字水印") && watermarkText.Trim().Length > 0)
            {
                SizeF size = g.MeasureString(watermarkText, new Font(new FontFamily(fontFamily), fontSize));
                intWatermarkWidth = (int)size.Width;
                intWatermarkHeight = (int)size.Height;
            }

            switch (watermarkPosition)
            {
                case WatermarkPosition.TopLeft:
                    xpos = (int)(img.Width * (float).01);
                    ypos = (int)(img.Height * (float).01);
                    break;
                case WatermarkPosition.TopCenter:
                    xpos = (int)((img.Width * (float).50) - (intWatermarkWidth / 2));
                    ypos = (int)(img.Height * (float).01);
                    break;
                case WatermarkPosition.TopRight:
                    xpos = (int)((img.Width * (float).99) - (intWatermarkWidth));
                    ypos = (int)(img.Height * (float).01);
                    break;
                case WatermarkPosition.MiddleLeft:
                    xpos = (int)(img.Width * (float).01);
                    ypos = (int)((img.Height * (float).50) - (intWatermarkHeight / 2));
                    break;
                case WatermarkPosition.MiddleCenter:
                    xpos = (int)((img.Width * (float).50) - (intWatermarkWidth / 2));
                    ypos = (int)((img.Height * (float).50) - (intWatermarkHeight / 2));
                    break;
                case WatermarkPosition.MiddleRight:
                    xpos = (int)((img.Width * (float).99) - (intWatermarkWidth));
                    ypos = (int)((img.Height * (float).50) - (intWatermarkHeight / 2));
                    break;
                case WatermarkPosition.BottomLeft:
                    xpos = (int)(img.Width * (float).01);
                    ypos = (int)((img.Height * (float).99) - intWatermarkHeight);
                    break;
                case WatermarkPosition.BottomCenter:
                    xpos = (int)((img.Width * (float).50) - (intWatermarkWidth / 2));
                    ypos = (int)((img.Height * (float).99) - intWatermarkHeight);
                    break;
                case WatermarkPosition.BottomRight:
                    xpos = (int)((img.Width * (float).99) - (intWatermarkWidth));
                    ypos = (int)((img.Height * (float).99) - intWatermarkHeight);
                    break;
            }

            if (watermark != null) //在原图上画图片水印
                g.DrawImage(watermark, new Rectangle(xpos, ypos, intWatermarkWidth, intWatermarkHeight), 0, 0, intWatermarkWidth, intWatermarkHeight, GraphicsUnit.Pixel, imageAttributes);
            else
            {
                //在原图上画文本水印
                Font font = new Font(fontFamily, fontSize); //文字字体
                Color fColor = ColorTranslator.FromHtml(fontColor);
                Color txtColor = Color.FromArgb(Convert.ToInt32(alpha * 255), fColor);//文字颜色
                SolidBrush brush = new SolidBrush(txtColor);
                g.DrawString(watermarkText, font, brush, xpos, ypos);
            }

            ImageCodecInfo[] codecs = ImageCodecInfo.GetImageEncoders();
            ImageCodecInfo ici = null;
            foreach (ImageCodecInfo codec in codecs)
            {
                if (codec.MimeType.IndexOf("jpeg") > -1)
                {
                    ici = codec;
                }
            }
            EncoderParameters encoderParams = new EncoderParameters();
            long[] qualityParam = new long[1];
            qualityParam[0] = 80; //图片质量

            EncoderParameter encoderParam = new EncoderParameter(Encoder.Quality, qualityParam);
            encoderParams.Param[0] = encoderParam;

            if (ici != null)
            {
                bmPhoto.Save(strWatermarkFile, ici, encoderParams);
            }
            else
            {
                bmPhoto.Save(strWatermarkFile);
            }

            g.Dispose();
            img.Dispose();
            if (watermark != null)
                watermark.Dispose();
            imageAttributes.Dispose();

            return strWatermarkFile;
        }

        #endregion
    }
}