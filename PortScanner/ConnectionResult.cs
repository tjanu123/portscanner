using System.Net;

namespace PortScanner
{
    public class ConnectionResult
    {
        public IPAddress IpAddress { get; set; }
        public int Port { get; set; }
    }
}