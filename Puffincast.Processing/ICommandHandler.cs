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

        private IEnumerable<Command> commands;

        public CommandHandler(ISettingsProvider settings, IWinampControl control = null)
        {
            Contract.Requires(settings != null);

            this.settings = settings;
            this.control = control ?? new HttpQWinampControl(this.settings);
            this.commands = this.InitCommands();
        }

        public async Task<string> Execute(string user, string commandText)
        {
            commandText = commandText ?? string.Empty;
            var command = commandText.Split(new[] { ' ' }, 1).First();
            Command cmd = this.commands.FirstOrDefault(c => command.StartsWith(c.Name, StringComparison.InvariantCultureIgnoreCase));
            if (cmd == null)
            {
                return "Huh? " + await this.Help(user, command);
            }
            return await cmd.Invoke(user, command);
        }

        private Task<string> Help(string user, string cmd) =>
            Task.FromResult("Here are the commands:\n" +
                string.Join(Environment.NewLine,
                    this.commands.OrderBy(c=>c.Name)
                    .Select((c, i) =>
                    $"*{c.Name.PadRight(15)}* {c.Description}")));

        private async Task<string> List(string user, string cmd) =>
            string.Join(Environment.NewLine,
                (await this.control.GetPlaylist())
                .Select((t, i) => $"{i + 1}) {t}"));

        private Task<string> Status(string user, string cmd) =>
            this.control.GetNowPlaying();

        private async Task<string> Try(Task<bool> cmd) => await cmd ? ":+1:" : ":skull:";

        private IEnumerable<Command> InitCommands() => new[]
        {
            new Command
            {
                Name = "Help",
                Description = "List available commands",
                Invoke = this.Help
            },
            new Command
            {
                Name = "List",
                Description = "View the current playlist",
                Invoke = this.List
            },
            new Command
            {
                Name = "?",
                Description = "Get the current track",
                Invoke = (_, __) => this.control.GetNowPlaying()
            },
            new Command
            {
                Name = "Next",
                Description = "Skip to the next track",
                Invoke = (_, __) => Try(control.Next())
            },
            new Command
            {
                Name = "Prev",
                Description = "You know what this does",
                Invoke = (_, __) => Try(control.Prev())
            },
            new Command
            {
                Name = "Play",
                Description = "Play the currently queued track",
                Invoke = (_, __) => Try(control.Play())
            },
            new Command
            {
                Name = "Pause",
                Description = "Pause PuffinCast radio",
                Invoke = (_, __) => Try(control.Pause())
            }

        };

        class Command
        {
            public string Name { get; set; }
            public string Description { get; set; }
            public Func<string, string, Task<string>> Invoke { get; set; }
        }
    }
}
