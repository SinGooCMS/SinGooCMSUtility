﻿using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace SinGooCMS.Utility.Extension
{
    /// <summary>
    /// 流扩展
    /// </summary>
    public static class StreamExtension
    {
        /// <summary>
        /// 将流转换为内存流
        /// </summary>
        /// <param name="stream"></param>
        /// <returns></returns>
        public static MemoryStream SaveAsMemoryStream(this Stream stream)
        {
            stream.Position = 0;
            return new MemoryStream(stream.ToArray());
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="stream"></param>
        /// <returns></returns>
        public static byte[] ToArray(this Stream stream)
        {
            stream.Position = 0;
            byte[] bytes = new byte[stream.Length];
            stream.Read(bytes, 0, bytes.Length);

            // 设置当前流的位置为流的开始
            stream.Seek(0, SeekOrigin.Begin);
            return bytes;
        }

        /// <summary>
        /// 以文件流的形式复制大文件
        /// </summary>
        /// <param name="fs">源</param>
        /// <param name="dest">目标地址</param>
        /// <param name="bufferSize">缓冲区大小，默认8MB</param>
        public static void CopyToFile(this Stream fs, string dest, int bufferSize = 1024 * 8 * 1024)
        {
            using (var fsWrite = new FileStream(dest, FileMode.OpenOrCreate, FileAccess.ReadWrite))
            {
                byte[] buf = new byte[bufferSize];
                int len;
                while ((len = fs.Read(buf, 0, buf.Length)) != 0)
                {
                    fsWrite.Write(buf, 0, len);
                }
            }
        }

        /// <summary>
        /// 以文件流的形式复制大文件(异步方式)
        /// </summary>
        /// <param name="fs">源</param>
        /// <param name="dest">目标地址</param>
        /// <param name="bufferSize">缓冲区大小，默认8MB</param>
        public static async void CopyToFileAsync(this Stream fs, string dest, int bufferSize = 1024 * 1024 * 8)
        {
            using (var fsWrite = new FileStream(dest, FileMode.OpenOrCreate, FileAccess.ReadWrite))
            {
                byte[] buf = new byte[bufferSize];
                int len;
                await Task.Run(() =>
                {
                    while ((len = fs.Read(buf, 0, buf.Length)) != 0)
                    {
                        fsWrite.Write(buf, 0, len);
                    }
                }).ConfigureAwait(true);
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
            if (algo == "sha1")
                crypto = new SHA1CryptoServiceProvider();
            else
                crypto = new MD5CryptoServiceProvider();

            byte[] retVal = crypto.ComputeHash(fs);

            StringBuilder sb = new StringBuilder();
            foreach (var t in retVal)
            {
                sb.Append(t.ToString("x2"));
            }
            return sb.ToString();
        }
    }
}