using HtmlAgilityPack;
using MyToolkit.Networking;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
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
    public sealed partial class MainPage : Page, INotifyPropertyChanged
    {
        #region INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        public bool SetProperty<T>(ref T storage, T value, [CallerMemberName] string propertyName = "")
        {
            if (EqualityComparer<T>.Default.Equals(storage, value))
            {
                return false;
            }
            storage = value;

            OnPropertyChanged(propertyName);
            return true;
        }
        #endregion

        #region VideoList
        private ObservableCollection<Video> _videoList = new ObservableCollection<Video>();
        public ObservableCollection<Video> VideoList
        {
            get { return _videoList; }
            set { SetProperty(ref _videoList, value); }
        }
        #endregion

        #region Page
        private int _page = 0;
        public int Page
        {
            get { return _page; }
            set { SetProperty(ref _page, value); }
        }
        #endregion

        #region MaxPage
        private int _maxPage = 3;
        public int MaxPage
        {
            get { return _maxPage; }
            set { SetProperty(ref _maxPage, value); }
        }
        #endregion

        #region Loading
        private bool _loading = true;
        public bool Loading
        {
            get { return _loading; }
            set { SetProperty(ref _loading, value); }
        }
        #endregion

        #region CanLoadMore
        private bool _canLoadMore = true;
        public bool CanLoadMore
        {
            get { return _canLoadMore; }
            set { SetProperty(ref _canLoadMore, value); }
        }
        #endregion

        #region IsError
        private bool _isError = false;
        public bool IsError
        {
            get { return _isError; }
            set
            {
                SetProperty(ref _isError, value);
                if (value)
                {
                    CanLoadMore = false;
                }
            }
        }
        #endregion

        public MainPage()
        {
            this.InitializeComponent();
            DataContext = this;

            NavigationCacheMode = NavigationCacheMode.Required;
#if DEBUG
            //Loaded += async (s, e) =>
            //{
            //    ListPickerFlyout fo = new ListPickerFlyout();
            //    fo.ItemsSource = new Dictionary<string, string>() 
            //    { 
            //        {"normální", "http://www.videacesky.cz/serialy-online-zdarma/odvazni-valecnici-2x06-loutkovy-horor"},
            //        {"playlist 1", "http://www.videacesky.cz/ostatni-zabavna-videa/conan-policejnim-straznikem"},
            //        {"playlist 2", "http://www.videacesky.cz/talk-show-rozhovory/arnold-schwarzenegger-u-jimmyho-fallona"},
            //        {"více videí", "http://www.videacesky.cz/talk-show-rozhovory/russell-brand-u-conana-obriena"},
            //        {"xml", "http://www.videacesky.cz/reklamy-reklamni-spot-video/nekompromisni-maskot"},
            //    };
            //    fo.ItemsPicked += (s2, e2) =>
            //    {
            //        if (e2.AddedItems.Count > 0)
            //        {
            //            Frame.Navigate(typeof(VideoPage), ((KeyValuePair<string, string>)e2.AddedItems[0]).Value);
            //        }
            //    };

            //    await fo.ShowAtAsync(ContentRoot);
            //};
#endif
        }

        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            if (e.NavigationMode == NavigationMode.New)
            {
                await LoadMore();
            }
        }

        public async Task LoadMore()
        {
            Loading = true;
            try
            {
                Page++;

                HttpResponse response = await Http.GetAsync(string.Format(@"http://www.videacesky.cz/page/{0}", Page));
                HtmlDocument doc = new HtmlDocument();
                doc.LoadHtml(response.Response);

                HtmlNode contentArea = doc.GetElementbyId("contentArea");
                foreach (var node in contentArea.ChildNodes)
                {
                    if (node.NodeType == HtmlNodeType.Element && node.Id != "")
                    {
                        Uri uri = new Uri(node.ChildNodes.FindFirst("a").Attributes["href"].Value);
                        string title = WebUtility.HtmlDecode(node.ChildNodes.FindFirst("span").InnerText);
                        Uri imageUri = new Uri(node.ChildNodes.FindFirst("img").Attributes["src"].Value);

                        var descendants = node.Descendants();

                        // 8.7.2014 v 08:00
                        HtmlNode dateNode = descendants.First(n => n.Attributes.Contains("class") && n.Attributes["class"].Value == "postDate");
                        DateTime date = DateTime.ParseExact(
                            dateNode.InnerText,
                            "d'.'M'.'yyyy' v 'HH':'mm",
                            CultureInfo.InvariantCulture);

                        HtmlNode detailNode = descendants.First(n => n.Attributes.Contains("class") && n.Attributes["class"].Value == "obs");
                        string detail = WebUtility.HtmlDecode(Regex.Replace(detailNode.InnerText.Replace("(Celý příspěvek...)", ""), @"<!--[^>]*-->", "")).Trim();

                        VideoList.Add(new Video()
                        {
                            Uri = uri,
                            Title = title,
                            Detail = detail,
                            ImageUri = imageUri,
                            Date = date
                        });
                    }
                }

                if (MaxPage == 0)
                {
                    var pagination = contentArea.LastChild.PreviousSibling.ChildNodes.FindFirst("ol");
                    foreach (var p in pagination.ChildNodes)
                    {
                        if (p.Attributes.Contains("class") && p.Attributes["class"].Value == "gap")
                        {
                            MaxPage = Convert.ToInt32(p.NextSibling.ChildNodes.FindFirst("span").InnerText);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                IsError = true;
                Debug.WriteLine("[{0}] {1}", Page, e.Message);
            }

            Loading = false;
            if (Page >= MaxPage && MaxPage != 0)
            {
                CanLoadMore = false;
            }

            if (Page == 3)
                IsError = true;
        }

        private async void LoadMoreButton_Tapped(object sender, TappedRoutedEventArgs e)
        {
            await LoadMore();
        }

        private void AppBarButton_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(Guide));
        }

        private void ListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ListView lw = (ListView)sender;
            if (lw.SelectedItem != null)
            {
                Video video = (Video)lw.SelectedItem;
                lw.SelectedItem = null;

                Frame.Navigate(typeof(VideoPage), video.Uri.ToString());
            }
        }
    }
}
