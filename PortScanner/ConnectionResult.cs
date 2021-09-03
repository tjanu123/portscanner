using System.Net;

namespace PortScanner
{
    internal class ConnectionResult
    {
        public IPAddress IpAddress { get; set; }
        public int Port { get; set; }
        public bool CanConnect { get; set; }
    }
}