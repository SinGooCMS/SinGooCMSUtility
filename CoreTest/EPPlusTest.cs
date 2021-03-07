using System;
using System.Text;
using System.Data;
using NUnit.Framework;
using SinGooCMS.Utility;

namespace CoreTest
{
    public class EPPlusTest
    {
        [Test]
        public void ReadExcel()
        {
            var excelFilePath = SystemUtils.GetMapPath("TestSource/通讯录.xlsx");
            var dt = EPPlusUtils.Read(excelFilePath);
            Assert.AreEqual(true, dt.Rows.Count > 0);
            Assert.AreEqual("Jsonlee", EPPlusUtils.ReadCell(excelFilePath, "B2"));
        }

        [Test]
        public void Export2Excel()
        {
            var dt = new DataTable();
            dt.Columns.Add("AutoID", typeof(int));
            dt.Columns.Add("UserName", typeof(string));
            dt.Columns.Add("Birthday", typeof(DateTime));
            dt.Columns.Add("RegTime", typeof(DateTime));

            var row1 = dt.NewRow();
            row1["AutoID"] = 1;
            row1["UserName"] = "jsonlee";
            row1["Birthday"] = new DateTime(1999, 9, 9);
            row1["RegTime"] = DateTime.Now;
            dt.Rows.Add(row1);

            var row2 = dt.NewRow();
            row2["AutoID"] = 2;
            row2["UserName"] = "刘备";
            row2["Birthday"] = new DateTime(2020, 9, 9);
            row2["RegTime"] = DateTime.Now;
            dt.Rows.Add(row2);

            //只输出日期
            EPPlusUtils.Export(dt, SystemUtils.GetMapPath("/TestSource/EPPlusTest1.xlsx"), "Sheet1", "E3", "yyyy-mm-dd");
            //分别输出日期和时间
            EPPlusUtils.Export(dt, SystemUtils.GetMapPath("/TestSource/EPPlusTest2.xlsx"), "Sheet1", "E3", "yyyy-mm-dd,yyyy-mm-dd HH:mm:ss");
        }
    }
}
