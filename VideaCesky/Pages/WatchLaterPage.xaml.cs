using VideaCesky.Controls;
using VideaCesky.Models;
using Windows.UI.Xaml;

namespace VideaCesky.Pages
{
    public sealed partial class WatchLaterPage : VideoListBasePage
    {
        public WatchLaterPage()
        {
            this.InitializeComponent();
            DataContext = this;
        }

        protected override VideoList GetVideListControl()
        {
            return VideoListControl;
        }

        protected override void SetFeed(object parameter)
        {
            VideoList.OfflineData = Settings.Current.WatchLaterList;
        }

        private async void DeleteAllAppBarButton_Click(object sender, RoutedEventArgs e)
        {
            await Settings.Current.ClearWatchLaterList();
            await VideoList.Refresh();
        }
    }
}
