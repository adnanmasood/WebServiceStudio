using System;
using System.Collections;
using System.Xml;

namespace WebServiceStudio
{
    internal class XmlElementProperty : ClassProperty
    {
        private static readonly Type[] attrArrayType = {typeof (XmlAttribute[])};
        private static readonly Type[] elemArrayType = {typeof (XmlElement[])};
        private static readonly Type[] stringType = {typeof (string)};

        public XmlElementProperty(Type[] possibleTypes, string name, object val) : base(possibleTypes, name, val)
        {
        }

        private XmlElement xmlElement
        {
            get { return (base.InternalValue as XmlElement); }
            set { base.InternalValue = value; }
        }

        protected override void CreateChildren()
        {
            base.TreeNode.Nodes.Clear();
            if (base.InternalValue != null)
            {
                CreateTreeNodeProperty(stringType, "Name", xmlElement.Name).RecreateSubtree(base.TreeNode);
                CreateTreeNodeProperty(stringType, "NamespaceURI", xmlElement.NamespaceURI)
                    .RecreateSubtree(base.TreeNode);
                CreateTreeNodeProperty(stringType, "TextValue", xmlElement.InnerText).RecreateSubtree(base.TreeNode);
                var list = new ArrayList();
                var list2 = new ArrayList();
                if (xmlElement != null)
                {
                    for (XmlNode node = xmlElement.FirstChild; node != null; node = node.NextSibling)
                    {
                        if (node.NodeType == XmlNodeType.Element)
                        {
                            list2.Add(node);
                        }
                    }
                    foreach (XmlAttribute attribute in xmlElement.Attributes)
                    {
                        if ((attribute.Name != "xmlns") && !attribute.Name.StartsWith("xmlns:"))
                        {
                            list.Add(attribute);
                        }
                    }
                }
                XmlAttribute[] val = ((list.Count == 0) && !IsInput())
                    ? null
                    : (list.ToArray(typeof (XmlAttribute)) as XmlAttribute[]);
                XmlElement[] elementArray = ((list2.Count == 0) && !IsInput())
                    ? null
                    : (list2.ToArray(typeof (XmlElement)) as XmlElement[]);
                CreateTreeNodeProperty(attrArrayType, "Attributes", val).RecreateSubtree(base.TreeNode);
                CreateTreeNodeProperty(elemArrayType, "SubElements", elementArray).RecreateSubtree(base.TreeNode);
            }
        }

        public XmlDocument GetXmlDocument()
        {
            var parent = base.GetParent() as ArrayProperty;
            XmlElementProperty property2 = null;
            if (parent != null)
            {
                property2 = parent.GetParent() as XmlElementProperty;
            }
            if (property2 == null)
            {
                return xmlElement.OwnerDocument;
            }
            return property2.GetXmlDocument();
        }

        public override object ReadChildren()
        {
            XmlElement element3;
            if (base.InternalValue == null)
            {
                return null;
            }
            string qualifiedName = ((TreeNodeProperty) base.TreeNode.Nodes[0].Tag).ReadChildren().ToString();
            string namespaceURI = ((TreeNodeProperty) base.TreeNode.Nodes[1].Tag).ReadChildren().ToString();
            string str3 = ((TreeNodeProperty) base.TreeNode.Nodes[2].Tag).ReadChildren().ToString();
            var attributeArray = (XmlAttribute[]) ((TreeNodeProperty) base.TreeNode.Nodes[3].Tag).ReadChildren();
            var elementArray = (XmlElement[]) ((TreeNodeProperty) base.TreeNode.Nodes[4].Tag).ReadChildren();
            XmlElement element = GetXmlDocument().CreateElement(qualifiedName, namespaceURI);
            if (attributeArray != null)
            {
                foreach (XmlAttribute attribute in attributeArray)
                {
                    element.SetAttributeNode(attribute);
                }
            }
            element.InnerText = str3;
            if (elementArray != null)
            {
                foreach (XmlElement element2 in elementArray)
                {
                    element.AppendChild(element2);
                }
            }
            xmlElement = element3 = element;
            return element3;
        }
    }
}