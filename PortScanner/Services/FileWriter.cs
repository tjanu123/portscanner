using PortScanner.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PortScanner.Service
{
    public class FileWriter : IWriter
    {
        public async Task WriteAsync(List<ConnectionResult> connectionResults)
        {
            await using var writer = new StreamWriter("PortScannerResults.txt");
            foreach (var result in connectionResults)
            {
                await writer.WriteLineAsync($"CONNECTED  IP: {result.IpAddress} - port number: {result.Port}");
            }
        }
    }
}
