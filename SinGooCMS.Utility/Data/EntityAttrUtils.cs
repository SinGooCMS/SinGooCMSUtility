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
    public class EntityAttrUtils
    {
        /// <summary>
        /// 表名
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static string GetTableName(Type type)
        {
            var tableAttr = type.GetCustomAttributes(false).Where(attr => attr.GetType().Name == "TableAttribute").SingleOrDefault() as dynamic;
            if (tableAttr != null)
                return tableAttr.Name;

            return "";
        }

        /// <summary>
        /// 读取主键 - 只支持单主键（主键ID是自增的）
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static string GetKey(Type type)
        {
            var pi = type.GetProperties().Where(p => p.GetCustomAttributes(true).Any(a => a is KeyAttribute)).FirstOrDefault();
            return pi == null ? "" : pi.Name;
        }

        /// <summary>
        /// 读取主键值 - 只支持单主键（主键ID是自增的）
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="entity"></param>
        /// <returns></returns>
        public static object GetKeyValue<TEntity>(TEntity entity) where TEntity : class
        {
            var type = typeof(TEntity);
            var pi = type.GetProperties().Where(p => p.GetCustomAttributes(true).Any(a => a is KeyAttribute)).FirstOrDefault();
            if (pi != null)
                return pi.GetValue(entity, null);

            return null;
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
    }
}
