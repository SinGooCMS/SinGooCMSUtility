using System;
using System.Collections.Generic;
using System.Data;

namespace SinGooCMS.Utility.Extension
{
    /// <summary>
    /// 数据类型转换,利用TryParse判断是否可转换，如果能则返回转换后的值，否则返回默认值
    /// </summary>
    public static class DataTypeChangeExtension
    {
        #region 数据类型转换

        /// <summary>
        /// 转化为 byte 整形(0 到 255)
        /// </summary>
        /// <param name="thisValue"></param>
        /// <param name="defValue"></param>
        /// <returns></returns>
        public static byte ToByte(this object thisValue, byte defValue = 0)
        {
            if (thisValue != null)
            {
                //判断是否小数
                if (thisValue.ToString().IsDecimal())
                    return (byte)(thisValue.ToDecimal());

                if (byte.TryParse(thisValue.ToString(), out byte result))
                    return result;
            }

            return defValue;
        }

        /// <summary>
        /// 转化为短整型
        /// </summary>
        /// <param name="thisValue"></param>
        /// <param name="defValue"></param>
        /// <returns></returns>
        public static short ToShort(this object thisValue, short defValue = 0)
        {
            if (thisValue != null)
            {
                //判断是否小数
                if (thisValue.ToString().IsDecimal())
                    return (short)(thisValue.ToDecimal());

                if (Int16.TryParse(thisValue.ToString(), out short result))
                    return result;
            }

            return defValue;
        }

        /// <summary>
        /// 转化为整型，如果是小数，将截断小数部分
        /// </summary>
        /// <param name="thisValue"></param>
        /// <param name="defValue"></param>
        /// <returns></returns>
        public static int ToInt(this object thisValue, int defValue = 0)
        {
            if (thisValue != null)
            {
                //判断是否小数
                if (thisValue.ToString().IsDecimal())
                    return (int)(thisValue.ToDecimal());

                //非小数
                if (Int32.TryParse(thisValue.ToString(), out int result))
                    return result;
            }

            return defValue;
        }

        /// <summary>
        /// 转化为长整型
        /// </summary>
        /// <param name="thisValue"></param>
        /// <param name="defValue"></param>
        /// <returns></returns>
        public static long ToLong(this object thisValue, long defValue = 0)
        {
            if (thisValue != null)
            {
                //判断是否小数
                if (thisValue.ToString().IsDecimal())
                    return (long)(thisValue.ToDecimal());

                if (Int64.TryParse(thisValue.ToString(), out long result))
                    return result;
            }

            return defValue;
        }

        /// <summary>
        /// 转化为布尔型
        /// </summary>
        /// <param name="thisValue"></param>
        /// <param name="defValue"></param>
        /// <returns></returns>
        public static bool ToBool(this object thisValue, bool defValue = false)
        {
            if (thisValue != null)
            {
                if (string.Compare(thisValue.ToString(), "true", true).Equals(0))
                    return true;

                if (thisValue.ToInt() == 1) //sqlite没有布尔型
                    return true;

                if (Boolean.TryParse(thisValue.ToString(), out bool result))
                    return result;
            }

            return defValue;
        }

        /// <summary>
        /// 转化为浮点型
        /// </summary>
        /// <param name="thisValue"></param>
        /// <param name="defValue"></param>
        /// <returns></returns>
        public static float ToFloat(this object thisValue, float defValue = 0.0f)
        {
            if (thisValue != null)
            {
                if (float.TryParse(thisValue.ToString(), out float result))
                    return result;
            }

            return defValue;
        }

        /// <summary>
        /// 转化为双精度数据类型
        /// </summary>
        /// <param name="thisValue"></param>
        /// <param name="defValue"></param>
        /// <returns></returns>
        public static double ToDouble(this object thisValue, double defValue = 0.0d)
        {
            if (thisValue != null)
            {
                if (Double.TryParse(thisValue.ToString(), out double result))
                    return result;
            }

            return defValue;
        }

        /// <summary>
        /// 转化为小数类型
        /// </summary>
        /// <param name="thisValue"></param>
        /// <param name="defValue"></param>
        /// <returns></returns>
        public static decimal ToDecimal(this object thisValue, decimal defValue = 0.0m)
        {
            if (thisValue != null)
            {
                if (decimal.TryParse(thisValue.ToString(), out decimal result))
                    return result;
            }

            return defValue;
        }

        /// <summary>
        /// 转化为日期类型
        /// </summary>
        /// <param name="thisValue"></param>
        /// <returns></returns>
        public static DateTime ToDateTime(this object thisValue) =>
            thisValue.ToDateTime(new DateTime(1900, 1, 1));

        /// <summary>
        /// 转化为日期类型
        /// </summary>
        /// <param name="thisValue"></param>
        /// <param name="defValue"></param>
        /// <returns></returns>
        public static DateTime ToDateTime(this object thisValue, DateTime defValue)
        {
            if (thisValue != null)
            {
                if (DateTime.TryParse(thisValue.ToString(), out DateTime result))
                    return result;
            }

            return defValue;
        }

        /// <summary>
        /// ids字符串转整型数组,如 "1,2,3" 转 int[]{1,2,3}
        /// </summary>
        /// <param name="ids">带分隔符的字符串</param>
        /// <param name="splitter">分隔符</param>
        /// <returns></returns>
        public static int[] ToIntArray(this string ids, char splitter = ',')
        {
            var result = new List<int>();

            if (!ids.IsNullOrEmpty())
            {
                string[] arr = ids.Split(splitter);
                foreach (string item in arr)
                {
                    if (int.TryParse(item, out int outValue))
                    {
                        result.Add(outValue);
                    }
                }
            }

            return result.ToArray();
        }

        #endregion

        #region Dictionary安全取值

        /// <summary>
        /// Dictionary安全取值
        /// </summary>
        /// <typeparam name="Tkey"></typeparam>
        /// <typeparam name="Tvalue"></typeparam>
        /// <param name="dict"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public static Tvalue GetSafeValue<Tkey, Tvalue>(this Dictionary<Tkey, Tvalue> dict, Tkey key)
        {
            if (dict.ContainsKey(key))
                return dict[key];

            return Activator.CreateInstance<Tvalue>(); // default(Tvalue);
        }

        #endregion

        #region SqlTypeToC#Type

        /// <summary>
        /// 数据库数据类型转c#数据类型
        /// </summary>
        /// <param name="sqlType"></param>
        /// <returns></returns>
        public static Type ToCsharpType(this SqlDbType sqlType)
        {
            switch (sqlType)
            {
                case SqlDbType.BigInt:
                    return typeof(Int64);
                case SqlDbType.Binary:
                    return typeof(Object);
                case SqlDbType.Bit:
                    return typeof(Boolean);
                case SqlDbType.Char:
                    return typeof(String);
                case SqlDbType.DateTime:
                    return typeof(DateTime);
                case SqlDbType.Decimal:
                    return typeof(Decimal);
                case SqlDbType.Float:
                    return typeof(Double);
                case SqlDbType.Image:
                    return typeof(Object);
                case SqlDbType.Int:
                    return typeof(Int32);
                case SqlDbType.Money:
                    return typeof(Decimal);
                case SqlDbType.NChar:
                    return typeof(String);
                case SqlDbType.NText:
                    return typeof(String);
                case SqlDbType.NVarChar:
                    return typeof(String);
                case SqlDbType.Real:
                    return typeof(Single);
                case SqlDbType.SmallDateTime:
                    return typeof(DateTime);
                case SqlDbType.SmallInt:
                    return typeof(Int16);
                case SqlDbType.SmallMoney:
                    return typeof(Decimal);
                case SqlDbType.Text:
                    return typeof(String);
                case SqlDbType.Timestamp:
                    return typeof(Object);
                case SqlDbType.TinyInt:
                    return typeof(Byte);
                case SqlDbType.Udt://自定义的数据类型
                    return typeof(Object);
                case SqlDbType.UniqueIdentifier:
                    return typeof(Object);
                case SqlDbType.VarBinary:
                    return typeof(Object);
                case SqlDbType.VarChar:
                    return typeof(String);
                case SqlDbType.Variant:
                    return typeof(Object);
                case SqlDbType.Xml:
                    return typeof(Object);
                default:
                    return null;
            }
        }

        #endregion
    }
}
