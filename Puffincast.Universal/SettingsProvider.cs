using Puffincast.Processing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Puffincast.Universal
{
    public class SettingsProvider : ISettingsProvider
    {
        public string ControlConnectionString { get; }

        public SettingsProvider(string connectionString)
        {
            ControlConnectionString = connectionString;
        }
    }
}
