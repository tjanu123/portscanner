using PortScanner.Interfaces;
using System;
using System.Net;
using System.Net.Sockets;

namespace PortScanner.Service
{
    public class NetworkInterface : INetworkInterface
    {
        public IPAddress GetSubnetMask(IPAddress ipAddress)
        {
            foreach (var adapter in System.Net.NetworkInformation.NetworkInterface.GetAllNetworkInterfaces())
            {
                foreach (var unicastIpAddressInformation in adapter.GetIPProperties()
                    .UnicastAddresses)
                {
                    if (unicastIpAddressInformation.Address.AddressFamily == AddressFamily.InterNetwork &&
                        ipAddress.Equals(unicastIpAddressInformation.Address))
                    {
                        return unicastIpAddressInformation.IPv4Mask;
                    }
                }
            }

            throw new ArgumentException($"Can't find subnet mask for IP address '{ipAddress}'");
        }

        public IPAddress GetLocalIpAddress()
        {
            using Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, 0);
            socket.Connect("8.8.8.8", 65530);
            var endPoint = socket.LocalEndPoint as IPEndPoint;
            return endPoint.Address;
        }
    }
}