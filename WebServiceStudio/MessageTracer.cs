using System;
using System.IO;
using System.Text;
using System.Xml;

namespace WebServiceStudio
{
    internal class MessageTracer
    {
        public const string ApplicationXml = "application/soap+xml";
        public const string SoapEnvUri = "http://schemas.xmlsoap.org/soap/envelope/";
        public const string TextHtml = "text/html";
        public const string TextPlain = "text/plain";
        public const string TextXml = "text/xml";

        private static void DumpBytes(Stream stream, int length, StringBuilder sb)
        {
            var buffer = new byte[0x400];
            int num = 0x10;
            var chArray = new char[0x30];
            var chArray2 = new char[0x10];
            int num2 = 0;
            int num3 = 0;
            do
            {
                num3 = stream.Read(buffer, 0, min(length - num2, buffer.Length));
                int index = 0;
                while (index < num3)
                {
                    if ((num2%num) == 0)
                    {
                        for (int i = 0; i < num; i++)
                        {
                            char ch;
                            chArray2[i] = ch = ' ';
                            chArray[(3*i) + 2] = ch;
                            chArray[3*i] = chArray[(3*i) + 1] = ch;
                        }
                    }
                    byte num6 = buffer[index];
                    int num7 = num2%num;
                    chArray[3*num7] = HexChar((num6 >> 4) & 15);
                    chArray[(3*num7) + 1] = HexChar(num6 & 15);
                    chArray2[num7] = char.IsControl((char) num6) ? '.' : ((char) num6);
                    if (((num2%num) == (num - 1)) || (index == (num3 - 1)))
                    {
                        sb.Append(string.Format("{0,8}: {1} {2}\n", (num2/0x10)*0x10, new string(chArray),
                            new string(chArray2)));
                    }
                    index++;
                    num2++;
                }
            } while ((num2 < length) && (num3 > 0));
        }

        private static string GetCharset(string contentType)
        {
            int index = contentType.IndexOf(';');
            if (index >= 0)
            {
                string strA = contentType.Substring(index + 1).TrimStart(null);
                if (string.Compare(strA, 0, "charset", 0, "charset".Length, true) == 0)
                {
                    string str2 = strA.Substring("charset".Length);
                    int num2 = str2.IndexOf('=');
                    if (num2 >= 0)
                    {
                        return str2.Substring(num2 + 1).Trim(new[] {' ', '\'', '"', '\t'});
                    }
                }
            }
            return string.Empty;
        }

        private static Encoding GetEncoding(string contentType)
        {
            string charset = GetCharset(contentType);
            Encoding encoding = null;
            try
            {
                if (charset.Length > 0)
                {
                    encoding = Encoding.GetEncoding(charset);
                }
            }
            catch (Exception)
            {
            }
            return ((encoding == null) ? new ASCIIEncoding() : encoding);
        }

        private static char HexChar(int nibble)
        {
            if (nibble < 10)
            {
                return (char) (nibble + 0x30);
            }
            return (char) (nibble + 0x37);
        }

        private static int min(int a, int b)
        {
            return ((a < b) ? a : b);
        }

        internal static string ReadMessage(Stream from, string contentType)
        {
            return ReadMessage(from, (int) from.Length, contentType);
        }

        internal static string ReadMessage(Stream from, int len, string contentType)
        {
            if ((contentType.StartsWith("text/xml") || contentType.StartsWith("application/soap+xml")) ||
                (contentType == "http://schemas.xmlsoap.org/soap/envelope/"))
            {
                XmlDocument document = new XmlDocument();
                if (len >= 0)
                {
                    byte[] bytes = ReadStream(from, len);
                    document.InnerXml = GetEncoding(contentType).GetString(bytes);
                }
                else
                {
                    document = ReadStream(from, GetEncoding(contentType));
                }
                var w = new StringWriter();
                var writer2 = new XmlTextWriter(w);
                writer2.Formatting = Formatting.Indented;
                document.Save(writer2);
                return w.ToString();
            }
            if (contentType.StartsWith("text"))
            {
                byte[] buffer = ReadStream(from, len);
                from.Read(buffer, 0, len);
                return GetEncoding(contentType).GetString(buffer, 0, len);
            }
            var sb = new StringBuilder();
            DumpBytes(from, len, sb);
            return sb.ToString();
        }

        private static byte[] ReadStream(Stream stream, int len)
        {
            if (len >= 0)
            {
                var buffer = new byte[len];
                stream.Read(buffer, 0, len);
                return buffer;
            }
            Chunk next = null;
            Chunk chunk2 = null;
            int num = 0;
            while (true)
            {
                var chunk3 = new Chunk();
                if (next == null)
                {
                    next = chunk3;
                }
                chunk3.Buffer = new byte[0x400];
                chunk3.Size = stream.Read(chunk3.Buffer, 0, chunk3.Buffer.Length);
                chunk3.Next = null;
                if (chunk2 != null)
                {
                    chunk2.Next = chunk3;
                }
                num += chunk3.Size;
                if (chunk3.Size < chunk3.Buffer.Length)
                {
                    break;
                }
                chunk2 = chunk3;
            }
            var destinationArray = new byte[num];
            while (next != null)
            {
                Array.Copy(next.Buffer, 0, destinationArray, 0, next.Size);
                next = next.Next;
            }
            return destinationArray;
        }
		
		private static XmlDocument ReadStream(Stream stream, Encoding encoder)
        {
            StreamReader sr = new StreamReader(stream, encoder);
            String retXml = sr.ReadToEnd();
            sr.Close();
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(retXml);
            return doc;
        }

        internal static int WriteMessage(Stream stream, string contentType, string str)
        {
            byte[] bytes = new UTF8Encoding(true).GetBytes(str);
            stream.Write(bytes, 0, bytes.Length);
            return bytes.Length;
        }

        private class Chunk
        {
            public byte[] Buffer;
            public Chunk Next;
            public int Size;
        }
    }
}