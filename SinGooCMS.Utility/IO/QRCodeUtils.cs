using System;
using QRCoder;

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
        /// <returns></returns>
        public static System.Drawing.Bitmap GenerateQrCode(string codeText, int size)
        {
            QRCodeGenerator qrGenerator = new QRCodeGenerator();
            QRCodeData qrCodeData = qrGenerator.CreateQrCode(codeText, QRCodeGenerator.ECCLevel.Q);
            QRCode qrCode = new QRCode(qrCodeData);
            return qrCode.GetGraphic(size);
        }
    }
}
