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

        public override int GetHashCode()
        {
            return (this.Last?.GetHashCode() ?? 0)
                ^ (this.Current?.GetHashCode() ?? 0)
                ^ (this.Next?.Aggregate(0, (a, v) => a ^ v.GetHashCode()) ?? 0);
        }

        public override bool Equals(object obj)
        {
            var pl = obj as Playlist;
            if (pl != null)
            {
                return this.Last == pl.Last
                    && this.Current == pl.Current
                    && this.Next.SequenceEqual(pl.Next);
            }
            return base.Equals(obj);
        }
    }

}
