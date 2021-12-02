using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using PortScanner.Interfaces;

namespace PortScanner.Tests
{
    public class Tests
    {
        private Mock<IWriter> _writer;
        private Mock<IPortScanner> _portScanner;

        [SetUp]
        public void Setup()
        {
            _writer = new Mock<IWriter>();
            _portScanner = new Mock<IPortScanner>();
        }


        [Test]
        public async Task Test1()
        {
            //_writer.Setup(x => x.WriteToFileAsync(It.IsAny<List<ConnectionResult>>()));
            _portScanner.Setup(x=>x.)
            var portScanner = new PortScanner(_writer.Object);
            await portScanner.StartScanningAsync();

            _writer.Verify(x => x.WriteToFileAsync(It.IsAny<List<ConnectionResult>>()), Times.Once());
            //Assert.Pass();
        }
    }
}