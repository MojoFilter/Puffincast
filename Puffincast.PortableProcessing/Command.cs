using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Puffincast.Processing
{
    class Command
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public Func<string, string, Task<string>> Invoke { get; set; }
    }
}
