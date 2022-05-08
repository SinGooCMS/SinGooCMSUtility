using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.SS.Util;
using NPOI.XSSF.UserModel;
using SinGooCMS.Utility.Extension;

namespace SinGooCMS.Utility
{
    /// <summary>
    /// NPOI工具类<br/>
    /// 来自: <seealso href="https://github.com/nissl-lab/npoi"/>
    /// </summary>
    public class NPOIUtils
    {
        #region Read

        /// <summary>
        /// 读取单元格的值
        /// </summary>
        /// <param name="excelFileFullPath"></param>
        /// <param name="sheetName"></param>
        /// <param name="addr">位置 如A1</param>
        /// <returns></returns>
        public static object ReadCell(string excelFileFullPath, string sheetName = "", string addr = "A1")
        {
            using (FileStream stream = new FileStream(excelFileFullPath, FileMode.Open, FileAccess.Read))
            {
                IWorkbook workbook = WorkbookFactory.Create(stream);
                ISheet sheet = !string.IsNullOrEmpty(sheetName)
                    ? workbook.GetSheet(sheetName)
                    : workbook.GetSheetAt(0);

                var cellAddr = new CellAddress(addr);
                var row = sheet.GetRow(cellAddr.Row);

                if (row != null)
                {
                    var cell = row.GetCell(cellAddr.Column);
                    return cell == null ? null : GetCellValue(cell);
                }

                return null;
            }
        }

        /// <summary>
        /// 读取单元格的值
        /// </summary>
        /// <param name="excelFileFullPath">excel文件路径</param>
        /// <param name="sheetName">表单名称，未指定默认第一个表单</param>
        /// <param name="rowIndex">行</param>
        /// <param name="colIndex">列</param>        
        /// <returns></returns>
        public static object ReadCell(string excelFileFullPath, string sheetName = "", int rowIndex = 1, int colIndex = 1)
        {
            using (FileStream stream = new FileStream(excelFileFullPath, FileMode.Open, FileAccess.Read))
            {
                IWorkbook workbook = WorkbookFactory.Create(stream);
                ISheet sheet = !string.IsNullOrEmpty(sheetName)
                    ? workbook.GetSheet(sheetName)
                    : workbook.GetSheetAt(0);

                var row = sheet.GetRow(rowIndex);

                if (row != null)
                {
                    var cell = row.GetCell(colIndex);
                    return cell == null ? null : GetCellValue(cell);
                }

                return null;
            }
        }

        /// <summary>
        /// 读取第一个表单
        /// </summary>
        /// <param name="excelFileFullPath"></param>
        /// <returns></returns>
        public static DataTable Read(string excelFileFullPath)
        {
            var dict = Read(excelFileFullPath, true);
            return dict != null && dict.Count > 0 ? dict.GetValue(0) : null;
        }

        /// <summary>
        /// 读所有表单
        /// </summary>
        /// <param name="excelFileFullPath"></param>
        /// <param name="hasHeaderTile"></param>
        /// <param name="startPoint"></param>
        /// <param name="endPoint"></param>
        /// <returns></returns>
        public static Dictionary<string, DataTable> Read(string excelFileFullPath, bool hasHeaderTile, (int startRow, int startCol)? startPoint = null, (int endRow, int endCol)? endPoint = null)
        {
            var dict = new Dictionary<string, DataTable>();
            using (FileStream stream = new FileStream(excelFileFullPath, FileMode.Open, FileAccess.Read))
            {
                //HSSFWorkbook hssfworkbook = new HSSFWorkbook(stream);
                //XSSFWorkbook hssfworkbook = new XSSFWorkbook(stream);
                IWorkbook workbook = WorkbookFactory.Create(stream);
                for (var i = 0; i < workbook.NumberOfSheets; i++)
                {
                    ISheet sheet = workbook.GetSheetAt(i);
                    if (sheet == null) continue;

                    dict.Add(sheet.SheetName, ReadSheet(sheet, hasHeaderTile, startPoint, endPoint));
                }

                return dict;
            }
        }

        /// <summary>
        /// 读取表单
        /// </summary>
        /// <param name="sheet"></param>
        /// <param name="hasHeaderTile"></param>
        /// <param name="startPoint"></param>
        /// <param name="endPoint"></param>
        /// <returns></returns>
        public static DataTable ReadSheet(ISheet sheet, bool hasHeaderTile, (int startRow, int startCol)? startPoint = null, (int endRow, int endCol)? endPoint = null)
        {
            //第一行，一般来说是标题行
            IRow headerRow = sheet.GetRow(startPoint?.startRow ?? 0);
            if (headerRow.Cells.Count <= 0)
                return null;

            int startCol = startPoint?.startCol ?? 0;
            int endCol = endPoint?.endCol ?? headerRow.Cells.Count - 1;
            if (endCol < startCol)
                return null;

            var dt = new DataTable();
            for (var a = startCol; a <= endCol; a++)
            {
                //创建标题行
                if (hasHeaderTile)
                {
                    dt.Columns.Add(headerRow.Cells[a].StringCellValue);
                }
                else
                {
                    dt.Columns.Add($"SingooCol{a}"); //自动列名
                }
            }

            //填充数据行
            int startRow = hasHeaderTile
                ? startPoint?.startRow ?? 0 + 1
                : startPoint?.startRow ?? 0;
            int endRow = endPoint?.endRow ?? sheet.LastRowNum;

            for (var b = startRow; b <= endRow; b++)
            {
                var row = sheet.GetRow(b);
                if (row == null) continue;

                DataRow dataRow = dt.NewRow();
                int colIndex = 0;
                for (var c = startCol; c <= endCol; c++)
                {
                    var cell = row.GetCell(c);
                    if (cell == null)
                        dataRow[colIndex] = "";
                    else
                    {
                        switch (cell.CellType)
                        {
                            case CellType.Boolean:
                                dataRow[colIndex] = cell.BooleanCellValue;
                                break;
                            case CellType.Numeric:
                                if (cell.CellStyle.DataFormat != 0)
                                    dataRow[colIndex] = cell.DateCellValue;
                                else
                                    dataRow[colIndex] = cell.NumericCellValue;
                                break;
                            case CellType.String:
                            default:
                                dataRow[colIndex] = cell.StringCellValue;
                                break;
                        }
                    }

                    colIndex++;
                }

                dt.Rows.Add(dataRow);
            }

            return dt;
        }

        /// <summary>
        /// 读表单
        /// </summary>
        /// <param name="excelFileFullPath"></param>
        /// <param name="hasHeaderTile"></param>
        /// <param name="addrRange">地址范围，如 A1:C2</param>
        /// <returns></returns>
        public static DataTable ReadSheet(string excelFileFullPath, bool hasHeaderTile, string addrRange)
        {
            using (FileStream stream = new FileStream(excelFileFullPath, FileMode.Open, FileAccess.Read))
            {
                IWorkbook workbook = WorkbookFactory.Create(stream);
                ISheet sheet = workbook.GetSheetAt(0);
                return ReadSheet(sheet, hasHeaderTile, addrRange);
            }
        }

        /// <summary>
        /// 读表单
        /// </summary>
        /// <param name="sheet"></param>
        /// <param name="hasHeaderTile"></param>
        /// <param name="addrRange">地址范围，如 A1:C2</param>
        /// <returns></returns>
        public static DataTable ReadSheet(ISheet sheet, bool hasHeaderTile, string addrRange)
        {
            var cellRange = CellRangeAddress.ValueOf(addrRange);
            if (cellRange == null) return null;

            //第一行，一般来说是标题行
            IRow headerRow = sheet.GetRow(0);
            if (headerRow.Cells.Count <= 0)
                return null;

            int firstRow = cellRange.FirstRow;
            if (hasHeaderTile)
                firstRow += 1; //有标题的话，第一行是标题，数据行从第二行开始

            var dt = new DataTable();
            for (var a = cellRange.FirstColumn; a <= cellRange.LastColumn; a++)
            {
                //创建标题行
                if (hasHeaderTile)
                {
                    dt.Columns.Add(headerRow.Cells[a].StringCellValue);
                }
                else
                {
                    dt.Columns.Add($"SingooCol{a}"); //自动列名
                }
            }

            for (var i = firstRow; i <= cellRange.LastRow; i++)
            {
                var row = sheet.GetRow(i);
                if (row == null) continue;

                DataRow dataRow = dt.NewRow();
                int colIndex = 0;
                for (var j = cellRange.FirstColumn; j <= cellRange.LastColumn; j++)
                {
                    var cell = row.GetCell(j);
                    if (cell == null)
                        dataRow[colIndex] = "";
                    else
                    {
                        switch (cell.CellType)
                        {
                            case CellType.Boolean:
                                dataRow[colIndex] = cell.BooleanCellValue;
                                break;
                            case CellType.Numeric:
                                if (cell.CellStyle.DataFormat != 0)
                                    dataRow[colIndex] = cell.DateCellValue;
                                else
                                    dataRow[colIndex] = cell.NumericCellValue;
                                break;
                            case CellType.String:
                            default:
                                dataRow[colIndex] = cell.StringCellValue;
                                break;
                        }
                    }

                    colIndex++;
                }

                dt.Rows.Add(dataRow);
            }

            return dt;
        }

        /// <summary>
        /// 获取单元格的值
        /// </summary>
        /// <param name="cell"></param>
        /// <returns></returns>
        public static object GetCellValue(ICell cell)
        {
            if (cell != null)
            {
                switch (cell.CellType)
                {
                    case CellType.Boolean:
                        return cell.BooleanCellValue;
                    case CellType.Numeric:
                        if (cell.CellStyle.DataFormat != 0)
                            return cell.DateCellValue;
                        else
                            return cell.NumericCellValue;
                    case CellType.String:
                    default:
                        return cell.StringCellValue;
                }
            }

            return "";
        }

        #endregion

        #region Write

        /// <summary>
        /// 保存到excel
        /// </summary>
        /// <param name="table"></param>
        /// <param name="filePath"></param>
        /// <param name="sheetName"></param>
        /// <returns></returns>
        public static bool Save(DataTable table, string filePath, string sheetName = "")
        {
            if (string.IsNullOrEmpty(sheetName))
                sheetName = "Sheet1";

            var dict = new Dictionary<string, DataTable>();
            dict.Add(sheetName, table);

            return Save(dict, filePath);
        }

        /// <summary>
        /// 保存到excel
        /// </summary>
        /// <param name="dict"></param>
        /// <param name="saveFilePath"></param>
        /// <returns></returns>
        public static bool Save(Dictionary<string, DataTable> dict, string saveFilePath)
        {
            IWorkbook workbook = null;
            IRow rowTitle = null;
            IRow rowData = null;
            ISheet sheet = null;
            ICell cell = null;
            DataTable dt = null;

            try
            {
                string ext = System.IO.Path.GetExtension(saveFilePath).ToLower();

                if (ext.Equals(".xlsx"))
                    workbook = new XSSFWorkbook();
                else
                    workbook = new HSSFWorkbook();

                //数据单元格样式
                NPOI.SS.UserModel.ICellStyle style = workbook.CreateCellStyle();
                style.BorderBottom = NPOI.SS.UserModel.BorderStyle.Thin;
                style.BorderLeft = NPOI.SS.UserModel.BorderStyle.Thin;
                style.BorderRight = NPOI.SS.UserModel.BorderStyle.Thin;
                style.BorderTop = NPOI.SS.UserModel.BorderStyle.Thin;
                style.Alignment = HorizontalAlignment.Left;                

                //标题样式
                NPOI.SS.UserModel.ICellStyle styleHeader = workbook.CreateCellStyle();
                styleHeader.CloneStyleFrom(style);//克隆公共的样式
                styleHeader.Alignment = HorizontalAlignment.Left;//单元格靠左
                styleHeader.VerticalAlignment = VerticalAlignment.Center;
                IFont fontHeader = workbook.CreateFont();//新建一个字体样式对象
                fontHeader.Color = NPOI.HSSF.Util.HSSFColor.White.Index;
                fontHeader.IsBold = true; //设置字体加粗样式
                styleHeader.SetFont(fontHeader);//使用SetFont方法将字体样式添加到单元格样式中
                //styleHeader.FillForegroundColor = NPOI.HSSF.Util.HSSFColor.Yellow.Index;
                styleHeader.FillPattern = FillPattern.SolidForeground;

                styleHeader.FillForegroundColor = 0;
                styleHeader.FillPattern = FillPattern.SolidForeground;
                ((XSSFColor)styleHeader.FillForegroundColorColor).SetRgb(new byte[] { 80, 130, 190 });

                int counter = 1;
                string sheetName = "";
                foreach (KeyValuePair<string, DataTable> kvp in dict)
                {
                    dt = kvp.Value;
                    sheetName = kvp.Key;

                    if (sheetName.Length > 30)
                        sheetName = sheetName.Substring(0, 30);

                    sheet = workbook.CreateSheet(sheetName);
                    sheet.DefaultRowHeight = 20 * 20;

                    //设置列头  
                    rowTitle = sheet.CreateRow(0);//excel第一行设为列头  
                    rowTitle.Height = 16 * 20;
                    for (int i = 0; i < dt.Columns.Count; i++)
                    {
                        cell = rowTitle.CreateCell(i);
                        cell.CellStyle = styleHeader;
                        cell.SetCellValue(dt.Columns[i].ColumnName);
                    }

                    //设置每行每列的单元格,  
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        rowData = sheet.CreateRow(i + 1); //excel第二行开始写入数据  
                        for (int j = 0; j < dt.Columns.Count; j++)
                        {
                            cell = rowData.CreateCell(j);
                            cell.CellStyle = style;
                            cell.SetCellValue(dt.Rows[i][j].ToString());
                        }
                    }

                    //自动调节行宽度
                    for (int i = 0; i < dt.Columns.Count; i++)
                    {
                        sheet.AutoSizeColumn(i);
                    }

                    //CellRangeAddress c = CellRangeAddress.ValueOf("A1");
                    //sheet.SetAutoFilter(c);

                    counter++;
                }

                using (var fs = File.OpenWrite(saveFilePath))
                {
                    workbook.Write(fs);//写入文件 
                    return true;
                }
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// 根据模板写入excel
        /// </summary>
        /// <param name="dict"></param>
        /// <param name="tmplFilePath"></param>
        /// <param name="saveFilePath"></param>
        /// <returns></returns>
        public static bool SaveWithTmpl(Dictionary<string, DataTable> dict, string tmplFilePath, string saveFilePath)
        {
            IWorkbook workbook = null;
            IRow rowData = null;
            ISheet sheet = null;
            ICell cell = null;
            DataTable dt = null;

            try
            {
                if (!File.Exists(tmplFilePath))
                    throw new Exception("模板文件不存在");

                workbook = WorkbookFactory.Create(tmplFilePath);

                int counter = 0;
                string sheetName = "";
                foreach (KeyValuePair<string, DataTable> kvp in dict)
                {
                    dt = kvp.Value;
                    sheetName = kvp.Key;

                    if (sheetName.Length > 30)
                        sheetName = sheetName.Substring(0, 30);

                    sheet = workbook.GetSheetAt(counter);
                    var headerRow = sheet.GetRow(0); //第一行是标题行
                    var colCount = headerRow.Cells.Count;
                    string titleVal = string.Empty;

                    //设置每行每列的单元格,  
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        rowData = sheet.CreateRow(i + 1); //excel第二行开始写入数据  
                        for (int j = 0; j < colCount; j++)
                        {
                            cell = rowData.CreateCell(j);

                            //标题值
                            titleVal = headerRow.Cells[j].StringCellValue; //标题行是字符串
                            if (string.IsNullOrEmpty(titleVal))
                                continue;

                            var dataColumn = dt.Columns[titleVal];
                            if (dataColumn == null) continue;

                            var colVal = dt.Rows[i][titleVal];
                            cell.SetCellValue(colVal.ToString());
                        }
                    }

                    counter++;
                }

                using (var fs = File.OpenWrite(saveFilePath))
                {
                    workbook.Write(fs);//写入文件 
                    return true;
                }
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        #endregion
    }
}
