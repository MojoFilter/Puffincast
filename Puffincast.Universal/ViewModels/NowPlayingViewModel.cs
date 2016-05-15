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
    class NowPlayingViewModel : AreaViewModel
    {
        public NowPlayingViewModel(IPuffinClient client) : base("Now Playing")
        {
            var playlist = client.WhenAnyValue(x => x.Playlist);

            this._NowPlaying = playlist.Select(l => l?.Current).ToProperty(this, x => x.NowPlaying);
            this._LastPlayed = playlist.Select(l => l?.Last).ToProperty(this, x => x.LastPlayed);
            this._Next = playlist.Select(l => l?.Next).ToProperty(this, x => x.Next);

            playlist.Subscribe(l => Debug.WriteLine("Here you go: " + l?.Current), 
                e => Debug.WriteLine("Something bad: " + e.Message),
                () => Debug.WriteLine("Finished unexpectedly"));
        }

        private ObservableAsPropertyHelper<string> _NowPlaying;
        public string NowPlaying { get { return _NowPlaying.Value; } }

        private ObservableAsPropertyHelper<string> _LastPlayed;
        public string LastPlayed { get { return _LastPlayed.Value; } }

        private ObservableAsPropertyHelper<IEnumerable<string>> _Next;
        public IEnumerable<string> Next { get { return this._Next.Value; } }
    }
}
