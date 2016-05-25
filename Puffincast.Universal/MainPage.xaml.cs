using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Popups;

using Puffincast.Universal.ViewModels;
using ReactiveUI;
using System.Diagnostics;
using System.Reactive.Linq;

namespace Puffincast.Universal
{
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.ViewModel = new MainViewModel();
            this.InitializeComponent();
        }

        public MainViewModel ViewModel
        {
            get { return (MainViewModel)GetValue(ViewModelProperty); }
            set { SetValue(ViewModelProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ViewModel.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ViewModelProperty =
            DependencyProperty.Register("ViewModel", typeof(MainViewModel), typeof(MainPage), null);
        
        
    }
}

