using MyToolkit.Paging;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using VideaCesky.Helpers;
using VideaCesky.Models;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Graphics.Display;
using Windows.UI;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

namespace VideaCesky.Pages
{
    public sealed partial class CommentsPage : MtPage, INotifyPropertyChanged
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

        private Uri _uri = null;

        public CommentsPage()
        {
            this.InitializeComponent();
            DataContext = this;
        }

        protected override async void OnNavigatedTo(MtNavigationEventArgs args)
        {
            base.OnNavigatedTo(args);
            DisplayInformation.AutoRotationPreferences = DisplayOrientations.Portrait | DisplayOrientations.PortraitFlipped;

            if (args.Parameter != null && args.Parameter is Uri)
            {
                _uri = args.Parameter as Uri;
                await Refresh();
            }
            else
            {
                _uri = null;
            }
        }

        #region Properties
        private int? CommentsCurrentPage { get; set; }

        private bool CanLoadMoreComments
        {
            get
            {
                return CommentsCurrentPage != null && CommentsCurrentPage > 1;
            }
        }

        private ObservableCollection<Comment> _comments = new ObservableCollection<Comment>();
        public ObservableCollection<Comment> Comments
        {
            get { return _comments; }
            set { SetProperty(ref _comments, value); }
        }
        #endregion // end of Private Properties

        #region Private Methods
        private void Reset()
        {
            CommentsCurrentPage = null;
            Comments = null;
        }

        private async Task Refresh()
        {
            Reset();
            await LoadMore();
        }

        private async Task LoadMore()
        {
            StatusBar sb = StatusBar.GetForCurrentView();
            sb.ProgressIndicator.Text = "Načítám komentáře...";
            await sb.ProgressIndicator.ShowAsync();

            if (_uri != null)
            {
                Uri uri = _uri;
                if (CanLoadMoreComments)
                {
                    uri = new Uri(uri.ToString() + "/comment-page-" + (CommentsCurrentPage.Value - 1).ToString());
                }

                Tuple<List<Comment>, int?> tuple = await Downloader.GetComments(uri);
                if (tuple != null)
                {
                    List<Comment> comments = tuple.Item1 ?? new List<Comment>();
                    if (CanLoadMoreComments)
                    {
                        if (Comments == null)
                        {
                            Comments = new ObservableCollection<Comment>();
                        }
                        foreach (Comment comment in comments)
                        {
                            Comments.Add(comment);
                        }
                    }
                    else
                    {
                        Comments = new ObservableCollection<Comment>(comments);
                    }
                    CommentsCurrentPage = tuple.Item2;
                }
                else
                {
                    CommentsCurrentPage = null;
                }
            }

            await sb.ProgressIndicator.HideAsync();
        }
        #endregion // end of Private Methods

        #region Event Handlers
        private async void RefreshButton_Click(object sender, RoutedEventArgs e)
        {
            await Refresh();
        }
        #endregion // end of Event Handlers

        #region ScrollViewer auto load more
        private void ListView_Loaded(object sender, RoutedEventArgs e)
        {
            ScrollViewer sv = Utils.GetScrollViewer(sender as ListView);
            if (sv != null)
            {
                sv.ViewChanged -= sv_ViewChanged;
                sv.ViewChanged += sv_ViewChanged;
            }
        }

        private async void sv_ViewChanged(object sender, ScrollViewerViewChangedEventArgs e)
        {
            ScrollViewer sv = sender as ScrollViewer;
            if (sv != null)
            {
                if (e.IsIntermediate)
                {
                    double verticalOffset = sv.VerticalOffset;
                    double maxVerticalOffset = sv.ScrollableHeight;

                    if (maxVerticalOffset < 0 || verticalOffset == maxVerticalOffset)
                    {
                        if (CanLoadMoreComments)
                        {
                            await LoadMore();
                        }
                    }
                }
            }
        }
        #endregion // end of ScrollViewer auto load more
    }
}
