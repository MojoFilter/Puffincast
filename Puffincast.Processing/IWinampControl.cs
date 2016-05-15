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
        Task<Playlist> GetPlaylist();
        Task<string[]> GetPreviousPlaylist();
        Task<int> GetCurrentPlaylistPosition();
        Task<string> GetNowPlaying();
        Task<bool> Next();
        Task<bool> Play();
        Task<bool> Pause();
        Task<bool> Prev();
        Task<bool> Clear();
    }

    public class HttpQWinampControl : IWinampControl
    {
        private static readonly char Delimiter = '\n';

        private ISettingsProvider settings;

        public HttpQWinampControl(ISettingsProvider settings)
        {
            this.settings = settings;
        }

        public async Task<Playlist> GetPlaylist()
        {
            var parameters = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("delim", Delimiter.ToString())
            };

            var fullList =
            (await this.Request("getplaylisttitlelist", parameters))
            .Split(Delimiter);
            var pos = await GetCurrentPlaylistPosition();
            return new Playlist
            {
                Last = (pos == 0) ? null : fullList[pos - 1],
                Current = fullList[pos],
                Next = fullList.Skip(pos + 1)
            };
        }

        public async Task<string[]> GetPreviousPlaylist()
        {
            var parameters = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("delim", Delimiter.ToString())
            };

            var fullList =
            (await this.Request("getplaylisttitlelist", parameters))
            .Split(Delimiter);

            return fullList;
        }

        public async Task<int> GetCurrentPlaylistPosition()
        {
            var pos = Convert.ToInt32(await this.Request("getlistpos"));
            return pos;
        }

        public Task<string> GetNowPlaying() => this.Request("getcurrenttitle");

        private async Task<string> Request(string command, IEnumerable<KeyValuePair<string, string>> parameters = null)
        {
            Func<string, string> encode = WebUtility.UrlEncode;
            var cfg = new ConnectionInfo(this.settings.ControlConnectionString);

            var uriParams = parameters == null ? string.Empty :
                string.Join(string.Empty,
                    parameters
                    .Select(p => $"&{encode(p.Key)}={encode(p.Value)}"));

            var uri = $"{cfg.BaseUri}/{command}?p={encode(cfg.Password)}{uriParams}";

            using (var response = (HttpWebResponse)(await WebRequest.CreateHttp(uri).GetResponseAsync()))
            using (var reader = new StreamReader(response.GetResponseStream()))
            {
                return await reader.ReadToEndAsync();
            }
        }

        private async Task<bool> Try(string command, IEnumerable<KeyValuePair<string, string>> parameters = null) =>
            await this.Request(command, parameters) == "1";

        public Task<bool> Next() => Try("next");

        public Task<bool> Play() => Try("play");

        public Task<bool> Pause() => Try("pause");

        public Task<bool> Prev() => Try("prev");

        public Task<bool> Clear() => Try("delete");
    }
}
