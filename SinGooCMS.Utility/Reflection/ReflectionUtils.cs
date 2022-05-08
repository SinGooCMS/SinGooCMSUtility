using System;
using System.IO;
using System.Reflection;

namespace SinGooCMS.Utility
{
    /// <summary>
    /// 反射操作
    /// </summary>
    public sealed class ReflectionUtils
    {
        /// <summary>
        /// 加载组件
        /// </summary>
        /// <param name="dllFileName">组件名称，如a.dll</param>
        /// <param name="dir">组件所在目录，默认为当前bin目录</param>
        /// <returns></returns>
        public static Assembly Load(string dllFileName, string dir = "")
        {
            if (string.IsNullOrEmpty(dir))
                dir = AppDomain.CurrentDomain.BaseDirectory;

            return Assembly.LoadFrom(Path.Combine(dir, dllFileName));
        }

        #region 通过dll文件创建实例

        /// <summary>
        /// 创建实例
        /// </summary>
        /// <typeparam name="T">泛型</typeparam>
        /// <param name="assemblyFile">ddl文件全路径</param>
        /// <param name="parameters">参数集合</param>
        /// <returns></returns>
        public static T CreateInstance<T>(string assemblyFile, object[] parameters = null) =>
            (T)CreateInstance(assemblyFile, typeof(T).FullName, parameters);

        /// <summary>
        /// 创建实例
        /// </summary>
        /// <param name="assemblyFile">ddl文件全路径</param>
        /// <param name="fullName">类限定名 如 SinGooCMS.Ado.DbAccess.SqlServerAccess</param>
        /// <param name="parameters">参数集合</param>
        /// <returns></returns>
        public static object CreateInstance(string assemblyFile, string fullName, object[] parameters = null)
        {
            Type type = GetType(assemblyFile, fullName);
            return CreateInstance(type, parameters);
        }

        /// <summary>
        /// 创建实例
        /// </summary>
        /// <param name="type">类型</param>
        /// <param name="parameters">参数集合</param>
        /// <returns></returns>
        public static object CreateInstance(Type type, object[] parameters = null)
        {
            return parameters == null
                ? Activator.CreateInstance(type)
                : Activator.CreateInstance(type, parameters);
        }

        /// <summary>
        /// 读取类型
        /// </summary>
        /// <param name="assemblyFile">ddl文件全路径 如 F:\SinGooCMS.Ado.dll</param>
        /// <param name="fullName">类限定名 如 SinGooCMS.Ado.DbAccess.SqlServerAccess</param>
        /// <returns></returns>
        public static Type GetType(string assemblyFile, string fullName)
        {
            Assembly tempAssembly = Assembly.LoadFrom(assemblyFile);
            return tempAssembly.GetType(fullName);
        }

        #endregion
    }
}