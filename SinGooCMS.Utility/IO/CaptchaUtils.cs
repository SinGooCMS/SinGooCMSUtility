using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

namespace SinGooCMS.Utility
{
    /// <summary>
    /// 图形验证码工具
    /// </summary>
    public class CaptchaUtils
    {
        #region 公共属性

        /// <summary>
        /// 验证码
        /// </summary>
        public string CheckCodeString { get; set; }
        /// <summary>
        /// 验证码图片
        /// </summary>
        public byte[] CheckCodeImg { get; set; }

        #endregion

        /// <summary>
        /// 生成验证码
        /// </summary>
        /// <returns></returns>
        public static CaptchaUtils Create()
        {
            string validCode = GenerateCheckCode();
            return new CaptchaUtils
            {
                CheckCodeString = validCode,
                CheckCodeImg = CreateCheckCodeImage(validCode)
            };
        }

        #region 私有方法

        private static string GenerateCheckCode()
        {
            string chkCode = string.Empty;
            //验证码的字符集，去掉了一些容易混淆的字符 
            char[] character = { '2', '3', '4', '5', '6', '8', '9', 'a', 'b', 'd', 'e', 'f', 'h', 'k', 'm', 'n', 'r', 'x', 'y', 'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'J', 'K', 'L', 'M', 'N', 'P', 'R', 'S', 'T', 'W', 'X', 'Y' };
            Random rnd = new Random();
            //生成验证码字符串 
            for (int i = 0; i < 4; i++)
            {
                chkCode += character[rnd.Next(character.Length)];
            }

            return chkCode;
        }

        private static byte[] CreateCheckCodeImage(string codeString)
        {
            string checkCode = codeString;
            if (checkCode == null || checkCode.Trim() == String.Empty)
                return null;

            int codeW = 80;
            int codeH = 22;
            int fontSize = 16;

            Random rnd = new Random();
            //颜色列表，用于验证码、噪线、噪点 
            Color[] color = { Color.Black, Color.Red, Color.Blue, Color.Green, Color.Orange, Color.Brown, Color.Brown, Color.DarkBlue };
            //字体列表，用于验证码 
            string[] font = { "Times New Roman", "Verdana", "Arial", "Gungsuh", "Impact" };

            //创建画布
            Bitmap bmp = new Bitmap(codeW, codeH);
            Graphics g = Graphics.FromImage(bmp);
            g.Clear(Color.White);
            //画噪线 
            for (int i = 0; i < 1; i++)
            {
                int x1 = rnd.Next(codeW);
                int y1 = rnd.Next(codeH);
                int x2 = rnd.Next(codeW);
                int y2 = rnd.Next(codeH);
                Color clr = color[rnd.Next(color.Length)];
                g.DrawLine(new Pen(clr), x1, y1, x2, y2);
            }
            //画验证码字符串 
            for (int i = 0; i < checkCode.Length; i++)
            {
                string fnt = font[rnd.Next(font.Length)];
                Font ft = new Font(fnt, fontSize);
                Color clr = color[rnd.Next(color.Length)];
                g.DrawString(checkCode[i].ToString(), ft, new SolidBrush(clr), (float)i * 18 + 2, (float)0);
            }
            //画噪点 
            for (int i = 0; i < 100; i++)
            {
                int x = rnd.Next(bmp.Width);
                int y = rnd.Next(bmp.Height);
                Color clr = color[rnd.Next(color.Length)];
                bmp.SetPixel(x, y, clr);
            }

            //将验证码图片写入内存流，并将其以 "image/Png" 格式输出 
            MemoryStream ms = new MemoryStream();
            bmp.Save(ms, ImageFormat.Png);

            //显式释放资源 
            bmp.Dispose();
            g.Dispose();

            return ms.ToArray();
        }

        #endregion
    }
}
