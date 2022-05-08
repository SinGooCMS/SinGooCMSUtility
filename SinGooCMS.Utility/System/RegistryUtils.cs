using System;
using Microsoft.Win32;

namespace SinGooCMS.Utility
{
    /// <summary>
    /// 注册表操作，Win平台专有(需要有管理员权限)
    /// <para>1、添加启动项 RegistryUtils.SetRunValue("CTTools", @"D:\MyDev\CTTool\CTTool.v1.2\CTTools.exe");</para>
    /// <para>2、取消启动项 RegistryUtils.DeleteRunValue("CTTools");</para>
    /// </summary>
    public class RegistryUtils
    {
        /// <summary>
        /// 读取注册表键值
        /// </summary>
        /// <param name="fullKey"></param>
        /// <param name="itemName"></param>
        /// <returns></returns>
        public static object GetValue(string fullKey, string itemName)
        {
            RegistryKey registry = Registry.CurrentUser.OpenSubKey(fullKey, true);
            return registry?.GetValue(itemName);
        }

        /// <summary>
        /// 读取注册表(启动项)键值
        /// </summary>
        /// <param name="itemName"></param>
        /// <returns></returns>
        public static object GetRunValue(string itemName) =>
            GetValue("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", itemName);

        /// <summary>
        /// 保存注册表键值
        /// </summary>
        /// <param name="fullKey"></param>
        /// <param name="itemName"></param>
        /// <param name="itemValue"></param>
        /// <returns></returns>
        public static bool SetValue(string fullKey, string itemName, string itemValue)
        {
            try
            {
                RegistryKey registry = Registry.CurrentUser.OpenSubKey(fullKey, true);
                if (registry == null)//若指定的子项不存在
                    registry = Registry.CurrentUser.CreateSubKey(fullKey);//则创建指定的子项

                registry.SetValue(itemName, itemValue);//设置该子项的新的“键值对”
                registry.Close();
            }
            catch
            {
                return false;
            }

            return false;
        }

        /// <summary>
        /// 保存注册表(启动项)键值
        /// </summary>
        /// <param name="itemName"></param>
        /// <param name="itemValue"></param>
        /// <returns></returns>
        public static bool SetRunValue(string itemName, string itemValue) =>
            SetValue("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", itemName, itemValue);

        /// <summary>
        /// 删除键值
        /// </summary>
        /// <param name="itemName"></param>
        public static void DeleteRunValue(string itemName) =>
            DeleteValue("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", itemName);

        /// <summary>
        /// 删除键值
        /// </summary>
        /// <param name="fullKey"></param>
        /// <param name="itemName"></param>
        public static void DeleteValue(string fullKey, string itemName)
        {
            RegistryKey registry = Registry.CurrentUser.OpenSubKey(fullKey, true);
            if (registry != null)
                registry.DeleteValue(itemName);
        }
    }
}
