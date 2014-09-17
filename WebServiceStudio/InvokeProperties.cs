using System.Collections;
using System.ComponentModel;
using System.Xml.Serialization;

namespace WebServiceStudio
{
    [TypeConverter(typeof (ExpandableObjectConverter))]
    public class InvokeProperties
    {
        private readonly ArrayList uris = new ArrayList();

        [XmlArrayItem("Uri", typeof (string)), Browsable(false)]
        public string[] RecentlyUsedUris
        {
            get { return (uris.ToArray(typeof (string)) as string[]); }
            set
            {
                uris.Clear();
                if (value != null)
                {
                    uris.AddRange(value);
                }
            }
        }

        public void AddUri(string uri)
        {
            uris.Remove(uri);
            uris.Insert(0, uri);
            Configuration.SaveMasterConfig();
        }

        public override string ToString()
        {
            return "";
        }
    }
}