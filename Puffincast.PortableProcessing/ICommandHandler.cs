using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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
        private ILibraryProvider library;
        private IPuffinBot puffinBot;

        private IEnumerable<Command> commands;

        public CommandHandler(ISettingsProvider settings, IWinampControl control = null, 
            ILibraryProvider library = null, IPuffinBot puffinBot = null)
        {
            Contract.Requires(settings != null);

            this.settings = settings;
            this.control = control ?? new HttpQWinampControl(this.settings);
            this.library = library ?? new MlwwwLibraryProvider();
            this.puffinBot = puffinBot ?? new PuffinBot();

            this.commands = this.InitCommands();
        }

        public async Task<string> Execute(string user, string commandText)
        {
            //commandText = commandText ?? string.Empty;
            var command = commandText ?? string.Empty; //.Split(new[] { ' ' }, 1).First();
            Command cmd = this.commands.FirstOrDefault(c => command.StartsWith(c.Name,StringComparison.CurrentCultureIgnoreCase));
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

        private async Task<string> List(string user, string cmd)
        {
            var playlist = await this.control.GetPlaylist();
            var result = new StringBuilder();
            result.AppendFormat("You're hearing:\n\t*{0}*\n\n", playlist.Current);
            if (playlist.Last != null)
            {
                result.AppendFormat("You just heard:\n\t*{0}*\n\n", playlist.Last);
            }
            result.AppendLine("Coming Up:");
            result.Append(string.Join(Environment.NewLine, playlist.Next.Select((t, i) => $"{i + 1}) {t}")));
            return result.ToString();
        }

        private Task<string> Status(string user, string cmd) =>
            this.control.GetNowPlaying();

        private Random rnd = new Random();

        private Task<string> Play(string user, string cmd) => this.Play(user, cmd, true);

        private async Task<string> Play(string user, string cmd, bool allowFuzzy) {
            if (cmd.Length > 5)
            {
                var query = cmd.Substring(5);
                IEnumerable<Track> matches;
                var rxMatch = Regex.Match(query, "(?<artist>.+) - (?<title>.+)");
                if (rxMatch.Success)
                {
                    var searchStuff = new List<KeyValuePair<string, string>>
                    {
                        new KeyValuePair<string, string>("Artist", rxMatch.Groups["artist"].Value),
                        new KeyValuePair<string, string>("Title", rxMatch.Groups["title"].Value)
                    };

                    matches = await this.library.Search(searchStuff);
                }
                else
                {
                    matches = (await this.library.Search(query)).ToList();
                }
                if (matches.Any())
                {
                    if (!allowFuzzy && matches.Count() > 1)
                    {
                        return "Be more specific. Your search matched the following tracks:\n" +
                            string.Join("\n", matches.Select(t => t.Name));
                    }
                    var pick = matches.ElementAt(rnd.Next(0, matches.Count()));
                    if (await this.library.Enqueue(pick.Key))
                    {
                        await puffinBot.NotifyEnqueue(user, pick.Name);
                        return $":+1: Loaded up _{pick.Name}_";
                    }
                    else
                    {
                        return $":-1: Had some kind of problem trying to play _{pick.Name}_";
                    }
                }
                else
                {
                    return $"Couldn't find anything to match _{query}_";
                }
            }
            else
            {
                return await Try(control.Play());
            }
        }

        private Task<string> Pick(string user, string cmd) => this.Play(user, cmd, false);


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
                Description = "Play the currently queued track, or queue up a random track that matches the search term provided",
                Invoke = Play
            },
            new Command
            {
                Name = "Pause",
                Description = "Pause PuffinCast radio",
                Invoke = (_, __) => Try(control.Pause())
            },
            new Command
            {
                Name = "Pick",
                Description = "Enqueue a specific track",
                Invoke = Pick
            },
            new Command
            {
                Name = "Clear",
                Description = "Clears the queue.  These things exist because",
                Invoke = (_, __) => Try(control.Clear())
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
