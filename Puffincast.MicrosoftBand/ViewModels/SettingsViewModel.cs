using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;

namespace Puffincast.Universal.ViewModels
{
    class SettingsViewModel : INotifyPropertyChanged
    {
        private string httpQUri;

        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged(string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        public string HttpQUri
        {
            get { return httpQUri; }
            set
            {
                httpQUri = value;
                NotifyPropertyChanged("HttpQUri");
            }
        }

        public SettingsViewModel()
        {
  
        }

        public string RetrieveHttpQUri()
        {
            ApplicationDataContainer AppSettings = ApplicationData.Current.LocalSettings;
            HttpQUri = AppSettings.Values["puffincastUri"].ToString();
            return HttpQUri;
        }
        public void SaveHttpQUri()
        {
            ApplicationDataContainer AppSettings = ApplicationData.Current.LocalSettings;
            AppSettings.Values["puffincastUri"] = HttpQUri;
        }
    }
}
