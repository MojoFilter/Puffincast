using Puffincast.Processing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;

namespace Puffincast.Universal
{
    public class SettingsProvider : ISettingsProvider
    {
        public string ControlConnectionString { get; }

        public SettingsProvider(string connectionString = null)
        {
            ApplicationDataContainer AppSettings = ApplicationData.Current.LocalSettings;      
            ControlConnectionString = (connectionString ?? AppSettings.Values["puffincastUri"]?.ToString())
                ?? "pass@localhost:4800";
        }
    }
}
