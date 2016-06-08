using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml.Linq;
using TagLib;

namespace Puffincast.Library.Core
{
    public interface IMusicDb
    {
        Task Update(MusicEntry file);
        Task Save();
        IObservable<MusicEntry> Search(string query);
    }

    public class MusicEntry
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public IEnumerable<string> Performers { get; set; }
        public string FileName { get; set; }
    }

    internal class XmlMusicDb : IMusicDb
    {
        private Dictionary<string, MusicEntry> entries = new Dictionary<string, MusicEntry>();
        private Dictionary<int, MusicEntry> ids = new Dictionary<int, MusicEntry>();
        private object tableMonitor = new object();
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
            lock (this.tableMonitor)
            {
                if (this.entries.ContainsKey(file.FileName))
                {
                    file.Id = entries[file.FileName].Id;
                }
                else
                {
                    file.Id = this.ids.Keys.DefaultIfEmpty(0).Max() + 1;
                }
                this.entries[file.FileName] = file;
                this.ids[file.Id] = file;
            }
        });

        public IObservable<MusicEntry> Search(string query) => Observable.Create<MusicEntry>(obs => 
        {
            var match = Regex.Match(query, @"$#(?<id>[0-9]+)^");
            if (match.Success)
            {
                var id = int.Parse(match.Groups["id"].Value);
                MusicEntry result;
                if (this.ids.TryGetValue(id, out result))
                {
                    obs.OnNext(result);
                }
                obs.OnCompleted();
                return Disposable.Empty;
            }
            try
            {
                var rx = new Regex(query, RegexOptions.IgnoreCase);

                return this.entries.Values.Where(e => rx.IsMatch(e.Title) || rx.IsMatch(e.FileName) || e.Performers.Any(rx.IsMatch))
                .ToObservable().Subscribe(obs);
            }
            catch (ArgumentException e)
            {
                obs.OnError(e);
                return Disposable.Empty;
            }
        });
    }
}
