using Puffincast.Universal.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Puffincast.Universal.ViewModels
{
    class MainViewModel
    {
        public IEnumerable<AreaViewModel> Areas { get; }

        public MainViewModel() : this(new PuffinClient())
        {
        }

        

        public MainViewModel(IPuffinClient client)
        {
            this.Areas = new AreaViewModel[] {
                new NowPlayingViewModel(client),
                new PlaylistViewModel(),
                new QueueViewModel()
            };

        }
    }
}
