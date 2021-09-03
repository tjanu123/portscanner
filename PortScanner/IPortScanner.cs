using System.Threading.Tasks;

namespace PortScanner
{
    public interface IPortScanner
    {
        Task StartScanningAsync();
        Task WriteResultsToFileAsync();
    }
}