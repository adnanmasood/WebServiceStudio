using System;
using System.Collections;
using System.Reflection;
using System.Web.Services.Protocols;
using System.Windows.Forms;

namespace WebServiceStudio
{
    internal class MethodProperty : TreeNodeProperty
    {
        private readonly bool isIn;
        private readonly MethodInfo method;
        private readonly ProxyProperty proxyProperty;
        private readonly object result;
        private object[] paramValues;

        public MethodProperty(ProxyProperty proxyProperty, MethodInfo method)
            : base(new[] {method.ReturnType}, method.Name)
        {
            this.proxyProperty = proxyProperty;
            this.method = method;
            isIn = true;
        }

        public MethodProperty(ProxyProperty proxyProperty, MethodInfo method, object result, object[] paramValues)
            : base(new[] {method.ReturnType}, method.Name)
        {
            this.proxyProperty = proxyProperty;
            this.method = method;
            isIn = false;
            this.result = result;
            this.paramValues = paramValues;
        }

        private void AddBody()
        {
            TreeNode parentNode = base.TreeNode.Nodes.Add("Body");
            if (!isIn && (method.ReturnType != typeof (void)))
            {
                Type type = (result != null) ? result.GetType() : method.ReturnType;
                CreateTreeNodeProperty(new[] {type}, "result", result).RecreateSubtree(parentNode);
            }
            ParameterInfo[] parameters = method.GetParameters();
            for (int i = 0; i < parameters.Length; i++)
            {
                if ((!isIn && (parameters[i].IsOut || parameters[i].ParameterType.IsByRef)) ||
                    (isIn && !parameters[i].IsOut))
                {
                    Type parameterType = parameters[i].ParameterType;
                    if (parameterType.IsByRef)
                    {
                        parameterType = parameterType.GetElementType();
                    }
                    object val = (paramValues != null)
                        ? paramValues[i]
                        : (isIn ? CreateNewInstance(parameterType) : null);
                    CreateTreeNodeProperty(base.GetIncludedTypes(parameterType), parameters[i].Name, val)
                        .RecreateSubtree(parentNode);
                }
            }
            parentNode.ExpandAll();
        }

        private void AddHeaders()
        {
            TreeNode parentNode = base.TreeNode.Nodes.Add("Headers");
            FieldInfo[] soapHeaders = GetSoapHeaders(method, isIn);
            HttpWebClientProtocol proxy = proxyProperty.GetProxy();
            foreach (FieldInfo info in soapHeaders)
            {
                object val = (proxy != null) ? info.GetValue(proxy) : null;
                CreateTreeNodeProperty(base.GetIncludedTypes(info.FieldType), info.Name, val)
                    .RecreateSubtree(parentNode);
            }
            parentNode.ExpandAll();
        }

        protected override void CreateChildren()
        {
            AddHeaders();
            AddBody();
        }

        protected override MethodInfo GetCurrentMethod()
        {
            return method;
        }

        protected override object GetCurrentProxy()
        {
            return proxyProperty.GetProxy();
        }

        public MethodInfo GetMethod()
        {
            return method;
        }

        public ProxyProperty GetProxyProperty()
        {
            return proxyProperty;
        }

        public static FieldInfo[] GetSoapHeaders(MethodInfo method, bool isIn)
        {
            Type declaringType = method.DeclaringType;
            var customAttributes =
                (SoapHeaderAttribute[]) method.GetCustomAttributes(typeof (SoapHeaderAttribute), true);
            var list = new ArrayList();
            for (int i = 0; i < customAttributes.Length; i++)
            {
                SoapHeaderAttribute attribute = customAttributes[i];
                if (((attribute.Direction == SoapHeaderDirection.InOut) ||
                     (isIn && (attribute.Direction == SoapHeaderDirection.In))) ||
                    (!isIn && (attribute.Direction == SoapHeaderDirection.Out)))
                {
                    FieldInfo field = declaringType.GetField(attribute.MemberName);
                    list.Add(field);
                }
            }
            return (FieldInfo[]) list.ToArray(typeof (FieldInfo));
        }

        protected override bool IsInput()
        {
            return isIn;
        }

        private void ReadBody()
        {
            TreeNode node = base.TreeNode.Nodes[1];
            ParameterInfo[] parameters = method.GetParameters();
            paramValues = new object[parameters.Length];
            int index = 0;
            int num2 = 0;
            while (index < paramValues.Length)
            {
                ParameterInfo info = parameters[index];
                if (!info.IsOut)
                {
                    TreeNode node2 = node.Nodes[num2++];
                    var tag = node2.Tag as TreeNodeProperty;
                    if (tag != null)
                    {
                        paramValues[index] = tag.ReadChildren();
                    }
                }
                index++;
            }
        }

        public override object ReadChildren()
        {
            ReadHeaders();
            ReadBody();
            return paramValues;
        }

        private void ReadHeaders()
        {
            TreeNode node = base.TreeNode.Nodes[0];
            Type declaringType = method.DeclaringType;
            HttpWebClientProtocol proxy = proxyProperty.GetProxy();
            foreach (TreeNode node2 in node.Nodes)
            {
                var tag = node2.Tag as ClassProperty;
                if (tag != null)
                {
                    declaringType.GetField(tag.Name).SetValue(proxy, tag.ReadChildren());
                }
            }
        }

        public override string ToString()
        {
            return base.Name;
        }
    }
}