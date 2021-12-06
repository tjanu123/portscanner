using PortScanner.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace PortScanner
{
    public class PortScanner : IPortScanner
    {
        private readonly ITcpClient _tcpClient;
        private readonly IWriter _writer;
        private readonly INetworkInterface _networkInterface;

        private uint _ip;
        private uint _mask;

        private uint NetworkAddress => _ip & _mask;
        private uint BroadcastAddress => NetworkAddress + ~_mask;
        private List<IPAddress> HostsToScan { get; }

        public PortScanner(ITcpClient tcpClient, IWriter writer, INetworkInterface networkInterface, List<IPAddress> hostsToScan = null)
        {
            _tcpClient = tcpClient;
            _writer = writer;
            _networkInterface = networkInterface;

            var clientIp = _networkInterface.GetLocalIpAddress();
            _ip = clientIp.ParseToIp();
            _mask = _networkInterface.GetSubnetMask(clientIp).ParseToIp();
            HostsToScan = hostsToScan ?? IpHelper.GetSubnetHosts(_ip, NetworkAddress, BroadcastAddress);
        }

        public async Task StartScanningAsync(int maxPortToScan = 65535)
        {
            var taskResults = new List<Task<ConnectionResult>>();

            foreach (var ip in HostsToScan)
            {
                Parallel.For(0, maxPortToScan, (port) =>
              {
                  taskResults.Add(_tcpClient.ConnectAsync(ip, port));
              });
            }

            await Task.WhenAll(taskResults);

            var connectionResults = taskResults.Where(x => x.Result.Connected).Select(x => x.Result).ToList();

            if (connectionResults.Count > 0)
                await _writer.WriteAsync(connectionResults);
        }
    }
}