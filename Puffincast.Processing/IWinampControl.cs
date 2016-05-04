using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Puffincast.Processing
{
    public interface IWinampControl
    {
        Task<IEnumerable<string>> GetPlaylist();
    }

    public class HttpQWinampControl : IWinampControl
    {
        private static readonly char Delimiter = '\n';

        private ISettingsProvider settings;

        public HttpQWinampControl(ISettingsProvider settings)
        {
            this.settings = settings;
        }

        public async Task<IEnumerable<string>> GetPlaylist()
        {
            return (await this.Request("getplaylisttitlelist", new { delim = Environment.NewLine })).Split(Delimiter);
        }

        private async Task<string> Request(string command, object parameters)
        {
            Func<string, string> encode = WebUtility.UrlEncode;
            var cfg = new ConnectionInfo(this.settings.ControlConnectionString);
            var uri = $"{cfg.BaseUri}/{command}?p={encode(cfg.Password)}" +
                string.Join(string.Empty,
                    parameters.GetType().GetProperties()
                    .Select(p => $"&{encode(p.Name)}={encode(p.GetValue(parameters) as string)}"));

            using (var response = (HttpWebResponse)(await WebRequest.CreateHttp(uri).GetResponseAsync()))
            using (var reader = new StreamReader(response.GetResponseStream()))
            {
                return await reader.ReadToEndAsync();
            }
            
        }

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
}
