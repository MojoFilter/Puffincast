using Puffincast.Universal.Models;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Puffincast.Universal.ViewModels
{
    public class MainViewModel : ReactiveObject
    {
        public IEnumerable<AreaViewModel> Areas { get; }

        public ICommand Play { get; }
        public ICommand Previous { get; }
        public ICommand Pause { get; }
        public ICommand Next { get; }
        public ICommand Refresh { get; }
        public ICommand Settings { get; }
        public ICommand Clear { get; }

        public MainViewModel() : this(new PuffinClient())
        {
        }

        

        public MainViewModel(IPuffinClient client)
        {
            var playlist = client.WhenAnyValue(x => x.Playlist).ObserveOn(RxApp.MainThreadScheduler);

            this.Areas = new AreaViewModel[] {
                new NowPlayingViewModel(playlist),
                new PlaylistViewModel(),
                new QueueViewModel()
            };

            this.Play = ReactiveCommand.CreateAsyncTask(_ => client.Play());
            this.Pause = ReactiveCommand.CreateAsyncTask(_ => client.Pause());
            this.Next = ReactiveCommand.CreateAsyncTask(_ => client.Next());
            this.Previous = ReactiveCommand.CreateAsyncTask(_ => client.Previous());
            this.Clear = ReactiveCommand.CreateAsyncTask(_ => client.Clear());
        }
       
    }
}
