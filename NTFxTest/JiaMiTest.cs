using System;
using System.Data;
using SinGooCMS.Utility.Extension;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SinGooCMS.Utility;

namespace NTFxTest
{
    [TestClass]
    public class JiaMiTest
    {
        string key = "jsonlee@singoo.top";
        string txt = "123456";

        [TestMethod]
        public void TestRevEnc()
        {
            //可逆加密
            //des
            string desEncode = DEncryptUtils.DESEncrypt(txt, key);
            Console.WriteLine("des加密值：" + desEncode);
            Assert.AreEqual(txt, DEncryptUtils.DESDecrypt(desEncode, key));

            //aes
            string aesEncode = DEncryptUtils.AESEncrypt(txt);
            Console.WriteLine("aes加密值：" + aesEncode);
            Assert.AreEqual(txt, DEncryptUtils.AESDecrypt(aesEncode));

            //base64
            string base64Encode = DEncryptUtils.Base64Encrypt(txt);
            Console.WriteLine("base64加密值：" + aesEncode);
            Assert.AreEqual(txt, DEncryptUtils.Base64Decrypt(base64Encode));

            //xor 
            string xor = DEncryptUtils.GetXORCode(txt, key);
            Console.WriteLine("xor加密值：" + xor);
            Assert.AreEqual(txt, DEncryptUtils.GetXORCode(xor, key));

        }

        [TestMethod]
        public void TestUnRevEnc()
        {
            //不可逆加密
            //md5
            string md5 = DEncryptUtils.MD5Encrypt(txt); //md5是2次加密，且会截断字符串，这样网上的暴力破解不了
            Console.WriteLine("MD5:" + md5);
            Assert.AreEqual(md5, DEncryptUtils.MD5Encrypt(txt));

            //sha512 比md5更安全的不可逆加密方法
            string sha512 = DEncryptUtils.SHA512Encrypt(txt);
            Console.WriteLine("sha512:" + sha512);
            Assert.AreEqual(sha512, DEncryptUtils.SHA512Encrypt(txt));
        }

        [TestMethod]
        public void TestRSA()
        {
            //RAS的公钥和私钥是分别保存的，保证了安全

            //公钥
            string publicKey = "";
            //私钥
            string privateKey = "";

            RSACryption rsa = new RSACryption();
            //生成公钥和私钥
            rsa.RSAKey(out privateKey, out publicKey);
            Console.WriteLine($"公钥：{publicKey} 私钥：{privateKey}");

            //利用公钥加密
            string encode = rsa.RSAEncrypt(publicKey, txt);
            Console.WriteLine($"加密后的值：{encode}");
            //利用私钥解密
            string decode = rsa.RSADecrypt(privateKey, encode);
            Console.WriteLine($"解密后的值：{decode}");
            //判断
            Assert.AreEqual(txt, decode);
        }
    }
}
