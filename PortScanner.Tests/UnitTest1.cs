using System;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;

namespace PortScanner.Tests
{
    public class Tests
    {
        [SetUp]
        public void Setup()
        {
        }


        [Test]
        public async Task Test1()
        {
            //var scannerMock = new Mock<IPortScanner>().Setup(x=>).

            var localIp = IpHelpers.GetLocalIpAddress();
            var subnetMask = localIp.GetSubnetMask();
            var portScanner = new PortScanner(IPAddress.Parse("127.0.0.3"), IPAddress.Parse("255.255.255.254"));
            await portScanner.StartScanningAsync();
            await portScanner.WriteResultsToFileAsync();
            Assert.Pass();
        }
    }
}