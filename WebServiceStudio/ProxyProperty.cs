using System.Reflection;
using System.Web.Services.Protocols;
using System.Windows.Forms;

namespace WebServiceStudio
{
    internal class ProxyProperty : TreeNodeProperty
    {
        private readonly HttpWebClientProtocol proxy;
        private readonly ProxyProperties proxyProperties;

        public ProxyProperty(HttpWebClientProtocol proxy) : base(new[] {typeof (ProxyProperties)}, "Proxy")
        {
            this.proxy = proxy;
            proxyProperties = new ProxyProperties(proxy);
        }

        protected override void CreateChildren()
        {
            base.TreeNode.Nodes.Clear();
            foreach (PropertyInfo info in Type.GetProperties(BindingFlags.Public | BindingFlags.Instance))
            {
                object val = info.GetValue(proxyProperties, null);
                CreateTreeNodeProperty(base.GetIncludedTypes(info.PropertyType), info.Name, val)
                    .RecreateSubtree(base.TreeNode);
            }
        }

        public HttpWebClientProtocol GetProxy()
        {
            ((ProxyProperties) ReadChildren()).UpdateProxy(proxy);
            return proxy;
        }

        public override object ReadChildren()
        {
            object proxyProperties = this.proxyProperties;
            if (proxyProperties == null)
            {
                return null;
            }
            int num = 0;
            foreach (PropertyInfo info in Type.GetProperties(BindingFlags.Public | BindingFlags.Instance))
            {
                TreeNode node = base.TreeNode.Nodes[num++];
                var tag = node.Tag as TreeNodeProperty;
                if (tag != null)
                {
                    info.SetValue(proxyProperties, tag.ReadChildren(), null);
                }
            }
            return proxyProperties;
        }
    }
}