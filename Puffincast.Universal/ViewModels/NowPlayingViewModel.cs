using Puffincast.Processing;
using Puffincast.Universal.Models;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Puffincast.Universal.ViewModels
{
    public class NowPlayingViewModel : AreaViewModel
    {
        public NowPlayingViewModel(IObservable<Playlist> playlist) : base("Now Playing")
        {
            this._NowPlaying = playlist.Select(p=>p?.Current).ToProperty(this, x => x.NowPlaying);
            this._LastPlayed = playlist.Select(l => l?.Last).ToProperty(this, x => x.LastPlayed);
            this._Next = playlist.Select(l => l?.Next.Take(5)).ToProperty(this, x => x.Next);
        }

        private ObservableAsPropertyHelper<string> _NowPlaying;
        public string NowPlaying { get { return _NowPlaying.Value; } }

        private ObservableAsPropertyHelper<string> _LastPlayed;
        public string LastPlayed { get { return _LastPlayed.Value; } }

        private ObservableAsPropertyHelper<IEnumerable<string>> _Next;
        public IEnumerable<string> Next { get { return this._Next.Value; } }
    }
}
