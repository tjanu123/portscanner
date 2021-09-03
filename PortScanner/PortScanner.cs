using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace PortScanner
{
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
}