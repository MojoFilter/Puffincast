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

    }
}
