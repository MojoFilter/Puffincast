using Puffincast.Processing;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;

namespace Puffincast.Models
{
    public class SettingsProvider : ISettingsProvider
    {
        public string ControlConnectionString
        {
            get
            {
                return ConfigurationManager.ConnectionStrings["Control"].ConnectionString;
            }
        }
    }
}