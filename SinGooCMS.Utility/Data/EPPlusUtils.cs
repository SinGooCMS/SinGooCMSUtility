using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using OfficeOpenXml;
using OfficeOpenXml.Drawing;
using OfficeOpenXml.Style;
using SinGooCMS.Utility.Extension;

namespace SinGooCMS.Utility
{
    /// <summary>
    /// EPPlus操作excel工具类
    /// </summary>
    public sealed class EPPlusUtils
    {
        #region 读取

        /// <summary>
        /// 读取单元格的值
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="address"></param>
        /// <returns></returns>
        public static object ReadCell(string filePath, string address = "A1") => ReadCell(filePath, "", address);

        /// <summary>
        /// 读取单元格的值
        /// </summary>
        /// <param name="filePath">excel文件路径</param>
        /// <param name="sheetName">工作表名</param>
        /// <param name="address">地址</param>
        /// <returns></returns>
        public static object ReadCell(string filePath, string sheetName = "", string address = "A1")
        {
            FileInfo file = new FileInfo(filePath);
            using (ExcelPackage package = new ExcelPackage(file))
            {
                ExcelWorksheet worksheet = sheetName.IsNullOrEmpty()
                ? package.Workbook.Worksheets[0]
                : package.Workbook.Worksheets[sheetName];

                return worksheet.Cells[address].Value;
            }
        }

        /// <summary>
        /// 读取单元格的值
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="row"></param>
        /// <param name="col"></param>
        /// <returns></returns>
        public static object ReadCell(string filePath, int row = 1, int col = 1) => ReadCell(filePath, "", row, col);

        /// <summary>
        /// 读取单元格的值
        /// </summary>
        /// <param name="filePath">excel文件路径</param>
        /// <param name="sheetName">表单名称，未指定默认第一个表单</param>
        /// <param name="row">行</param>
        /// <param name="col">列</param>        
        /// <returns></returns>
        public static object ReadCell(string filePath, string sheetName = "", int row = 1, int col = 1)
        {
            FileInfo file = new FileInfo(filePath);
            using (ExcelPackage package = new ExcelPackage(file))
            {
                ExcelWorksheet worksheet = sheetName.IsNullOrEmpty()
                ? package.Workbook.Worksheets[0]
                : package.Workbook.Worksheets[sheetName];

                return worksheet.Cells[row, col].Value;
            }
        }

        /// <summary>
        /// 读取excel到dt,图片保存到文件夹：/upload/epplus/
        /// </summary>
        /// <param name="filePath">excel文件路径</param>
        /// <param name="sheetName">工作表名</param>
        /// <param name="startRowNum">指定起始行</param>
        /// <param name="endRowNum">指定结束行</param>
        /// <param name="isSavePic">是否保存图片</param>
        /// <param name="picSavePath">图片保存目录</param>
        /// <returns></returns>
        public static DataTable Read(string filePath, string sheetName = "", int startRowNum = 1, int endRowNum = 0, bool isSavePic = true, string picSavePath = "/upload/epplus/")
        {
            FileInfo file = new FileInfo(filePath);
            using (ExcelPackage package = new ExcelPackage(file))
            {
                ExcelWorksheet worksheet = sheetName.IsNullOrEmpty()
                ? package.Workbook.Worksheets[0]
                : package.Workbook.Worksheets[sheetName];

                return Read(worksheet, startRowNum, endRowNum, isSavePic, picSavePath);
            }
        }

        /// <summary>
        /// 读取excel,图片保存到文件夹：/upload/epplus/
        /// </summary>
        /// <param name="worksheet"></param>
        /// <param name="startRowNum">起始行（从表头算起）</param>
        /// <param name="endRowNum">结束行（不设置默认）</param>
        /// <param name="isSavePic">是否保存图片（保存到/update/年/月/ 目录下 如/upload/2020/06/）</param>
        /// <param name="picSavePath">图片保存目录</param>
        /// <returns></returns>
        public static DataTable Read(ExcelWorksheet worksheet, int startRowNum = 1, int endRowNum = 0, bool isSavePic = true, string picSavePath = "/upload/epplus/")
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
                                if (val.GetType() == typeof(System.DateTime))
                                    dr[j - 1] = worksheet.Cells[i, j].GetValue<DateTime>(); //日期格式
                                else if (val.GetType() == typeof(System.Double) && fmt.IndexOf("yyyy") != -1)
                                    dr[j - 1] = worksheet.Cells[i, j].GetValue<DateTime>(); //日期格式
                                else if (val.GetType() == typeof(System.Double) && fmt.IndexOf("%") != -1)
                                    dr[j - 1] = worksheet.Cells[i, j].GetValue<decimal>() % 100; //百分比格式
                                else
                                    dr[j - 1] = val.ToString(); //其它格式保存为字符串
                            }
                            else
                                val = "";
                        }
                    }
                }
            }

            return dt.RemoveEmptyRow(); //清除空白行并返回
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
        /// <param name="dt">数据表</param>
        /// <param name="filePath">导入的文件路径</param>
        /// <param name="sheetName">工作表名</param>
        /// <param name="startPosition">在工作表里写入的起始位置</param>
        /// <param name="dateFormat">日期格式，指定多个格式如：yyyy-mm-dd,yyyy-mm-dd HH:mm:ss</param>
        /// <param name="printHeader">是否输出标题</param>
        /// <param name="isSetStyle">是否设置样式</param>
        /// <param name="headerHeight">标题行高 默认26</param>
        /// <param name="headerFontSize">标题字体大小 默认12</param>
        /// <param name="headerBgColor">标题的背景颜色</param>
        public static void Export(DataTable dt, string filePath, string sheetName = "Sheet1", string startPosition = "A1", string dateFormat = "yyyy-mm-dd", bool printHeader = true, bool isSetStyle = true, int headerHeight = 26, int headerFontSize = 12, Color? headerBgColor = null)
        {
            if (File.Exists(filePath))
                File.Delete(filePath); //删除已有的旧文件

            FileInfo file = new FileInfo(filePath);
            using (ExcelPackage pack = new ExcelPackage(file))
            {
                //加入工作表
                ExcelWorksheet sheet = pack.Workbook.Worksheets.Add(sheetName);
                //从起始位置导入dt数据集
                sheet.Cells[startPosition].LoadFromDataTable(dt, printHeader); //第二个参数设置为true则显示datable表头 
                var address = sheet.Cells[startPosition].Start; //起始的位置

                //遍历工作表的列
                int counter = 0;
                string[] arrDateFormat = dateFormat.Split(',');
                for (var i = 0; i < dt.Columns.Count; i++)
                {
                    if (dt.Columns[i].DataType == typeof(DateTime))
                    {
                        if (arrDateFormat.Length > 0)
                        {
                            //指定了多个格式，按顺序赋日期格式
                            sheet.Column(address.Column + i).Style.Numberformat.Format =
                                arrDateFormat.Length > counter
                                ? arrDateFormat[counter]
                                : arrDateFormat[arrDateFormat.Length - 1];
                        }
                        else
                            sheet.Column(address.Column + i).Style.Numberformat.Format = dateFormat; //指定日期格式，否则只是数字

                        counter++;
                    }

                    //设置表头样式
                    if (printHeader && isSetStyle)
                    {
                        if (headerBgColor == null)
                            headerBgColor = Color.FromArgb(0, 155, 70);
                        sheet.Row(address.Row).Height = headerHeight;
                        sheet.Cells[address.Row, address.Column + i].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;//水平居中
                        sheet.Cells[address.Row, address.Column + i].Style.VerticalAlignment = ExcelVerticalAlignment.Center;//垂直居中
                        sheet.Cells[address.Row, address.Column + i].Style.Font.Bold = true;//字体为粗体
                        sheet.Cells[address.Row, address.Column + i].Style.Font.Color.SetColor(Color.White);//字体颜色
                        sheet.Cells[address.Row, address.Column + i].Style.Font.Size = headerFontSize;//字体大小
                        sheet.Cells[address.Row, address.Column + i].Style.Fill.PatternType = ExcelFillStyle.Solid;
                        sheet.Cells[address.Row, address.Column + i].Style.Fill.BackgroundColor.SetColor(headerBgColor.Value);//设置单元格背景色
                    }
                }

                sheet.Cells[sheet.Dimension.Address].AutoFitColumns(); //自动列宽
                pack.Save();
            }
        }

        #endregion        

        #region helper

        /// <summary>
        /// 保存图片并返回路径
        /// </summary>
        /// <param name="pic">excel图片</param>
        /// <param name="picSavePath">图片保存的路径</param>
        /// <returns></returns>
        private static string SavePicture(ExcelPicture pic, string picSavePath = "/upload/epplus/")
        {
            //目录的绝对路径
            string absoluteSavePath = SystemUtils.GetMapPath(picSavePath);
            if (!Directory.Exists(absoluteSavePath))
                Directory.CreateDirectory(absoluteSavePath);

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
            string filename = FileUtils.Combine(picSavePath, StringUtils.GetNewFileName() + "." + pic.ImageFormat.ToString().ToLower());
            string physicFilePath = SystemUtils.GetMapPath(filename);
            pic.Image.Save(physicFilePath);
            if (File.Exists(physicFilePath))
            {
                return filename;
            }

            return "";
        }

        #endregion
    }
}
