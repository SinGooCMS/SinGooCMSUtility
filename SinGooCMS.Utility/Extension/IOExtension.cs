using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace SinGooCMS.Utility.Extension
{
    /// <summary>
    /// IO扩展
    /// </summary>
    public static class IOExtension
    {
        #region 文件流

        /// <summary>
        /// byte[] 转为stream
        /// </summary>
        /// <param name="bytes">参数</param>
        /// <returns></returns>
        public static Stream ToStream(this byte[] bytes)
        {
            Stream stream = new MemoryStream(bytes);
            stream.Seek(0, SeekOrigin.Begin);
            return stream;
        }

        /// <summary>
        /// stream转为byte[]
        /// </summary>
        /// <param name="stream">参数</param>
        /// <returns></returns>
        public static byte[] ToBytes(this Stream stream)
        {
            byte[] bytes = new byte[stream.Length];
            stream.Read(bytes, 0, bytes.Length);
            return bytes;
        }

        /// <summary>
        /// 写byte[]到fileName
        /// </summary>
        /// <param name="bytes">byte[]</param>
        /// <param name="fileName">保存至硬盘路径</param>
        /// <returns></returns>
        public static void WriteToFile(this byte[] bytes, string fileName)
        {
            using (var fileStream = new FileStream(fileName, FileMode.OpenOrCreate))
            {
                fileStream.Write(bytes, 0, bytes.Length);
                fileStream.Flush();
            }
        }

        /// <summary>
        /// 将内存流转储成文件
        /// </summary>
        /// <param name="ms"></param>
        /// <param name="filename"></param>
        public static void SaveFile(this MemoryStream ms, string filename)
        {
            using (var fs = new FileStream(filename, FileMode.OpenOrCreate, FileAccess.ReadWrite))
            {
                byte[] buffer = ms.ToArray(); // 转化为byte格式存储
                fs.Write(buffer, 0, buffer.Length);
                fs.Flush();
            }
        }

        #endregion

        #region 计算哈希值

        /// <summary>
        /// 计算文件的 MD5 值
        /// </summary>
        /// <param name="fs">源文件流</param>
        /// <returns>MD5 值16进制字符串</returns>
        public static string GetFileMD5(this FileStream fs) => HashFile(fs, "md5");

        /// <summary>
        /// 计算文件的 sha1 值
        /// </summary>
        /// <param name="fs">源文件流</param>
        /// <returns>sha1 值16进制字符串</returns>
        public static string GetFileSha1(this Stream fs) => HashFile(fs, "sha1");

        /// <summary>
        /// 计算文件的哈希值
        /// </summary>
        /// <param name="fs">被操作的源数据流</param>
        /// <param name="algo">加密算法</param>
        /// <returns>哈希值16进制字符串</returns>
        private static string HashFile(Stream fs, string algo)
        {
            HashAlgorithm crypto = null;
            switch (algo)
            {
                case "sha1":
                    crypto = new SHA1CryptoServiceProvider();
                    break;
                default:
                    crypto = new MD5CryptoServiceProvider();
                    break;
            }
            byte[] retVal = crypto.ComputeHash(fs);

            StringBuilder sb = new StringBuilder();
            foreach (var t in retVal)
            {
                sb.Append(t.ToString("x2"));
            }
            return sb.ToString();
        }

        #endregion
    }
}
