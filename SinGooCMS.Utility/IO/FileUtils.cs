using System;
using System.Text;
using System.IO;
using System.Web;
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

        #region 读文件类型(content_type)

        /// <summary>
        /// 读取文件类型Content_Type
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public static string GetContentType(string filePath)
        {
#if NET451_OR_GREATER || NET461
            return MimeMapping.GetMimeMapping(filePath);
#elif NETSTANDARD2_1
            return MimeMapping.GetMimeMapping(filePath);
#else
            return "application/octet-stream"; //默认的
#endif
        }

        #endregion
    }

#if NETSTANDARD2_1
    internal static class MimeMapping
    {
        private abstract class MimeMappingDictionaryBase
        {
            private readonly Dictionary<string, string> _mappings = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

            private static readonly char[] _pathSeparatorChars = new char[3]
            {
                Path.DirectorySeparatorChar,
                Path.AltDirectorySeparatorChar,
                Path.VolumeSeparatorChar
            };

            private bool _isInitialized;

            protected void AddMapping(string fileExtension, string mimeType)
            {
                _mappings.Add(fileExtension, mimeType);
            }

            private void AddWildcardIfNotPresent()
            {
                if (!_mappings.ContainsKey(".*"))
                {
                    AddMapping(".*", "application/octet-stream");
                }
            }

            private void EnsureMapping()
            {
                if (!_isInitialized)
                {
                    lock (this)
                    {
                        if (!_isInitialized)
                        {
                            PopulateMappings();
                            AddWildcardIfNotPresent();
                            _isInitialized = true;
                        }
                    }
                }
            }

            protected abstract void PopulateMappings();
            private static string GetFileName(string path)
            {
                int num = path.LastIndexOfAny(_pathSeparatorChars);
                if (num < 0)
                {
                    return path;
                }
                return path.Substring(num);
            }

            public string GetMimeMapping(string fileName)
            {
                EnsureMapping();
                fileName = GetFileName(fileName);
                for (int i = 0; i < fileName.Length; i++)
                {
                    if (fileName[i] == '.' && _mappings.TryGetValue(fileName.Substring(i), out string result))
                    {
                        return result;
                    }
                }
                return _mappings[".*"];
            }
        }

        private sealed class MimeMappingDictionaryClassic : MimeMappingDictionaryBase
        {
            protected override void PopulateMappings()
            {
                base.AddMapping(".323", "text/h323");
                base.AddMapping(".aaf", "application/octet-stream");
                base.AddMapping(".aca", "application/octet-stream");
                base.AddMapping(".accdb", "application/msaccess");
                base.AddMapping(".accde", "application/msaccess");
                base.AddMapping(".accdt", "application/msaccess");
                base.AddMapping(".acx", "application/internet-property-stream");
                base.AddMapping(".afm", "application/octet-stream");
                base.AddMapping(".ai", "application/postscript");
                base.AddMapping(".aif", "audio/x-aiff");
                base.AddMapping(".aifc", "audio/aiff");
                base.AddMapping(".aiff", "audio/aiff");
                base.AddMapping(".application", "application/x-ms-application");
                base.AddMapping(".art", "image/x-jg");
                base.AddMapping(".asd", "application/octet-stream");
                base.AddMapping(".asf", "video/x-ms-asf");
                base.AddMapping(".asi", "application/octet-stream");
                base.AddMapping(".asm", "text/plain");
                base.AddMapping(".asr", "video/x-ms-asf");
                base.AddMapping(".asx", "video/x-ms-asf");
                base.AddMapping(".atom", "application/atom+xml");
                base.AddMapping(".au", "audio/basic");
                base.AddMapping(".avi", "video/x-msvideo");
                base.AddMapping(".axs", "application/olescript");
                base.AddMapping(".bas", "text/plain");
                base.AddMapping(".bcpio", "application/x-bcpio");
                base.AddMapping(".bin", "application/octet-stream");
                base.AddMapping(".bmp", "image/bmp");
                base.AddMapping(".c", "text/plain");
                base.AddMapping(".cab", "application/octet-stream");
                base.AddMapping(".calx", "application/vnd.ms-office.calx");
                base.AddMapping(".cat", "application/vnd.ms-pki.seccat");
                base.AddMapping(".cdf", "application/x-cdf");
                base.AddMapping(".chm", "application/octet-stream");
                base.AddMapping(".class", "application/x-java-applet");
                base.AddMapping(".clp", "application/x-msclip");
                base.AddMapping(".cmx", "image/x-cmx");
                base.AddMapping(".cnf", "text/plain");
                base.AddMapping(".cod", "image/cis-cod");
                base.AddMapping(".cpio", "application/x-cpio");
                base.AddMapping(".cpp", "text/plain");
                base.AddMapping(".crd", "application/x-mscardfile");
                base.AddMapping(".crl", "application/pkix-crl");
                base.AddMapping(".crt", "application/x-x509-ca-cert");
                base.AddMapping(".csh", "application/x-csh");
                base.AddMapping(".css", "text/css");
                base.AddMapping(".csv", "application/octet-stream");
                base.AddMapping(".cur", "application/octet-stream");
                base.AddMapping(".dcr", "application/x-director");
                base.AddMapping(".deploy", "application/octet-stream");
                base.AddMapping(".der", "application/x-x509-ca-cert");
                base.AddMapping(".dib", "image/bmp");
                base.AddMapping(".dir", "application/x-director");
                base.AddMapping(".disco", "text/xml");
                base.AddMapping(".dll", "application/x-msdownload");
                base.AddMapping(".dll.config", "text/xml");
                base.AddMapping(".dlm", "text/dlm");
                base.AddMapping(".doc", "application/msword");
                base.AddMapping(".docm", "application/vnd.ms-word.document.macroEnabled.12");
                base.AddMapping(".docx", "application/vnd.openxmlformats-officedocument.wordprocessingml.document");
                base.AddMapping(".dot", "application/msword");
                base.AddMapping(".dotm", "application/vnd.ms-word.template.macroEnabled.12");
                base.AddMapping(".dotx", "application/vnd.openxmlformats-officedocument.wordprocessingml.template");
                base.AddMapping(".dsp", "application/octet-stream");
                base.AddMapping(".dtd", "text/xml");
                base.AddMapping(".dvi", "application/x-dvi");
                base.AddMapping(".dwf", "drawing/x-dwf");
                base.AddMapping(".dwp", "application/octet-stream");
                base.AddMapping(".dxr", "application/x-director");
                base.AddMapping(".eml", "message/rfc822");
                base.AddMapping(".emz", "application/octet-stream");
                base.AddMapping(".eot", "application/octet-stream");
                base.AddMapping(".eps", "application/postscript");
                base.AddMapping(".etx", "text/x-setext");
                base.AddMapping(".evy", "application/envoy");
                base.AddMapping(".exe", "application/octet-stream");
                base.AddMapping(".exe.config", "text/xml");
                base.AddMapping(".fdf", "application/vnd.fdf");
                base.AddMapping(".fif", "application/fractals");
                base.AddMapping(".fla", "application/octet-stream");
                base.AddMapping(".flr", "x-world/x-vrml");
                base.AddMapping(".flv", "video/x-flv");
                base.AddMapping(".gif", "image/gif");
                base.AddMapping(".gtar", "application/x-gtar");
                base.AddMapping(".gz", "application/x-gzip");
                base.AddMapping(".h", "text/plain");
                base.AddMapping(".hdf", "application/x-hdf");
                base.AddMapping(".hdml", "text/x-hdml");
                base.AddMapping(".hhc", "application/x-oleobject");
                base.AddMapping(".hhk", "application/octet-stream");
                base.AddMapping(".hhp", "application/octet-stream");
                base.AddMapping(".hlp", "application/winhlp");
                base.AddMapping(".hqx", "application/mac-binhex40");
                base.AddMapping(".hta", "application/hta");
                base.AddMapping(".htc", "text/x-component");
                base.AddMapping(".htm", "text/html");
                base.AddMapping(".html", "text/html");
                base.AddMapping(".htt", "text/webviewhtml");
                base.AddMapping(".hxt", "text/html");
                base.AddMapping(".ico", "image/x-icon");
                base.AddMapping(".ics", "application/octet-stream");
                base.AddMapping(".ief", "image/ief");
                base.AddMapping(".iii", "application/x-iphone");
                base.AddMapping(".inf", "application/octet-stream");
                base.AddMapping(".ins", "application/x-internet-signup");
                base.AddMapping(".isp", "application/x-internet-signup");
                base.AddMapping(".IVF", "video/x-ivf");
                base.AddMapping(".jar", "application/java-archive");
                base.AddMapping(".java", "application/octet-stream");
                base.AddMapping(".jck", "application/liquidmotion");
                base.AddMapping(".jcz", "application/liquidmotion");
                base.AddMapping(".jfif", "image/pjpeg");
                base.AddMapping(".jpb", "application/octet-stream");
                base.AddMapping(".jpe", "image/jpeg");
                base.AddMapping(".jpeg", "image/jpeg");
                base.AddMapping(".jpg", "image/jpeg");
                base.AddMapping(".js", "application/x-javascript");
                base.AddMapping(".jsx", "text/jscript");
                base.AddMapping(".latex", "application/x-latex");
                base.AddMapping(".lit", "application/x-ms-reader");
                base.AddMapping(".lpk", "application/octet-stream");
                base.AddMapping(".lsf", "video/x-la-asf");
                base.AddMapping(".lsx", "video/x-la-asf");
                base.AddMapping(".lzh", "application/octet-stream");
                base.AddMapping(".m13", "application/x-msmediaview");
                base.AddMapping(".m14", "application/x-msmediaview");
                base.AddMapping(".m1v", "video/mpeg");
                base.AddMapping(".m3u", "audio/x-mpegurl");
                base.AddMapping(".man", "application/x-troff-man");
                base.AddMapping(".manifest", "application/x-ms-manifest");
                base.AddMapping(".map", "text/plain");
                base.AddMapping(".mdb", "application/x-msaccess");
                base.AddMapping(".mdp", "application/octet-stream");
                base.AddMapping(".me", "application/x-troff-me");
                base.AddMapping(".mht", "message/rfc822");
                base.AddMapping(".mhtml", "message/rfc822");
                base.AddMapping(".mid", "audio/mid");
                base.AddMapping(".midi", "audio/mid");
                base.AddMapping(".mix", "application/octet-stream");
                base.AddMapping(".mmf", "application/x-smaf");
                base.AddMapping(".mno", "text/xml");
                base.AddMapping(".mny", "application/x-msmoney");
                base.AddMapping(".mov", "video/quicktime");
                base.AddMapping(".movie", "video/x-sgi-movie");
                base.AddMapping(".mp2", "video/mpeg");
                base.AddMapping(".mp3", "audio/mpeg");
                base.AddMapping(".mpa", "video/mpeg");
                base.AddMapping(".mpe", "video/mpeg");
                base.AddMapping(".mpeg", "video/mpeg");
                base.AddMapping(".mpg", "video/mpeg");
                base.AddMapping(".mpp", "application/vnd.ms-project");
                base.AddMapping(".mpv2", "video/mpeg");
                base.AddMapping(".ms", "application/x-troff-ms");
                base.AddMapping(".msi", "application/octet-stream");
                base.AddMapping(".mso", "application/octet-stream");
                base.AddMapping(".mvb", "application/x-msmediaview");
                base.AddMapping(".mvc", "application/x-miva-compiled");
                base.AddMapping(".nc", "application/x-netcdf");
                base.AddMapping(".nsc", "video/x-ms-asf");
                base.AddMapping(".nws", "message/rfc822");
                base.AddMapping(".ocx", "application/octet-stream");
                base.AddMapping(".oda", "application/oda");
                base.AddMapping(".odc", "text/x-ms-odc");
                base.AddMapping(".ods", "application/oleobject");
                base.AddMapping(".one", "application/onenote");
                base.AddMapping(".onea", "application/onenote");
                base.AddMapping(".onetoc", "application/onenote");
                base.AddMapping(".onetoc2", "application/onenote");
                base.AddMapping(".onetmp", "application/onenote");
                base.AddMapping(".onepkg", "application/onenote");
                base.AddMapping(".osdx", "application/opensearchdescription+xml");
                base.AddMapping(".p10", "application/pkcs10");
                base.AddMapping(".p12", "application/x-pkcs12");
                base.AddMapping(".p7b", "application/x-pkcs7-certificates");
                base.AddMapping(".p7c", "application/pkcs7-mime");
                base.AddMapping(".p7m", "application/pkcs7-mime");
                base.AddMapping(".p7r", "application/x-pkcs7-certreqresp");
                base.AddMapping(".p7s", "application/pkcs7-signature");
                base.AddMapping(".pbm", "image/x-portable-bitmap");
                base.AddMapping(".pcx", "application/octet-stream");
                base.AddMapping(".pcz", "application/octet-stream");
                base.AddMapping(".pdf", "application/pdf");
                base.AddMapping(".pfb", "application/octet-stream");
                base.AddMapping(".pfm", "application/octet-stream");
                base.AddMapping(".pfx", "application/x-pkcs12");
                base.AddMapping(".pgm", "image/x-portable-graymap");
                base.AddMapping(".pko", "application/vnd.ms-pki.pko");
                base.AddMapping(".pma", "application/x-perfmon");
                base.AddMapping(".pmc", "application/x-perfmon");
                base.AddMapping(".pml", "application/x-perfmon");
                base.AddMapping(".pmr", "application/x-perfmon");
                base.AddMapping(".pmw", "application/x-perfmon");
                base.AddMapping(".png", "image/png");
                base.AddMapping(".pnm", "image/x-portable-anymap");
                base.AddMapping(".pnz", "image/png");
                base.AddMapping(".pot", "application/vnd.ms-powerpoint");
                base.AddMapping(".potm", "application/vnd.ms-powerpoint.template.macroEnabled.12");
                base.AddMapping(".potx", "application/vnd.openxmlformats-officedocument.presentationml.template");
                base.AddMapping(".ppam", "application/vnd.ms-powerpoint.addin.macroEnabled.12");
                base.AddMapping(".ppm", "image/x-portable-pixmap");
                base.AddMapping(".pps", "application/vnd.ms-powerpoint");
                base.AddMapping(".ppsm", "application/vnd.ms-powerpoint.slideshow.macroEnabled.12");
                base.AddMapping(".ppsx", "application/vnd.openxmlformats-officedocument.presentationml.slideshow");
                base.AddMapping(".ppt", "application/vnd.ms-powerpoint");
                base.AddMapping(".pptm", "application/vnd.ms-powerpoint.presentation.macroEnabled.12");
                base.AddMapping(".pptx", "application/vnd.openxmlformats-officedocument.presentationml.presentation");
                base.AddMapping(".prf", "application/pics-rules");
                base.AddMapping(".prm", "application/octet-stream");
                base.AddMapping(".prx", "application/octet-stream");
                base.AddMapping(".ps", "application/postscript");
                base.AddMapping(".psd", "application/octet-stream");
                base.AddMapping(".psm", "application/octet-stream");
                base.AddMapping(".psp", "application/octet-stream");
                base.AddMapping(".pub", "application/x-mspublisher");
                base.AddMapping(".qt", "video/quicktime");
                base.AddMapping(".qtl", "application/x-quicktimeplayer");
                base.AddMapping(".qxd", "application/octet-stream");
                base.AddMapping(".ra", "audio/x-pn-realaudio");
                base.AddMapping(".ram", "audio/x-pn-realaudio");
                base.AddMapping(".rar", "application/octet-stream");
                base.AddMapping(".ras", "image/x-cmu-raster");
                base.AddMapping(".rf", "image/vnd.rn-realflash");
                base.AddMapping(".rgb", "image/x-rgb");
                base.AddMapping(".rm", "application/vnd.rn-realmedia");
                base.AddMapping(".rmi", "audio/mid");
                base.AddMapping(".roff", "application/x-troff");
                base.AddMapping(".rpm", "audio/x-pn-realaudio-plugin");
                base.AddMapping(".rtf", "application/rtf");
                base.AddMapping(".rtx", "text/richtext");
                base.AddMapping(".scd", "application/x-msschedule");
                base.AddMapping(".sct", "text/scriptlet");
                base.AddMapping(".sea", "application/octet-stream");
                base.AddMapping(".setpay", "application/set-payment-initiation");
                base.AddMapping(".setreg", "application/set-registration-initiation");
                base.AddMapping(".sgml", "text/sgml");
                base.AddMapping(".sh", "application/x-sh");
                base.AddMapping(".shar", "application/x-shar");
                base.AddMapping(".sit", "application/x-stuffit");
                base.AddMapping(".sldm", "application/vnd.ms-powerpoint.slide.macroEnabled.12");
                base.AddMapping(".sldx", "application/vnd.openxmlformats-officedocument.presentationml.slide");
                base.AddMapping(".smd", "audio/x-smd");
                base.AddMapping(".smi", "application/octet-stream");
                base.AddMapping(".smx", "audio/x-smd");
                base.AddMapping(".smz", "audio/x-smd");
                base.AddMapping(".snd", "audio/basic");
                base.AddMapping(".snp", "application/octet-stream");
                base.AddMapping(".spc", "application/x-pkcs7-certificates");
                base.AddMapping(".spl", "application/futuresplash");
                base.AddMapping(".src", "application/x-wais-source");
                base.AddMapping(".ssm", "application/streamingmedia");
                base.AddMapping(".sst", "application/vnd.ms-pki.certstore");
                base.AddMapping(".stl", "application/vnd.ms-pki.stl");
                base.AddMapping(".sv4cpio", "application/x-sv4cpio");
                base.AddMapping(".sv4crc", "application/x-sv4crc");
                base.AddMapping(".swf", "application/x-shockwave-flash");
                base.AddMapping(".t", "application/x-troff");
                base.AddMapping(".tar", "application/x-tar");
                base.AddMapping(".tcl", "application/x-tcl");
                base.AddMapping(".tex", "application/x-tex");
                base.AddMapping(".texi", "application/x-texinfo");
                base.AddMapping(".texinfo", "application/x-texinfo");
                base.AddMapping(".tgz", "application/x-compressed");
                base.AddMapping(".thmx", "application/vnd.ms-officetheme");
                base.AddMapping(".thn", "application/octet-stream");
                base.AddMapping(".tif", "image/tiff");
                base.AddMapping(".tiff", "image/tiff");
                base.AddMapping(".toc", "application/octet-stream");
                base.AddMapping(".tr", "application/x-troff");
                base.AddMapping(".trm", "application/x-msterminal");
                base.AddMapping(".tsv", "text/tab-separated-values");
                base.AddMapping(".ttf", "application/octet-stream");
                base.AddMapping(".txt", "text/plain");
                base.AddMapping(".u32", "application/octet-stream");
                base.AddMapping(".uls", "text/iuls");
                base.AddMapping(".ustar", "application/x-ustar");
                base.AddMapping(".vbs", "text/vbscript");
                base.AddMapping(".vcf", "text/x-vcard");
                base.AddMapping(".vcs", "text/plain");
                base.AddMapping(".vdx", "application/vnd.ms-visio.viewer");
                base.AddMapping(".vml", "text/xml");
                base.AddMapping(".vsd", "application/vnd.visio");
                base.AddMapping(".vss", "application/vnd.visio");
                base.AddMapping(".vst", "application/vnd.visio");
                base.AddMapping(".vsto", "application/x-ms-vsto");
                base.AddMapping(".vsw", "application/vnd.visio");
                base.AddMapping(".vsx", "application/vnd.visio");
                base.AddMapping(".vtx", "application/vnd.visio");
                base.AddMapping(".wav", "audio/wav");
                base.AddMapping(".wax", "audio/x-ms-wax");
                base.AddMapping(".wbmp", "image/vnd.wap.wbmp");
                base.AddMapping(".wcm", "application/vnd.ms-works");
                base.AddMapping(".wdb", "application/vnd.ms-works");
                base.AddMapping(".wks", "application/vnd.ms-works");
                base.AddMapping(".wm", "video/x-ms-wm");
                base.AddMapping(".wma", "audio/x-ms-wma");
                base.AddMapping(".wmd", "application/x-ms-wmd");
                base.AddMapping(".wmf", "application/x-msmetafile");
                base.AddMapping(".wml", "text/vnd.wap.wml");
                base.AddMapping(".wmlc", "application/vnd.wap.wmlc");
                base.AddMapping(".wmls", "text/vnd.wap.wmlscript");
                base.AddMapping(".wmlsc", "application/vnd.wap.wmlscriptc");
                base.AddMapping(".wmp", "video/x-ms-wmp");
                base.AddMapping(".wmv", "video/x-ms-wmv");
                base.AddMapping(".wmx", "video/x-ms-wmx");
                base.AddMapping(".wmz", "application/x-ms-wmz");
                base.AddMapping(".wps", "application/vnd.ms-works");
                base.AddMapping(".wri", "application/x-mswrite");
                base.AddMapping(".wrl", "x-world/x-vrml");
                base.AddMapping(".wrz", "x-world/x-vrml");
                base.AddMapping(".wsdl", "text/xml");
                base.AddMapping(".wvx", "video/x-ms-wvx");
                base.AddMapping(".x", "application/directx");
                base.AddMapping(".xaf", "x-world/x-vrml");
                base.AddMapping(".xaml", "application/xaml+xml");
                base.AddMapping(".xap", "application/x-silverlight-app");
                base.AddMapping(".xbap", "application/x-ms-xbap");
                base.AddMapping(".xbm", "image/x-xbitmap");
                base.AddMapping(".xdr", "text/plain");
                base.AddMapping(".xla", "application/vnd.ms-excel");
                base.AddMapping(".xlam", "application/vnd.ms-excel.addin.macroEnabled.12");
                base.AddMapping(".xlc", "application/vnd.ms-excel");
                base.AddMapping(".xlm", "application/vnd.ms-excel");
                base.AddMapping(".xls", "application/vnd.ms-excel");
                base.AddMapping(".xlsb", "application/vnd.ms-excel.sheet.binary.macroEnabled.12");
                base.AddMapping(".xlsm", "application/vnd.ms-excel.sheet.macroEnabled.12");
                base.AddMapping(".xlsx", "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
                base.AddMapping(".xlt", "application/vnd.ms-excel");
                base.AddMapping(".xltm", "application/vnd.ms-excel.template.macroEnabled.12");
                base.AddMapping(".xltx", "application/vnd.openxmlformats-officedocument.spreadsheetml.template");
                base.AddMapping(".xlw", "application/vnd.ms-excel");
                base.AddMapping(".xml", "text/xml");
                base.AddMapping(".xof", "x-world/x-vrml");
                base.AddMapping(".xpm", "image/x-xpixmap");
                base.AddMapping(".xps", "application/vnd.ms-xpsdocument");
                base.AddMapping(".xsd", "text/xml");
                base.AddMapping(".xsf", "text/xml");
                base.AddMapping(".xsl", "text/xml");
                base.AddMapping(".xslt", "text/xml");
                base.AddMapping(".xsn", "application/octet-stream");
                base.AddMapping(".xtp", "application/octet-stream");
                base.AddMapping(".xwd", "image/x-xwindowdump");
                base.AddMapping(".z", "application/x-compress");
                base.AddMapping(".zip", "application/x-zip-compressed");
            }
        }
        private static MimeMappingDictionaryBase _mappingDictionary = new MimeMappingDictionaryClassic();

        public static string GetMimeMapping(string fileName)
        {
            if (fileName == null)
            {
                throw new ArgumentNullException("fileName");
            }
            return _mappingDictionary.GetMimeMapping(fileName);
        }
    }
#endif
}