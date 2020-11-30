using System;
using System.IO;
using System.Xml;
using System.Data;

namespace SinGooCMS.Utility
{
    /// <summary>
    /// xml工具
    /// </summary>
    public class XmlUtils
    {
        #region 字段定义
        /// <summary>
        /// XML文件的物理路径
        /// </summary>
        private string _filePath = string.Empty;
        /// <summary>
        /// Xml文档
        /// </summary>
        private XmlDocument _xml;
        /// <summary>
        /// XML的根节点
        /// </summary>
        private XmlElement _element;
        #endregion

        #region 构造方法
        /// <summary>
        /// 实例化XmlHelper对象
        /// </summary>
        /// <param name="xmlFilePath">Xml文件的相对路径</param>
        public XmlUtils(string xmlFilePath)
        {
            //获取XML文件的绝对路径
            _filePath = SystemUtils.GetMapPath(xmlFilePath);
        }
        #endregion

        #region 创建XML的根节点
        /// <summary>
        /// 创建XML的根节点
        /// </summary>
        private void CreateXMLElement()
        {
            //创建一个XML对象
            _xml = new XmlDocument();

            if (File.Exists(_filePath))
            {
                //加载XML文件
                _xml.Load(this._filePath);
            }

            //为XML的根节点赋值
            _element = _xml.DocumentElement;
        }
        #endregion

        #region 获取指定XPath表达式的节点对象
        /// <summary>
        /// 获取指定XPath表达式的节点对象
        /// </summary>        
        /// <param name="xPath">XPath表达式,
        /// 范例1: @"Skill/First/SkillItem", 等效于 @"//Skill/First/SkillItem"
        /// 范例2: @"Table[USERNAME='a']" , []表示筛选,USERNAME是Table下的一个子节点.
        /// 范例3: @"ApplyPost/Item[@itemName='岗位编号']",@itemName是Item节点的属性.
        /// </param>
        public XmlNode GetNode(string xPath)
        {
            //创建XML的根节点
            CreateXMLElement();

            //返回XPath节点
            return _element.SelectSingleNode(xPath);
        }
        #endregion

        #region 获取指定XPath表达式节点的值
        /// <summary>
        /// 获取指定XPath表达式节点的值
        /// </summary>
        /// <param name="xPath">XPath表达式,
        /// 范例1: @"Skill/First/SkillItem", 等效于 @"//Skill/First/SkillItem"
        /// 范例2: @"Table[USERNAME='a']" , []表示筛选,USERNAME是Table下的一个子节点.
        /// 范例3: @"ApplyPost/Item[@itemName='岗位编号']",@itemName是Item节点的属性.
        /// </param>
        public string GetValue(string xPath)
        {
            //创建XML的根节点
            CreateXMLElement();

            //返回XPath节点的值
            return _element.SelectSingleNode(xPath).InnerText;
        }
        #endregion

        #region 获取指定XPath表达式节点的属性值
        /// <summary>
        /// 获取指定XPath表达式节点的属性值
        /// </summary>
        /// <param name="xPath">XPath表达式,
        /// 范例1: @"Skill/First/SkillItem", 等效于 @"//Skill/First/SkillItem"
        /// 范例2: @"Table[USERNAME='a']" , []表示筛选,USERNAME是Table下的一个子节点.
        /// 范例3: @"ApplyPost/Item[@itemName='岗位编号']",@itemName是Item节点的属性.
        /// </param>
        /// <param name="attributeName">属性名</param>
        public string GetAttributeValue(string xPath, string attributeName)
        {
            //创建XML的根节点
            CreateXMLElement();

            //返回XPath节点的属性值
            return _element.SelectSingleNode(xPath).Attributes[attributeName].Value;
        }
        #endregion

        #region 新增节点
        /// <summary>
        /// 1. 功能：新增节点。
        /// 2. 使用条件：将任意节点插入到当前Xml文件中。
        /// </summary>        
        /// <param name="xmlNode">要插入的Xml节点</param>
        public void AppendNode(XmlNode xmlNode)
        {
            //创建XML的根节点
            CreateXMLElement();

            //导入节点
            XmlNode node = _xml.ImportNode(xmlNode, true);

            //将节点插入到根节点下
            _element.AppendChild(node);
        }
#if NET461
        /// <summary>
        /// 1. 功能：新增节点。
        /// 2. 使用条件：将DataSet中的第一条记录插入Xml文件中。
        /// </summary>        
        /// <param name="ds">DataSet的实例，该DataSet中应该只有一条记录</param>
        public void AppendNode(DataSet ds)
        {
            //创建XmlDataDocument对象
            XmlDataDocument xmlDataDocument = new XmlDataDocument(ds);

            //导入节点
            XmlNode node = xmlDataDocument.DocumentElement.FirstChild;

            //将节点插入到根节点下
            AppendNode(node);
        }
#endif
        #endregion

        #region 删除节点
        /// <summary>
        /// 删除指定XPath表达式的节点
        /// </summary>        
        /// <param name="xPath">XPath表达式,
        /// 范例1: @"Skill/First/SkillItem", 等效于 @"//Skill/First/SkillItem"
        /// 范例2: @"Table[USERNAME='a']" , []表示筛选,USERNAME是Table下的一个子节点.
        /// 范例3: @"ApplyPost/Item[@itemName='岗位编号']",@itemName是Item节点的属性.
        /// </param>
        public void RemoveNode(string xPath)
        {
            //创建XML的根节点
            CreateXMLElement();

            //获取要删除的节点
            XmlNode node = _xml.SelectSingleNode(xPath);

            //删除节点
            _element.RemoveChild(node);
        }
        #endregion //删除节点

        #region 保存XML文件
        /// <summary>
        /// 保存XML文件
        /// </summary>        
        public void Save()
        {
            //创建XML的根节点
            CreateXMLElement();

            //保存XML文件
            _xml.Save(this._filePath);
        }
        #endregion //保存XML文件

        #region 静态方法

        #region 创建根节点对象
        /// <summary>
        /// 创建根节点对象
        /// </summary>
        /// <param name="xmlFilePath">Xml文件的相对路径</param>        
        private static XmlElement CreateRootElement(string xmlFilePath)
        {
            //定义变量，表示XML文件的绝对路径
            string filePath = "";

            //获取XML文件的绝对路径
            filePath = SystemUtils.GetMapPath(xmlFilePath);

            //创建XmlDocument对象
            XmlDocument xmlDocument = new XmlDocument();
            //加载XML文件
            xmlDocument.Load(filePath);

            //返回根节点
            return xmlDocument.DocumentElement;
        }
        #endregion

        #region 获取指定XPath表达式节点的值
        /// <summary>
        /// 获取指定XPath表达式节点的值
        /// </summary>
        /// <param name="xmlFilePath">Xml文件的相对路径</param>
        /// <param name="xPath">XPath表达式,
        /// 范例1: @"Skill/First/SkillItem", 等效于 @"//Skill/First/SkillItem"
        /// 范例2: @"Table[USERNAME='a']" , []表示筛选,USERNAME是Table下的一个子节点.
        /// 范例3: @"ApplyPost/Item[@itemName='岗位编号']",@itemName是Item节点的属性.
        /// </param>
        public static string GetValue(string xmlFilePath, string xPath)
        {
            //创建根对象
            XmlElement rootElement = CreateRootElement(xmlFilePath);

            //返回XPath节点的值
            return rootElement.SelectSingleNode(xPath).InnerText;
        }
        #endregion

        #region 获取指定XPath表达式节点的属性值
        /// <summary>
        /// 获取指定XPath表达式节点的属性值
        /// </summary>
        /// <param name="xmlFilePath">Xml文件的相对路径</param>
        /// <param name="xPath">XPath表达式,
        /// 范例1: @"Skill/First/SkillItem", 等效于 @"//Skill/First/SkillItem"
        /// 范例2: @"Table[USERNAME='a']" , []表示筛选,USERNAME是Table下的一个子节点.
        /// 范例3: @"ApplyPost/Item[@itemName='岗位编号']",@itemName是Item节点的属性.
        /// </param>
        /// <param name="attributeName">属性名</param>
        public static string GetAttributeValue(string xmlFilePath, string xPath, string attributeName)
        {
            //创建根对象
            XmlElement rootElement = CreateRootElement(xmlFilePath);

            //返回XPath节点的属性值
            return rootElement.SelectSingleNode(xPath).Attributes[attributeName].Value;
        }
        #endregion

        #region 其它操作

        /// <summary>
        /// 创建xml文档
        /// </summary>
        /// <returns></returns>
        public static XmlDocument CreateXmlDocument()
        {
            XmlDocument document = new XmlDocument();
            XmlDeclaration newChild = document.CreateXmlDeclaration("1.0", "utf-8", "");
            document.InsertBefore(newChild, document.DocumentElement);
            return document;
        }

        /// <summary>
        /// 创建xml文档
        /// </summary>
        /// <param name="rootName"></param>
        /// <returns></returns>
        public static XmlDocument CreateXmlDocument(string rootName)
        {
            XmlDocument document = new XmlDocument();
            XmlDeclaration newChild = document.CreateXmlDeclaration("1.0", "utf-8", "");
            document.InsertBefore(newChild, document.DocumentElement);
            XmlNode node = document.CreateElement(rootName);
            document.AppendChild(node);
            return document;
        }

        /// <summary>
        /// 读取节点属性
        /// </summary>
        /// <param name="node"></param>
        /// <param name="attributeName"></param>
        /// <returns></returns>
        public static string GetAttributeValue(XmlNode node, string attributeName)
        {
            XmlAttribute attribute = node.Attributes[attributeName];
            if (attribute == null)
            {
                return null;
            }
            return attribute.Value;
        }

        /// <summary>
        /// 创建xml节点文本
        /// </summary>
        /// <param name="node"></param>
        /// <param name="itemName"></param>
        /// <returns></returns>
        public static string GetItemValue(XmlNode node, string itemName)
        {
            if ((node != null) && (node[itemName] != null))
            {
                return node[itemName].InnerText;
            }
            return null;
        }

        /// <summary>
        /// 节点选择
        /// </summary>
        /// <param name="parentNode"></param>
        /// <param name="attributeName"></param>
        /// <param name="attributeValue"></param>
        /// <returns></returns>
        public static XmlNode SelectNode(XmlNode parentNode, string attributeName, string attributeValue)
        {
            XmlNode node = null;
            if (parentNode.HasChildNodes)
            {
                string name = parentNode.ChildNodes[0].Name;
                string xpath = name + "[@" + attributeName + "='" + attributeValue + "']";
                node = parentNode.SelectSingleNode(xpath);
            }
            return node;
        }

        /// <summary>
        /// 设置节点属性值
        /// </summary>
        /// <param name="node"></param>
        /// <param name="attributeName"></param>
        /// <param name="attributeValue"></param>
        /// <returns></returns>
        public static bool SetAttributeValue(XmlNode node, string attributeName, string attributeValue)
        {
            if (node != null)
            {
                XmlAttribute attribute = node.Attributes[attributeName];
                if (attribute != null)
                {
                    attribute.Value = attributeValue;
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// 设置节点值
        /// </summary>
        /// <param name="node"></param>
        /// <param name="itemName"></param>
        /// <param name="itemValue"></param>
        /// <returns></returns>
        public static bool SetItemValue(XmlNode node, string itemName, string itemValue)
        {
            if ((node != null) && (node[itemName] != null))
            {
                node[itemName].InnerText = itemValue;
                return true;
            }
            return false;
        }

        /// <summary>
        /// xml编码
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string XmlEncode(string value)
        {
            if (!string.IsNullOrEmpty(value))
            {
                value = value.Replace("&", "&amp;");
                value = value.Replace("<", "&lt;");
                value = value.Replace(">", "&gt;");
                value = value.Replace("'", "&apos;");
                value = value.Replace("\"", "&quot;");
            }
            return value;
        }

        #endregion

        #endregion
    }
}

