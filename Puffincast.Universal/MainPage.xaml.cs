using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Popups;

using Puffincast.Universal.ViewModels;


namespace Puffincast.Universal
{
    public sealed partial class MainPage : Page
    {
        public MainPageViewModel Vm { get; set; }

        public MainPage()
        {
            this.InitializeComponent();
            Vm = new MainPageViewModel();
            this.DataContext = Vm;
            Loaded += OnLoad;
        }

        private void OnLoad(object sender, RoutedEventArgs e)
        {
            Refresh();
        }

        private async void Previous_Click(object sender, RoutedEventArgs e)
        {
            await Vm.CommandHandler.Control.Prev();
            Refresh();
        }

        private async void Play_Click(object sender, RoutedEventArgs e)
        {
            await Vm.CommandHandler.Control.Play();
            Refresh();
        }

        private async void Next_Click(object sender, RoutedEventArgs e)
        {
            await Vm.CommandHandler.Control.Next();
            Refresh();
        }

        private void Refresh_Click(object sender, RoutedEventArgs e)
        {
            Refresh();
        }

        private async void Pause_Click(object sender, RoutedEventArgs e)
        {
            await Vm.CommandHandler.Control.Pause();
        }

        private async void Clear_Click(object sender, RoutedEventArgs e)
        {
            await Vm.CommandHandler.Control.Clear();
        }

        private async void Queue_Click(object sender, RoutedEventArgs e)
        {
            queueMessage.Text = "";
            var message = await Vm.Queue(artist.Text, song.Text);

            switch (message)
            {
                case "The artist name is optional, but a song name is required.":
                    var dialog = new MessageDialog(message);

                    dialog.Commands.Add(new Windows.UI.Popups.UICommand("Got it.") { Id = 0 });
                    await dialog.ShowAsync();
                    break;
                default:
                    queueMessage.Text = message.Replace(":+1:","").Replace("_","");
                    artist.Text = "";
                    song.Text = "";
                    break;
            }
            Refresh();
        }

        private void Setting_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(Puffincast.Universal.Settings));
        }

        private async void Refresh()
        {
            await Task.Delay(2000);
            await Vm.GetPlaylist();
            await Vm.GetPreviousPlaylist();
        }
    }
}

