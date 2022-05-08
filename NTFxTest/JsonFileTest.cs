using System;
using System.Data;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SinGooCMS.Utility.Extension;
using SinGooCMS.Utility;
using System.Text;
using System.IO;
using System.Threading.Tasks;

namespace NTFxTest
{
    [TestClass]
    public class JsonFileTest
    {
        [TestMethod]
        public void TestWrite()
        {
            var student = new StudentInfo()
            {
                UserName = "jasonlee",
                Age = 18
            };

            string filePath = SystemUtils.GetMapPath("TestSource/jsonfile1.json");
            JsonFileUtils.Save(student, filePath);
            Assert.IsTrue(File.Exists(filePath));

            var students = new List<StudentInfo>()
            {
                new StudentInfo(){
                    UserName = "jasonlee",
                    Age = 18
                },
                new StudentInfo(){
                    UserName = "刘备",
                    Age = 38
                },
            };

            string filePath2 = SystemUtils.GetMapPath("TestSource/jsonfile2.json");
            JsonFileUtils.Save(students, filePath2);
            Assert.IsTrue(File.Exists(filePath2));
        }

        [TestMethod]
        public void TestRead()
        {
            string filePath = SystemUtils.GetMapPath("TestSource/jsonfile1.json");
            var model = JsonFileUtils.Read<StudentInfo>(filePath);
            Assert.IsTrue(model!=null);

            string filePath2 = SystemUtils.GetMapPath("TestSource/jsonfile2.json");
            var lst = JsonFileUtils.ReadList<StudentInfo>(filePath2);
            Assert.IsTrue(lst != null && lst.Count>0);
        }
    }
}
