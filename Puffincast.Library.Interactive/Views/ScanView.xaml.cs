using Puffincast.Library.Interactive.ViewModels;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Puffincast.Library.Interactive.Views
{
    /// <summary>
    /// Interaction logic for ScanView.xaml
    /// </summary>
    public partial class ScanView : UserControl, IViewFor<ScanViewModel>
    {
        public ScanView()
        {
            InitializeComponent();
            this.WhenActivated(d =>
            {
                d(this.OneWayBind(this.ViewModel, vm => vm.Progress, v => v.progress.Value));
                d(this.OneWayBind(this.ViewModel, vm => vm.Status, v => v.status.Text));
            });
        }

        public ScanViewModel ViewModel
        {
            get { return (ScanViewModel)GetValue(ViewModelProperty); }
            set { SetValue(ViewModelProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ViewModel.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ViewModelProperty =
            DependencyProperty.Register("ViewModel", typeof(ScanViewModel), typeof(ScanView));



        object IViewFor.ViewModel
        {
            get { return this.ViewModel; }
            set { this.ViewModel = value as ScanViewModel; }
        }
    }
}
