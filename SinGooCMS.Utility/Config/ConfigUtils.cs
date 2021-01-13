using System;
using System.Configuration;
using System.Collections.Generic;
using System.Text;
using System.Linq;

#if NETSTANDARD2_1
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Json;
#endif

namespace SinGooCMS.Utility
{
    /// <summary>
    /// 配置管理
    /// </summary>
    public static class ConfigUtils
    {
#if NETSTANDARD2_1
        /// <summary>
        /// 配置
        /// </summary>
        public static IConfiguration Configuration { get; set; }

        static ConfigUtils()
        {
            Configuration = new ConfigurationBuilder()
            .Add(new JsonConfigurationSource { Path = "appsettings.json", ReloadOnChange = true })
            .Build();
        }

        #region 常用配置

        /// <summary>
        /// 连接设置
        /// </summary>
        public static IConfigurationSection ConnectionStrings => Configuration.GetSection("ConnectionStrings");

        /// <summary>
        /// app设置
        /// </summary>
        public static IConfigurationSection AppSettings => Configuration.GetSection("AppSettings");

        /// <summary>
        /// 数据库类型
        /// </summary>
        public static string ProviderName => Configuration.GetConnectionString("ProviderName");

        /// <summary>
        /// 数据库库连接字符串
        /// </summary>
        public static string DefConnStr => Configuration.GetConnectionString("SQLConnSTR");

        #endregion

        #region 读取配置

        /// <summary>
        /// 根据层级路径读取配置值
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="path">如 EPPlus:ExcelPackage:LicenseContext</param>
        /// <returns></returns>
        public static T GetByPath<T>(string path)
        {
            var section= Configuration.GetSection(path.Split(':')[0]);
            return GetByPath<T>(section, path, 0);
        }

        private static T GetByPath<T>(IConfigurationSection section, string path, int depth)
        {
            var arr = path.Split(':');
            var childs = section.GetChildren();

            if (childs.Count() > 0 && depth < arr.Length - 1)
            {
                depth++;
                var sectionNew = childs.Where(p => p.Key == arr[depth]).FirstOrDefault();
                if (sectionNew != null)
                    return GetByPath<T>(sectionNew, path, depth);
                else
                    return (T)Convert.ChangeType(section.Value, typeof(T));
            }
            else
            {
                return (T)Convert.ChangeType(section.Value, typeof(T));
            }
        }

        /// <summary>
        /// 获取配置值
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="appSettingName"></param>
        /// <returns></returns>
        public static T GetAppSetting<T>(string appSettingName) where T : IConvertible
        {
            return GetAppSettingInternal<T>(appSettingName, false, default);
        }
        /// <summary>
        /// 获取配置值
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="appSettingName"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public static T GetAppSetting<T>(string appSettingName, T defaultValue) where T : IConvertible
        {
            return GetAppSettingInternal<T>(appSettingName, true, defaultValue);
        }

        private static T GetAppSettingInternal<T>(string appSettingName, bool useDefaultOnUndefined, T defaultValue) where T : IConvertible
        {
            string value = AppSettings[appSettingName];
            if (value == null)
            {
                if (useDefaultOnUndefined)
                    return defaultValue;
                else
                    throw new Exception(string.Format("{0} not defined in appSettings.", appSettingName));
            }

            return (T)Convert.ChangeType(value, typeof(T));
        }

        #endregion

#else
        /// <summary>
        /// 读取配置
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="appSettingName"></param>
        /// <returns></returns>
        public static T GetAppSetting<T>(string appSettingName) where T : IConvertible
        {
            return GetAppSettingInternal<T>(appSettingName, false, default(T));
        }
        /// <summary>
        /// 读取配置
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="appSettingName"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public static T GetAppSetting<T>(string appSettingName, T defaultValue) where T : IConvertible
        {
            return GetAppSettingInternal<T>(appSettingName, true, defaultValue);
        }
        /// <summary>
        /// 读取配置
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="appSettingName"></param>
        /// <param name="useDefaultOnUndefined"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        private static T GetAppSettingInternal<T>(string appSettingName, bool useDefaultOnUndefined, T defaultValue) where T : IConvertible
        {
            string value = ConfigurationManager.AppSettings[appSettingName];
            if (value == null)
            {
                if (useDefaultOnUndefined)
                    return defaultValue;
                else
                    throw new Exception(string.Format("{0} not defined in appSettings.", appSettingName));
            }

            return (T)Convert.ChangeType(value, typeof(T));
        }
        /// <summary>
        /// 添加配置属性
        /// </summary>
        /// <param name="collection"></param>
        /// <param name="properties"></param>
        public static void AddConfigurationProperties(ConfigurationPropertyCollection collection, IEnumerable<ConfigurationProperty> properties)
        {
            if (collection == null)
                throw new ArgumentNullException("collection");
            if (properties == null)
                throw new ArgumentNullException("properties");

            foreach (ConfigurationProperty property in properties)
                collection.Add(property);
        }

        /// <summary>
        /// 数据库类型
        /// </summary>
        public static string ProviderName => GetConnectionString().ProviderName;

        /// <summary>
        /// 数据库库连接字符串
        /// </summary>
        public static string DefConnStr => GetConnectionString().ConnectionString;

        /// <summary>
        /// 获取连接字符串 配置如 <add name="SQLConnSTR" providerName="SqlServer" connectionString="server=(local);database=SinGooCMS-v1.6;uid=sa;pwd=123"/>
        /// </summary>
        /// <param name="connectionStringName"></param>
        /// <returns></returns>
        public static ConnectionStringSettings GetConnectionString(string connectionStringName= "SQLConnSTR")
        {
            var settings = ConfigurationManager.ConnectionStrings[connectionStringName];

            if (settings == null)
                throw new Exception(string.Format("没有找到名称为'{0}'的连接字符串", connectionStringName));

            return settings;
        }
#endif
    }
}