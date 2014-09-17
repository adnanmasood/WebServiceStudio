using System.Web.Services.Protocols;

namespace WebServiceStudio
{
    public interface IAdditionalProperties
    {
        void UpdateProxy(HttpWebClientProtocol proxy);
    }
}