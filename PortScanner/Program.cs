using PortScanner.Interfaces;
using PortScanner.Service;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;

namespace PortScanner
{
    internal class Program
    {
        private static async Task Main(string[] args)
        {
            IWriter writer = new FileWriter();
            ITcpClient tcpClient = new TcpClient();
            INetworkInterface networkInterface = new NetworkInterface();

            IPortScanner portScanner = new PortScanner(tcpClient, writer, networkInterface);

            await portScanner.StartScanningAsync();

            Console.WriteLine("Scanning run to completion");
        }
    }
}