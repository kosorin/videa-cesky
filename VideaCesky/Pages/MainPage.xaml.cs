using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using System;
using Windows.System;
using Windows.Phone.UI.Input;
using VideaCesky.Controls;
using VideaCesky.Models;

namespace VideaCesky.Pages
{
    public sealed partial class MainPage : VideoListBasePage
    {
        public MainPage()
        {
            this.InitializeComponent();
            IsSuspendable = false;
        }

        protected override VideoList GetVideListControl()
        {
            return VideoListControl;
        }

        protected override void OnNavigatedTo(MyToolkit.Paging.MtNavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            HardwareButtons.BackPressed -= HardwareButtons_BackPressed;
            HardwareButtons.BackPressed += HardwareButtons_BackPressed;
        }

        protected override void OnNavigatedFrom(MyToolkit.Paging.MtNavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);
            HardwareButtons.BackPressed -= HardwareButtons_BackPressed;
        }

        void HardwareButtons_BackPressed(object sender, BackPressedEventArgs e)
        {
            e.Handled = true;
            App.Current.Exit();
        }

        #region AppBar
        private void LaterAppBarButton_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(WatchLaterPage));
        }

        private void TagsAppBarButton_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(SavedTagsPage));
        }
        #endregion // end of AppBar
    }
}
