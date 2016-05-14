using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Imaging;
using Microsoft.Band;
using Microsoft.Band.Tiles;
using Microsoft.Band.Tiles.Pages;

using Puffincast.Processing;
using Windows.Storage;
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

        public async void OnLoad(object sender, RoutedEventArgs e)
        {
            await Vm.GetPlaylist();
        }

        private void button_Click(object sender, RoutedEventArgs e)
        {
           Vm.ConnectAndSetupBand();
        }

        private async void Previous_Click(object sender, RoutedEventArgs e)
        {
            await Vm.Control.Prev();
        }

        private async void Play_Click(object sender, RoutedEventArgs e)
        {
            await Vm.Control.Play();
        }

        private async void Next_Click(object sender, RoutedEventArgs e)
        {
            await Vm.Control.Next();
        }

        private void Setting_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(Puffincast.Universal.Settings));
        }
    }
}

