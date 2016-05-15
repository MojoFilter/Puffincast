using Microsoft.Band;
using Microsoft.Band.Tiles;
using Microsoft.Band.Tiles.Pages;
using Puffincast.Universal.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Imaging;

namespace Puffincast.Universal
{
   public sealed partial class Settings : Page
    {
        public SettingsViewModel Vm { get; set; }

        public Settings()
        {
            this.InitializeComponent();
            Vm = new SettingsViewModel();
            this.DataContext = Vm;
            Loaded += OnLoad;
        }


        private void OnLoad(object sender, RoutedEventArgs e)
        {
            Vm.RetrieveHttpQUri();
        }

        private void button_Click(object sender, RoutedEventArgs e)
        {
            Vm.ConnectAndSetupBand();
        }

        private void Accept_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            Vm.SaveHttpQUri();
            this.Frame.Navigate(typeof(MainPage));
        }

        private void Cancel_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(MainPage));
        }
    }
}
