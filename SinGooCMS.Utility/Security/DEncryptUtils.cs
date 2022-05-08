using System;
using System.Text;
using System.Security.Cryptography;
using System.IO;
using System.Collections.Generic;
using System.Linq;

namespace SinGooCMS.Utility
{
    /// <summary>
    /// 加密解密工具
    /// </summary>
    public sealed class DEncryptUtils
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
            return Encoding.UTF8.GetString(stream.ToArray());
        }

        #endregion

        #region AES    

        /// <summary>
        /// 获取Aes32位密钥
        /// </summary>
        /// <param name="key">Aes密钥字符串</param>
        /// <returns>Aes32位密钥</returns>
        private static byte[] GetAesKey(string key)
        {
            if (string.IsNullOrEmpty(key))
            {
                throw new ArgumentNullException("key", "Aes密钥不能为空");
            }
            if (key.Length < 32)
            {
                // 不足32补全
                key = key.PadRight(32, '0');
            }
            if (key.Length > 32)
            {
                key = key.Substring(0, 32);
            }
            return Encoding.UTF8.GetBytes(key);
        }

        /// <summary>
        /// AES加密
        /// </summary>
        /// <param name="text">要加密的文本</param>
        /// <param name="key">key</param>
        /// <returns></returns>
        public static string AESEncrypt(string text,string key)
        {
            using (AesCryptoServiceProvider aesProvider = new AesCryptoServiceProvider())
            {
                aesProvider.Key = GetAesKey(key);
                aesProvider.Mode = CipherMode.ECB;
                aesProvider.Padding = PaddingMode.PKCS7;
                using (ICryptoTransform cryptoTransform = aesProvider.CreateEncryptor())
                {
                    byte[] inputBuffers = Encoding.UTF8.GetBytes(text);
                    byte[] results = cryptoTransform.TransformFinalBlock(inputBuffers, 0, inputBuffers.Length);
                    aesProvider.Clear();
                    aesProvider.Dispose();
                    return Convert.ToBase64String(results, 0, results.Length);
                }
            }
        }

        /// <summary>
        /// AES解密
        /// </summary>
        /// <param name="text">要解密的文本</param>
        /// <param name="key">key</param>
        /// <returns></returns>
        public static string AESDecrypt(string text,string key)
        {
            using (AesCryptoServiceProvider aesProvider = new AesCryptoServiceProvider())
            {
                aesProvider.Key = GetAesKey(key);
                aesProvider.Mode = CipherMode.ECB;
                aesProvider.Padding = PaddingMode.PKCS7;
                using (ICryptoTransform cryptoTransform = aesProvider.CreateDecryptor())
                {
                    byte[] inputBuffers = Convert.FromBase64String(text);
                    byte[] results = cryptoTransform.TransformFinalBlock(inputBuffers, 0, inputBuffers.Length);
                    aesProvider.Clear();
                    return Encoding.UTF8.GetString(results);
                }
            }
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

        #region Base36

        private const string Base36Characters = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ";

        /// <summary>
        /// 将给定的数字编码为字符串。
        /// </summary>
        /// <param name="input">要编码的数字。</param>
        /// <returns>编码 <paramref name="input"/> 为字符串。</returns>
        public static string Base36Encode(long input)
        {
            var arr = Base36Characters.ToCharArray();
            var result = new Stack<char>();
            while (input != 0)
            {
                result.Push(arr[input % 36]);
                input /= 36;
            }
            return new string(result.ToArray());
        }

        /// <summary>
        /// 将编码的字符串解码为长整数。
        /// </summary>
        /// <param name="input">要解码的数字。</param>
        /// <returns>解码 <paramref name="input"/> 为长整数。</returns> 
        public static long Base36Decode(string input)
        {
            var reversed = input.ToUpper().Reverse();
            long result = 0;
            var pos = 0;
            foreach (var c in reversed)
            {
                result += Base36Characters.IndexOf(c) * (long)Math.Pow(36, pos);
                pos++;
            }
            return result;
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
