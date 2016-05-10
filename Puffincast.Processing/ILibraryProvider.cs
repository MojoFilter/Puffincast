using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Puffincast.Processing
{
    public interface ILibraryProvider
    {
        Task<IEnumerable<Track>> Search(string yolo);
        Task<bool> Enqueue(string key);
    }

    public struct Track
    {
        public string Name { get; }
        public string Key { get; }

        public Track(string name, string key)
        {
            this.Name = name;
            this.Key = key;
        }
    }

    public class MlwwwLibraryProvider : ILibraryProvider
    {
        private static readonly string UriBase = "http://shout.danhax.com:81/";

        private static readonly string YoloTemplate =
            "right.html?query=%3FARTIST+HAS+\"{0}\"+or+ALBUM+HAS+\"{0}\"+or+TITLE+HAS+\"{0}\"+or+TRACKNO+HAS+\"{0}\"+or+GENRE+HAS+\"{0}\"+or+FILENAME+HAS+\"{0}\"+or+YEAR+HAS+\"{0}\"+or+COMMENT+HAS+\"{0}\"";

        private async Task<HttpWebResponse> Get(string uri) =>
            await WebRequest.CreateHttp(UriBase + uri).GetResponseAsync() as HttpWebResponse;

        public async Task<bool> Enqueue(string key)
        {
            var uri = "left.html?enqueue=q&query=" + key;
            return (await Get(uri)).StatusCode == HttpStatusCode.OK;
        }

        public async Task<IEnumerable<Track>> Search(string yolo)
        {
            var uri = string.Format(YoloTemplate, yolo);
            XNamespace ns = "http://www.w3.org/1999/xhtml";
            using (var response = await Get(uri))
            {
                return
                    from album in XDocument.Load(response.GetResponseStream()).Descendants(ns + "table")
                    where (string)album.Attribute("class") == "album"
                    let artist = album.Descendants(ns + "a").First().Value
                    from track in album.Descendants(ns + "tr").Skip(2)
                    let title = track.Elements(ns + "td").First(t => (string)t.Attribute("class") == "track_name").Value
                    let name = $"{artist} - {title}"
                    let key = FormatKey(artist, title)
                    select new Track(name, key);
            }
        }

        static string FormatKey(string artist, string title) =>
            $"%3FARTIST+HAS+\"{WebUtility.UrlEncode(artist)}\"+and+TITLE+HAS+\"{WebUtility.UrlEncode(title)}\"";
        

    }
}
