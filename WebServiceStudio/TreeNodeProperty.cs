using System;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Reflection;
using System.Web.Services.Protocols;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Serialization;

namespace WebServiceStudio
{
    internal class TreeNodeProperty
    {
        private static readonly Hashtable includedTypesLookup = new Hashtable();

        private static readonly Type[] systemTypes =
        {
            typeof (bool), typeof (byte), typeof (byte[]), typeof (sbyte), typeof (short), typeof (int), typeof (long),
            typeof (float), typeof (double), typeof (string), typeof (string[]), typeof (DateTime), typeof (TimeSpan),
            typeof (XmlElement), typeof (XmlAttribute), typeof (XmlNode[]),
            typeof (object[])
        };

        public string Name;

        public TreeNode TreeNode;
        private Type[] types;

        public TreeNodeProperty(Type[] types, string name)
        {
            this.types = types;
            Name = name;
        }

        [TypeConverter(typeof (ListStandardValues))]
        public virtual Type Type
        {
            get { return types[0]; }
            set { }
        }

        public void AddToTypeList(object o)
        {
            var type = o as Type;
            var destinationArray = new Type[types.Length + 1];
            Array.Copy(types, destinationArray, types.Length);
            destinationArray[types.Length] = type;
            types = destinationArray;
        }

        private void AddTypeToList(Type[] includedTypes, ArrayList list)
        {
            var type = list[0] as Type;
            foreach (Type type2 in includedTypes)
            {
                if (type.IsAssignableFrom(type2) && !list.Contains(type2))
                {
                    list.Add(type2);
                }
            }
        }

        public static void ClearIncludedTypes()
        {
            includedTypesLookup.Clear();
        }

        protected virtual void CreateChildren()
        {
        }

        protected static object CreateNewInstance(Type type)
        {
            var obj2 = new Object();
            try
            {
                if (type.IsArray)
                {
                    return Array.CreateInstance(type.GetElementType(), 1);
                }
                if (type == typeof (string))
                {
                    return "";
                }
                if (type == typeof (Guid))
                {
                    return Guid.NewGuid();
                }
                if (type == typeof (XmlElement))
                {
                    var document = new XmlDocument();
                    return document.CreateElement("MyElement");
                }
                if (type == typeof (XmlAttribute))
                {
                    var document2 = new XmlDocument();
                    return document2.CreateAttribute("MyAttribute");
                }
                obj2 = Activator.CreateInstance(type);
            }
            catch
            {
            }
            return obj2;
        }

        public static TreeNodeProperty CreateTreeNodeProperty(TreeNodeProperty tnp)
        {
            if (tnp is ClassProperty)
            {
                var property = tnp as ClassProperty;
                return CreateTreeNodeProperty(property.types, property.Name, property.InternalValue);
            }
            if (tnp is PrimitiveProperty)
            {
                var property2 = tnp as PrimitiveProperty;
                return CreateTreeNodeProperty(property2.types, property2.Name, property2.Value);
            }
            return CreateTreeNodeProperty(tnp.types, tnp.Name, null);
        }

        public static TreeNodeProperty CreateTreeNodeProperty(TreeNodeProperty tnp, object val)
        {
            return CreateTreeNodeProperty(tnp.types, tnp.Name, val);
        }

        public static TreeNodeProperty CreateTreeNodeProperty(Type[] possibleTypes, string name, object val)
        {
            Type elementType = (val == null) ? possibleTypes[0] : val.GetType();
            if (elementType.IsByRef)
            {
                elementType = elementType.GetElementType();
            }
            if (IsPrimitiveType(possibleTypes[0]))
            {
                if (val == null)
                {
                    val = CreateNewInstance(elementType);
                }
                return new PrimitiveProperty(name, val);
            }
            if (IsNullablePrimitiveType(elementType) || IsPrimitiveType(elementType))
            {
                return new NullablePrimitiveProperty(possibleTypes, name, val);
            }
            if (typeof (XmlElement).IsAssignableFrom(elementType))
            {
                return new XmlElementProperty(possibleTypes, name, val);
            }
            if (typeof (XmlAttribute).IsAssignableFrom(elementType))
            {
                return new XmlAttributeProperty(possibleTypes, name, val);
            }
            if (elementType.IsArray)
            {
                return new ArrayProperty(possibleTypes, name, val as Array);
            }
            if (elementType.Name.IndexOf("Nullable") >= 0)
            {
                return new NullableGenericProperty(possibleTypes, name, val);
            }
            return new ClassProperty(possibleTypes, name, val);
        }

        private static Type[] GetAllIncludedTypes(Type webService)
        {
            var typeArray = includedTypesLookup[webService] as Type[];
            if (typeArray == null)
            {
                var list = new ArrayList();
                var customAttributes =
                    webService.GetCustomAttributes(typeof (SoapIncludeAttribute), true) as SoapIncludeAttribute[];
                foreach (SoapIncludeAttribute attribute in customAttributes)
                {
                    list.Add(attribute.Type);
                }
                var attributeArray2 =
                    webService.GetCustomAttributes(typeof (XmlIncludeAttribute), true) as XmlIncludeAttribute[];
                foreach (XmlIncludeAttribute attribute2 in attributeArray2)
                {
                    list.Add(attribute2.Type);
                }
                foreach (Type type in systemTypes)
                {
                    list.Add(type);
                }
                typeArray = (Type[]) list.ToArray(typeof (Type));
                includedTypesLookup[webService] = typeArray;
            }
            return typeArray;
        }

        protected virtual MethodInfo GetCurrentMethod()
        {
            TreeNodeProperty parent = GetParent();
            if (parent == null)
            {
                return null;
            }
            return parent.GetCurrentMethod();
        }

        protected virtual object GetCurrentProxy()
        {
            TreeNodeProperty parent = GetParent();
            if (parent == null)
            {
                return null;
            }
            return parent.GetCurrentProxy();
        }

        protected Type[] GetIncludedTypes(Type type)
        {
            var list = new ArrayList();
            list.Add(type);
            if (type.IsByRef)
            {
                type = type.GetElementType();
            }
            MethodInfo currentMethod = GetCurrentMethod();
            if (currentMethod != null)
            {
                AddTypeToList(GetAllIncludedTypes(currentMethod.DeclaringType), list);
            }
            AddTypeToList(GetAllIncludedTypes(type), list);
            return (Type[]) list.ToArray(typeof (Type));
        }

        public TreeNodeProperty GetParent()
        {
            if (TreeNode != null)
            {
                TreeNode treeNode = TreeNode;
                while (treeNode.Parent != null)
                {
                    treeNode = treeNode.Parent;
                    var tag = treeNode.Tag as TreeNodeProperty;
                    if (tag != null)
                    {
                        return tag;
                    }
                }
            }
            return null;
        }

        public Type[] GetTypeList()
        {
            return types;
        }

        protected static bool IsDeepNesting(TreeNodeProperty tnp)
        {
            if (tnp != null)
            {
                int num = 0;
                while ((tnp = tnp.GetParent()) != null)
                {
                    num++;
                    if (num > 12)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        protected virtual bool IsInput()
        {
            TreeNodeProperty parent = GetParent();
            if (parent == null)
            {
                return false;
            }
            return parent.IsInput();
        }

        protected static bool IsInternalType(Type type)
        {
            return typeof (Type).IsAssignableFrom(type);
        }

        private static bool IsNullablePrimitiveType(Type type)
        {
            return ((((typeof (string) == type) || (typeof (Guid) == type)) || typeof (DataSet).IsAssignableFrom(type)) ||
                    (DynamicEditor.IsEditorDefined(type) || DynamicConverter.IsConverterDefined(type)));
        }

        protected static bool IsPrimitiveType(Type type)
        {
            return (((type.IsEnum || type.IsPrimitive) || ((type == typeof (DateTime)) || (type == typeof (TimeSpan)))) ||
                    (type == typeof (decimal)));
        }

        public static bool IsWebMethod(MethodInfo method)
        {
            object[] customAttributes = method.GetCustomAttributes(typeof (SoapRpcMethodAttribute), true);
            if ((customAttributes != null) && (customAttributes.Length > 0))
            {
                return true;
            }
            customAttributes = method.GetCustomAttributes(typeof (SoapDocumentMethodAttribute), true);
            if ((customAttributes != null) && (customAttributes.Length > 0))
            {
                return true;
            }
            customAttributes = method.GetCustomAttributes(typeof (HttpMethodAttribute), true);
            return ((customAttributes != null) && (customAttributes.Length > 0));
        }

        public static bool IsWebService(Type type)
        {
            return typeof (HttpWebClientProtocol).IsAssignableFrom(type);
        }

        public virtual object ReadChildren()
        {
            return null;
        }

        public void RecreateSubtree(TreeNode parentNode)
        {
            int index = -1;
            if (TreeNode != null)
            {
                if (parentNode == null)
                {
                    parentNode = TreeNode.Parent;
                }
                if (TreeNode.Parent == parentNode)
                {
                    index = TreeNode.Index;
                }
                TreeNode.Remove();
            }
            TreeNode = new TreeNode(ToString());
            TreeNode.Tag = this;
            if (parentNode != null)
            {
                if (index < 0)
                {
                    parentNode.Nodes.Add(TreeNode);
                }
                else
                {
                    parentNode.Nodes.Insert(index, TreeNode);
                }
            }
            CreateChildren();
        }

        public override string ToString()
        {
            return Name;
        }
    }
}