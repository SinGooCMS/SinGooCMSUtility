using System;
using System.Text;
using NUnit.Framework;
using System.Data;
using SinGooCMS.Utility;
using SinGooCMS.Utility.Extension;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CoreTest
{
    public class CsvTest
    {
        [Test]
        public void ReadTest1()
        {
            var dt = CsvUtils.Read(SystemUtils.GetMapPath("/TestSource/工位.csv"), "gb2312");
            Assert.IsNotNull(dt);
        }

        [Test]
        public void ReadTest2()
        {
            var lst = CsvUtils.Read<Demo>(SystemUtils.GetMapPath("/TestSource/工位.csv"), "gb2312");
            Assert.IsNotNull(lst);
        }

        [Test]
        public void WriteTest()
        {
            var lst = new List<Demo>();
            lst.Add(new Demo() { 工位名称 = "镭雕" });
            lst.Add(new Demo() { 工位名称 = "拉头" });

            CsvUtils.Write<Demo>(lst, SystemUtils.GetMapPath("/TestSource/工位.csv"), "gb2312");
        }

        [Test]
        public void WriteTest2()
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("工位ID", typeof(int));
            dt.Columns.Add("工位名称", typeof(string));
            DataRow dr1 = dt.NewRow();
            dr1["工位ID"] = "1";
            dr1["工位名称"] = "镭雕";
            dt.Rows.Add(dr1);

            DataRow dr2 = dt.NewRow();
            dr2["工位ID"] = "1";
            dr2["工位名称"] = "外观检测";
            dt.Rows.Add(dr2);

            //写入csv
            dt.SaveToCsvAsync(SystemUtils.GetMapPath("/TestSource/工位.csv"), "gb2312").GetAwaiter().GetResult();
            //dt.SaveToCsv<Demo>(SystemUtils.GetMapPath("/TestSource/工位.csv"), "gb2312");
            //dt.SaveToCsvAsync<Demo>(SystemUtils.GetMapPath("/TestSource/工位.csv"), "gb2312").GetAwaiter().GetResult();
        }

        [Test]
        public async Task WriteTest3()
        {
            var lst = new List<Demo>();
            lst.Add(new Demo() { 工位名称 = "镭雕" });
            lst.Add(new Demo() { 工位名称 = "拉头" });

            await CsvUtils.WriteAsync<Demo>(lst, SystemUtils.GetMapPath("/TestSource/工位.csv"), "gb2312");
        }
    }

    public class Demo
    {
        public string 工位名称 { get; set; }
    }
}
