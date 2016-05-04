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
        Task<string> GetNowPlaying();
        Task<bool> Next();
        Task<bool> Play();
        Task<bool> Pause();
        Task<bool> Prev();
    }

    public class HttpQWinampControl : IWinampControl
    {
        private static readonly char Delimiter = '\n';

        private ISettingsProvider settings;

        public HttpQWinampControl(ISettingsProvider settings)
        {
            this.settings = settings;
        }

        public async Task<IEnumerable<string>> GetPlaylist() =>
            (await this.Request("getplaylisttitlelist", new { delim = Environment.NewLine }))
            .Split(Delimiter);

        public Task<string> GetNowPlaying() => this.Request("getcurrenttitle");
        

        private async Task<string> Request(string command, object parameters = null)
        {
            Func<string, string> encode = WebUtility.UrlEncode;
            var cfg = new ConnectionInfo(this.settings.ControlConnectionString);
            var uriParams = parameters == null ? string.Empty :
                string.Join(string.Empty,
                    parameters.GetType().GetProperties()
                    .Select(p => $"&{encode(p.Name)}={encode(p.GetValue(parameters) as string)}"));
            
            var uri = $"{cfg.BaseUri}/{command}?p={encode(cfg.Password)}{uriParams}";
                
            using (var response = (HttpWebResponse)(await WebRequest.CreateHttp(uri).GetResponseAsync()))
            using (var reader = new StreamReader(response.GetResponseStream()))
            {
                return await reader.ReadToEndAsync();
            }
            
        }

        private async Task<bool> Try(string command, object parameters = null) =>
            await this.Request(command, parameters) == "1";

        public Task<bool> Next() => Try("next");

        public Task<bool> Play() => Try("play");

        public Task<bool> Pause() => Try("pause");

        public Task<bool> Prev() => Try("prev");

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
