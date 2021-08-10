using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace PortScanner
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var portScanner = new PortScanner();

            await portScanner.StartScanningAsync();
            await portScanner.WriteResultsToFileAsync();

            Console.ReadKey();
            Console.ReadKey();
        }
    }
    internal class PortScanner
    {
        const int MAX_PORT = 65535;

        private uint _ip;
        private uint _mask;

        public PortScanner()
        {
            var clientIp = IpHelpers.GetLocalIPAddress();
            _ip = clientIp.ParseIp();
            _mask = clientIp.GetSubnetMask().ParseIp();
            GetSubnetHosts();
        }

        private uint NetworkAddress => _ip & _mask;

        private uint BroadcastAddress => NetworkAddress + ~_mask;

        private List<ConnectionResult> ConnectionResults { get; } = new List<ConnectionResult>();
        private List<IPAddress> SubnetHosts { get; } = new List<IPAddress>();

        public async Task StartScanningAsync()
        {
            var taskResults = new List<Task>();
            var stopWatch = new Stopwatch();
            stopWatch.Start();

            Parallel.ForEach(SubnetHosts, ip =>
            {
                for (var port = 0; port <= MAX_PORT; port++)
                {
                    taskResults.Add(CheckConnectionAsync(ip, port));
                }
            });

            await Task.WhenAll(taskResults);

            stopWatch.Stop();
            Console.WriteLine("Time elapsed:" + stopWatch.ElapsedMilliseconds);
        }

        public async Task WriteResultsToFileAsync()
        {
            await using var writer = new StreamWriter("PortScannerResults.txt");
            foreach (var result in ConnectionResults)
            {
                await writer.WriteLineAsync((result.CanConnect
                    ? "CONNECTED"
                    : "REJECTED") + $" IP: {result.IpAddress} - port number: {result.Port}");
            }
        }

        private void GetSubnetHosts()
        {
            for (var host = NetworkAddress + 1; host < BroadcastAddress; host++)
            {
                if (host != _ip)
                    SubnetHosts.Add(host.ParseToIp());
            }
        }

        private async Task CheckConnectionAsync(IPAddress ip, int port)
        {
            Console.WriteLine($"thread = {Thread.CurrentThread.ManagedThreadId}");
            using var tcpClient = new TcpClient();
            try
            {
                await tcpClient.ConnectAsync(ip, port);
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine($"CONNECTED IP: {ip} - port number: {port}");
                ConnectionResults.Add(new ConnectionResult {CanConnect = true, IpAddress = ip, Port = port});
            }
            catch (SocketException)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"REJECTED IP: {ip} - port number: {port}");
                ConnectionResults.Add(new ConnectionResult {CanConnect = false, IpAddress = ip, Port = port});
            }
        }
    }

    internal class ConnectionResult
    {
        public IPAddress IpAddress { get; set; }
        public int Port { get; set; }
        public bool CanConnect { get; set; }
    }

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