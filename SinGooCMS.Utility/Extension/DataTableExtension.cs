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
        /// <summary>
        /// 获得一个去重的新表
        /// </summary>
        /// <param name="table">源表</param>
        /// <param name="arrFilter">新表字段</param>
        /// <returns></returns>
        public static DataTable ToDistinctTable(this DataTable table, string[] arrFilter)
        {
            var dv = table.AsDataView();
            return dv.ToTable(true, arrFilter);
        }

        #region DataTable转对象

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
        public static IEnumerable<T> ToEnumerable<T>(this DataTable table) where T : class, new()
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

        #endregion

        #region dt转arraylist

        /// <summary>
        /// dt转arraylist
        /// </summary>
        /// <param name="table"></param>
        /// <returns></returns>
        public static ArrayList ToArrayList(this DataTable table)
        {
            ArrayList array = new ArrayList();
            if (table != null && table.Rows.Count > 0)
            {
                for (int i = 0; i < table.Rows.Count; i++)
                {
                    DataRow row = table.Rows[i];
                    Hashtable record = new Hashtable();
                    for (int j = 0; j < table.Columns.Count; j++)
                    {
                        object cellValue = row[j];
                        if (cellValue.GetType() == typeof(DBNull))
                        {
                            cellValue = null;
                        }
                        record[table.Columns[j].ColumnName] = cellValue;
                    }
                    array.Add(record);
                }
            }

            return array;
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

        #region 列处理

        /// <summary>
        /// 更换列名
        /// </summary>
        /// <param name="table"></param>
        /// <param name="newColNames"></param>
        /// <returns></returns>
        public static DataTable ChangeColNames(this DataTable table, Dictionary<string, string> newColNames)
        {
            foreach (DataColumn col in table.Columns)
            {
                if (newColNames.Any(p => p.Key == col.ColumnName))
                {
                    col.ColumnName = newColNames[col.ColumnName]; //换成新的列名，比如说导出的excel列头要有意义的中文名
                }
            }

            return table;
        }

        #endregion

        #region datatable写入excel

        /// <summary>
        /// Datatable写入excel
        /// </summary>
        /// <param name="table"></param>
        /// <param name="excelFileFullPath"></param>
        /// <param name="sheetName"></param>
        public static void SaveToExcel(this DataTable table, string excelFileFullPath, string sheetName = "sheet1") =>
           NPOIUtils.Save(table, excelFileFullPath, sheetName);

        /// <summary>
        /// Datatable写入excel
        /// </summary>
        /// <param name="tables"></param>
        /// <param name="excelFileFullPath"></param>
        public static void SaveToExcel(this Dictionary<string, DataTable> tables, string excelFileFullPath) =>
            NPOIUtils.Save(tables, excelFileFullPath);

        #endregion

        #region datatable写入csv

        /// <summary>
        /// datatable写入csv
        /// </summary>
        /// <param name="table"></param>
        /// <param name="csvFileFullPath"></param>
        /// <param name="encoding"></param>
        public static void SaveToCsv(this DataTable table, string csvFileFullPath, string encoding = "utf-8") =>
            CsvUtils.Write(table, csvFileFullPath, encoding);

        /// <summary>
        /// datatable写入csv
        /// </summary>
        /// <param name="table"></param>
        /// <param name="csvFileFullPath"></param>
        /// <param name="encoding"></param>
        public static async Task SaveToCsvAsync(this DataTable table, string csvFileFullPath, string encoding = "utf-8") =>
            await CsvUtils.WriteAsync(table, csvFileFullPath, encoding);

        /// <summary>
        /// datatable写入csv
        /// </summary>
        /// <param name="table"></param>
        /// <param name="csvFileFullPath"></param>
        /// <param name="encoding"></param>
        public static void SaveToCsv<T>(this DataTable table, string csvFileFullPath, string encoding = "utf-8") where T : class, new()
        {
            CsvUtils.Write<T>(table.ToEnumerable<T>(), csvFileFullPath, encoding);
        }

        /// <summary>
        /// datatable写入csv
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="table"></param>
        /// <param name="csvFileFullPath"></param>
        /// <param name="encoding"></param>
        /// <returns></returns>
        public static async Task SaveToCsvAsync<T>(this DataTable table, string csvFileFullPath, string encoding = "utf-8") where T : class, new()
        {
            await CsvUtils.WriteAsync<T>(table.ToEnumerable<T>(), csvFileFullPath, encoding);
        }

        #endregion
    }
}
