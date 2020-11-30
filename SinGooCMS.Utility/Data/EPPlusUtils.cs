using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using OfficeOpenXml;
using OfficeOpenXml.Drawing;
using SinGooCMS.Utility.Extension;

namespace SinGooCMS.Utility
{
    /// <summary>
    /// EPPlus操作excel工具类
    /// </summary>
    public class EPPlusUtils
    {
        static readonly string folder = $"/upload/{DateTime.Now:yyyy}/{DateTime.Now:MM}/";

        #region 读取

        /// <summary>
        /// 读取单元格的值
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="row">行</param>
        /// <param name="col">列</param>
        /// <param name="sheetName">表单名称，未指定默认第一个表单</param>
        /// <returns></returns>
        public static object ReadCell(string filePath, int row = 1, int col = 1, string sheetName = "")
        {
            FileInfo file = new FileInfo(filePath);
            ExcelPackage package = new ExcelPackage(file);

            ExcelWorksheet worksheet = null;
            if (!string.IsNullOrEmpty(sheetName))
                worksheet = package.Workbook.Worksheets[sheetName];
            else
                worksheet = package.Workbook.Worksheets[0];

            return worksheet.Cells[row, col].Value;
        }

        /// <summary>
        /// 读取excel到dt
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="sheetName"></param>
        /// <param name="startRowNum"></param>
        /// <param name="endRowNum"></param>
        /// <param name="isSavePic"></param>
        /// <param name="picSavePath"></param>
        /// <returns></returns>
        public static DataTable Read(string filePath, string sheetName = "", int startRowNum = 1, int endRowNum = 0, bool isSavePic = true, string picSavePath = "")
        {
            FileInfo file = new FileInfo(filePath);
            ExcelPackage package = new ExcelPackage(file);
            ExcelWorksheet worksheet = null;
            if (!string.IsNullOrEmpty(sheetName))
                worksheet = package.Workbook.Worksheets[sheetName];
            else
                worksheet = package.Workbook.Worksheets[0];

            return Read(worksheet, startRowNum, endRowNum, isSavePic, picSavePath);
        }

        /// <summary>
        /// 读取excel
        /// </summary>
        /// <param name="worksheet"></param>
        /// <param name="startRowNum">起始行（从表头算起）</param>
        /// <param name="endRowNum">结束行（不设置默认）</param>
        /// <param name="isSavePic">是否保存图片（保存到/update/年/月/ 目录下 如/upload/2020/06/）</param>
        /// <returns></returns>
        public static DataTable Read(ExcelWorksheet worksheet, int startRowNum = 1, int endRowNum = 0, bool isSavePic = true, string picSavePath = "")
        {
            //获取worksheet的行数
            int rows = worksheet.Dimension.End.Row;
            if (endRowNum > 0)
                rows = endRowNum;

            //获取worksheet的列数
            int cols = worksheet.Dimension.End.Column;

            DataTable dt = new DataTable(worksheet.Name);
            DataRow dr = null;
            for (int i = 1; i <= rows; i++)
            {
                if (i < startRowNum)
                    continue;

                //隐藏的行，跳过
                //if (worksheet.Row(i).Hidden)
                //    continue;

                if (i > startRowNum)
                    dr = dt.Rows.Add();

                for (int j = 1; j <= cols; j++)
                {
                    if (i == startRowNum) //datatable的标题
                        dt.Columns.Add(worksheet.Cells[i, j].Value == null ? "" : worksheet.Cells[i, j].Value.ToString());
                    else //剩下的写入datatable
                    {
                        var draws = worksheet.Drawings.Count > 0
                            ? worksheet.Drawings.Where(p => p.From.Row == i - 1 && p.From.Column == j - 1).ToList()
                            : null;

                        if (draws != null && draws.Count > 0) //单元格有图片，保存图片并把附件表ID存储在这里
                        {
                            dr[j - 1] = "";

                            var builder = new StringBuilder();
                            foreach (var item in draws)
                            {
                                if (item is ExcelPicture)
                                {
                                    var picFilePath = SavePicture((ExcelPicture)item, picSavePath);
                                    builder.Append("{\"pic\":\"" + picFilePath + "\"},");
                                }
                            }

                            string json = builder.ToString().TrimEnd(',');
                            if (isSavePic)
                            {
                                if (json.IndexOf(",") == -1)
                                    dr[j - 1] = json;
                                else
                                    dr[j - 1] = "[" + json + "]";
                            }
                        }
                        else
                        {
                            var fmt = worksheet.Cells[i, j].Style.Numberformat.Format;
                            var val = worksheet.Cells[i, j].Value;

                            if (val != null)
                            {
                                var type = val.GetType().ToString();
                                if (fmt == "m/d;@" && type == "System.Double")
                                {
                                    dr[j - 1] = worksheet.Cells[i, j].GetValue<DateTime>(); //日期格式
                                }
                                else if (fmt.IndexOf("%") != -1 && type == "System.Double")
                                {
                                    dr[j - 1] = worksheet.Cells[i, j].GetValue<decimal>() % 100; //百分比格式
                                }
                                else
                                    dr[j - 1] = val.ToString(); //其它格式保存为字符串
                            }
                            else
                                val = "";
                        }
                    }
                }
            }

            RemoveEmptyRow(ref dt); //清除空白行
            return dt;
        }

        /// <summary>
        /// 读取单元格的图片信息
        /// </summary>
        /// <param name="worksheet"></param>
        /// <param name="row"></param>
        /// <param name="col"></param>
        /// <returns></returns>
        public static IList<ExcelPicture> GetPictures(ExcelWorksheet worksheet, int row, int col)
        {
            IList<ExcelPicture> result = new List<ExcelPicture>();
            if (worksheet.Drawings.Count > 0)
            {
                var draws = worksheet.Drawings.Where(p => p.From.Row == row && p.From.Column == col);
                if (draws != null)
                {
                    foreach (var item in draws)
                    {
                        result.Add((ExcelPicture)item);
                    }
                }
            }

            return null;
        }

        #endregion

        #region 导出

        /// <summary>
        /// 导出excel
        /// </summary>
        /// <param name="dt"></param>
        /// <param name="filePath"></param>
        /// <param name="sheetName"></param>
        /// <param name="startPosition"></param>
        /// <param name="printHeader"></param>
        public static void Export(DataTable dt, string filePath, string sheetName = "Sheet1", string startPosition = "A1", bool printHeader = true)
        {
            FileInfo file = new FileInfo(filePath);
            using (ExcelPackage pack = new ExcelPackage(file))
            {
                ExcelWorksheet w = pack.Workbook.Worksheets[sheetName];
                if (w != null && w.Name.Equals(sheetName)) //判断是否存在该sheet表，存在则删除
                    pack.Workbook.Worksheets.Delete(w);
                ExcelWorksheet sheet = pack.Workbook.Worksheets.Add(sheetName);
                sheet.Cells[startPosition].LoadFromDataTable(dt, printHeader); //第二个参数设置为true则显示datable表头
                pack.Save();
            }
        }

        #endregion

        #region 数据检验

        /// <summary>
        /// 数据校验
        /// </summary>
        /// <param name="dt">数据表</param>
        /// <param name="filters">对照的字段</param>
        /// <param name="valRequired">是否必填</param>
        /// <param name="valLens">值长度</param>
        /// <param name="valTypes">值类型</param>
        /// <param name="limitRegion">单元格限定取值范围</param>
        /// <param name="groupKey">多个字段的唯一组</param>
        /// <returns></returns>
        public static string ValidateData(DataTable dt, string[] filters, string[] valRequired, int[] valLens, Type[] valTypes,
            Dictionary<string, string[]> limitRegion = null, string[] groupKey = null)
        {
            if (dt == null)
                return "数据集是空的";
            else if (dt.Rows.Count == 0)
                return "数据集是空的";
            else
            {
                //清除空白行
                RemoveEmptyRow(ref dt);

                //1、检查结构是否完整
                //string[] filters = { "线长工号", "线长姓名", "线别", "效率百分比", "日期", "备注" };
                for (int i = 0; i < dt.Columns.Count; i++)
                {
                    if (dt.Columns[i].ColumnName != filters[i])
                    {
                        return "缺少字段[" + filters[i] + "]或者未按导入模板字段排列顺序";
                    }
                }

                //2、检查数据格式是否正确            
                //string[] needVal = { "NotNull", "NotNull", "NotNull", "NotNull", "NotNull", "Null" };
                //int[] rowLens = { 50, 50, 50, -1, -1, 100 };
                //Type[] types = { typeof(String), typeof(String), typeof(String), typeof(Decimal), typeof(DateTime), typeof(String) };
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    var row = dt.Rows[i];
                    for (var j = 0; j < dt.Columns.Count; j++)
                    {
                        if (valRequired[j] == "NotNull" && (row[j] == DBNull.Value || row[j].ToString().Trim().Length == 0))
                            return string.Format("字段[{0}]在第 {1} 行数据的值不能为空", filters[j], i + 1);
                        else if (valLens[j] > -1 && row[j].ToString().Trim().Length > valLens[j])
                            return string.Format("字段[{0}]在第 {1} 行数据的长度超过 {2}", filters[j], i + 1, valLens[j]);
                        else
                        {
                            if (valTypes[j] == typeof(Decimal) && !row[j].ToString().IsDecimal())
                                return string.Format("字段[{0}]在第 {1} 行数据的值格式不正确，应为数值格式", filters[j], i + 1);
                            else if (valTypes[j] == typeof(DateTime) && !row[j].ToString().IsDate())
                                return string.Format("字段[{0}]在第 {1} 行数据的值格式不正确，应为日期格式", filters[j], i + 1);
                            else if (valTypes[j] == typeof(Int32) && !row[j].ToString().IsInt())
                                return string.Format("字段[{0}]在第 {1} 行数据的值格式不正确，应为整数格式", filters[j], i + 1);
                        }

                        //检查限定值
                        if (limitRegion != null && limitRegion.Count > 0)
                        {
                            foreach (KeyValuePair<string, string[]> item in limitRegion)
                            {
                                string colName = dt.Columns[j].ColumnName;
                                if (colName == item.Key
                                    && !item.Value.Contains(dt.Rows[i][colName].ToString()))
                                    return string.Format("数据列[{0}]限制取值范围[{1}]", colName, string.Join(",", item.Value));
                            }
                        }
                    }
                }

                //3、判断是否有重复数据
                //string[] groupKey = { "线长工号", "线长姓名","日期" };
                if (groupKey != null)
                {
                    DataView dv = new DataView(dt);
                    if (dv.ToTable(true, groupKey).Rows.Count < dt.Rows.Count)
                        return string.Format("组[{0}]存在重复数据", String.Join(",", groupKey));
                }
            }

            return "success";
        }

        #endregion

        /// <summary>
        /// 将DataTable转换为标准的CSV标准格式
        /// </summary>
        /// <param name="table"></param>
        /// <returns></returns>
        public static string DataTableToCsvFormat(DataTable table)
        {
            //以半角逗号（即,）作分隔符，列为空也要表达其存在。  
            //列内容如存在半角逗号（即,）则用半角引号（即""）将该字段值包含起来。  
            //列内容如存在半角引号（即"）则应替换成半角双引号（""）转义，并用半角引号（即""）将该字段值包含起来。  
            StringBuilder sb = new StringBuilder();
            DataColumn colum;
            foreach (DataRow row in table.Rows)
            {
                for (int i = 0; i < table.Columns.Count; i++)
                {
                    colum = table.Columns[i];
                    if (i != 0) sb.Append(",");
                    if (colum.DataType == typeof(string) && row[colum].ToString().Contains(","))
                    {
                        sb.Append("\"" + row[colum].ToString().Replace("\"", "\"\"") + "\"");
                    }
                    else sb.Append(row[colum].ToString());
                }
                sb.AppendLine();
            }
            return sb.ToString();
        }

        #region helper

        /// <summary>
        /// 保存图片并返回路径
        /// </summary>
        /// <param name="pic">excel图片</param>
        /// <param name="picSavePath">图片保存的路径</param>
        /// <returns></returns>
        private static string SavePicture(ExcelPicture pic, string picSavePath = "")
        {
            /*
            ExcelDrawing 包括 图片名字、说明、和位置信息，位置即在单元格里的位置，
            worksheet.Drawings[1].From
            {OfficeOpenXml.Drawing.ExcelPosition}
            Column: 12
            ColumnOff: 105833
            Row: 2
            RowOff: 148166

            ExcelDrawing 是指画在单元格里的图片 包括位置信息
            ExcelPicture 是指独立于表单的图片信息，包括图片流信息
            */
            string filename = (picSavePath.Length == 0 ? folder : picSavePath) + StringUtils.GetNewFileName() + "." + pic.ImageFormat.ToString().ToLower();
            string physicFilePath = SystemUtils.GetMapPath(filename);
            pic.Image.Save(physicFilePath);
            if (File.Exists(physicFilePath))
            {
                return filename;
            }

            return "";
        }

        /// <summary>
        /// 清除空白行，空白行即整行都是空字符串的
        /// </summary>
        /// <param name="dt"></param>
        private static void RemoveEmptyRow(ref DataTable dt)
        {
            List<DataRow> lst = new List<DataRow>();
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
        }

        #endregion
    }
}
