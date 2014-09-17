using System.Net;
using System.Web.Services.Protocols;

namespace WebServiceStudio
{
    internal class RequestProperties
    {
        public enum HttpMethod
        {
            GET,
            POST
        }

        public bool allowAutoRedirect = true;
        public bool allowWriteStreamBuffering = true;
        public string basicAuthPassword;
        public string basicAuthUserName;
        public string contentType;
        public bool keepAlive;
        public HttpMethod method;
        public bool pipelined;
        public bool preAuthenticate;
        public string proxy;
        public string requestPayLoad;
        public string responsePayLoad;
        public bool sendChunked;
        public string soapAction;
        public int timeout = 0x2710;
        public string url;
        public bool useCookieContainer;
        public bool useDefaultCredential;

        public RequestProperties(HttpWebClientProtocol proxy)
        {
            if (proxy != null)
            {
                Method = HttpMethod.POST;
                preAuthenticate = proxy.PreAuthenticate;
                timeout = proxy.Timeout;
                useCookieContainer = proxy.CookieContainer != null;
                var protocol = proxy as SoapHttpClientProtocol;
                if (protocol != null)
                {
                    allowAutoRedirect = protocol.AllowAutoRedirect;
                    allowWriteStreamBuffering = protocol.AllowAutoRedirect;
                    var proxy2 = protocol.Proxy as WebProxy;
                    HttpProxy = ((proxy2 != null) && (proxy2.Address != null)) ? proxy2.Address.ToString() : null;
                }
            }
        }

        public string __RequestProperties__
        {
            get { return ""; }
        }

        public bool AllowAutoRedirect
        {
            get { return allowAutoRedirect; }
            set { allowAutoRedirect = value; }
        }

        public bool AllowWriteStreamBuffering
        {
            get { return allowWriteStreamBuffering; }
            set { allowWriteStreamBuffering = value; }
        }

        public string BasicAuthPassword
        {
            get { return basicAuthPassword; }
            set { basicAuthPassword = value; }
        }

        public string BasicAuthUserName
        {
            get { return basicAuthUserName; }
            set { basicAuthUserName = value; }
        }

        public string ContentType
        {
            get { return contentType; }
            set { contentType = value; }
        }

        public string HttpProxy
        {
            get { return proxy; }
            set { proxy = ((value == null) || (value.Length == 0)) ? null : new WebProxy(value).Address.ToString(); }
        }

        public bool KeepAlive
        {
            get { return keepAlive; }
            set { keepAlive = value; }
        }

        public HttpMethod Method
        {
            get { return method; }
            set { method = value; }
        }

        public bool Pipelined
        {
            get { return pipelined; }
            set { pipelined = value; }
        }

        public bool PreAuthenticate
        {
            get { return preAuthenticate; }
            set { preAuthenticate = value; }
        }

        public bool SendChunked
        {
            get { return sendChunked; }
            set { sendChunked = value; }
        }

        public string SOAPAction
        {
            get { return soapAction; }
            set { soapAction = value; }
        }

        public int Timeout
        {
            get { return timeout; }
            set { timeout = value; }
        }

        public string Url
        {
            get { return url; }
            set { url = value; }
        }

        public bool UseCookieContainer
        {
            get { return useCookieContainer; }
            set { useCookieContainer = value; }
        }

        public bool UseDefaultCredential
        {
            get { return useDefaultCredential; }
            set { useDefaultCredential = value; }
        }
    }
}