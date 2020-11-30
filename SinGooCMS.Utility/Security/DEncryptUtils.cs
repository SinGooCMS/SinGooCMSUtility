using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security.Cryptography;
using System.IO;

namespace SinGooCMS.Utility
{
    /// <summary>
    /// 加密解密工具
    /// </summary>
    public static class DEncryptUtils
    {
        #region 对称加密

        #region DES

        /// <summary>
        /// DES加密
        /// </summary>
        /// <param name="strTxt"></param>
        /// <param name="strKey"></param>
        /// <returns></returns>
        public static string DESEncrypt(string strTxt, string strKey)
        {
            DESCryptoServiceProvider provider = new DESCryptoServiceProvider();
            provider.Key = Encoding.ASCII.GetBytes(strKey.Substring(0, 8));
            provider.IV = Encoding.ASCII.GetBytes(strKey.Substring(0, 8));
            //byte[] bytes = Encoding.GetEncoding("GB2312").GetBytes(strTxt); 20120530pm1734
            byte[] bytes = Encoding.UTF8.GetBytes(strTxt);
            MemoryStream stream = new MemoryStream();
            CryptoStream stream2 = new CryptoStream(stream, provider.CreateEncryptor(), CryptoStreamMode.Write);
            stream2.Write(bytes, 0, bytes.Length);
            stream2.FlushFinalBlock();
            StringBuilder builder = new StringBuilder();
            foreach (byte num in stream.ToArray())
            {
                builder.AppendFormat("{0:X2}", num);
            }
            stream.Close();
            return builder.ToString();
        }

        /// <summary>
        /// Des 解密
        /// </summary>
        /// <param name="strTxt"></param>
        /// <param name="strKey"></param>
        /// <returns></returns>
        public static string DESDecrypt(string strTxt, string strKey)
        {
            DESCryptoServiceProvider provider = new DESCryptoServiceProvider();
            provider.Key = Encoding.ASCII.GetBytes(strKey.Substring(0, 8));
            provider.IV = Encoding.ASCII.GetBytes(strKey.Substring(0, 8));
            byte[] buffer = new byte[strTxt.Length / 2];
            for (int i = 0; i < (strTxt.Length / 2); i++)
            {
                int num2 = Convert.ToInt32(strTxt.Substring(i * 2, 2), 0x10);
                buffer[i] = (byte)num2;
            }
            MemoryStream stream = new MemoryStream();
            CryptoStream stream2 = new CryptoStream(stream, provider.CreateDecryptor(), CryptoStreamMode.Write);
            stream2.Write(buffer, 0, buffer.Length);
            stream2.FlushFinalBlock();
            stream.Close();
            //return Encoding.GetEncoding("GB2312").GetString(stream.ToArray());
            //20120510pm1449修改
            return Encoding.UTF8.GetString(stream.ToArray());
        }

        #endregion

        #region AES        

        /// <summary>
        /// AES加密
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public static string AESEncrypt(string text)
        {
            Rijndael rijndael = Rijndael.Create();
            byte[] buffer = new byte[] {
                0xa6, 0x7d, 0xe1, 0x3f, 0x35, 14, 0xe1, 0xa9, 0x83, 0xa5, 0x62, 170, 0x7a, 0xae, 0x79, 0x98,
                0xa7, 0x33, 0x49, 0xff, 230, 0xae, 0xbf, 0x8d, 0x8d, 0x20, 0x8a, 0x49, 0x31, 0x3a, 0x12, 0x40
             };
            byte[] buffer2 = new byte[] { 0xf8, 0x8b, 1, 0xfb, 8, 0x85, 0x9a, 0xa4, 190, 0x45, 40, 0x56, 3, 0x42, 0xf6, 0x19 };
            rijndael.Key = buffer;
            rijndael.IV = buffer2;
            MemoryStream stream = new MemoryStream();
            ICryptoTransform transform = new ToBase64Transform();
            CryptoStream stream2 = new CryptoStream(stream, transform, CryptoStreamMode.Write);
            CryptoStream stream3 = new CryptoStream(stream2, rijndael.CreateEncryptor(), CryptoStreamMode.Write);
            UTF8Encoding encoding = new UTF8Encoding();
            byte[] bytes = encoding.GetBytes(text);
            stream3.Write(bytes, 0, bytes.Length);
            stream3.FlushFinalBlock();
            byte[] buffer4 = new byte[stream.Length];
            stream.Position = 0;
            stream.Read(buffer4, 0, (int)stream.Length);
            return encoding.GetString(buffer4);
        }

        /// <summary>
        /// AES解密
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public static string AESDecrypt(string text)
        {
            Rijndael rijndael = Rijndael.Create();
            byte[] buffer = new byte[] {
                0xa6, 0x7d, 0xe1, 0x3f, 0x35, 14, 0xe1, 0xa9, 0x83, 0xa5, 0x62, 170, 0x7a, 0xae, 0x79, 0x98,
                0xa7, 0x33, 0x49, 0xff, 230, 0xae, 0xbf, 0x8d, 0x8d, 0x20, 0x8a, 0x49, 0x31, 0x3a, 0x12, 0x40
             };
            byte[] buffer2 = new byte[] { 0xf8, 0x8b, 1, 0xfb, 8, 0x85, 0x9a, 0xa4, 190, 0x45, 40, 0x56, 3, 0x42, 0xf6, 0x19 };
            rijndael.Key = buffer;
            rijndael.IV = buffer2;
            MemoryStream stream = new MemoryStream();
            CryptoStream stream2 = new CryptoStream(stream, rijndael.CreateDecryptor(), CryptoStreamMode.Write);
            ICryptoTransform transform = new FromBase64Transform();
            CryptoStream stream3 = new CryptoStream(stream2, transform, CryptoStreamMode.Write);
            UTF8Encoding encoding = new UTF8Encoding();
            byte[] bytes = encoding.GetBytes(text);
            stream3.Write(bytes, 0, bytes.Length);
            stream3.FlushFinalBlock();
            byte[] buffer4 = new byte[stream.Length];
            stream.Position = 0;
            stream.Read(buffer4, 0, (int)stream.Length);
            return encoding.GetString(buffer4);
        }

        #endregion

        #region Base64

        /// <summary>
        /// Base64加密
        /// </summary>
        /// <param name="strTxt"></param>
        /// <returns></returns>
        public static string Base64Encrypt(string strTxt)
        {
            byte[] encbuff = System.Text.Encoding.UTF8.GetBytes(strTxt);
            return Convert.ToBase64String(encbuff);
        }

        /// <summary>
        /// Base64解密
        /// </summary>
        /// <param name="strTxt"></param>
        /// <returns></returns>
        public static string Base64Decrypt(string strTxt)
        {
            byte[] decbuff = Convert.FromBase64String(strTxt);
            return System.Text.Encoding.UTF8.GetString(decbuff);
        }

        #endregion        

        #region 异或加密

        /// <summary>
        /// 异或加密,对原文加密成密文,对密文加密成原文
        /// </summary>
        /// <param name="strTxt"></param>
        /// <param name="strKey"></param>
        /// <returns></returns>
        public static string GetXORCode(string strTxt, string strKey)
        {
            byte[] bStr = (new UnicodeEncoding()).GetBytes(strTxt);
            byte[] bKey = (new UnicodeEncoding()).GetBytes(strKey);

            for (int i = 0; i < bStr.Length; i += 2)
            {
                for (int j = 0; j < bKey.Length; j += 2)
                {
                    bStr[i] = Convert.ToByte(bStr[i] ^ bKey[j]);
                }
            }

            return (new UnicodeEncoding()).GetString(bStr).TrimEnd('\0');
        }

        #endregion

        #endregion

        #region 非对称加密

        #region MD5加密
        /// <summary>
        /// md5加密
        /// </summary>
        /// <param name="password">密码明文</param>
        /// <returns></returns>
        public static string MD5Encrypt(string password)
        {
            MD5CryptoServiceProvider hashmd5 = new MD5CryptoServiceProvider();
            string md5 = BitConverter.ToString(hashmd5.ComputeHash(Encoding.Default.GetBytes(password))).Replace("-", "").Substring(6, 13);
            return BitConverter.ToString(hashmd5.ComputeHash(Encoding.Default.GetBytes(md5))).Replace("-", "").Substring(6, 13);
        }
        #endregion

        #region SHA512加密
        /// <summary>
        /// SHA512 加密 
        /// </summary>
        /// <param name="strTxt"></param>
        /// <returns></returns>
        public static string SHA512Encrypt(string strTxt)
        {
            byte[] byteTemp;
            SHA512 sha512 = new SHA512Managed();

            UTF8Encoding utf8 = new UTF8Encoding();
            byteTemp = utf8.GetBytes(strTxt);
            byteTemp = sha512.ComputeHash(byteTemp);

            String strTemp = "";
            for (int i = 0; i < byteTemp.Length; i++)
                strTemp += byteTemp[i].ToString("x2"); //返回密码串小写x2 大写X2

            return strTemp;
        }
        #endregion

        #endregion
    }
}
