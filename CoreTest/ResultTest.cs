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
    public class ResultTest
    {
        [Test]
        public void TestOperateResult()
        {
            //API返回结果
            var apiResult = OperationResult.success;
            Console.WriteLine(apiResult.ToString());

            var apiResult2 = new OperationResult(1, "200", "Ok", "Ok")
            {
                Timeout = 5 * 60, //5分钟有效
                //Sign = "abc" //指定的签名
            };
            Console.WriteLine(apiResult2.ToString());

            //解密签名 
            var decodeSign = DEncryptUtils.DESDecrypt(apiResult2.Sign, $"singoo{apiResult2.TimeStamp.ToString("yyyyMMdd")}");
            var arr = decodeSign.Split('&');
            /*
            验签
            1）匹配ReturnStatusType
            2）匹配StatusCode
            3）匹配TimeStamp
            4）TimeStamp加上Timeout未过期，即仍然有时效性
             */
            var isValid = arr.Length == 4
                && arr[0].Split('=')[1] == apiResult2.ReturnStatusType.ToString()
                && arr[1].Split('=')[1] == apiResult2.StatusCode
                && Convert.ToDateTime(arr[2].Split('=')[1]).ToString("yyyy-MM-dd HH:mm:ss") == apiResult2.TimeStamp.ToString("yyyy-MM-dd HH:mm:ss")
                && Convert.ToDateTime(arr[2].Split('=')[1]).AddSeconds(Convert.ToDouble(arr[3].Split('=')[1])) > DateTime.Now;

            Assert.IsTrue(isValid);
        }
    }
}
