using Puffincast.Processing;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Puffincast.Universal.Models
{
    interface IPuffinClient : IReactiveObject
    {
        Playlist Playlist { get; }
    }

    class PuffinClient : ReactiveObject, IPuffinClient
    {
        private ObservableAsPropertyHelper<Playlist> _Playlist;
        public Playlist Playlist { get { return _Playlist.Value; } }


        public PuffinClient(TimeSpan? updateRate = null, IWinampControl control = null)
        {
            TimeSpan rate = updateRate ?? TimeSpan.FromSeconds(2.0);
            control = control ?? new HttpQWinampControl(new SettingsProvider());
            var updateLoop = Observable.Interval(rate)
                .SelectMany(_ => control.GetPlaylist())
                .Retry()
                .Distinct();

            this._Playlist = updateLoop
                .ToProperty(this, x => x.Playlist);
            updateLoop.Subscribe(p=>Debug.WriteLine(p.Current));
            this.WhenAnyValue(x => x.Playlist).Subscribe(p => Debug.WriteLine("From property: " + p?.Current));
            
        }
    }
}
