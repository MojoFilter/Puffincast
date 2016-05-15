using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Puffincast.Processing
{
    public struct Track
    {
        public string Name { get; }
        public string Key { get; }

        public Track(string name, string key)
        {
            this.Name = name;
            this.Key = key;
        }
    }
}
