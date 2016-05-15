using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Puffincast.Processing
{
    public class Playlist
    {
        public string Last { get; set; }
        public string Current { get; set; }
        public IEnumerable<string> Next { get; set; }
    }

}
