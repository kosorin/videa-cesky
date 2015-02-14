using MyToolkit.Paging;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Threading.Tasks;
using VideaCesky.Helpers;
using VideaCesky.Models;
using Windows.Graphics.Display;
using Windows.System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;

namespace VideaCesky.Pages
{
    public sealed partial class VideoDetailPage : MtPage
    {
        public VideoDetailPage()
        {
            this.InitializeComponent();
        }

        public Video Video { get; set; }

        public int? CurrentPage { get; set; }

        protected override void OnNavigatedTo(MtNavigationEventArgs args)
        {
            base.OnNavigatedTo(args);
            DisplayInformation.AutoRotationPreferences = DisplayOrientations.Portrait | DisplayOrientations.PortraitFlipped;

            if (args.Parameter != null && args.Parameter is Video)
            {
                Video = args.Parameter as Video;
                DataContext = Video;
            }
            else
            {
                Video = null;
            }
        }

        private async void Tag_Tapped(object sender, TappedRoutedEventArgs e)
        {
            TextBlock tb = sender as TextBlock;
            if (tb != null)
            {
                Tag tag = tb.DataContext as Tag;
                await Frame.NavigateAsync(typeof(CategoryPage), new Category(tag.Name, "", tag.Feed));
            }
        }

        private async void PlayButton_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            if (Video != null)
            {
                await Frame.NavigateAsync(typeof(VideoPage), Video.Uri.ToString());
            }
        }

        private async void WebButton_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            if (Video != null)
            {
                await Launcher.LaunchUriAsync(Video.Uri);
            }
        }

        private async void TagButton_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            FrameworkElement fe = sender as FrameworkElement;
            if (fe != null)
            {
                Tag tag = fe.DataContext as Tag;
                await Frame.NavigateAsync(typeof(CategoryPage), new Category(tag.Name, "", tag.Feed));
            }
        }

        private async void MtPage_Loaded(object sender, RoutedEventArgs e)
        {
            if (Video != null && Video.Comments == null)
            {
                await LoadMore();
            }
        }

        private async void LoadMoreButton_Tapped(object sender, TappedRoutedEventArgs e)
        {
            await LoadMore();
        }

        private async Task LoadMore()
        {
            LoadMoreButton.Visibility = Visibility.Collapsed;
            LoadingComments.IsActive = true;

            if (Video != null)
            {
                Uri uri = Video.Uri;
                if (CurrentPage != null && CurrentPage > 1)
                {
                    uri = new Uri(uri.ToString() + "/comment-page-" + (CurrentPage.Value - 1).ToString());
                }

                Tuple<List<Comment>, int?> tuple = await Downloader.GetComments(uri);
                if (tuple != null)
                {
                    List<Comment> comments = tuple.Item1 ?? new List<Comment>();
                    if (CurrentPage != null && CurrentPage > 1)
                    {
                        if (Video.Comments == null)
                        {
                            Video.Comments = new ObservableCollection<Comment>();
                        }
                        foreach (Comment comment in comments)
                        {
                            Video.Comments.Add(comment);
                        }
                    }
                    else
                    {
                        Video.Comments = new ObservableCollection<Comment>(comments);
                    }
                    CurrentPage = tuple.Item2;
                }
                else
                {
                    CurrentPage = null;
                }

                LoadMoreButton.Visibility = (CurrentPage != null && CurrentPage > 1) ? Visibility.Visible : Visibility.Collapsed;
                LoadingComments.IsActive = false;
            }
        }

        private async void CommentsRefreshButton_Click(object sender, RoutedEventArgs e)
        {
            CurrentPage = null;
            if (Video != null)
            {
                Video.Comments = null;
            }
            await LoadMore();
        }
    }
}
