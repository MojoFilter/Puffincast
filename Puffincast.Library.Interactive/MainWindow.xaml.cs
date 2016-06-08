using Puffincast.Library.Core;
using Puffincast.Library.Interactive.ViewModels;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reactive.Linq;
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

namespace Puffincast.Library.Interactive
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private IDisposable s;

        public MainWindow()
        {
            string lib = @"C:\Users\jweeks\Music";
            var vm = new ScanViewModel(new LibraryScanner(), lib);
            this.DataContext = vm;
            vm.Scan.Subscribe(p => Trace.WriteLine(p.Status));
            InitializeComponent();
        }
    }
}
