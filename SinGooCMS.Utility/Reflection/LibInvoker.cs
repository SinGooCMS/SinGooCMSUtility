using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;

namespace SinGooCMS.Utility
{
    /// <summary>
    /// 非托管方法调用
    /// </summary>
    public class LibInvoker
    {
        [DllImport("kernel32.dll")]
        private extern static IntPtr LoadLibrary(string path);

        [DllImport("kernel32.dll")]
        private extern static IntPtr GetProcAddress(IntPtr lib, string funcName);

        [DllImport("kernel32.dll")]
        private extern static bool FreeLibrary(IntPtr lib);

        private IntPtr hLib;
        /// <summary>
        /// 构造方法
        /// </summary>
        /// <param name="DLLPath"></param>
        public LibInvoker(string DLLPath)
        {
            hLib = LoadLibrary(DLLPath);
        }

        /// <summary>
        /// 释放
        /// </summary>
        ~LibInvoker()
        {
            FreeLibrary(hLib);
        }

        /// <summary>
        /// 利用LoadLibrary方式，动态显示调用非托管DLL函数。
        /// </summary>
        /// <param name="APIName"></param>
        /// <param name="t"></param>
        /// <returns></returns>
        public Delegate Invoke(string APIName, Type t)
        {
            IntPtr api = GetProcAddress(hLib, APIName);
            return (Delegate)Marshal.GetDelegateForFunctionPointer(api, t);
        }

        /// <summary>
        /// 利用反射技术，动态显示调用托管DLL函数。
        /// </summary>
        /// <param name="DLLPath"></param>
        /// <param name="dllName"></param>
        /// <param name="className"></param>
        /// <param name="methodName"></param>
        /// <param name="methodParam"></param>
        /// <returns></returns>
        public static Object Invoke(string DLLPath, string dllName, string className, string methodName, params object[] methodParam)
        {
            string specifyName = dllName.Replace("dll", className);
            AppDomain newDomain = AppDomain.CreateDomain(specifyName);
            try
            {
                Assembly assem = Assembly.LoadFrom(DLLPath);

                Object o = assem.CreateInstance(specifyName, false,
                BindingFlags.ExactBinding,
                null, new Object[] { }, null, null);

                MethodInfo m = assem.GetType(specifyName).GetMethod(methodName);
                Object ret = m.Invoke(o, methodParam);
                return ret;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                AppDomain.Unload(newDomain);
            }
        }
    }
}
