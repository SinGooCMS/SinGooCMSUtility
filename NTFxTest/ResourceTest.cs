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
using NTFxTest.Locale;
using System.Globalization;

namespace NTFxTest
{
    [TestClass]
    public class ResourceTest
    {
        [TestMethod]
        public void TestField()
        {
            var model = new StudentInfo();
            model.UserName = "jsonlee";

            var type = typeof(Resource);
            var field = type.GetResString("Name",new CultureInfo("en-US"));

            Console.WriteLine(field);
        }
    }
}
