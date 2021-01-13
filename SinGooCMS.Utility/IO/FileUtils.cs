using System;
using System.Text;
using System.IO;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SinGooCMS.Utility
{
    /// <summary>
    /// 文件管理工具
    /// </summary>
    public class FileUtils
    {
        #region 文件操作        

        /// <summary>
        /// 创建文件
        /// </summary>
        /// <param name="absolutePath"></param>
        /// <param name="fileContent"></param>
        public static void CreateFile(string absolutePath, string fileContent) =>
            CreateFile(absolutePath, fileContent, "utf-8");

        /// <summary>
        /// 创建文件
        /// </summary>
        /// <param name="absolutePath"></param>
        /// <param name="fileContent"></param>
        /// <param name="strCodeType"></param>
        public static void CreateFile(string absolutePath, string fileContent, string strCodeType)
        {
            Encoding code = Encoding.GetEncoding(strCodeType);
            StreamWriter mySream = new StreamWriter(absolutePath, false, code);
            mySream.WriteLine(fileContent);
            mySream.Flush();
            mySream.Close();
        }

        /// <summary>
        /// 异步创建文件
        /// </summary>
        /// <param name="absolutePath"></param>
        /// <param name="fileContent"></param>
        /// <param name="strCodeType"></param>
        public static async void CreateFileAsync(string absolutePath, string fileContent, string strCodeType)
        {
            Encoding code = Encoding.GetEncoding(strCodeType);
            StreamWriter mySream = new StreamWriter(absolutePath, false, code);
            await mySream.WriteLineAsync(fileContent);
            await mySream.FlushAsync();
            mySream.Close();
        }

        /// <summary>
        /// 读取文件内容
        /// </summary>
        /// <param name="absolutePath"></param>
        /// <param name="encoding"></param>
        /// <returns></returns>
        public static string ReadFileContent(string absolutePath, Encoding encoding = null)
        {
            if (encoding == null)
                encoding = Encoding.UTF8;

            string fileContent = string.Empty;
            using (StreamReader sr = new StreamReader(absolutePath, encoding))
            {
                fileContent = sr.ReadToEnd();
            }
            return fileContent;

        }

        /// <summary>
        /// 异步读取文件内容
        /// </summary>
        /// <param name="absolutePath"></param>
        /// <param name="encoding"></param>
        /// <returns></returns>
        public static async Task<string> ReadFileContentAsync(string absolutePath, Encoding encoding = null)
        {
            if (encoding == null)
                encoding = Encoding.UTF8;

            string fileContent = string.Empty;
            using (StreamReader sr = new StreamReader(absolutePath, encoding))
            {
                fileContent = await sr.ReadToEndAsync();
            }
            return fileContent;

        }


        /// <summary>
        /// 写文件
        /// </summary>
        /// <param name="absolutePath"></param>
        /// <param name="fileContent"></param>
        /// <param name="encoding"></param>
        /// <param name="isAppend"></param>
        public static void WriteFileContent(string absolutePath, string fileContent, bool isAppend, Encoding encoding = null)
        {
            if (!File.Exists(absolutePath))
                CreateFile(absolutePath, string.Empty);

            if (encoding == null)
                encoding = Encoding.UTF8;

            StreamWriter sw = new StreamWriter(absolutePath, isAppend, encoding);
            sw.WriteLine(fileContent);
            sw.Flush();
            sw.Close();
        }

        /// <summary>
        /// 异步写文件
        /// </summary>
        /// <param name="absolutePath"></param>
        /// <param name="fileContent"></param>
        /// <param name="isAppend"></param>
        /// <param name="encoding"></param>
        public static async void WriteFileContentAsync(string absolutePath, string fileContent, bool isAppend, Encoding encoding = null)
        {
            if (!File.Exists(absolutePath))
                CreateFile(absolutePath, string.Empty);

            if (encoding == null)
                encoding = Encoding.UTF8;

            StreamWriter sw = new StreamWriter(absolutePath, isAppend, encoding);
            await sw.WriteLineAsync(fileContent);
            await sw.FlushAsync();
            sw.Close();
        }

        /// <summary>
        /// 删除文件
        /// </summary>
        /// <param name="absolutePath"></param>
        /// <returns></returns>
        public static void DeleteFile(string absolutePath)
        {
            if (File.Exists(absolutePath))
                File.Delete(absolutePath);
        }

        /// <summary>
        /// 重命名文件
        /// </summary>
        /// <param name="absolutePath">文件所在的目录,不要最后那个斜杠例如E:\\Dir\\GG</param>
        /// <param name="oldName">原名称</param>
        /// <param name="newName">修改的名称</param>
        /// <param name="fileType">文件类型 0为文件夹 1是文件</param>
        /// <returns></returns>
        public static void ReNameFile(string absolutePath, string oldName, string newName, int fileType)
        {
            if (fileType.Equals(0))
            {
                if (Directory.Exists(absolutePath + "\\" + oldName))
                    Directory.Move(absolutePath + "\\" + oldName, absolutePath + "\\" + newName.Replace(".", ""));
            }
            else
            {
                if (File.Exists(absolutePath + "\\" + oldName))
                    File.Move(absolutePath + "\\" + oldName, absolutePath + "\\" + newName);
            }
        }
        /// <summary>
        /// 获取文件扩展名
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public static string GetExtension(string fileName) => System.IO.Path.GetExtension(fileName);

        /// <summary>
        /// 获取文件大小
        /// </summary>
        /// <param name="length"></param>
        /// <param name="unit"></param>
        /// <returns></returns>
        public static string GetFileSize(decimal length, string unit = null)
        {
            decimal num = 0m;
            if (unit != null) //指定单位
            {
                switch (unit)
                {
                    case "GB":
                        num = Math.Round((decimal)(length / (1024m * 1024m * 1024m)), 2, MidpointRounding.AwayFromZero);
                        break;
                    case "MB":
                        num = Math.Round((decimal)(length / (1024m * 1024m)), 2, MidpointRounding.AwayFromZero);
                        break;
                    case "KB":
                    default:
                        num = Math.Round((decimal)(length / 1024m), 2, MidpointRounding.AwayFromZero);
                        break;
                }
            }
            else //不指定单位
            {
                if (length >= 1024m * 1024m * 1024m)
                {
                    num = Math.Round((decimal)(length / (1024m * 1024m * 1024m)), 2, MidpointRounding.AwayFromZero);
                    unit = "GB";
                }
                else if (length >= 1024m * 1024m)
                {
                    num = Math.Round((decimal)(length / (1024m * 1024m)), 2, MidpointRounding.AwayFromZero);
                    unit = "MB";
                }
                else
                {
                    num = Math.Round((decimal)(length / 1024m), 2, MidpointRounding.AwayFromZero);
                    unit = "KB";
                }
            }

            return num.ToString() + unit;
        }
        #endregion

        #region 目录操作

        /// <summary>
        /// 创建文件夹
        /// </summary>
        /// <param name="absoluteDir"></param>
        /// <returns></returns>
        public static void CreateDirectory(string absoluteDir)
        {
            if (!Directory.Exists(absoluteDir))
            {
                Directory.CreateDirectory(absoluteDir);
            }
        }

        /// <summary>
        /// 删除文件夹
        /// </summary>
        /// <param name="absoluteDir"></param>
        /// <returns></returns>
        public static void DeleteDirectory(string absoluteDir)
        {
            if (Directory.Exists(absoluteDir))
                Directory.Delete(absoluteDir, true);
        }

        /// <summary>
        /// 获取目录容量
        /// </summary>
        /// <param name="absoluteDir"></param>
        /// <returns></returns>
        public static long GetDirectoryLength(string absoluteDir)
        {
            //判断给定的路径是否存在,如果不存在则退出
            if (!Directory.Exists(absoluteDir))
                return 0;
            long len = 0;

            //定义一个DirectoryInfo对象
            DirectoryInfo di = new DirectoryInfo(absoluteDir);

            //通过GetFiles方法,获取di目录中的所有文件的大小
            foreach (FileInfo fi in di.GetFiles())
            {
                len += fi.Length;
            }

            //获取di中所有的文件夹,并存到一个新的对象数组中,以进行递归
            DirectoryInfo[] dis = di.GetDirectories();
            if (dis.Length > 0)
            {
                for (int i = 0; i < dis.Length; i++)
                {
                    len += GetDirectoryLength(dis[i].FullName);
                }
            }
            return len;
        }

        /// <summary>
        /// 复制文件夹
        /// </summary>
        /// <param name="sourcesAbsoluteDir">源路径</param>
        /// <param name="destAbsoluteDir">新路径</param>
        private static void CopyFolder(string sourcesAbsoluteDir, string destAbsoluteDir)
        {
            DirectoryInfo dinfo = new DirectoryInfo(sourcesAbsoluteDir);
            //注，这里面传的是路径，并不是文件，所以不能保含带后缀的文件                
            foreach (FileSystemInfo f in dinfo.GetFileSystemInfos())
            {
                //目标路径destName = 目标文件夹路径 + 原文件夹下的子文件(或文件夹)名字                
                //Path.Combine(string a ,string b) 为合并两个字符串                     
                String destName = Path.Combine(destAbsoluteDir, f.Name);
                if (f is FileInfo)
                {
                    //如果是文件就复制       
                    File.Copy(f.FullName, destName, true);//true代表可以覆盖同名文件                     
                }
                else
                {
                    //如果是文件夹就创建文件夹然后复制然后递归复制              
                    Directory.CreateDirectory(destName);
                    CopyFolder(f.FullName, destName);
                }
            }
        }

        /// <summary>
        /// 复制目录及目录下所有文件
        /// </summary>
        /// <param name="SourcePath"></param>
        /// <param name="DestinationPath"></param>
        public static void CopyDirectory(string SourcePath, string DestinationPath)
        {
            //创建目标目录
            if (!Directory.Exists(DestinationPath))
                Directory.CreateDirectory(DestinationPath);

            //创建所有子目录
            foreach (string dirPath in Directory.GetDirectories(SourcePath, "*", SearchOption.AllDirectories))
                Directory.CreateDirectory(dirPath.Replace(SourcePath, DestinationPath));

            //复制所有文件 并覆盖现有的文件
            foreach (string newPath in Directory.GetFiles(SourcePath, "*.*", SearchOption.AllDirectories))
                File.Copy(newPath, newPath.Replace(SourcePath, DestinationPath), true);
        }

        /// <summary>
        /// 递归获取目录下的所有文件信息
        /// </summary>
        /// <param name="absoluteDir"></param>
        /// <returns></returns>
        public static IEnumerable<FileInfo> GetAllFiles(string absoluteDir)
        {
            var lst = new List<FileInfo>();
            foreach (var item in new DirectoryInfo(absoluteDir).GetFiles())
                lst.Add(item);

            foreach (var dir in new DirectoryInfo(absoluteDir).GetDirectories())
                lst.AddRange(GetAllFiles(dir.FullName));

            return lst;
        }

        #endregion     

        #region 备份与恢复

        /// <summary>
        /// 备份文件
        /// </summary>
        /// <param name="sourceFileName">源文件名</param>
        /// <param name="destFileName">目标文件名</param>
        /// <param name="overwrite">当目标文件存在时是否覆盖</param>
        /// <returns>操作是否成功</returns>
        public static bool BackupFile(string sourceFileName, string destFileName, bool overwrite)
        {
            if (!System.IO.File.Exists(sourceFileName))
                throw new FileNotFoundException(sourceFileName + "文件不存在！");

            if (!overwrite && System.IO.File.Exists(destFileName))
                return false;

            try
            {
                System.IO.File.Copy(sourceFileName, destFileName, true);
                return true;
            }
            catch (Exception e)
            {
                throw e;
            }
        }


        /// <summary>
        /// 备份文件,当目标文件存在时覆盖
        /// </summary>
        /// <param name="sourceFileName">源文件名</param>
        /// <param name="destFileName">目标文件名</param>
        /// <returns>操作是否成功</returns>
        public static bool BackupFile(string sourceFileName, string destFileName)
        {
            return BackupFile(sourceFileName, destFileName, true);
        }


        /// <summary>
        /// 恢复文件
        /// </summary>
        /// <param name="backupFileName">备份文件名</param>
        /// <param name="targetFileName">要恢复的文件名</param>
        /// <param name="backupTargetFileName">要恢复文件再次备份的名称,如果为null,则不再备份恢复文件</param>
        /// <returns>操作是否成功</returns>
        public static bool RestoreFile(string backupFileName, string targetFileName, string backupTargetFileName)
        {
            try
            {
                if (!System.IO.File.Exists(backupFileName))
                    throw new FileNotFoundException(backupFileName + "文件不存在！");

                if (backupTargetFileName != null)
                {
                    if (!System.IO.File.Exists(targetFileName))
                        throw new FileNotFoundException(targetFileName + "文件不存在！无法备份此文件！");
                    else
                        System.IO.File.Copy(targetFileName, backupTargetFileName, true);
                }
                System.IO.File.Delete(targetFileName);
                System.IO.File.Copy(backupFileName, targetFileName);
            }
            catch (Exception e)
            {
                throw e;
            }
            return true;
        }

        public static bool RestoreFile(string backupFileName, string targetFileName)
        {
            return RestoreFile(backupFileName, targetFileName, null);
        }

        #endregion

        #region 文件流            

        /// <summary>
        /// 读文件到Stream
        /// </summary>
        /// <param name="fileName">硬盘文件路径</param>
        /// <returns></returns>
        public static Stream ReadFileToStream(string fileName)
        {
            byte[] bytes = ReadFileToBytes(fileName);
            Stream stream = new MemoryStream(bytes);
            return stream;
        }

        /// <summary>
        /// 读文件到byte[]
        /// </summary>
        /// <param name="fileName">硬盘文件路径</param>
        /// <returns></returns>
        public static byte[] ReadFileToBytes(string fileName)
        {
            FileStream pFileStream = null;
            byte[] bytes = new byte[0];
            try
            {
                pFileStream = new FileStream(fileName, FileMode.Open, FileAccess.Read);
                BinaryReader r = new BinaryReader(pFileStream);
                r.BaseStream.Seek(0, SeekOrigin.Begin);    //将文件指针设置到文件开
                bytes = r.ReadBytes((int)r.BaseStream.Length);
                return bytes;
            }
            catch
            {
                return bytes;
            }
            finally
            {
                if (pFileStream != null)
                    pFileStream.Close();
            }
        }

        #endregion
    }
}