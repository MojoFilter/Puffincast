using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Puffincast.Processing
{
    public interface ICommandHandler
    {
        Task<string> Execute(string user, string command);
    }

    public class CommandHandler : ICommandHandler
    {
        public Task<string> Execute(string user, string command)
        {
            return Task.FromResult("Send the mothership.");
        }
    }
}
