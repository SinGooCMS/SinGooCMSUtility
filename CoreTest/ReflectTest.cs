using System;
using System.Text;
using NUnit.Framework;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using SinGooCMS.Utility;
using SinGooCMS.Utility.Extension;
using SinGooCMS.Ado;
using SinGooCMS.Ado.Interface;

namespace CoreTest
{
    public class ReflectTest
    {
        readonly string connStr = "server=(local);database=TestDB;uid=sa;pwd=123;";
        readonly string path = AppDomain.CurrentDomain.BaseDirectory + "SinGooCMS.Ado.dll";
        readonly string className = "SinGooCMS.Ado.DbAccess.SqlServerAccess";

        [Test]
        public void TestInstance()
        {
            IDbAccess dbAccess = (IDbAccess)ReflectionUtil.CreateInstance(path, className, new object[] { connStr });
            var model = dbAccess.Find<DbMaintenanceTestInfo>(1);
            Assert.AreEqual("jsonlee", model?.UserName);
        }

        [Test]
        public void TestInvokeMethod()
        {
            //var instance = ReflectionUtil.CreateInstance(path, "SinGooCMS.Ado.DbAccess.DbAccessBase", new object[] { connStr,SinGooCMS.Ado.DbProviderType.SqlServer });

            //普通方法
            //int count = (int)instance.InvokeMethod("GetCount", null, new object[] { "DbMaintenanceTest","" });
            //Assert.AreEqual(1099998, count);

            var instance = ReflectionUtil.CreateInstance(path, className, new object[] { connStr });
            //泛型方法
            var model = (DbMaintenanceTestInfo)instance.InvokeMethod("Find", new Type[] { typeof(DbMaintenanceTestInfo) }, new object[] { 1 });
            Assert.AreEqual("jsonlee", model?.UserName);
        }

        [Test]
        public void TestProperty()
        {
            var test = new DbMaintenanceTestInfo() { AutoID = 1, UserName = "jsonlee" };
            var userName = test.GetProperty<string>("UserName");
            test.SetProperty("UserName", "刘备");
            Assert.AreEqual("刘备", test.UserName);
        }

        [Test]
        public void TestField()
        {
            var test = new DbMaintenanceTestInfo() { AutoID = 1, UserName = "jsonlee" };
            var fields = test.GetFields();
            var val = test.GetField<string>("uname");
            test.SetField("uname", "张飞");

            Assert.AreEqual("张飞", test.uname);
        }
    }

    [Table("DbMaintenanceTest")]
    public class DbMaintenanceTestInfo
    {
        private int id = 0;
        public string uname = "jsonlee"; //private 读不到

        [Key]
        [NotMapped]
        public int AutoID { get; set; }
        public string UserName { get; set; }
    }
}
