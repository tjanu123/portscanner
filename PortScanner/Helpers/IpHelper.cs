using System.Collections.Generic;
using System.Globalization;
using System.Net;

namespace PortScanner
{
    public static class IpHelper
    {
        public static IPAddress ParseToIp(this uint value)
        {
            var bitmask = 0xff000000;
            var parts = new string[4];
            for (var i = 0; i < 4; i++)
            {
                var masked = (value & bitmask) >> ((3 - i) * 8);
                bitmask >>= 8;
                parts[i] = masked.ToString(CultureInfo.InvariantCulture);
            }

            return IPAddress.Parse(string.Join(".", parts));
        }

        public static uint ParseToIp(this IPAddress ipAddress)
        {
            var addressBytes = ipAddress.GetAddressBytes();
            uint ip = 0;
            for (var i = 0; i < 4; i++)
            {
                ip = (ip << 8) + addressBytes[i];
            }

            return ip;
        }

        public static List<IPAddress> GetSubnetHosts(uint clientIp, uint networkAddress, uint broadcastAddress)
        {
            var subnetHosts = new List<IPAddress>();

            for (var host = networkAddress + 1; host < broadcastAddress; host++)
            {
                if (host != clientIp)
                    subnetHosts.Add(host.ParseToIp());
            }

            return subnetHosts;
        }
    }
}