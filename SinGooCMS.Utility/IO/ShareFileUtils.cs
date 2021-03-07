using System;
using System.Collections.Generic;
using System.IO;

namespace SinGooCMS.Utility
{
    /// <summary>
    /// 局域网共享文件工具
    /// </summary>
    public sealed class ShareFileUtils
    {
        #region 属性

        private string SharePath { get; set; }
        private string UserName { get; set; }
        private string Password { get; set; }

        #endregion

        /// <summary>
        /// 连接远程共享文件夹,并返回对象
        /// </summary>
        /// <param name="sharePath"></param>
        /// <param name="userName"></param>
        /// <param name="password"></param>
        /// <param name="localDisk"></param>
        /// <returns></returns>
        public static ShareFileUtils Connect(string sharePath, string userName, string password, string localDisk = "")
        {
            if (!string.IsNullOrEmpty(localDisk) && !localDisk.EndsWith(":"))
                localDisk += ":"; //盘符

            if (!string.IsNullOrEmpty(localDisk))
                sharePath = localDisk + " " + sharePath;

            var commandTexts = new List<string>() {
                $"net use {sharePath} {password} /user:{userName}"
            };

            if (ProcessUtils.ExecuteCMD(commandTexts))
                return new ShareFileUtils
                {
                    SharePath = sharePath,
                    UserName = userName,
                    Password = password
                };
            else
                return null;
        }

        #region 成员方法

        /// <summary>
        /// 读取共享文件目录信息
        /// </summary>
        /// <param name="dirPath"></param>
        /// <returns></returns>
        public DirectoryInfo GetDir(string dirPath) =>
            new DirectoryInfo(System.IO.Path.Combine(SharePath, dirPath));

        /// <summary>
        /// 读取共享文件信息
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public FileInfo GetFile(string fileName) =>
            new FileInfo(System.IO.Path.Combine(SharePath, fileName));

        /// <summary>
        /// 从共享文件夹下载到本地目录下
        /// </summary>
        /// <param name="shareFileName">共享文件名</param>
        /// <param name="localDir">本地目录</param>
        public void DownFile(string shareFileName,string localDir)
        {
            var file = GetFile(shareFileName);
            FileStream inFileStream = new FileStream(file.FullName, FileMode.Open);

            if (!Directory.Exists(localDir))
                Directory.CreateDirectory(localDir);

            string localFileName = System.IO.Path.Combine(localDir, file.Name);
            FileStream outFileStream = new FileStream(localFileName, FileMode.OpenOrCreate);

            byte[] buf = new byte[inFileStream.Length];
            int byteCount;
            while ((byteCount = inFileStream.Read(buf, 0, buf.Length)) > 0)
            {
                outFileStream.Write(buf, 0, byteCount);
            }

            inFileStream.Flush();
            inFileStream.Close();
            outFileStream.Flush();
            outFileStream.Close();
        }

        /// <summary>
        /// 从本地复制到共享目录下
        /// </summary>
        /// <param name="localFileName">本地文件名（全路径）</param>
        /// <param name="shareDir">共享目录</param>
        public void UpFile(string localFileName, string shareDir)
        {
            var dir = GetDir(shareDir);
            if (!Directory.Exists(dir.FullName))
                Directory.CreateDirectory(dir.FullName);

            var file = new FileInfo(localFileName);
            FileStream inFileStream = new FileStream(localFileName, FileMode.Open);

            string shareFileName = System.IO.Path.Combine(dir.FullName, file.Name);
            FileStream outFileStream = new FileStream(shareFileName, FileMode.OpenOrCreate);

            byte[] buf = new byte[inFileStream.Length];
            int byteCount;
            while ((byteCount = inFileStream.Read(buf, 0, buf.Length)) > 0)
            {
                outFileStream.Write(buf, 0, byteCount);
            }

            inFileStream.Flush();
            inFileStream.Close();
            outFileStream.Flush();
            outFileStream.Close();
        }

        /// <summary>
        /// 断开连接
        /// </summary>
        public void Disconnect()
        {
            string commandText = $"net use {SharePath} /del";
            ProcessUtils.ExecuteCMD(new List<string>() { commandText });
        }

        #endregion

        #region 静态方法

        /// <summary>
        /// 断开所有连接
        /// </summary>
        public static void DisconnectAll()
        {
            string commandText = "net use * /del /y";
            ProcessUtils.ExecuteCMD(new List<string>() { commandText });
        }

        /// <summary>
        /// 查看当前所有连接
        /// </summary>
        /// <returns></returns>
        public static string ShowConnect()
        {
            string result = "";
            string errMsg = "";
            ProcessUtils.ExecuteCMD(new List<string>() { "net use" }, out result, out errMsg);
            return result;
        }

        #endregion        
    }
}
