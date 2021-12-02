using PortScanner.Interfaces;
using PortScanner.Service;
using System;
using System.Threading.Tasks;

namespace PortScanner
{
    class Program
    {
        static async Task Main(string[] args)
        {
            IWriter writer = new Writer();

            IPortScanner portScanner = new PortScanner(writer);

            await portScanner.StartScanningAsync();

            Console.ReadKey();
            Console.ReadKey();
        }
    }
}