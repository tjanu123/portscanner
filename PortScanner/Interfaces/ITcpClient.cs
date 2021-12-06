using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace PortScanner.Interfaces
{
    public interface ITcpClient
    {
        Task<ConnectionResult> ConnectAsync(IPAddress ipAddress, int port);
    }
}
