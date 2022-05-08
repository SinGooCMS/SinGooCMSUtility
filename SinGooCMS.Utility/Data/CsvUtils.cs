using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CsvHelper;
using CsvHelper.Configuration;

namespace SinGooCMS.Utility
{
    /// <summary>
    /// csv工具
    /// </summary>
    public class CsvUtils
    {
        #region Read

        /// <summary>
        /// 读csv到对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="csvFilePath"></param>
        /// <param name="encoding"></param>
        /// <returns></returns>
        public static IList<T> Read<T>(string csvFilePath, string encoding = "utf-8")
        {
            if (File.Exists(csvFilePath))
            {
#if NETSTANDARD2_1
                Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
#endif
                using (var reader = new StreamReader(csvFilePath, Encoding.GetEncoding(encoding)))
                {
                    using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
                    {
                        return csv.GetRecords<T>()?.ToList();
                    }
                }
            }

            return null;
        }

        /// <summary>
        /// 读csv到datatable
        /// </summary>
        /// <param name="csvFilePath">csv文件路径，包括文件名</param>
        /// <param name="encoding">编码，默认utf-8</param>
        /// <returns></returns>
        public static DataTable Read(string csvFilePath, string encoding = "utf-8")
        {
            if (File.Exists(csvFilePath))
            {
                var csvData = new DataTable();

#if NETSTANDARD2_1
                Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
#endif
                using (var reader = new StreamReader(csvFilePath, Encoding.GetEncoding(encoding)))
                {
                    using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
                    {
                        // Do any configuration to `CsvReader` before creating CsvDataReader.
                        using (var dr = new CsvDataReader(csv))
                        {
                            csvData.Load(dr);
                        }
                    }
                }

                return csvData;
            }

            return null;
        }

        #endregion

        #region Write

        /// <summary>
        /// 写csv文件
        /// </summary>
        /// <param name="dt">DataTable</param>
        /// <param name="csvFilePath">物理路径</param>
        /// <param name="encoding">编码</param>
        public static void Write(DataTable dt, string csvFilePath, string encoding = "utf-8")
        {
            var builder = new StringBuilder();
            for (var i = 0; i < dt.Columns.Count; i++)
            {
                builder.Append(dt.Columns[i].ColumnName);
                if (i < dt.Columns.Count - 1)
                    builder.Append(",");
            }

            string colNames = builder.ToString();

#if NETSTANDARD2_1
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
#endif
            StreamWriter strmWriterObj = new StreamWriter(csvFilePath, false, Encoding.GetEncoding(encoding));

            strmWriterObj.WriteLine(colNames);
            string strBufferLine = "";
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                strBufferLine = "";
                for (int j = 0; j < dt.Columns.Count; j++)
                {
                    if (j > 0)
                        strBufferLine += ",";
                    strBufferLine += dt.Rows[i][j].ToString();
                }
                strmWriterObj.WriteLine(strBufferLine);
            }
            strmWriterObj.Close();
        }

        /// <summary>
        /// 写csv文件
        /// </summary>
        /// <param name="dt">DataTable</param>
        /// <param name="csvFilePath">物理路径</param>
        /// <param name="encoding">编码</param>
        public static async Task WriteAsync(DataTable dt, string csvFilePath, string encoding = "utf-8")
        {
            var builder = new StringBuilder();
            for (var i = 0; i < dt.Columns.Count; i++)
            {
                builder.Append(dt.Columns[i].ColumnName);
                if (i < dt.Columns.Count - 1)
                    builder.Append(",");
            }

            string colNames = builder.ToString();
            
#if NETSTANDARD2_1
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
#endif
            StreamWriter strmWriterObj = new StreamWriter(csvFilePath, false, Encoding.GetEncoding(encoding));
            
            await strmWriterObj.WriteLineAsync(colNames);
            string strBufferLine = "";
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                strBufferLine = "";
                for (int j = 0; j < dt.Columns.Count; j++)
                {
                    if (j > 0)
                        strBufferLine += ",";
                    strBufferLine += dt.Rows[i][j].ToString();
                }
                await strmWriterObj.WriteLineAsync(strBufferLine);
            }
            strmWriterObj.Close();
        }

        /// <summary>
        /// 写csv文件
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="csvLst"></param>
        /// <param name="csvFilePath"></param>        
        /// <param name="encoding"></param>
        public static void Write<T>(IEnumerable<T> csvLst, string csvFilePath, string encoding = "utf-8")
        {
#if NETSTANDARD2_1
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
#endif
            using (var writer = new StreamWriter(csvFilePath, false, Encoding.GetEncoding(encoding)))
            {
                var cfg = new CsvConfiguration(CultureInfo.InvariantCulture) { Encoding = Encoding.GetEncoding(encoding) };
                using (var csv = new CsvWriter(writer, cfg))
                {
                    csv.WriteRecords(csvLst);
                    csv.Flush();
                }
            }
        }

        /// <summary>
        /// 异步写csv文件
        /// </summary>
        /// <typeparam name="T"></typeparam>        
        /// <param name="csvLst"></param>
        /// <param name="csvFilePath"></param>
        /// <param name="encoding"></param>
        public async static Task WriteAsync<T>(IEnumerable<T> csvLst, string csvFilePath, string encoding = "utf-8")
        {
#if NETSTANDARD2_1
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
#endif
            using (var writer = new StreamWriter(csvFilePath, false, Encoding.GetEncoding(encoding)))
            {
                var cfg = new CsvConfiguration(CultureInfo.InvariantCulture) { Encoding = Encoding.GetEncoding(encoding) };
                using (var csv = new CsvWriter(writer, cfg))
                {
                    await csv.WriteRecordsAsync(csvLst);
                    await csv.FlushAsync();
                }
            }
        }

        #endregion
    }
}
