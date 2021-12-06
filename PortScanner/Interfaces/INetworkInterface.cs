using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;

namespace PortScanner.Interfaces
{
    public interface INetworkInterface
    {
        IPAddress GetSubnetMask(IPAddress ipAddress);
        IPAddress GetLocalIpAddress();
    }
}
