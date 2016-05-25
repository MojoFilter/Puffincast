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
    public interface IPuffinClient : IReactiveObject
    {
        Playlist Playlist { get; }
        Task Previous();
        Task Play();
        Task Pause();
        Task Next();
        Task Clear();
    }

    class PuffinClient : ReactiveObject, IPuffinClient
    {
        private IWinampControl control;

        private ObservableAsPropertyHelper<Playlist> _Playlist;
        public Playlist Playlist { get { return _Playlist.Value; } }


        public PuffinClient(TimeSpan? updateRate = null, IWinampControl control = null)
        {
            TimeSpan rate = updateRate ?? TimeSpan.FromSeconds(2.0);
            this.control = control ?? new HttpQWinampControl(new SettingsProvider());
            var updateLoop = Observable.Interval(rate)
                .SelectMany(_ => this.control.GetPlaylist())
                .Retry()
                .Distinct()
                .Publish()
                .RefCount();

            this._Playlist = updateLoop
                .ToProperty(this, x => x.Playlist);
        }

        public Task Previous()
        {
            throw new NotImplementedException();
        }

        public Task Play()
        {
            Debug.WriteLine("PLAY");
            return Task.CompletedTask;
        }

        public Task Pause()
        {
            throw new NotImplementedException();
        }

        public Task Next()
        {
            throw new NotImplementedException();
        }

        public Task Clear()
        {
            throw new NotImplementedException();
        }
    }
}
