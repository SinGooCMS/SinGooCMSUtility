using System;
using System.Data;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SinGooCMS.Utility.Extension;
using SinGooCMS.Utility;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace NTFxTest
{
    [TestClass]
    public class NPOITest
    {
        [TestMethod]
        public void TestRead()
        {
            var excelFilePath = SystemUtils.GetMapPath("TestSource/通讯录.xlsx");

            //读取单元格
            var cellValue1 = NPOIUtils.ReadCell(excelFilePath, "Sheet1", 3, 1);
            Assert.AreEqual("张飞", cellValue1);

            var cellValue2 = NPOIUtils.ReadCell(excelFilePath, "Sheet1", "B4");
            Assert.AreEqual("张飞", cellValue1);

            var dict = NPOIUtils.Read(excelFilePath, true, (0, 0), (3, 1));
            var dt = dict.GetValue(0);
            Assert.AreEqual(true, dt!=null && dt.Rows.Count>0);

            var dt2 = NPOIUtils.ReadSheet(excelFilePath, true, "A1:C2");
            Assert.AreEqual(true, dt != null && dt.Rows.Count > 0);
        }

        [TestMethod]
        public void TestWrite()
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

            DataTable dt2 = new DataTable();
            dt2.Columns.Add("姓名", typeof(string));
            dt2.Columns.Add("年龄", typeof(int));

            DataRow dr3 = dt2.NewRow();
            dr3["姓名"] = "吕布";
            dr3["年龄"] = 18;
            dt2.Rows.Add(dr3);

            var dict=new Dictionary<string, DataTable>();
            dict.Add("三国武将1", dt);
            dict.Add("三国武将2",dt2);

            var result1 = NPOIUtils.Save(dict, SystemUtils.GetMapPath("TestSource/a.xlsx"));
            Assert.AreEqual(true, result1);

            var result2 = NPOIUtils.SaveWithTmpl(dict, SystemUtils.GetMapPath("TestSource/template.xlsx"), SystemUtils.GetMapPath("TestSource/template_a.xlsx"));
            Assert.AreEqual(true, result2);
        }
    }
}
