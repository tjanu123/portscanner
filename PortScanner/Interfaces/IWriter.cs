using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PortScanner.Interfaces
{
    public interface IWriter
    {
         Task WriteToFileAsync(List<ConnectionResult> connectionResults);
    }
}
