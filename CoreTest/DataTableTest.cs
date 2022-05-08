using System;
using System.Text;
using NUnit.Framework;
using System.Data;
using SinGooCMS.Utility;
using SinGooCMS.Utility.Extension;
using System.Collections.Generic;
using System.Linq;

namespace CoreTest
{
    public class DataTableTest
    {
        [Test]
        public void ObjToDt()
        {
            var lst = new StudentInfo[] {
                new StudentInfo(){ UserName="jasonlee",Age=18 },
                new StudentInfo(){ UserName="刘备",Age=20},
                new StudentInfo(){ UserName="刘备",Age=30}
            };

            //var dt = lst.ToDataTable();
            var dict = new Dictionary<string, string>();
            dict.Add("UserName", "姓名");

            var dt = lst.ToDataTable(); //默认转换
            Assert.AreEqual("刘备", dt.Rows[1]["UserName"]);

            var dt1 = lst.ToDataTable(dict); //指定转换，并改变列名
            Assert.AreEqual("刘备", dt1.Rows[1]["姓名"]);

            var dt2 = lst.ToDataTable().ToDistinctTable(new string[] { "UserName" }); //去重
            Assert.AreEqual(2, dt2.Rows.Count);

            //转arraylist
            var arr = dt.ToArrayList();
            Assert.AreEqual(true, arr.Count > 0);

            //改列名
            var dict2 = new Dictionary<string, string>();
            dict2.Add("姓名", "User_Name");
            var dt4 = dt1.ChangeColNames(dict2);
            Assert.AreEqual("刘备", dt4.Rows[1]["User_Name"]);

            //写入excel
            var file1 = SystemUtils.GetMapPath("/TestSource/students.xlsx");
            dt.SaveToExcel(file1);
            Assert.AreEqual(true, System.IO.File.Exists(file1));

            //写入csv
            var file2 = SystemUtils.GetMapPath("/TestSource/students.csv");
            dt.SaveToCsvAsync(file2).GetAwaiter().GetResult();
            Assert.AreEqual(true, System.IO.File.Exists(file2));
        }

        [Test]
        public void DtToObject()
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("UserName", typeof(string));
            dt.Columns.Add("Age", typeof(int));

            DataRow dr1 = dt.NewRow();
            dr1["UserName"] = "jsonlee";
            dr1["Age"] = 18;
            dt.Rows.Add(dr1);

            DataRow dr2 = dt.NewRow();
            dr2["UserName"] = "刘备";
            dr2["Age"] = 20;
            dt.Rows.Add(dr2);

            var lst = dt.ToEnumerable<StudentInfo>().ToList();
            Assert.AreEqual("jsonlee", lst[0].UserName);

        }
    }
}
