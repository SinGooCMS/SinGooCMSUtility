using System;
using System.Linq;
using System.Reflection;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SinGooCMS.Utility
{
    /// <summary>
    /// 实体类 特性工具
    /// </summary>
    public sealed class EntityAttrUtils
    {
        /// <summary>
        /// 表名
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static string GetTableName(Type type)
        {
            var tableAttr = type.GetCustomAttributes(false).Where(attr => attr.GetType().Name == "TableAttribute").SingleOrDefault() as dynamic;
            return tableAttr?.Name ?? string.Empty;
        }

        /// <summary>
        /// 读取主键名称 - 只支持单主键
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static string GetKey(Type type)
        {
            var pi = type.GetProperties().Where(p => p.GetCustomAttributes(true).Any(a => a is KeyAttribute)).FirstOrDefault();
            return pi?.Name ?? string.Empty;
        }

        /// <summary>
        /// 读取主键值 - 只支持单主键
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="entity"></param>
        /// <returns></returns>
        public static object GetKeyValue<TEntity>(TEntity entity) where TEntity : class
        {
            var pi = typeof(TEntity).GetProperties()
                .Where(p => p.GetCustomAttributes(true)
                .Any(a => a is KeyAttribute))
                .FirstOrDefault();

            return pi?.GetValue(entity, null);
        }

        /// <summary>
        /// 是否主键 - 只支持单主键（主键ID是自增的）
        /// </summary>
        /// <param name="pi"></param>
        /// <returns></returns>
        public static bool IsKey(PropertyInfo pi)
        {
            return pi.GetCustomAttributes(true).Any(a => a is KeyAttribute);
        }

        /// <summary>
        /// 是否映射
        /// </summary>
        /// <param name="pi"></param>
        /// <returns></returns>
        public static bool IsNotMapped(PropertyInfo pi)
        {
            return pi.GetCustomAttributes(true).Any(a => a is NotMappedAttribute);
        }

        /// <summary>
        /// 读取实体的属性
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="PropertyName"></param>
        /// <returns></returns>
        public static PropertyInfo GetProperty<TEntity>(string PropertyName)
        {
            return typeof(TEntity).GetProperty(PropertyName);
            //return typeof(TEntity).GetProperties().Where(p => p.Name == PropertyName).FirstOrDefault();
        }

        /// <summary>
        /// 最大值特性
        /// </summary>
        /// <param name="pi"></param>
        /// <returns></returns>
        public static int GetMaxLength(PropertyInfo pi)
        {
            var attrMax = pi.GetCustomAttribute(typeof(MaxLengthAttribute)) as MaxLengthAttribute;
            //var attrMax = pi.GetCustomAttributes(true).Where(a => a is MaxLengthAttribute).FirstOrDefault() as MaxLengthAttribute;
            return attrMax?.Length ?? 0;
        }

        /// <summary>
        /// 最小值特性
        /// </summary>
        /// <param name="pi"></param>
        /// <returns></returns>
        public static int GetMinLength(PropertyInfo pi)
        {
            var attrMin = pi.GetCustomAttribute(typeof(MinLengthAttribute)) as MinLengthAttribute;
            //var attrMin = pi.GetCustomAttributes(true).Where(a => a is MinLengthAttribute).FirstOrDefault() as MinLengthAttribute;
            return attrMin?.Length ?? 0;
        }
    }
}
