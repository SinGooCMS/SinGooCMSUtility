using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Collections.Generic;

namespace SinGooCMS.Utility.Extension
{
    /// <summary>
    /// linq扩展方法
    /// </summary>
    public static class LinqExtension
    {
        #region order by 字符串转 linq

        /// <summary>
        /// 多个OrderBy用逗号隔开，exp:"Sort asc,AutoID desc"
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="query"></param>
        /// <param name="orderBy"></param>
        /// <returns></returns>
        public static IEnumerable<T> OrderByBatch<T>(this IEnumerable<T> query, string orderBy)
        {
            var index = 0;
            var arr = orderBy.Split(',');
            foreach (var item in arr)
            {
                var m = index++ > 0 ? "ThenBy" : "OrderBy";
                if (item.ToLower().Contains("desc"))
                {
                    m += "Descending";
                    orderBy = item.Replace("desc", "").Replace("DESC", "").Replace("Desc", "");
                }
                else
                {
                    orderBy = item.Replace("asc", "").Replace("ASC", "").Replace("Asc", "");
                }
                orderBy = orderBy.Trim();

                var propInfo = GetPropertyInfo(typeof(T), orderBy);
                var expr = GetOrderExpression(typeof(T), propInfo);
                var method = typeof(Enumerable).GetMethods().FirstOrDefault(mt => mt.Name == m && mt.GetParameters().Length == 2);
                var genericMethod = method.MakeGenericMethod(typeof(T), propInfo.PropertyType);
                query = (IEnumerable<T>)genericMethod.Invoke(null, new object[] { query, expr.Compile() });
            }
            return query;
        }

        /// <summary>
        /// 多个OrderBy用逗号隔开，exp:"Sort asc,AutoID desc"
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="query"></param>
        /// <param name="orderBy"></param>
        /// <returns></returns>
        public static IQueryable<T> OrderByBatch<T>(this IQueryable<T> query, string orderBy)
        {
            var index = 0;
            var arr = orderBy.Split(',');
            foreach (var item in arr)
            {
                var m = index++ > 0 ? "ThenBy" : "OrderBy";
                if (item.ToLower().Contains("desc"))
                {
                    m += "Descending";
                    orderBy = item.Replace("desc", "").Replace("DESC", "").Replace("Desc", "");
                }
                else
                {
                    orderBy = item.Replace("asc", "").Replace("ASC", "").Replace("Asc", "");
                }
                orderBy = orderBy.Trim();

                var propInfo = GetPropertyInfo(typeof(T), orderBy);
                var expr = GetOrderExpression(typeof(T), propInfo);
                var method = typeof(Queryable).GetMethods().FirstOrDefault(mt => mt.Name == m && mt.GetParameters().Length == 2);
                var genericMethod = method.MakeGenericMethod(typeof(T), propInfo.PropertyType);
                query = (IQueryable<T>)genericMethod.Invoke(null, new object[] { query, expr });
            }
            return query;
        }        

        private static PropertyInfo GetPropertyInfo(Type objType, string name)
        {
            var properties = objType.GetProperties();
            var matchedProperty = properties.FirstOrDefault(p => p.Name == name);
            if (matchedProperty == null)
            {
                throw new ArgumentException("name");
            }

            return matchedProperty;
        }
        private static LambdaExpression GetOrderExpression(Type objType, PropertyInfo pi)
        {
            var paramExpr = Expression.Parameter(objType);
            var propAccess = Expression.PropertyOrField(paramExpr, pi.Name);
            var expr = Expression.Lambda(propAccess, paramExpr);
            return expr;
        }

        #endregion        
    }
}