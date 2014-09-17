using System.IO;

namespace WebServiceStudio
{
    internal class NoCloseMemoryStream : MemoryStream
    {
        public override void Close()
        {
        }
    }
}