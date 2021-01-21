using System;
using System.IO;
using System.Text;
using System.Xml.Serialization;

namespace SinGooCMS.Utility.Extension
{
    /// <summary>
    /// xml序列化扩展
    /// </summary>
    public static class XmlserializeExtension
    {
        #region 序列化

        /// <summary>
        /// 序列化
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="t"></param>
        /// <returns></returns>
        public static string ToXml<T>(this T t)
        {
            XmlSerializerNamespaces namespaces = new XmlSerializerNamespaces();
            namespaces.Add(string.Empty, string.Empty);
            XmlSerializer serializer = new XmlSerializer(typeof(T));
            using (StringUTF8Writer writer = new StringUTF8Writer(new StringBuilder()))
            {
                serializer.Serialize(writer, t, namespaces);
                return writer.ToString();
            }
        }

        #endregion

        #region 反序列化

        /// <summary>
        /// 反序列化
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="strXml"></param>
        /// <returns></returns>
        public static T XmlToObject<T>(this string strXml)
        {
            T local = default;
            if (!string.IsNullOrEmpty(strXml))
            {
                try
                {
                    XmlSerializer serializer = new XmlSerializer(typeof(T));
                    TextReader textReader = new StringReader(strXml);
                    local = (T)serializer.Deserialize(textReader);
                    textReader.Close();
                    return local;
                }
                catch (InvalidOperationException)
                {
                    return default;
                }
            }

            return default;
        }

        #endregion

    }

    /// <summary>
    /// 转xml时编码为UTF8
    /// </summary>
    internal class StringUTF8Writer : System.IO.StringWriter
    {
        public StringUTF8Writer(StringBuilder builder)
            : base(builder)
        {
            //调用基类
        }
        public override Encoding Encoding
        {
            get { return Encoding.UTF8; }
        }
    }
}
