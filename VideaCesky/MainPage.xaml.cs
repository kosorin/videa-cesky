using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Graphics.Display;
using Windows.System;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

namespace VideaCesky
{
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();
#if DEBUG
            Loaded += Page_Loaded;
#endif
        }

#if DEBUG
        private async void Page_Loaded(object sender, RoutedEventArgs e)
        {
            ListPickerFlyout fo = new ListPickerFlyout();
            fo.ItemsSource = new Dictionary<string, string>() 
            { 
                {"normální", "http://www.videacesky.cz/serialy-online-zdarma/odvazni-valecnici-2x06-loutkovy-horor"},
                {"playlist 1", "http://www.videacesky.cz/ostatni-zabavna-videa/conan-policejnim-straznikem"},
                {"playlist 2", "http://www.videacesky.cz/talk-show-rozhovory/arnold-schwarzenegger-u-jimmyho-fallona"},
                {"více videí", "http://www.videacesky.cz/talk-show-rozhovory/russell-brand-u-conana-obriena"},
                {"xml", "http://www.videacesky.cz/reklamy-reklamni-spot-video/nekompromisni-maskot"},
            };
            fo.ItemsPicked += (s2, e2) =>
            {
                if (e2.AddedItems.Count > 0)
                {
                    Frame.Navigate(typeof(VideoPage), ((KeyValuePair<string, string>)e2.AddedItems[0]).Value);
                }
            };

            await fo.ShowAtAsync(ContentRoot);
        }
#endif
    }
}
