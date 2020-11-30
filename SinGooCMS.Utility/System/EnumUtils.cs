using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Reflection;
using System.Text;

namespace SinGooCMS.Utility
{
    /// <summary>
    /// 枚举帮助类
    /// </summary>
    public sealed class EnumUtils
    {
        /// <summary>
        /// 枚举转字典 枚举int-枚举说明
        /// </summary>
        /// <param name="enumType"></param>
        /// <returns></returns>
        public static Dictionary<int, string> TransIntDict(Type enumType)
        {
            Dictionary<int, string> dictionary = new Dictionary<int, string>();
            foreach (Enum enum2 in Enum.GetValues(enumType))
            {
                dictionary.Add(int.Parse(enum2.ToString("D")), GetDescription(enum2));
            }
            return dictionary;
        }

        /// <summary>
        /// 枚举转字典 枚举int-枚举说明
        /// </summary>
        /// <param name="enumType"></param>
        /// <returns></returns>
        public static Dictionary<string, string> TransStrDict(Type enumType)
        {
            Dictionary<string, string> dictionary = new Dictionary<string, string>();
            foreach (Enum enum2 in Enum.GetValues(enumType))
            {
                dictionary.Add(enum2.ToString(), GetDescription(enum2));
            }
            return dictionary;
        }

        /// <summary>
        /// 枚举转链表
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static List<T> EnumToList<T>()
        {
            Type enumType = typeof(T);
            if (enumType.BaseType != typeof(Enum))
            {
                throw new ArgumentException("T must be of type System.Enum");
            }
            Array values = Enum.GetValues(enumType);
            List<T> list = new List<T>(values.Length);
            foreach (int num in values)
            {
                list.Add((T)Enum.Parse(enumType, num.ToString()));
            }
            return list;
        }

        /// <summary>
        /// 读取枚举说明
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        private static string GetDescription(Enum value)
        {
            DescriptionAttribute[] customAttributes = (DescriptionAttribute[])value.GetType().GetField(value.ToString()).GetCustomAttributes(typeof(DescriptionAttribute), false);
            if (customAttributes.Length <= 0)
            {
                return value.ToString();
            }
            return customAttributes[0].Description;
        }

        /// <summary>
        /// 读取枚举说明
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="enums"></param>
        /// <returns></returns>
        public static string GetEnumCollectionDescription<T>(Collection<T> enums)
        {
            StringBuilder builder = new StringBuilder();
            Type type = typeof(T);
            if (type.BaseType != typeof(Enum))
            {
                throw new ArgumentException("T must be of type System.Enum");
            }
            foreach (T local in enums)
            {
                builder.AppendLine(GetEnumDescription<T>(local));
            }
            return builder.ToString();
        }

        /// <summary>
        /// 读取枚举说明
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="enumeratedType"></param>
        /// <returns></returns>
        public static string GetEnumDescription<T>(T enumeratedType)
        {
            string description = enumeratedType.ToString();
            Type type = typeof(T);
            if (type.BaseType != typeof(Enum))
            {
                throw new ArgumentException("T must be of type System.Enum");
            }
            FieldInfo field = enumeratedType.GetType().GetField(enumeratedType.ToString());
            if (field != null)
            {
                object[] customAttributes = field.GetCustomAttributes(typeof(DescriptionAttribute), false);
                if ((customAttributes != null) && (customAttributes.Length > 0))
                {
                    description = ((DescriptionAttribute)customAttributes[0]).Description;
                }
            }
            return description;
        }

        /// <summary>
        /// 读取枚举说明
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string GetEnumDescription(Enum value)
        {
            DescriptionAttribute[] customAttributes = (DescriptionAttribute[])value.GetType().GetField(value.ToString()).GetCustomAttributes(typeof(DescriptionAttribute), false);
            if ((customAttributes != null) && (customAttributes.Length > 0))
            {
                return customAttributes[0].Description;
            }
            return value.ToString();
        }

        /// <summary>
        /// 读取枚举说明
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static List<string> GetEnumDescriptions<T>() where T : struct
        {
            List<string> list = new List<string>();
            foreach (int num in Enum.GetValues(typeof(T)))
            {
                Enum enum2 = (Enum)Enum.ToObject(typeof(T), num);
                list.Add(GetEnumDescription(enum2));
            }
            return list;
        }

        /// <summary>
        /// 字符串转枚举类型
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="name"></param>
        /// <returns></returns>
        public static T StringToEnum<T>(string name) where T : struct
        {
            T? nullable = StringToEnumOrNull<T>(name);
            if (!nullable.HasValue)
            {
                return default(T);
            }
            return nullable.GetValueOrDefault();
        }

        /// <summary>
        /// 字符串转枚举类型
        /// </summary>
        /// <param name="enumType"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static object StringToEnum(Type enumType, string value)
        {
            if (enumType == null)
            {
                throw new ArgumentNullException("enumType");
            }
            return StringToEnum(enumType, value, false);
        }

        /// <summary>
        /// 字符串转枚举类型
        /// </summary>
        /// <param name="enumType"></param>
        /// <param name="value"></param>
        /// <param name="ignoreCase"></param>
        /// <returns></returns>
        public static object StringToEnum(Type enumType, string value, bool ignoreCase)
        {
            if (enumType == null)
            {
                throw new ArgumentNullException("enumType");
            }
            if (enumType.IsEnum)
            {
                try
                {
                    return Enum.Parse(enumType, value, ignoreCase);
                }
                catch (ArgumentException)
                {
                    return null;
                }
            }
            return null;
        }

        /// <summary>
        /// 字符串转枚举类型
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="name"></param>
        /// <returns></returns>
        public static T? StringToEnumOrNull<T>(string name) where T : struct
        {
            if (name != null)
            {
                foreach (T local in Enum.GetValues(typeof(T)))
                {
                    if (Enum.GetName(typeof(T), local) == name)
                    {
                        return new T?(local);
                    }
                }
            }
            return null;
        }
    }
}

