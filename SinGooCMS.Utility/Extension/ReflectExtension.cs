using System;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Resources;
using System.Text;

namespace SinGooCMS.Utility.Extension
{
    /// <summary>
    /// 反射扩展
    /// </summary>
    public static class ReflectExtension
    {
        private static readonly BindingFlags bindingFlags = BindingFlags.DeclaredOnly | BindingFlags.Public |
                                                  BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static;

        #region 方法调用            

        /// <summary>
        /// 执行方法
        /// <para>调用普通方法：InvokeMethod("GetCount", null, new object[] { "Entity","" })</para>
        /// <para>调用泛型方法：InvokeMethod("Find", new Type[] { typeof(EntityInfo) }, new object[] { 1 })</para>
        /// </summary>
        /// <param name="obj">实例对象</param>
        /// <param name="methodName">方法名称，区分大小写</param>
        /// <param name="typeParameters">类型集合（泛型的类型集合）</param>
        /// <param name="args">参数集合</param>
        /// <returns></returns>
        public static object InvokeMethod(this object obj, string methodName, Type[] typeParameters, object[] args)
        {
            object objResult;
            var methods = obj.GetType().GetMethods(bindingFlags);

            //这里要区别泛型和非泛型，否则同名的方法会提示 模糊匹配，typeParameters有值就是泛型，泛型才要指定类型参数
            MethodInfo method = null;
            if (typeParameters != null)
                method = methods.Where(p => p.IsGenericMethod && p.Name == methodName).FirstOrDefault();
            else
                method = methods.Where(p => !p.IsGenericMethod && p.Name == methodName).FirstOrDefault();

            if (method == null)
                return null;

            if (method.IsGenericMethod) //泛型方法
            {
                method = method.MakeGenericMethod(typeParameters);
                objResult = method.Invoke(obj, args);
            }
            else
            {
                objResult = method.Invoke(obj, args);
                //obj.GetType().InvokeMember(methodName, bindingFlags | BindingFlags.InvokeMethod, null, obj, args);
            }

            return objResult; //无返回值的方法 这里返回null
        }

        #endregion

        #region 属性与字段、成员读写

        #region 属性 Property

        /// <summary>
        /// 读取属性
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public static PropertyInfo GetProperty(this object obj, string name) =>
            obj.GetType().GetProperty(name, bindingFlags);

        /// <summary>
        /// 获取属性值
        /// </summary>
        /// <param name="obj">反射对象</param>
        /// <param name="name">属性名</param>
        /// <typeparam name="T">约束返回的T必须是引用类型</typeparam>
        /// <returns>T类型</returns>
        public static T GetPropertyVal<T>(this object obj, string name) =>
            (T)obj.GetProperty(name).GetValue(obj, null);

        /// <summary>
        /// 设置属性值
        /// </summary>
        /// <param name="obj">反射对象</param>
        /// <param name="name">属性名</param>
        /// <param name="value">值</param>
        public static void SetPropertyVal(this object obj, string name, object value)
        {
            var propertyInfo = obj.GetProperty(name);
            object objValue = Convert.ChangeType(value, propertyInfo.PropertyType);
            propertyInfo.SetValue(obj, objValue, null);
        }

        #endregion

        #region 字段 Field

        /// <summary>
        /// 读取字段信息
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public static FieldInfo GetField(this object obj, string name) =>
            obj.GetType().GetField(name, bindingFlags);

        /// <summary>
        /// 获取所有的字段信息
        /// </summary>
        /// <param name="obj">反射对象</param>
        /// <returns>字段信息</returns>
        public static FieldInfo[] GetFields(this object obj) =>
            obj.GetType().GetFields(bindingFlags);

        /// <summary>
        /// 获取字段
        /// </summary>
        /// <param name="obj">反射对象</param>
        /// <param name="name">字段名</param>
        /// <typeparam name="T">约束返回的T必须是引用类型</typeparam>
        /// <returns>T类型</returns>
        public static T GetFieldVal<T>(this object obj, string name) =>
            (T)obj.GetField(name).GetValue(obj);

        /// <summary>
        /// 设置字段
        /// </summary>
        /// <param name="obj">反射对象</param>
        /// <param name="name">字段名</param>
        /// <param name="value">值</param>
        public static void SetFieldVal(this object obj, string name, object value) =>
            obj.GetField(name).SetValue(obj, value);

        #endregion

        #endregion

        #region 资源获取

        /// <summary>
        /// 根据资源名称获取图片资源流
        /// </summary>
        /// <param name="assemblyType"></param>
        /// <param name="resourceName"></param>
        /// <returns></returns>
        public static Stream GetImageResource(this Type assemblyType, string resourceName)
        {
            Assembly thisAssembly = Assembly.GetAssembly(assemblyType);
            return thisAssembly.GetManifestResourceStream(resourceName);
        }

        /// <summary>
        /// 获取程序集资源的位图资源
        /// </summary>
        /// <param name="assemblyType">程序集中的某一对象类型</param>
        /// <param name="resourceHolder">资源的根名称。例如，名为“MyResource.en-US.resources”的资源文件的根名称为“MyResource”。</param>
        /// <param name="imageName">资源项名称</param>
        public static Bitmap LoadBitmap(this Type assemblyType, string resourceHolder, string imageName)
        {
            Assembly thisAssembly = Assembly.GetAssembly(assemblyType);
            ResourceManager rm = new ResourceManager(resourceHolder, thisAssembly);
            return (Bitmap)rm.GetObject(imageName);
        }

        /// <summary>
        ///  获取程序集资源的文本资源
        /// </summary>
        /// <param name="assemblyType">程序集中的某一对象类型</param>
        /// <param name="resName">资源项名称</param>
        /// <param name="resourceHolder">资源的根名称。例如，名为“MyResource.en-US.resources”的资源文件的根名称为“MyResource”。</param>
        public static string GetResString(this Type assemblyType, string resName, string resourceHolder)
        {
            Assembly thisAssembly = Assembly.GetAssembly(assemblyType);
            ResourceManager rm = new ResourceManager(resourceHolder, thisAssembly);
            return rm.GetString(resName);
        }

        /// <summary>
        /// 读取资源多语言文本
        /// </summary>
        /// <param name="assemblyType"></param>
        /// <param name="resName"></param>
        /// <param name="cultureInfo"></param>
        /// <returns></returns>
        public static string GetResString(this Type assemblyType, string resName, CultureInfo cultureInfo = null)
        {
            ResourceManager rm = new ResourceManager(assemblyType);
            return cultureInfo == null
                    ? rm.GetString(resName)
                    : rm.GetString(resName, cultureInfo);
        }

        /// <summary>
        /// 获取程序集嵌入资源的文本形式
        /// </summary>
        /// <param name="assemblyType">程序集中的某一对象类型</param>
        /// <param name="charset">字符集编码</param>
        /// <param name="resName">嵌入资源相对路径</param>
        /// <returns>如没找到该资源则返回空字符</returns>
        public static string GetManifestString(this Type assemblyType, string charset, string resName)
        {
            Assembly asm = Assembly.GetAssembly(assemblyType);
            Stream st = asm.GetManifestResourceStream(string.Concat(assemblyType.Namespace, ".", resName.Replace("/", ".")));
            if (st == null)
            {
                return "";
            }

            int iLen = (int)st.Length;
            byte[] bytes = new byte[iLen];
            st.Read(bytes, 0, iLen);
            return (bytes != null) ? Encoding.GetEncoding(charset).GetString(bytes) : "";
        }

        #endregion
    }
}
