using System;
using System.Text;
using System.IO;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SinGooCMS.Utility
{
    /// <summary>
    /// 文件尺寸大小单位
    /// </summary>
    public enum FileSizeUnit
    {
        KB,
        MB,
        GB,
        TB
    }

    /// <summary>
    /// 文件管理工具
    /// </summary>
    public sealed class FileUtils
    {
        #region 文件操作

        /// <summary>
        /// 异步创建文件并写入文本内容
        /// </summary>
        /// <param name="absolutePath"></param>
        /// <param name="fileContent"></param>
        /// <param name="encoding"></param>
        public static async Task CreateFileAsync(string absolutePath, string fileContent, Encoding encoding = null)
        {
            if (encoding == null)
                encoding = Encoding.UTF8;

            using (StreamWriter sw = new StreamWriter(absolutePath, false, encoding))
            {
                await sw.WriteLineAsync(fileContent);
                await sw.FlushAsync();
                sw.Close();
            }
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
        /// 异步写文件
        /// </summary>
        /// <param name="absolutePath"></param>
        /// <param name="fileContent"></param>
        /// <param name="isAppend"></param>
        /// <param name="encoding"></param>
        public static async Task WriteFileContentAsync(string absolutePath, string fileContent, bool isAppend = false, Encoding encoding = null)
        {
            if (!File.Exists(absolutePath))
                await CreateFileAsync(absolutePath, string.Empty);

            if (encoding == null)
                encoding = Encoding.UTF8;

            using (StreamWriter sw = new StreamWriter(absolutePath, isAppend, encoding))
            {
                await sw.WriteLineAsync(fileContent);
                await sw.FlushAsync();
                sw.Close();
            }
        }

        /// <summary>
        /// 重命名目录或者文件
        /// </summary>
        /// <param name="absolutePath">文件所在的目录</param>
        /// <param name="oldName">原名称</param>
        /// <param name="newName">修改的名称</param>
        /// <param name="isFile">true=文件 false=文件夹</param>
        /// <returns></returns>
        public static void ReNameFile(string absolutePath, string oldName, string newName, bool isFile = true)
        {
            if (isFile)
            {
                if (File.Exists(Combine(absolutePath, oldName)))
                    File.Move(Combine(absolutePath, oldName), Combine(absolutePath, newName));
            }
            else
            {
                if (Directory.Exists(Combine(absolutePath, oldName)))
                    Directory.Move(Combine(absolutePath, oldName), Combine(absolutePath, newName.Replace(".", "")));
            }
        }

        /// <summary>
        /// 获取文件大小
        /// </summary>
        /// <param name="length">长度：字节</param>
        /// <param name="unit">单位，枚举：FileSizeUnit</param>
        /// <returns></returns>
        public static string GetFileSize(decimal length, FileSizeUnit unit = FileSizeUnit.KB)
        {
            decimal num = 0m;
            switch (unit)
            {
                case FileSizeUnit.TB:
                    num = Math.Round((decimal)(length / (1024m * 1024m * 1024m * 1024m)), 2, MidpointRounding.AwayFromZero);
                    break;
                case FileSizeUnit.GB:
                    num = Math.Round((decimal)(length / (1024m * 1024m * 1024m)), 2, MidpointRounding.AwayFromZero);
                    break;
                case FileSizeUnit.MB:
                    num = Math.Round((decimal)(length / (1024m * 1024m)), 2, MidpointRounding.AwayFromZero);
                    break;
                case FileSizeUnit.KB:
                default:
                    num = Math.Round((decimal)(length / 1024m), 2, MidpointRounding.AwayFromZero);
                    break;
            }

            return num.ToString() + unit.ToString();
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
                Directory.CreateDirectory(absoluteDir);
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

        /// <summary>
        /// 合并目录和文件名成为新的文件路径
        /// </summary>
        /// <param name="dir">绝对路径或者相对路径的目录路径</param>
        /// <param name="fileName">文件名</param>
        /// <returns></returns>
        public static string Combine(string dir, string fileName)
        {
            if (dir.IndexOf(@"\") != -1 && !dir.EndsWith(@"\"))
                dir += @"\";
            else if (dir.IndexOf(@"/") != -1 && !dir.EndsWith(@"/"))
                dir += @"/";

            if (fileName.StartsWith(@"/"))
                fileName = fileName.TrimStart('/');
            else if (fileName.StartsWith(@"\"))
                fileName = fileName.TrimStart('\\');

            return Path.Combine(dir, fileName);
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
            if (!File.Exists(sourceFileName))
                throw new FileNotFoundException(sourceFileName + "文件不存在！");

            if (!overwrite && File.Exists(destFileName))
                return false;

            try
            {
                File.Copy(sourceFileName, destFileName, true);
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
                if (!File.Exists(backupFileName))
                    throw new FileNotFoundException(backupFileName + "文件不存在！");

                if (backupTargetFileName != null)
                {
                    if (!File.Exists(targetFileName))
                        throw new FileNotFoundException(targetFileName + "文件不存在！无法备份此文件！");
                    else
                        File.Copy(targetFileName, backupTargetFileName, true);
                }
                File.Delete(targetFileName);
                File.Copy(backupFileName, targetFileName);
            }
            catch (Exception e)
            {
                throw e;
            }
            return true;
        }

        /// <summary>
        /// 恢复文件
        /// </summary>
        /// <param name="backupFileName"></param>
        /// <param name="targetFileName"></param>
        /// <returns></returns>
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
            return new MemoryStream(ReadFileToBytes(fileName));
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