using System;
using System.Text;
using NUnit.Framework;
using System.Data;
using SinGooCMS.Utility;
using SinGooCMS.Utility.Extension;
using System.Collections.Generic;

namespace CoreTest
{
    public class DateTableTest
    {
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

            var lst = dt.ToEntities<Student>();
            Assert.AreEqual("jsonlee", lst[0].UserName);

        }

        [Test]
        public void ObjToDt()
        {
            var lst = new List<Student>();
            lst.AddRange(new Student[] {
                new Student(){ UserName="jsonlee",Age=18 },
                new Student(){ UserName="刘备",Age=20}
            });

            var dt = lst.ToDataTable();
            Assert.AreEqual("刘备", dt.Rows[1]["UserName"]);
        }
    }
}
