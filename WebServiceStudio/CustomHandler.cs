using System.ComponentModel;
using System.Xml.Serialization;

namespace WebServiceStudio
{
    [TypeConverter(typeof (ExpandableObjectConverter))]
    public class CustomHandler
    {
        private string handler;
        private string typeName;

        [XmlAttribute]
        public string Handler
        {
            get { return handler; }
            set { handler = value; }
        }

        [XmlAttribute]
        public string TypeName
        {
            get { return typeName; }
            set { typeName = value; }
        }

        public override string ToString()
        {
            return (handler + "{" + typeName + "}");
        }
    }
}