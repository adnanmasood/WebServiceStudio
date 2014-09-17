using System;
using System.IO;
using System.Net;

namespace WebServiceStudio
{
    public class WSSWebRequest : WebRequest
    {
        private static RequestProperties requestProperties;
        private readonly MemoryStream stream;
        private readonly WebRequest webRequest;

        public WSSWebRequest(WebRequest webRequest)
        {
            this.webRequest = webRequest;
            stream = new NoCloseMemoryStream();
        }

        public override string ConnectionGroupName
        {
            get { return webRequest.ConnectionGroupName; }
            set { webRequest.ConnectionGroupName = value; }
        }

        public override long ContentLength
        {
            get { return webRequest.ContentLength; }
            set { webRequest.ContentLength = value; }
        }

        public override string ContentType
        {
            get { return webRequest.ContentType; }
            set { webRequest.ContentType = value; }
        }

        public override ICredentials Credentials
        {
            get { return webRequest.Credentials; }
            set { webRequest.Credentials = value; }
        }

        public override WebHeaderCollection Headers
        {
            get { return webRequest.Headers; }
            set { webRequest.Headers = value; }
        }

        public override string Method
        {
            get { return webRequest.Method; }
            set { webRequest.Method = value; }
        }

        public override bool PreAuthenticate
        {
            get { return webRequest.PreAuthenticate; }
            set { webRequest.PreAuthenticate = value; }
        }

        public override IWebProxy Proxy
        {
            get { return webRequest.Proxy; }
            set { webRequest.Proxy = value; }
        }

        internal static RequestProperties RequestTrace
        {
            get { return requestProperties; }
            set { requestProperties = value; }
        }

        public override Uri RequestUri
        {
            get { return webRequest.RequestUri; }
        }

        public override int Timeout
        {
            get { return webRequest.Timeout; }
            set { webRequest.Timeout = value; }
        }

        public override IAsyncResult BeginGetRequestStream(AsyncCallback callback, object asyncState)
        {
            throw new NotSupportedException();
        }

        public override IAsyncResult BeginGetResponse(AsyncCallback callback, object asyncState)
        {
            throw new NotSupportedException();
        }

        public override Stream EndGetRequestStream(IAsyncResult result)
        {
            throw new NotSupportedException();
        }

        public override WebResponse EndGetResponse(IAsyncResult result)
        {
            throw new NotSupportedException();
        }

        public override Stream GetRequestStream()
        {
            return stream;
        }

        public override WebResponse GetResponse()
        {
            WebResponse response4;
            requestProperties.contentType = webRequest.ContentType;
            requestProperties.soapAction = webRequest.Headers["SOAPAction"];
            requestProperties.url = webRequest.RequestUri.ToString();
            if (webRequest.Method.ToUpper() == "POST")
            {
                requestProperties.Method = RequestProperties.HttpMethod.POST;
                Stream requestStream = webRequest.GetRequestStream();
                requestStream.Write(stream.GetBuffer(), 0, (int) stream.Length);
                requestStream.Close();
                stream.Position = 0L;
                requestProperties.requestPayLoad = MessageTracer.ReadMessage(stream, requestProperties.contentType);
            }
            else if (webRequest.Method.ToUpper() == "GET")
            {
                requestProperties.Method = RequestProperties.HttpMethod.GET;
            }
            try
            {
                var response2 = new WSSWebResponse(webRequest.GetResponse());
                requestProperties.responsePayLoad = response2.DumpResponse();
                response4 = response2;
            }
            catch (WebException exception)
            {
                if (exception.Response != null)
                {
                    var response = new WSSWebResponse(exception.Response);
                    requestProperties.responsePayLoad = response.DumpResponse();
                    throw new WebException(exception.Message, exception, exception.Status, response);
                }
                requestProperties.responsePayLoad = exception.ToString();
                throw;
            }
            catch (Exception exception2)
            {
                requestProperties.responsePayLoad = exception2.ToString();
                throw;
            }
            return response4;
        }
    }
}