using System.ComponentModel;
using System.IO;
using System.Reflection;
using System.Xml.Serialization;

namespace WebServiceStudio
{
    [TypeConverter(typeof (ExpandableObjectConverter))]
    public class Configuration
    {
        private static Configuration masterConfig;
        private InvokeProperties invokeSettings = new InvokeProperties();

        [Browsable(false)]
        public CustomHandler[] DataEditors { get; set; }

        [Browsable(false)]
        public InvokeProperties InvokeSettings
        {
            get { return invokeSettings; }
            set { invokeSettings = value; }
        }

        internal static Configuration MasterConfig
        {
            get
            {
                if (masterConfig == null)
                {
                    LoadMasterConfig();
                }
                return masterConfig;
            }
            set
            {
                masterConfig = value;
                SaveMasterConfig();
            }
        }

        [Browsable(false)]
        public CustomHandler[] ProxyProperties { get; set; }

        [Browsable(false)]
        public CustomHandler[] TypeConverters { get; set; }

        public UiProperties UiSettings { get; set; }

        public WsdlProperties WsdlSettings { get; set; }

        public Configuration Copy()
        {
            var stream = new MemoryStream();
            var serializer = new XmlSerializer(typeof (Configuration));
            serializer.Serialize(stream, masterConfig);
            stream.Position = 0L;
            return (serializer.Deserialize(stream) as Configuration);
        }

        private static string GetConfigFileName()
        {
            return (Assembly.GetExecutingAssembly().Location + ".options");
        }

        internal string[] GetProxyBaseTypes()
        {
            CustomHandler[] proxyProperties = ProxyProperties;
            var strArray = new string[(proxyProperties != null) ? (proxyProperties.Length + 1) : 1];
            strArray[0] = "";
            for (int i = 1; i < strArray.Length; i++)
            {
                strArray[i] = proxyProperties[i - 1].TypeName;
            }
            return strArray;
        }

        public static void LoadMasterConfig()
        {
            try
            {
                FileStream stream = File.OpenRead(GetConfigFileName());
                var textReader = new StreamReader(stream);
                var serializer = new XmlSerializer(typeof (Configuration));
                masterConfig = serializer.Deserialize(textReader) as Configuration;
                stream.Flush();
                stream.Close();
            }
            catch
            {
            }
            if (masterConfig == null)
            {
                masterConfig = new Configuration();
            }
            if (masterConfig.DataEditors == null)
            {
                masterConfig.DataEditors = new CustomHandler[0];
            }
            if (masterConfig.TypeConverters == null)
            {
                masterConfig.TypeConverters = new CustomHandler[0];
            }
            if (masterConfig.WsdlSettings == null)
            {
                masterConfig.WsdlSettings = new WsdlProperties();
            }
            if (masterConfig.UiSettings == null)
            {
                masterConfig.UiSettings = new UiProperties();
            }
        }

        public static void SaveMasterConfig()
        {
            FileStream stream = File.OpenWrite(GetConfigFileName());
            var writer = new StreamWriter(stream);
            new XmlSerializer(typeof (Configuration)).Serialize(writer, masterConfig);
            stream.SetLength(stream.Position);
            stream.Flush();
            stream.Close();
        }
    }
}