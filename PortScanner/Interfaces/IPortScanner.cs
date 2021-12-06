using System.Threading.Tasks;

namespace PortScanner
{
    public interface IPortScanner
    {
        Task StartScanningAsync(int maxPortToScan = 65536);
    }
}