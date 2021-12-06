using PortScanner.Interfaces;
using System;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace PortScanner.Service
{
    public class TcpClient : ITcpClient
    {

        public async Task<ConnectionResult> ConnectAsync(IPAddress ipAddress, int port)
        {
            using var tcpClient = new System.Net.Sockets.TcpClient();
            try
            {
                await tcpClient.ConnectAsync(ipAddress, port);
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine($"CONNECTED IP: {ipAddress} - port number: {port}");
                return new ConnectionResult { IpAddress = ipAddress, Port = port, Connected = true };
            }
            catch (SocketException)
            {
                return new ConnectionResult { IpAddress = ipAddress, Port = port, Connected = false };
            }
        }
    }
}