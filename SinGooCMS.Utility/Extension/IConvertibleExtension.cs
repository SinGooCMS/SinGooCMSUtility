using System;

namespace SinGooCMS.Utility.Extension
{
    /// <summary>
    /// 可转换扩展
    /// </summary>
    public static class IConvertibleExtension
    {
        /// <summary>
        /// 类型直转
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <returns></returns>
        public static T To<T>(this IConvertible value)
        {
            try
            {
                return (T)Convert.ChangeType(value, typeof(T));
            }
            catch
            {
                return default;
            }
        }
    }
}