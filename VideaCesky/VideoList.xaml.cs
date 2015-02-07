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

        #region Feed
        public string Feed
        {
            get { return (string)GetValue(FeedProperty); }
            set { SetValue(FeedProperty, value); }
        }
        public static readonly DependencyProperty FeedProperty =
            DependencyProperty.Register("Feed", typeof(string), typeof(VideoList), new PropertyMetadata("http://www.videacesky.cz", Feed_Changed));

        private async static void Feed_Changed(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            VideoList vl = d as VideoList;
            if (vl != null)
            {
                await vl.Refresh();
            }
        }
        #endregion // end of Feed

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

        #region Orientation
        public DisplayOrientations Orientation
        {
            get { return (DisplayOrientations)GetValue(OrientationProperty); }
            set { SetValue(OrientationProperty, value); }
        }
        public static readonly DependencyProperty OrientationProperty =
            DependencyProperty.Register("Orientation", typeof(DisplayOrientations), typeof(VideoList), new PropertyMetadata(DisplayOrientations.Portrait, Orientation_Changed));

        private static void Orientation_Changed(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            VideoList vl = d as VideoList;
            if (d != null)
            {
                DisplayOrientations orientation = (DisplayOrientations)e.NewValue;
                if (orientation == DisplayOrientations.Portrait || orientation == DisplayOrientations.PortraitFlipped)
                {
                    vl.VideoListView.ItemTemplate = vl.Resources["PortraitVideoTemplate"] as DataTemplate;
                }
                else
                {
                    vl.VideoListView.ItemTemplate = vl.Resources["LandscapeVideoTemplate"] as DataTemplate;
                }
            }
        }

        #endregion // end of VideoTemplate

        #region Events
        public event EventHandler<VideoEventArgs> Click;
        private void OnClick(Video video)
        {
            if (Click != null)
            {
                Click(this, new VideoEventArgs(video));
            }
        }
        #endregion // end of Events

        #region Event Handlers
        private void ListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ListView lw = (ListView)sender;
            if (lw.SelectedItem != null)
            {
                Video video = (Video)lw.SelectedItem;
                lw.SelectedItem = null;

                OnClick(video);
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
            Debug.WriteLine("Refresh...");
            Page = 0;
            List.Clear();
            await LoadMore();
        }

        public async Task LoadMore()
        {
            Loading = true;
            try
            {
                Page++;

                HttpResponse response = await Http.GetAsync(string.Format("{0}/page/{1}", Feed, Page));
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

                        List.Add(new Video()
                        {
                            Uri = uri,
                            Title = title,
                            Detail = detail,
                            ImageUri = imageUri,
                            Date = date
                        });
                    }
                }

                var pagination = contentArea.LastChild.PreviousSibling.ChildNodes.FindFirst("ol");
                foreach (var p in pagination.ChildNodes)
                {
                    if (p.Attributes.Contains("class") && p.Attributes["class"].Value == "gap")
                    {
                        MaxPage = Convert.ToInt32(p.NextSibling.ChildNodes.FindFirst("span").InnerText);
                    }
                }
                Debug.WriteLine("Page {0}/{1}", Page, MaxPage);
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
        }
        #endregion // end of Private/Public Methods
    }
}
