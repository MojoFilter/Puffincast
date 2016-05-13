using Puffincast.Universal.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Puffincast.Universal
{
   public sealed partial class Settings : Page
    {
        public SettingsViewModel VM { get; set; }

        public Settings()
        {
            this.InitializeComponent();
            VM = new SettingsViewModel();
            this.DataContext = VM;
            Loaded += OnLoad;
        }

        private void OnLoad(object sender, RoutedEventArgs e)
        {
            VM.RetrieveHttpQUri();
        }

        private void Accept_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            VM.SaveHttpQUri();
            this.Frame.Navigate(typeof(MainPage));
        }

        private void Cancel_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(MainPage));
        }
    }
}
