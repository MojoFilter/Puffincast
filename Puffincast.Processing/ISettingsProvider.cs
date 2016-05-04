using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Puffincast.Processing
{
    public interface ISettingsProvider
    {
        string ControlConnectionString { get; }
    }
}
