using System;
using System.Text;
using NUnit.Framework;
using System.Data;
using SinGooCMS.Utility;
using SinGooCMS.Utility.Extension;
using System.Collections.Generic;
using CoreTest.Locale;
using System.Globalization;

namespace CoreTest
{
    public class ResourceTest
    {
        [Test]
        public void TestField()
        {
            var model = new StudentInfo();
            model.UserName = "jsonlee";

            var type = typeof(Resource);
            var field = type.GetResString("Name", new CultureInfo("en-US"));

            Console.WriteLine(field);
        }
    }
}
