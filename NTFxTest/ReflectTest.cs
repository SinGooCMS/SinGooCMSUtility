using System;
using System.Data;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SinGooCMS.Utility.Extension;
using SinGooCMS.Utility;
using System.ComponentModel.DataAnnotations.Schema;
using SinGooCMS.Ado.Interface;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace NTFxTest
{
    [TestClass]
    public class ReflectTest
    {
        readonly string connStr = "server=(local);database=TestDB;uid=sa;pwd=123;";
        readonly string path = SystemUtils.GetMapPath("/SinGooCMS.Ado.dll");
        readonly string className = "SinGooCMS.Ado.DbAccess.SqlServerAccess";

        [TestMethod]
        public void TestInstance()
        {
            //创建实例
            var dbAccess = (IDbAccess)ReflectionUtils.CreateInstance(path, className, new object[] { connStr });
            var model = dbAccess.Find<StudentInfo>(7);
            Assert.AreEqual("赵云", model?.UserName);
        }

        [TestMethod]
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

        [TestMethod]
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

        [TestMethod]
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
