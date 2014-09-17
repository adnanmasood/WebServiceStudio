using System;
using System.Xml;

namespace WebServiceStudio
{
    internal class XmlAttributeProperty : ClassProperty
    {
        private static readonly Type[] stringType = {typeof (string)};

        public XmlAttributeProperty(Type[] possibleTypes, string name, object val) : base(possibleTypes, name, val)
        {
        }

        private XmlAttribute xmlAttribute
        {
            get { return (base.InternalValue as XmlAttribute); }
            set { base.InternalValue = value; }
        }

        protected override void CreateChildren()
        {
            base.TreeNode.Nodes.Clear();
            if (base.InternalValue != null)
            {
                CreateTreeNodeProperty(stringType, "Name", xmlAttribute.Name).RecreateSubtree(base.TreeNode);
                CreateTreeNodeProperty(stringType, "NamespaceURI", xmlAttribute.NamespaceURI)
                    .RecreateSubtree(base.TreeNode);
                CreateTreeNodeProperty(stringType, "Value", xmlAttribute.Value).RecreateSubtree(base.TreeNode);
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
                return xmlAttribute.OwnerDocument;
            }
            return property2.GetXmlDocument();
        }

        public override object ReadChildren()
        {
            if (base.InternalValue == null)
            {
                return null;
            }
            string qualifiedName = ((TreeNodeProperty) base.TreeNode.Nodes[0].Tag).ReadChildren().ToString();
            string namespaceURI = ((TreeNodeProperty) base.TreeNode.Nodes[1].Tag).ReadChildren().ToString();
            string str3 = ((TreeNodeProperty) base.TreeNode.Nodes[2].Tag).ReadChildren().ToString();
            xmlAttribute = GetXmlDocument().CreateAttribute(qualifiedName, namespaceURI);
            xmlAttribute.Value = str3;
            return xmlAttribute;
        }
    }
}