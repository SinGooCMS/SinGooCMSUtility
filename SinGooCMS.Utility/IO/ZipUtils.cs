using System;
using System.Text;
using System.IO;
using System.Linq;
using SharpCompress.Common;
using SharpCompress.Archives;
using SharpCompress.Archives.Zip; //zip
using SharpCompress.Archives.Rar; //rar
using SharpCompress.Archives.SevenZip; //7z
using SinGooCMS.Utility.Extension;

namespace SinGooCMS.Utility
{
    /// <summary>
    /// 压缩和解压
    /// <para>压缩、ZipUtils.Zip(@"f:\test")</para>
    /// <para>解压、ZipUtils.UnZip(@"f:\test.zip",@"f:\test")</para>
    /// </summary>
    public sealed class ZipUtils
    {
        /// <summary>
        /// 压缩(支持 zip、rar、7z)，默认zip
        /// </summary>
        /// <param name="dirPath">需要压缩的目录</param>
        /// <param name="zipFile">文件的绝对路径，如果不指定，将保存到当前压缩的目录下</param>
        public static string Zip(string dirPath, string zipFile = "")
        {
            if (!Directory.Exists(dirPath))
                throw new DirectoryNotFoundException("参数[dirPath]不是一个有效的文件夹路径");

            if (!zipFile.IsNullOrEmpty() && !(new string[] { ".zip", ".rar", ".7z" }.Contains(Path.GetExtension(zipFile).ToLower())))
                throw new DirectoryNotFoundException("文件名格式不正确");

            if (zipFile.IsNullOrEmpty())
                zipFile = FileUtils.Combine(dirPath, StringUtils.GetNewFileName() + ".zip");

            //解决中文乱码问题
            var options = new SharpCompress.Writers.WriterOptions(CompressionType.Deflate)
            {
                ArchiveEncoding = new ArchiveEncoding
                {
                    Default = Encoding.GetEncoding("utf-8")
                }
            };
            using (var archive = ZipArchive.Create())
            {
                archive.AddAllFromDirectory(dirPath);
                archive.SaveTo(zipFile, options);

                return zipFile;
            }
        }

        /// <summary>
        /// 解压
        /// </summary>
        /// <param name="zipFile">压缩包文件</param>
        /// <param name="extractPath">解压到目录</param>
        public static void UnZip(string zipFile, string extractPath)
        {
            //注意压缩包是zip还是rar
            string ext = System.IO.Path.GetExtension(zipFile).ToLower();
            switch (ext)
            {
                case ".7z":
                    using (var archive = SevenZipArchive.Open(zipFile))
                    {
                        foreach (var entry in archive.Entries.Where(entry => !entry.IsDirectory))
                        {
                            entry.WriteToDirectory(extractPath, new ExtractionOptions()
                            {
                                ExtractFullPath = true,
                                Overwrite = true
                            });
                        }
                    }
                    break;
                case ".zip":
                    using (var archive = ZipArchive.Open(zipFile))
                    {
                        foreach (var entry in archive.Entries.Where(entry => !entry.IsDirectory))
                        {
                            entry.WriteToDirectory(extractPath, new ExtractionOptions()
                            {
                                ExtractFullPath = true,
                                Overwrite = true
                            });
                        }
                    }
                    break;
                case ".rar":
                default:
                    using (var archive = RarArchive.Open(zipFile))
                    {
                        foreach (var entry in archive.Entries.Where(entry => !entry.IsDirectory))
                        {
                            entry.WriteToDirectory(extractPath, new ExtractionOptions()
                            {
                                ExtractFullPath = true,
                                Overwrite = true
                            });
                        }
                    }
                    break;

            }
        }
    }
}
