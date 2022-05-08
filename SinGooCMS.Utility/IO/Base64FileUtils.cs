using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace SinGooCMS.Utility
{
    /// <summary>
    /// base64字符串和文件互转
    /// </summary>
    public class Base64FileUtils
    {
        /// <summary>
        /// 文件转base64
        /// </summary>
        /// <param name="filePath">文件绝对路径</param>
        /// <param name="isJsFormat">是否js格式，js格式有前缀</param>
        /// <returns></returns>
        public static string ReadFileToString(string filePath, bool isJsFormat = false)
        {
            if (!File.Exists(filePath)) return null;

            FileStream fsForRead = new FileStream(filePath, FileMode.Open);//文件路径
            string base64str = ReadFileToString(fsForRead);

            if (isJsFormat)
            {
                //在前端读base64并轩化成文件，需要加上格式
                //如data:application/vnd.openxmlformats-officedocument.spreadsheetml.sheet;base64,
                base64str = $"data:{FileUtils.GetContentType(filePath)};base64,"+ base64str;
            }

            return base64str;
        }

        /// <summary>
        /// 文件流转为base64字符串
        /// </summary>
        /// <param name="fileStream"></param>
        /// <returns></returns>
        public static string ReadFileToString(FileStream fileStream)
        {
            string base64Str = "";
            try
            {
                //读写指针移到距开头10个字节处
                fileStream.Seek(0, SeekOrigin.Begin);
                byte[] bs = new byte[fileStream.Length];
                int log = Convert.ToInt32(fileStream.Length);
                //从文件中读取10个字节放到数组bs中
                fileStream.Read(bs, 0, log);
                base64Str = Convert.ToBase64String(bs);
                return base64Str;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                fileStream.Close();
            }
        }

        /// <summary>
        /// Base64字符串转文件并保存
        /// </summary>
        /// <param name="base64String">base64字符串</param>
        /// <param name="fileFullPath">保存的文件路径（全路径，包括文件名）</param>
        /// <returns>是否转换并保存成功</returns>
        public static bool SaveStringToFile(string base64String, string fileFullPath)
        {
            bool opResult = false;
            try
            {
                var filePath=Path.GetDirectoryName(fileFullPath);
                if (!Directory.Exists(filePath))
                {
                    Directory.CreateDirectory(filePath);
                }

                //如果有前缀格式，要去掉
                //如data:application/vnd.openxmlformats-officedocument.spreadsheetml.sheet;base64,
                string strbase64 = base64String.Trim().Substring(base64String.IndexOf(",") + 1);   //将‘，’以前的多余字符串删除
                using (FileStream fs = new FileStream(fileFullPath, FileMode.OpenOrCreate, FileAccess.Write))
                {
                    MemoryStream stream = new MemoryStream(Convert.FromBase64String(strbase64));
                    byte[] b = stream.ToArray();
                    fs.Write(b, 0, b.Length);
                    fs.Flush();
                } 

                opResult = true;
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return opResult;
        }

        /// <summary>
        /// Base64字符串转文件并保存
        /// </summary>
        /// <param name="base64String">base64字符串</param>
        /// <param name="fileFullPath">保存的文件路径（全路径，包括文件名）</param>
        /// <returns>是否转换并保存成功</returns>
        public static async Task<bool> SaveStringToFileAsync(string base64String, string fileFullPath)
        {
            bool opResult = false;
            try
            {
                var filePath = Path.GetDirectoryName(fileFullPath);
                if (!Directory.Exists(filePath))
                {
                    Directory.CreateDirectory(filePath);
                }

                //如果有前缀格式，要去掉
                //如data:application/vnd.openxmlformats-officedocument.spreadsheetml.sheet;base64,
                string strbase64 = base64String.Trim().Substring(base64String.IndexOf(",") + 1);   //将‘，’以前的多余字符串删除
                using (FileStream fs = new FileStream(fileFullPath, FileMode.OpenOrCreate, FileAccess.Write))
                {
                    MemoryStream stream = new MemoryStream(Convert.FromBase64String(strbase64));
                    byte[] b = stream.ToArray();
                    await fs.WriteAsync(b, 0, b.Length);
                    await fs.FlushAsync();
                }

                opResult = true;
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return opResult;
        }

        /// <summary>
        /// 是否base64格式
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static bool IsBase64Formatted(string input)
        {
            bool result;
            try
            {
                Convert.FromBase64String(input);
                result = true;
            }
            catch
            {
                result = false;
            }

            return result;
        }
    }
}
