using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace SinGooCMS.Utility.Extension
{
    /// <summary>
    /// 对象扩展
    /// </summary>
    public static class ObjectExtension
    {
        /// <summary>
        /// 深度克隆，注意T必须是 Serializable
        /// </summary>
        /// <typeparam name="T">泛型</typeparam>
        /// <param name="obj">原对象</param>
        /// <returns></returns>
        public static T DeepClone<T>(this T obj)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                BinaryFormatter bf = new BinaryFormatter();
                bf.Serialize(ms, obj);
                ms.Position = 0;
                return (T)bf.Deserialize(ms);
            }
        }

        /// <summary>
        /// 严格比较两个对象是否是同一对象(判断引用)
        /// </summary>
        /// <param name="obj">自己</param>
        /// <param name="o">需要比较的对象</param>
        /// <returns>是否同一对象</returns>
        public new static bool ReferenceEquals(this object obj, object o)
        {
            return object.ReferenceEquals(obj, o);
        }
    }
}