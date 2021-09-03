using System;
using System.Threading.Tasks;

namespace PortScanner
{
    class Program
    {
        static async Task Main(string[] args)
        {
            IPortScanner portScanner = new PortScanner();

            await portScanner.StartScanningAsync();
            await portScanner.WriteResultsToFileAsync();

            Console.ReadKey();
            Console.ReadKey();
        }
    }
}