using System.ComponentModel;
using System.Drawing;
using System.Xml.Serialization;

namespace WebServiceStudio
{
    [TypeConverter(typeof (ExpandableObjectConverter))]
    public class UiProperties
    {
        private static readonly FontConverter fontConverter = new FontConverter();
        private Font messageFont;
        private Font reqRespFont;
        private Font wsdlFont;

        [XmlIgnore, TypeConverter(typeof (FontConverter))]
        public Font MessageFont
        {
            get
            {
                if (messageFont == null)
                {
                    messageFont = new Font("Lucida Sans Unicode", 9.25f, FontStyle.Regular, GraphicsUnit.Point, 0);
                }
                return messageFont;
            }
            set { messageFont = value; }
        }

        [Browsable(false), XmlElement("MessageFont")]
        public string MessageFontX
        {
            get { return (string) fontConverter.ConvertTo(null, null, messageFont, typeof (string)); }
            set { messageFont = (Font) fontConverter.ConvertFrom(null, null, value); }
        }

        [TypeConverter(typeof (FontConverter)), XmlIgnore]
        public Font ReqRespFont
        {
            get
            {
                if (reqRespFont == null)
                {
                    reqRespFont = new Font("Lucida Sans Unicode", 9.25f, FontStyle.Regular, GraphicsUnit.Point, 0);
                }
                return reqRespFont;
            }
            set { reqRespFont = value; }
        }

        [Browsable(false), XmlElement("ReqRespFont")]
        public string ReqRespFontX
        {
            get { return (string) fontConverter.ConvertTo(null, null, reqRespFont, typeof (string)); }
            set { reqRespFont = (Font) fontConverter.ConvertFrom(null, null, value); }
        }

        [XmlIgnore, TypeConverter(typeof (FontConverter))]
        public Font WsdlFont
        {
            get
            {
                if (wsdlFont == null)
                {
                    wsdlFont = new Font("Lucida Sans Unicode", 8.25f, FontStyle.Regular, GraphicsUnit.Point, 0);
                }
                return wsdlFont;
            }
            set { wsdlFont = value; }
        }

        [XmlElement("WsdlFont"), Browsable(false)]
        public string WsdlFontX
        {
            get { return (string) fontConverter.ConvertTo(null, null, wsdlFont, typeof (string)); }
            set { wsdlFont = (Font) fontConverter.ConvertFrom(null, null, value); }
        }

        public override string ToString()
        {
            return "";
        }
    }
}