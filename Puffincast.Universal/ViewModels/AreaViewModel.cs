using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Puffincast.Universal.ViewModels
{
    public abstract class AreaViewModel : ReactiveObject
    {
        public string Title { get; }

        public AreaViewModel(string title)
        {
            this.Title = title;
        }
    }
}
