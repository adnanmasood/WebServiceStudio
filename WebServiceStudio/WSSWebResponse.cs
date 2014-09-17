using System;
using System.IO;
using System.Net;
using System.Text;

namespace WebServiceStudio
{
    public class WSSWebResponse : WebResponse
    {
        private readonly MemoryStream stream;
        private readonly WebResponse webResponse;

        public WSSWebResponse(WebResponse webResponse)
        {
            this.webResponse = webResponse;
            stream = new NoCloseMemoryStream();
            Stream responseStream = webResponse.GetResponseStream();
            var buffer = new byte[0x400];
            while (true)
            {
                int count = responseStream.Read(buffer, 0, buffer.Length);
                if (count <= 0)
                {
                    break;
                }
                stream.Write(buffer, 0, count);
            }
            stream.Position = 0L;
        }

        public override long ContentLength
        {
            get { return webResponse.ContentLength; }
            set { webResponse.ContentLength = value; }
        }

        public override string ContentType
        {
            get { return webResponse.ContentType; }
            set { webResponse.ContentType = value; }
        }

        public override WebHeaderCollection Headers
        {
            get { return webResponse.Headers; }
        }

        public override Uri ResponseUri
        {
            get { return webResponse.ResponseUri; }
        }

        public HttpStatusCode StatusCode
        {
            get
            {
                if (webResponse is HttpWebResponse)
                {
                    return ((HttpWebResponse) webResponse).StatusCode;
                }
                return HttpStatusCode.NotImplemented;
            }
        }

        public string StatusDescription
        {
            get
            {
                if (webResponse is HttpWebResponse)
                {
                    return ((HttpWebResponse) webResponse).StatusDescription;
                }
                return "";
            }
        }

        public override void Close()
        {
            webResponse.Close();
        }

        public string DumpResponse()
        {
            long position = stream.Position;
            stream.Position = 0L;
            string str = DumpResponse(this);
            stream.Position = position;
            return str;
        }

        public static string DumpResponse(WebResponse response)
        {
            Stream responseStream = response.GetResponseStream();
            var builder = new StringBuilder();
            if (response is HttpWebResponse)
            {
                var response2 = (HttpWebResponse) response;
                builder.Append(
                    string.Concat(new object[]
                    {"ResponseCode: ", (int) response2.StatusCode, " (", response2.StatusDescription, ")\n"}));
            }
            else if (response is WSSWebResponse)
            {
                var response3 = (WSSWebResponse) response;
                builder.Append(
                    string.Concat(new object[]
                    {"ResponseCode: ", (int) response3.StatusCode, " (", response3.StatusDescription, ")\n"}));
            }
            foreach (string str in response.Headers.Keys)
            {
                builder.Append(str + ":" + response.Headers[str] + "\n");
            }
            builder.Append("\n");
            builder.Append(MessageTracer.ReadMessage(responseStream, (int) response.ContentLength, response.ContentType));
            return builder.ToString();
        }

        public override Stream GetResponseStream()
        {
            return stream;
        }
    }
}