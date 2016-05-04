using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
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
        private IWinampControl control;
        private ISettingsProvider settings;

        public CommandHandler(ISettingsProvider settings, IWinampControl control = null)
        {
            Contract.Requires(settings != null);

            this.settings = settings;
            this.control = control ?? new HttpQWinampControl(this.settings);
        }
        public async Task<string> Execute(string user, string command)
        {
            var list = await this.control.GetPlaylist();
            return string.Join("\n",
                list.Select((t, i) => $"{i + 1}) {t}"));
        }
    }
}
