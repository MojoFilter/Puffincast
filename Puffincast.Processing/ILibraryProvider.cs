using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;

namespace Puffincast.Processing
{
    public interface ILibraryProvider
    {
        Task<IEnumerable<Track>> Search(string yolo);
        Task<IEnumerable<Track>> Search(object fields);
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
        private static readonly string UriBase = "http://shout.danhax.com:4801/";

        private static readonly string YoloTemplate =
            "right.html?query=%3FARTIST+HAS+\"{0}\"+or+ALBUM+HAS+\"{0}\"+or+TITLE+HAS+\"{0}\"+or+TRACKNO+HAS+\"{0}\"+or+GENRE+HAS+\"{0}\"+or+FILENAME+HAS+\"{0}\"+or+YEAR+HAS+\"{0}\"+or+COMMENT+HAS+\"{0}\"";

        private async Task<HttpWebResponse> Get(string uri) =>
            await WebRequest.CreateHttp(UriBase + uri).GetResponseAsync() as HttpWebResponse;

        public async Task<bool> Enqueue(string key)
        {
            var uri = "left.html?enqueue=q&query=" + key;
            return (await Get(uri)).StatusCode == HttpStatusCode.OK;
        }

        public Task<IEnumerable<Track>> Search(string yolo) =>
            Query(string.Format(YoloTemplate, yolo));

        private async Task<IEnumerable<Track>> Query(string uri)
        {
            XNamespace ns = "http://www.w3.org/1999/xhtml";
            using (var response = await Get(uri))
            using (var reader = new StreamReader(response.GetResponseStream()))
            {
                try
                {
                    var doc = XDocument.Parse(PreprocessXhtml(await reader.ReadToEndAsync()));
                    return
                        from album in doc.Descendants(ns + "table")
                        where (string)album.Attribute("class") == "album"
                        let artist = album.Descendants(ns + "a").First().Value
                        from track in album.Descendants(ns + "tr").Skip(2)
                        let title = track.Elements(ns + "td").First(t => (string)t.Attribute("class") == "track_name").Value
                        let name = $"{artist} - {title}"
                        let key = FormatKey(artist, title)
                        select new Track(name, key);
                }
                catch (XmlException)
                {
                    return Enumerable.Empty<Track>();
                }
            }
        }

        public Task<IEnumerable<Track>> Search(object fields)
        {
            var parameters = string.Join("+and+",
                fields.GetType()
                .GetProperties()
                .Select(p => $"{p.Name.ToUpper()}+LIKE+\"{p.GetValue(fields)}\""));
            var uri = "right.html?query=%3F" + parameters;
            return Query(uri);
        }

        static string FormatKey(string artist, string title) =>
            $"%3FARTIST+LIKE+\"{WebUtility.UrlEncode(artist)}\"+and+TITLE+LIKE+\"{WebUtility.UrlEncode(title)}\"";
        
        private static string PreprocessXhtml(string xhtml)
        {
            var docType = @"<!DOCTYPE .*>";
            var newDocType = @"<!DOCTYPE html PUBLIC ""-//W3C//DTD XHTML 1.0 Strict//EN"" ""http://www.w3.org/TR/xhtml1/DTD/xhtml1-strict.dtd"" [
   <!ENTITY euml ""&#235;"">
   <!ENTITY nbsp ""&#160;"">
]>";
            return Regex.Replace(xhtml, docType, newDocType);
        }
    }
}
