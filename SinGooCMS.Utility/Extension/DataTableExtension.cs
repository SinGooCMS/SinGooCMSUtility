using System;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace SinGooCMS.Utility.Extension
{
    /// <summary>
    /// 数据集扩展
    /// </summary>
    public static class DataTableExtension
    {
        #region dt和对象之间互转

        /// <summary>
        /// DataTable转实体
        /// </summary>
        /// <typeparam name="T">实体</typeparam>
        /// <param name="table">DataTable实例</param>
        /// <returns></returns>
        public static T ToEntity<T>(this DataTable table) where T : new()
        {
            var entity = new T();
            foreach (DataRow row in table.Rows)
            {
                foreach (var item in entity.GetType().GetProperties())
                {
                    if (!row.Table.Columns.Contains(item.Name)) continue;
                    if (DBNull.Value == row[item.Name]) continue;
                    var newType = item.PropertyType;
                    //判断type类型是否为泛型，因为nullable是泛型类,
                    if (newType.IsGenericType && newType.GetGenericTypeDefinition() == typeof(Nullable<>))
                    {
                        //如果type为nullable类，声明一个NullableConverter类，该类提供从Nullable类到基础基元类型的转换
                        var nullableConverter = new System.ComponentModel.NullableConverter(newType);
                        //将type转换为nullable对的基础基元类型
                        newType = nullableConverter.UnderlyingType;
                    }

                    item.SetValue(entity, Convert.ChangeType(row[item.Name], newType), null);
                }
            }
            return entity;
        }

        /// <summary>
        ///  DataTable转实体集合
        /// </summary>
        /// <typeparam name="T">实体</typeparam>
        /// <param name="table">DataTable实例</param>
        /// <returns></returns>
        public static List<T> ToEntities<T>(this DataTable table) where T : new()
        {
            var entities = new List<T>();
            if (table == null)
                return null;
            foreach (DataRow row in table.Rows)
            {
                var entity = new T();
                foreach (var item in entity.GetType().GetProperties())
                {
                    if (!table.Columns.Contains(item.Name)) continue;
                    if (DBNull.Value == row[item.Name]) continue;
                    var newType = item.PropertyType;
                    //判断type类型是否为泛型，因为nullable是泛型类,
                    if (newType.IsGenericType && newType.GetGenericTypeDefinition() == typeof(Nullable<>))
                    {
                        //如果type为nullable类，声明一个NullableConverter类，该类提供从Nullable类到基础基元类型的转换
                        System.ComponentModel.NullableConverter nullableConverter = new System.ComponentModel.NullableConverter(newType);
                        //将type转换为nullable对的基础基元类型
                        newType = nullableConverter.UnderlyingType;
                    }
                    item.SetValue(entity, Convert.ChangeType(row[item.Name], newType), null);
                }
                entities.Add(entity);
            }
            return entities;
        }


        /// <summary>
        /// 指定集合转DataTable
        /// </summary>
        /// <param name="list">指定集合</param>
        /// <returns></returns>
        public static DataTable ToDataTable(this IList list)
        {
            var table = new DataTable();
            if (list.Count <= 0) return table;
            var propertys = list[0].GetType().GetProperties();
            foreach (var pi in propertys)
            {
                var pt = pi.PropertyType;
                if (pt.IsGenericType && (pt.GetGenericTypeDefinition() == typeof(Nullable<>)))
                {
                    pt = pt.GetGenericArguments()[0];
                }
                table.Columns.Add(new DataColumn(pi.Name, pt));
            }
            foreach (var item in list)
            {
                var tempList = new ArrayList();
                foreach (var pi in propertys)
                {
                    var obj = pi.GetValue(item, null);
                    tempList.Add(obj);
                }
                var array = tempList.ToArray();
                table.LoadDataRow(array, true);
            }
            return table;
        }

        /// <summary>
        /// 指定实体集合转DataTable
        /// </summary>
        /// <typeparam name="T">实体</typeparam>
        /// <param name="list">实体集合</param>
        /// <returns></returns>
        public static DataTable ToDataTable<T>(this List<T> list)
        {
            var table = new DataTable();
            //创建列头
            var propertys = typeof(T).GetProperties();
            foreach (var pi in propertys)
            {
                var pt = pi.PropertyType;
                if (pt.IsGenericType && (pt.GetGenericTypeDefinition() == typeof(Nullable<>)))
                {
                    pt = pt.GetGenericArguments()[0];
                }
                table.Columns.Add(new DataColumn(pi.Name, pt));
            }
            //创建数据行
            if (list.Count <= 0) return table;
            {
                foreach (var item in list)
                {
                    var tempList = new ArrayList();
                    foreach (var pi in propertys)
                    {
                        var obj = pi.GetValue(item, null);
                        tempList.Add(obj);
                    }
                    var array = tempList.ToArray();
                    table.LoadDataRow(array, true);
                }
            }
            return table;
        }

        #endregion

        #region 行处理        

        /// <summary>
        /// 查找数据表的行
        /// </summary>
        /// <param name="dt">数据表</param>
        /// <param name="colKey">单元列名</param>
        /// <param name="colValue">单元列值</param>
        /// <returns></returns>
        public static DataRow FindRow(this DataTable dt, string colKey, string colValue)
        {
            return dt != null && dt.Rows.Count > 0
                ? dt.AsEnumerable().Where(p => p.Field<string>(colKey).ToString().Equals(colValue)).FirstOrDefault()
                : null;
        }

        /// <summary>
        /// 是否存在行值
        /// </summary>
        /// <param name="dt"></param>
        /// <param name="colKey"></param>
        /// <param name="colValue"></param>
        /// <returns></returns>
        public static bool ExistsRow(this DataTable dt, string colKey, string colValue)
        {
            return dt.FindRow(colKey, colValue) != null;
        }

        /// <summary>
        /// 删除空白行
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        public static DataTable RemoveEmptyRow(this DataTable dt)
        {
            var lst = new List<DataRow>();
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                bool IsNull = true;
                for (int j = 0; j < dt.Columns.Count; j++)
                {
                    if (!string.IsNullOrEmpty(dt.Rows[i][j].ToString().Trim()))
                    {
                        IsNull = false;
                        break;
                    }
                }

                if (IsNull)
                    lst.Add(dt.Rows[i]);
            }

            for (int i = 0; i < lst.Count; i++)
            {
                dt.Rows.Remove(lst[i]);
            }

            return dt;
        }

        #endregion        
    }
}
