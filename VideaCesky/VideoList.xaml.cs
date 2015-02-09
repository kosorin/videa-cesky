using HtmlAgilityPack;
using MyToolkit.Networking;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Windows.Graphics.Display;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;

namespace VideaCesky
{
    public class VideoEventArgs : RoutedEventArgs
    {
        public Video Video { get; set; }

        public VideoEventArgs(Video video)
        {
            Video = video;
        }
    }

    public sealed partial class VideoList : UserControl, INotifyPropertyChanged
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

        #region Private Fields

        private double _scrollPosition = 0;

        #endregion // end of Private Fields

        #region Feed
        private string _feed = "http://www.videacesky.cz";
        public string Feed
        {
            get { return _feed; }
            set { SetProperty(ref _feed, value); }
        }
        #endregion // end of Feed

        #region Search
        private string _search = null;
        public string Search
        {
            get { return _search; }
            set { SetProperty(ref _search, value); }
        }
        #endregion // end of Search

        #region PageFrame
        private Frame _pageFrame = null;
        public Frame PageFrame
        {
            get { return _pageFrame; }
            set { SetProperty(ref _pageFrame, value); }
        }
        #endregion // end of Frame

        #region Constructor
        public VideoList()
        {
            this.InitializeComponent();
            DataContext = this;
        }
        #endregion // end of Constructor

        #region List
        private ObservableCollection<Video> _list = new ObservableCollection<Video>();
        public ObservableCollection<Video> List
        {
            get { return _list; }
            set { SetProperty(ref _list, value); }
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

        #region States
        private bool _loading = true;
        public bool Loading
        {
            get { return _loading; }
            set { SetProperty(ref _loading, value); }
        }

        private bool _canLoadMore = true;
        public bool CanLoadMore
        {
            get { return _canLoadMore; }
            set { SetProperty(ref _canLoadMore, value); }
        }

        private bool _isError = false;
        public bool IsError
        {
            get { return _isError; }
            set { SetProperty(ref _isError, value); }
        }

        private bool _noVideos = false;
        public bool NoVideos
        {
            get { return _noVideos; }
            set { SetProperty(ref _noVideos, value); }
        }
        #endregion

        #region Events
        public event EventHandler<EventArgs> StartRefreshing;
        private void OnStartRefreshing()
        {
            if (StartRefreshing != null)
            {
                StartRefreshing(this, new EventArgs());
            }
        }

        public event EventHandler<EventArgs> Refreshed;
        private void OnRefreshed()
        {
            if (Refreshed != null)
            {
                Refreshed(this, new EventArgs());
            }
        }
        #endregion // end of Events

        #region Event Handlers
        public void OrientationChanged(object sender)
        {
            DisplayOrientations orientation = DisplayProperties.CurrentOrientation; ;
            if (orientation == DisplayOrientations.Portrait || orientation == DisplayOrientations.PortraitFlipped)
            {
                VideoListView.ItemTemplate = Resources["PortraitVideoTemplate"] as DataTemplate;
            }
            else
            {
                VideoListView.ItemTemplate = Resources["LandscapeVideoTemplate"] as DataTemplate;
            }
        }

        private void ListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ListView lw = (ListView)sender;
            if (lw.SelectedItem != null && lw.SelectedItem is Video)
            {
                Video video = (Video)lw.SelectedItem;
                lw.SelectedItem = null;

                PageFrame.Navigate(typeof(VideoPage), video.Uri.ToString());
            }
        }

        private async void LoadMoreButton_Tapped(object sender, TappedRoutedEventArgs e)
        {
            await LoadMore();
        }
        #endregion // end of Event Handlers

        #region Public Methods
        public async Task Refresh()
        {
            Debug.WriteLine("Refresh VideoList");
            OnStartRefreshing();

            List.Clear();
            Page = 0;

            await LoadMore();

            OnRefreshed();
        }

        public void UpdateOrientation()
        {
            OrientationChanged(null);
        }

        public void SaveScrollPosition()
        {
            _scrollPosition = ScrollViewer.VerticalOffset;
        }

        public void RefreshScrollPosition()
        {
            ScrollViewer.ScrollToVerticalOffset(_scrollPosition);
        }
        #endregion // end of Public Methods

        #region Private Methods
        private async Task LoadMore()
        {
            IsError = false;
            CanLoadMore = false;
            Loading = true;
            NoVideos = false;

            int countBeforeLoading = List.Count;
            try
            {
                Page++;

                string requestUri = string.Format("{0}/page/{1}", Feed, Page);
                if (!string.IsNullOrEmpty(Search))
                {
                    requestUri += "?s=" + Search;
                }

                HttpResponse response = await Http.GetAsync(requestUri);
                HtmlDocument doc = new HtmlDocument();
                doc.LoadHtml(response.Response);

                HtmlNode contentArea = doc.GetElementbyId("contentArea");
                foreach (var node in contentArea.ChildNodes)
                {
                    if (node.NodeType == HtmlNodeType.Element && node.Id != "")
                    {
                        try
                        {
                            Uri uri = new Uri(node.ChildNodes.FindFirst("a").Attributes["href"].Value);
                            if (uri.ToString().Contains("videacesky.cz/clanky-novinky-souteze"))
                            {
                                // toto nejsou videa...
                                continue;
                            }
                            string title = WebUtility.HtmlDecode(node.ChildNodes.FindFirst("span").InnerText);
                            Uri imageUri = new Uri(node.ChildNodes.FindFirst("img").Attributes["src"].Value);

                            var descendants = node.Descendants();

                            // Formát: 8.7.2014 v 08:00
                            HtmlNode dateNode = descendants.First(n => n.Attributes.Contains("class") && n.Attributes["class"].Value == "postDate");
                            DateTime date = DateTime.ParseExact(
                                dateNode.InnerText,
                                "d'.'M'.'yyyy' v 'HH':'mm",
                                CultureInfo.InvariantCulture);

                            HtmlNode detailNode = descendants.First(n => n.Attributes.Contains("class") && n.Attributes["class"].Value == "obs");
                            string detail = WebUtility.HtmlDecode(Regex.Replace(detailNode.InnerText.Replace("(Celý příspěvek...)", ""), @"<!--[^>]*-->", "")).Trim();

                            List.Add(new Video()
                            {
                                Uri = uri,
                                Title = title,
                                Detail = detail,
                                ImageUri = imageUri,
                                Date = date
                            });
                        }
                        catch
                        {
                            continue;
                        }
                    }
                }
                Debug.WriteLine("Loaded Page {0}", Page);
            }
            catch (Exception e)
            {
                IsError = true;
                Debug.WriteLine("[{0}] {1}", Page, e.Message);
            }

            Loading = false;
            if (!IsError)
            {
                if (List.Count > countBeforeLoading)
                {
                    CanLoadMore = true;
                }
                if (List.Count == 0)
                {
                    NoVideos = true;
                }
            }
        }
        #endregion // end of Private Methods
    }
}
