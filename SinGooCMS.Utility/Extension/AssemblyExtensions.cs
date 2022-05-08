using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Versioning;

namespace SinGooCMS.Utility.Extension
{
    /// <summary>
    /// <see cref="Assembly"/>扩展类
    /// </summary>
    public static class AssemblyExtension
    {
        /// <summary>
        /// 获取程序集标记的 <c>.net</c> 版本
        /// </summary>
        /// <param name="assembly">The assembly</param>
        public static string GetFrameworkVersion(this Assembly assembly)
        {
            var targetFrameAttribute = assembly.GetCustomAttributes(true)
                .OfType<TargetFrameworkAttribute>().FirstOrDefault();

            if (targetFrameAttribute is null) { return ".NET 2, 3 or 3.5"; }

            return targetFrameAttribute.FrameworkName;
        }

        /// <summary>
        /// 获取程序集文件所在的目录 从 <seealso cref="Assembly.Location"/> 处获取
        /// </summary>
        public static DirectoryInfo GetAssemblyLocation(this Assembly assembly)
        {
            return new DirectoryInfo(Path.GetDirectoryName(assembly.Location) ?? throw new InvalidOperationException());
        }

        /// <summary>
        /// 获取程序集文件所在的目录 从 <seealso cref="Assembly.CodeBase"/> 处实现
        /// </summary>
        public static DirectoryInfo GetAssemblyCodeBase(this Assembly assembly)
        {
            var uri = new Uri(assembly.CodeBase);
            return new DirectoryInfo(Path.GetDirectoryName(uri.LocalPath));
        }

        /// <summary>
        /// 当前程序集是否以Release模式编译 参照 <see href="http://www.hanselman.com/blog/HowToProgrammaticallyDetectIfAnAssemblyIsCompiledInDebugOrReleaseMode.aspx"/>
        /// </summary>
        public static bool IsOptimized(this Assembly assembly)
        {
            var attributes = assembly.GetCustomAttributes(typeof(DebuggableAttribute), false);

            if (attributes.Length == 0) { return true; }

            foreach (Attribute attr in attributes)
            {
                if (attr is DebuggableAttribute d)
                {
                    // FYI
                    // "Run time Optimizer is enabled: " + !d.IsJITOptimizerDisabled
                    // "Run time Tracking is enabled: " + d.IsJITTrackingEnabled
                    return !d.IsJITOptimizerDisabled;
                }
            }
            return false;
        }

        /// <summary>
        /// 当前程序集是否是32位
        /// </summary>
        public static bool Is32Bit(this Assembly assembly)
        {
            var location = assembly.Location;
            if (location.IsNullOrEmpty()) { location = assembly.CodeBase; }

            var uri = new Uri(location);
            var assemblyName = AssemblyName.GetAssemblyName(uri.LocalPath);
            return assemblyName.ProcessorArchitecture == ProcessorArchitecture.X86;
        }
    }
}