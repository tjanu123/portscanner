using Moq;
using NUnit.Framework;
using PortScanner.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Threading.Tasks;

namespace PortScanner.Tests
{
    public class Tests
    {
        private Mock<IWriter> _writer;
        private Mock<ITcpClient> _tcpClient;
        private Mock<INetworkInterface> _networkInterface;
        private IPortScanner _portScanner;

        private IPAddress _ipAddress;
        private IPAddress _subnetMask;

        [SetUp]
        public void Setup()
        {
            _writer = new Mock<IWriter>();
            _tcpClient = new Mock<ITcpClient>();
            _networkInterface = new Mock<INetworkInterface>();
            _ipAddress = IPAddress.Parse("192.168.0.1");
            _subnetMask = IPAddress.Parse("255.255.255.254");
            _networkInterface.Setup(x => x.GetLocalIpAddress()).Returns(_ipAddress);
            _networkInterface.Setup(x => x.GetSubnetMask(It.IsAny<IPAddress>())).Returns(_subnetMask);

            _portScanner = new PortScanner(_tcpClient.Object, _writer.Object, _networkInterface.Object, new List<IPAddress> { _ipAddress });
        }

        [Test]
        public async Task StartScanningAsync_Success()
        {
            _writer.Setup(x => x.WriteAsync(It.IsAny<List<ConnectionResult>>()));
            _tcpClient.Setup(x => x.ConnectAsync(It.IsAny<IPAddress>(), It.IsAny<int>())).ReturnsAsync(new ConnectionResult { IpAddress = IPAddress.Any, Port = 1, Connected = true });

            await _portScanner.StartScanningAsync(1);

            _tcpClient.Verify(x => x.ConnectAsync(It.IsAny<IPAddress>(), It.IsAny<int>()), Times.AtLeastOnce);
            _writer.Verify(x => x.WriteAsync(It.IsAny<List<ConnectionResult>>()), Times.Once());
        }

        [Test]
        public async Task StartScanningAsync_NoneOfPortsOpen_WriterDidNotWrite()
        {
            _writer.Setup(x => x.WriteAsync(It.IsAny<List<ConnectionResult>>()));
            _tcpClient.Setup(x => x.ConnectAsync(It.IsAny<IPAddress>(), It.IsAny<int>())).ReturnsAsync(new ConnectionResult { IpAddress = IPAddress.Any, Port = 1, Connected = false });

            await _portScanner.StartScanningAsync(1);

            _tcpClient.Verify(x => x.ConnectAsync(It.IsAny<IPAddress>(), It.IsAny<int>()), Times.AtLeastOnce);
            _writer.Verify(x => x.WriteAsync(It.IsAny<List<ConnectionResult>>()), Times.Never);
        }

        [Test]
        public void StartScanningAsync_WriterIOException()
        {
            _writer.Setup(x => x.WriteAsync(It.IsAny<List<ConnectionResult>>())).Throws<IOException>();
            _tcpClient.Setup(x => x.ConnectAsync(It.IsAny<IPAddress>(), It.IsAny<int>())).ReturnsAsync(new ConnectionResult { IpAddress = IPAddress.Any, Port = 1, Connected = true });

            Assert.ThrowsAsync<IOException>(async () => await _portScanner.StartScanningAsync(1));
            _tcpClient.Verify(x => x.ConnectAsync(It.IsAny<IPAddress>(), It.IsAny<int>()), Times.AtLeastOnce);
            _writer.Verify(x => x.WriteAsync(It.IsAny<List<ConnectionResult>>()), Times.Once());
        }

        [Test]
        public void StartScanningAsync_NoneOfPortsOpen_TcpClientThrowsAggregateException()
        {
            _tcpClient.Setup(x => x.ConnectAsync(It.IsAny<IPAddress>(), It.IsAny<int>())).Throws<Exception>();
            _writer.Setup(x => x.WriteAsync(It.IsAny<List<ConnectionResult>>()));

            Assert.ThrowsAsync<AggregateException>(async () => await _portScanner.StartScanningAsync(1));
            _tcpClient.Verify(x => x.ConnectAsync(It.IsAny<IPAddress>(), It.IsAny<int>()), Times.AtLeastOnce);
            _writer.Verify(x => x.WriteAsync(It.IsAny<List<ConnectionResult>>()), Times.Never);
        }
    }
}