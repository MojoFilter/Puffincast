using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Puffincast.Processing
{
    class ConnectionInfo
    {
        public string BaseUri { get; }
        public string Password { get; }

        public ConnectionInfo(string connectionString)
        {
            var match = Regex.Match(connectionString, "(?<pass>.*)@(?<host>.*)");
            if (!match.Success)
            {
                throw new ArgumentException("Connection string should be in the format pass@host:port");
            }
            this.Password = match.Groups["pass"].Value;
            this.BaseUri = "http://" + match.Groups["host"].Value;
        }

    }
}
