using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data;
using System.Text;
using NUnit.Framework;
using SinGooCMS.Ado.Interface;
using SinGooCMS.Utility;
using SinGooCMS.Utility.Extension;

namespace CoreTest
{
    public class ReflectTest
    {
        readonly string connStr = "server=(local);database=TestDB;uid=sa;pwd=123;";
        readonly string path = SystemUtils.GetMapPath("/SinGooCMS.Ado.dll");
        readonly string className = "SinGooCMS.Ado.DbAccess.SqlServerAccess";

        [Test]
        public void TestInstance()
        {
            //创建实例
            var dbAccess = (IDbAccess)ReflectionUtils.CreateInstance(path, className, new object[] { connStr });
            var model = dbAccess.Find<StudentInfo>(7);
            Assert.AreEqual("赵云", model?.UserName);
        }

        [Test]
        public void TestInvokeMethod()
        {
            var instance = ReflectionUtils.CreateInstance(path, className, new object[] { connStr });
            //普通方法
            var dt = (DataTable)instance.InvokeMethod("GetPagerDT", null, new object[] { "Student", "", "AutoID desc", 1, 100, "*" });
            Console.WriteLine($"行数：{dt.Rows.Count}");

            var instance2 = ReflectionUtils.CreateInstance(path, className, new object[] { connStr });
            //泛型方法
            var model = (StudentInfo)instance2.InvokeMethod("Find", new Type[] { typeof(StudentInfo) }, new object[] { 1 });
            Assert.AreEqual("刘备", model?.UserName);
        }

        [Test]
        public void TestProperty()
        {
            var model = new StudentInfo() { AutoID = 1, UserName = "jsonlee" };
            var builder = new StringBuilder();
            builder.Append($"读取属性：{model.GetProperty("UserName").Name} \r\n");
            builder.Append($"读取属性值：{model.GetPropertyVal<string>("UserName")} \r\n");

            //设置属性值
            model.SetPropertyVal("UserName", "刘备");
            builder.Append($"读取属性值：{model.UserName} \r\n");
            Console.WriteLine(builder.ToString());
        }

        [Test]
        public void TestField()
        {
            var model = new StudentInfo();
            var builder = new StringBuilder();
            builder.Append($"读取字段：{model.GetField("score").Name} \r\n");
            builder.Append($"读取字段值：{model.GetFieldVal<int>("score")} \r\n");

            model.SetFieldVal("score", 99);
            builder.Append($"读取字段值：{model.score} \r\n");
            Console.WriteLine(builder.ToString());
        }
    }
}
