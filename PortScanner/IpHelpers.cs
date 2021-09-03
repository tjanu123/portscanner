﻿using System;
using System.Globalization;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;

namespace PortScanner
{
    public static class IpHelpers
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

        public static uint ParseIp(this IPAddress ipAddress)
        {
            var addressBytes = ipAddress.GetAddressBytes();
            uint ip = 0;
            for (var i = 0; i < 4; i++)
            {
                ip = (ip << 8) + addressBytes[i];
            }

            return ip;
        }

        public static IPAddress GetSubnetMask(this IPAddress address)
        {
            foreach (var adapter in NetworkInterface.GetAllNetworkInterfaces())
            {
                foreach (var unicastIPAddressInformation in adapter.GetIPProperties()
                    .UnicastAddresses)
                {
                    if (unicastIPAddressInformation.Address.AddressFamily == AddressFamily.InterNetwork &&
                        address.Equals(unicastIPAddressInformation.Address))
                    {
                        return unicastIPAddressInformation.IPv4Mask;
                    }
                }
            }

            throw new ArgumentException($"Can't find subnet mask for IP address '{address}'");
        }

        public static IPAddress GetLocalIPAddress()
        {
            var host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (var ip in host.AddressList)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                {
                    return ip;
                }
            }

            throw new Exception("No network adapters with an IPv4 address in the system!");
        }
    }
}