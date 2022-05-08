using SinGooCMS.Utility.Extension;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace SinGooCMS.Utility
{
    /// <summary>
    /// JSON文件工具
    /// </summary>
    public class JsonFileUtils
    {
        #region Read

        /// <summary>
        /// 从文件中读对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="filePath">全文件路径，包括文件名</param>
        /// <returns></returns>
        public static T Read<T>(string filePath) where T : class, new()
        {
            if (File.Exists(filePath))
            {
                try
                {
                    string str = File.ReadAllText(filePath);
                    if (!string.IsNullOrEmpty(str))
                        return Newtonsoft.Json.JsonConvert.DeserializeObject<T>(str);
                }
                catch
                {
                    return default(T);
                }
            }

            return default(T);
        }

        /// <summary>
        /// 从文件中导出对象列表
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="filePath">全文件路径，包括文件名</param>
        /// <returns></returns>
        public static List<T> ReadList<T>(string filePath) where T : class, new()
        {
            if (File.Exists(filePath))
            {
                try
                {
                    string str = File.ReadAllText(filePath);
                    if (!string.IsNullOrEmpty(str))
                        return Newtonsoft.Json.JsonConvert.DeserializeObject<List<T>>(str);
                }
                catch
                {
                    return default(List<T>);
                }
            }

            return default(List<T>);
        }

        #endregion

        #region Save

        static readonly object locker = new object();
        /// <summary>
        /// 序列化对象到文件中(线程安全=线程同步)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="configModel"></param>
        /// <param name="filePath"></param>
        public static void Save<T>(T configModel, string filePath)
        {
            //线程同步，同时仅一个线程可访问资源
            lock (locker)
            {
                File.WriteAllText(filePath, configModel.ToJson());
            }
        }

        /// <summary>
        /// 序列化对象到文件中(线程安全=线程同步)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="lst"></param>
        /// <param name="filePath"></param>
        public static void Save<T>(List<T> lst, string filePath)
        {
            //线程同步，同时仅一个线程可访问资源
            lock (locker)
            {
                File.WriteAllText(filePath, lst.ToJson());
            }
        }

        #endregion
    }
}
