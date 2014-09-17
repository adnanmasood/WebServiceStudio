using System;
using System.Collections;
using System.Collections.Specialized;
using System.IO;
using System.Text;
using System.Windows.Forms;
using System.Xml;

namespace WebServiceStudio
{
    public class XmlTreeWriter : XmlWriter
    {
        private StringCollection attrNames;
        private StringCollection attrValues;
        private TreeNode current;
        private int[] linePositions;
        private string name;
        private XmlTextReader reader;
        private WriteState state;

        public XmlTreeWriter()
        {
            Init();
        }

        public override WriteState WriteState
        {
            get { return state; }
        }

        public override string XmlLang
        {
            get { throw new NotImplementedException(); }
        }

        public override XmlSpace XmlSpace
        {
            get { throw new NotImplementedException(); }
        }

        private void Ascend()
        {
            Update();
            current = current.Parent;
        }

        public override void Close()
        {
        }

        private void Descend()
        {
            Update();
            TreeNode node = new XmlTreeNode("", linePositions[reader.LineNumber - 1]);
            node.Tag = current.Tag;
            current.Nodes.Add(node);
            current = node;
        }

        public void FillTree(string xml, TreeNode root)
        {
            current = root;
            reader = new XmlTextReader(new StringReader(xml));
            initPositions(xml);
            WriteNode(reader, true);
        }

        public override void Flush()
        {
        }

        private int GetPosition(int lineNum, int linePos)
        {
            return ((linePositions[lineNum - 1] + linePos) - 1);
        }

        private void Init()
        {
            current = null;
            state = WriteState.Start;
            attrNames = new StringCollection();
            attrValues = new StringCollection();
        }

        private void initPositions(string text)
        {
            var list = new ArrayList();
            char ch = ' ';
            int num = 0;
            list.Add(0);
            for (int i = 0; i < text.Length; i++)
            {
                char ch2 = text[i];
                switch (ch)
                {
                    case '\n':
                    case '\r':
                        list.Add(i - num);
                        break;
                }
                if (((ch2 == '\r') && (ch == '\n')) || ((ch2 == '\n') && (ch == '\r')))
                {
                    ch = ' ';
                    num++;
                }
                else
                {
                    ch = ch2;
                }
            }
            list.Add(text.Length);
            linePositions = list.ToArray(typeof (int)) as int[];
        }

        public override string LookupPrefix(string ns)
        {
            throw new NotImplementedException();
        }

        private void Update()
        {
            var current = this.current as XmlTreeNode;
            if (current != null)
            {
                current.EndPosition = linePositions[reader.LineNumber];
            }
            if (name != null)
            {
                var builder = new StringBuilder();
                for (int i = 0; i < attrNames.Count; i++)
                {
                    builder.Append(" " + attrNames[i] + " = " + attrValues[i]);
                }
                this.current.Text = name + builder;
                attrNames.Clear();
                attrValues.Clear();
                name = null;
            }
        }

        public override void WriteBase64(byte[] buffer, int index, int count)
        {
            throw new NotImplementedException();
        }

        public override void WriteBinHex(byte[] buffer, int index, int count)
        {
            throw new NotImplementedException();
        }

        public override void WriteCData(string text)
        {
        }

        public override void WriteCharEntity(char ch)
        {
        }

        public override void WriteChars(char[] buffer, int index, int count)
        {
            WriteRaw(new string(buffer, index, count));
        }

        public override void WriteComment(string text)
        {
        }

        public override void WriteDocType(string name, string pubid, string sysid, string subset)
        {
        }

        public override void WriteEndAttribute()
        {
            state = WriteState.Element;
        }

        public override void WriteEndDocument()
        {
        }

        public override void WriteEndElement()
        {
            Ascend();
            state = WriteState.Element;
        }

        public override void WriteEntityRef(string name)
        {
        }

        public override void WriteFullEndElement()
        {
            Ascend();
            state = WriteState.Element;
        }

        public override void WriteName(string name)
        {
            throw new NotImplementedException();
        }

        public override void WriteNmToken(string name)
        {
            throw new NotImplementedException();
        }

        public override void WriteProcessingInstruction(string name, string text)
        {
        }

        public override void WriteQualifiedName(string localName, string ns)
        {
            WriteRaw(localName);
        }

        public override void WriteRaw(string data)
        {
            if (state == WriteState.Attribute)
            {
                attrValues.Add(data);
            }
            else
            {
                Descend();
                current.Text = current.Text + data;
            }
        }

        public override void WriteRaw(char[] buffer, int index, int count)
        {
            WriteRaw(new string(buffer, index, count));
        }

        public override void WriteStartAttribute(string prefix, string localName, string ns)
        {
            attrNames.Add(localName);
            state = WriteState.Attribute;
        }

        public override void WriteStartDocument()
        {
        }

        public override void WriteStartDocument(bool standalone)
        {
        }

        public override void WriteStartElement(string prefix, string localName, string ns)
        {
            Descend();
            name = ((prefix != null) && (prefix.Length > 0)) ? (prefix + ":" + localName) : localName;
            state = WriteState.Element;
        }

        public override void WriteString(string text)
        {
            WriteRaw(text);
        }

        public override void WriteSurrogateCharEntity(char lowChar, char highChar)
        {
            throw new NotImplementedException();
        }

        public override void WriteWhitespace(string ws)
        {
        }
    }
}