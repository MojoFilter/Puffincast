using Microsoft.Band;
using Microsoft.Band.Tiles;
using Microsoft.Band.Tiles.Pages;
using Puffincast.Processing;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.UI.Xaml.Media.Imaging;

namespace Puffincast.Universal.ViewModels
{
    public class MainPageViewModel : INotifyPropertyChanged
    {
        private Playlist playlist { get; set; }
        private IEnumerable<String> previousPlaylist { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;

        public CommandHandler CommandHandler { get; set; }

        public Playlist Playlist
        {
            get { return playlist; }
            set
            {
                playlist = value;
                NotifyPropertyChanged("Playlist");
            }
        }
        public IEnumerable<string> PreviousPlaylist
        {
            get { return previousPlaylist; }
            set
            {
                previousPlaylist = value;
                NotifyPropertyChanged("PreviousPlaylist");
            }
        }

        private void NotifyPropertyChanged(string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public MainPageViewModel()
        {
            ApplicationDataContainer AppSettings = ApplicationData.Current.LocalSettings;

            var connection = AppSettings.Values["puffincastUri"]?.ToString();

            ISettingsProvider settings = new SettingsProvider(connection);
            CommandHandler = new CommandHandler(settings);
        }

        public async Task GetPlaylist()
        {
            Playlist = await CommandHandler.Control.GetPlaylist();
        }
        public async Task GetPreviousPlaylist()
        {
           var list = await CommandHandler.Control.GetPreviousPlaylist();
            previousPlaylist = list.Reverse();
        }

        public async Task<string> Queue(string artist, string song)
        {
            var isPick = artist != string.Empty && song != string.Empty;
            var isYolo = artist == string.Empty && song != string.Empty;

            if (isPick)
            {
                var command = "pick " + artist + " - " + song;
                var message = await CommandHandler.Pick("uwp", command);
                return message;
            }
            else if (isYolo)
            {
                var command = "play " + song;
                var message = await CommandHandler.Pick("uwp", command);
                return message;
            }
            else
            {
                var message = "The artist name is optional, but a song name is required.";
                return message;
            }
        }
    }
}
