using Puffincast.Library.Core;
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
            InitializeComponent();
            string lib = @"C:\Users\jweeks\Music";

            this.s = new LibraryScanner().Scan(lib)
                .Select(p => $"[{(int)(p.Percent * 100.0)}]% {System.IO.Path.GetFileNameWithoutExtension(p.Status)}")
                .Subscribe(p => Debug.WriteLine(p), e => Debug.WriteLine(e.Message));
        }
    }
}
