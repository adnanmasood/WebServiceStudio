using System.ComponentModel;
using System.Xml.Serialization;

namespace WebServiceStudio
{
    [TypeConverter(typeof (ExpandableObjectConverter))]
    public class WsdlProperties
    {
        private string customCodeDomProvider;
        private Language language;

        [RefreshProperties(RefreshProperties.All), XmlAttribute]
        public string CustomCodeDomProvider
        {
            get { return ((language == Language.Custom) ? customCodeDomProvider : ""); }
            set
            {
                customCodeDomProvider = value;
                if ((value != null) && (value.Length > 0))
                {
                    language = Language.Custom;
                }
            }
        }

        [XmlAttribute]
        public string Domain { get; set; }

        [XmlAttribute, RefreshProperties(RefreshProperties.All)]
        public Language Language
        {
            get { return language; }
            set { language = value; }
        }

        [XmlAttribute]
        public string Password { get; set; }

        [XmlAttribute]
        public Protocol Protocol { get; set; }

        [TypeConverter(typeof (ListStandardValues)), XmlAttribute]
        public string ProxyBaseType { get; set; }

        [XmlAttribute]
        public string ProxyDomain { get; set; }

        [XmlAttribute]
        public string ProxyPassword { get; set; }

        [XmlAttribute]
        public string ProxyServer { get; set; }

        [XmlAttribute]
        public string ProxyUserName { get; set; }

        [XmlAttribute]
        public string UserName { get; set; }

        public string[] GetProxyBaseTypeList()
        {
            return Configuration.MasterConfig.GetProxyBaseTypes();
        }

        public override string ToString()
        {
            return "";
        }
    }
}