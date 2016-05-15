using Puffincast.Universal.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Puffincast.Universal.Views
{
    class DefaultDataTemplateSelector : DataTemplateSelector
    {
        protected override DataTemplate SelectTemplateCore(object item, DependencyObject container)
        {
            var c = container as FrameworkElement;
            if (c != null)
            {
                if (item is NowPlayingViewModel)
                {
                    return Find("NowPlayingTemplate");
                }
            }
            return base.SelectTemplateCore(item, container);
        }

        private static DataTemplate Find(string key)
        {
            return App.Current.Resources[key] as DataTemplate;
        }
    }
}
