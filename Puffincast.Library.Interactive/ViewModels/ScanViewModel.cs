using Puffincast.Library.Core;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Puffincast.Library.Interactive.ViewModels
{
    public class ScanViewModel : ReactiveObject
    {
        public ReactiveCommand<LibraryScanner.ScanProgress> Scan { get; }

        private ObservableAsPropertyHelper<double> _Progress;
        public double Progress { get { return this._Progress.Value; } }

        private ObservableAsPropertyHelper<string> _Status;
        public string Status { get { return this._Status.Value; } }

        public ScanViewModel(LibraryScanner scanner, string libraryPath)
        {
            this.Scan = ReactiveCommand.CreateAsyncObservable(_ => scanner.Scan(libraryPath).Publish().RefCount());
            var testPattern = Observable.Interval(TimeSpan.FromMilliseconds(100)).Select(t => (t % 30) / 30.0).ObserveOnDispatcher();
            //this.Scan.Select(p => p.Percent)
            this._Progress = this.Scan.Select(p => p.Percent).ToProperty(this, x => x.Progress);
            this._Status = this.Scan.Select(p => p.Status).ToProperty(this, x => x.Status);
        }
    }
}
