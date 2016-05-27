using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using TagLib;

namespace Puffincast.Library.Core
{
    public interface IMusicDb
    {
        Task Update(MusicEntry file);
        Task Save();
    }

    public class MusicEntry
    {
        public string Title { get; set; }
        public IEnumerable<string> Performers { get; set; }
        public string FileName { get; set; }
    }

    internal class XmlMusicDb : IMusicDb
    {
        private ConcurrentDictionary<string, MusicEntry> entries = new ConcurrentDictionary<string, MusicEntry>();
        private string path;

        public XmlMusicDb(string path)
        {
            this.path = path;
        }

        public Task Save() => Task.Run(() =>
            new XDocument(new XElement("PuffinDb",
                from entry in this.entries.Values
                select new XElement("Track",
                    new XAttribute("path", entry.FileName),
                    new XAttribute("title", entry.Title),
                    entry.Performers.Select(p => new XElement("Performer", p)))))
                .Save(this.path));


        public Task Update(MusicEntry file) => Task.Run(() =>
        {
            this.entries.AddOrUpdate(file.FileName, file, (p, e) => file);
        });
    }
}
