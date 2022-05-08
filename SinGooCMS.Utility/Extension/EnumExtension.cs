using System;
using System.Collections.Generic;
using System.Text;

namespace SinGooCMS.Utility.Extension
{
    /// <summary>
    /// 枚举扩展
    /// </summary>
    public static class EnumExtension
    {
        /// <summary>
        /// 读枚举的说明
        /// </summary>
        /// <param name="enum"></param>
        /// <returns></returns>
        public static string GetDescription(this System.Enum @enum) => EnumUtils.GetEnumDescription(@enum);

        /// <summary>
        /// 文本转枚举
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="enumName"></param>
        /// <returns></returns>
        public static T ToEnum<T>(this string enumName) where T : struct =>
            EnumUtils.StringToEnum<T>(enumName);
    }
}
