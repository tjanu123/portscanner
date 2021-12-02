﻿using PortScanner.Interfaces;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace PortScanner
{
    public class PortScanner : IPortScanner
    {
        //LOGGING INTERFACE
        //tcp interface
        //
        //



        private readonly IWriter _writer;
        private const int MAX_PORT = 65535;

        private uint _ip;
        private uint _mask;

        private uint NetworkAddress => _ip & _mask;
        private uint BroadcastAddress => NetworkAddress + ~_mask;
        private List<ConnectionResult> ConnectionResults { get; } = new List<ConnectionResult>();
        private List<IPAddress> SubnetHosts { get; } = new List<IPAddress>();

        public PortScanner(IWriter writer)
        {
            _writer = writer;

            var clientIp = IpHelpers.GetLocalIpAddress();
            _ip = clientIp.ParseIp();
            _mask = clientIp.GetSubnetMask().ParseIp();
            GetSubnetHosts();
        }

        public async Task StartScanningAsync()
        {
            var taskResults = new List<Task>();
            var stopWatch = new Stopwatch();
            stopWatch.Start();

            foreach (var ip in SubnetHosts)
            {
                Parallel.For(0, MAX_PORT, (port) =>
              {
                  taskResults.Add(CheckConnectionAsync(ip, port));
              });
            }

            await Task.WhenAll(taskResults);

            stopWatch.Stop();
            Console.WriteLine("Time elapsed:" + stopWatch.ElapsedMilliseconds);

            await _writer.WriteToFileAsync(ConnectionResults);
        }

        private async Task CheckConnectionAsync(IPAddress ip, int port)
        {
            using var tcpClient = new TcpClient();
            try
            {
                await tcpClient.ConnectAsync(ip, port);
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine($"CONNECTED IP: {ip} - port number: {port}");
                ConnectionResults.Add(new ConnectionResult { IpAddress = ip, Port = port });
            }
            catch (SocketException)
            {
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
    }
}