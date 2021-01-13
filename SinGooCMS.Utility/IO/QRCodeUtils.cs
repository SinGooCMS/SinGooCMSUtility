using System;
using System.Drawing;
using QRCoder;
using SinGooCMS.Utility.Extension;

namespace SinGooCMS.Utility
{
    /// <summary>
    /// 二维码工具类 开源地址：https://github.com/codebude/QRCoder/
    /// </summary>
    public class QRCodeUtils
    {
        /// <summary>
        /// 创建二维码
        /// </summary>
        /// <param name="codeText">二维码文本</param>
        /// <param name="size">尺寸大小</param>
        /// <param name="icoFilePath">中间显示的图标</param>
        /// <returns></returns>
        public static System.Drawing.Bitmap GenerateQrCode(string codeText, int size, string icoFilePath = "")
        {
            QRCodeGenerator qrGenerator = new QRCodeGenerator();
            QRCodeData qrCodeData = qrGenerator.CreateQrCode(codeText, QRCodeGenerator.ECCLevel.Q);
            QRCode qrCode = new QRCode(qrCodeData);

            return icoFilePath.IsNullOrEmpty()
                ? qrCode.GetGraphic(size)
                : qrCode.GetGraphic(size, Color.Black, Color.White, ImageUtils.ReadFileBitmap(icoFilePath));
        }
    }
}
