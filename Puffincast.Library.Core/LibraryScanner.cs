using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;
using System.Text;
using System.Threading.Tasks;

namespace Puffincast.Library.Core
{
    public class LibraryScanner
    {
        private static readonly HashSet<string> extensions = new HashSet<string>(
            typeof(TagLib.SupportedMimeType).Assembly.GetExportedTypes()
            .SelectMany(t => t.GetCustomAttributes(false))
            .OfType<TagLib.SupportedMimeType>()
            .Where(m => !string.IsNullOrWhiteSpace(m.Extension))
            .Select(m => m.Extension));

        private IEnumerable<string> DiscoverFiles(string path) =>
            new DirectoryInfo(path).EnumerateFiles("*", SearchOption.AllDirectories)
                .Where(f => !string.IsNullOrWhiteSpace(f.Extension) && extensions.Contains(f.Extension.Substring(1)))
                .Select(f => f.FullName);

        private Task<int> ProcessFile(TagLib.File file) => Task.FromResult(0);

        public IObservable<ScanProgress> Scan(string path) => Observable.Create<ScanProgress>(obs =>
            {
                var fileList = this.DiscoverFiles(path).ToList();
                var files = fileList
                .Select((filename, index) => new ScanProgress(index / (double)fileList.Count, filename))
                .ToObservable()
                .Publish()
                .RefCount();

                var processing = files
                    .Select(f => f.Status)
                    .Select(filename =>
                    {
                        TagLib.File file = null;
                        bool success = false;
                        try
                        {
                            file = TagLib.File.Create(filename);
                            success = true;
                        }
                        catch (TagLib.UnsupportedFormatException) { }
                        return new { file, success };
                    })
                    .Where(x => x.success)
                    .Select(x => x.file)
                    .SelectMany(this.ProcessFile);

                return new CompositeDisposable(
                        files.Subscribe(obs.OnNext, obs.OnError),
                        processing.Subscribe(_ => { }, obs.OnCompleted));
            });



        public struct ScanProgress
        {
            public double Percent { get; }
            public string Status { get; }
            public ScanProgress(double percent, string status)
            {
                this.Percent = percent;
                this.Status = status;
            }
        }

    }
}
