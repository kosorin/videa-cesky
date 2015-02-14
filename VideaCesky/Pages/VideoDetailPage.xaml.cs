using MyToolkit.Paging;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
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

        protected override async void OnNavigatedTo(MtNavigationEventArgs args)
        {
            base.OnNavigatedTo(args);
            DisplayInformation.AutoRotationPreferences = DisplayOrientations.Portrait | DisplayOrientations.PortraitFlipped;

            if (args.Parameter != null && args.Parameter is Video)
            {
                Video = args.Parameter as Video;
                Video.Comments = await Downloader.GetComments(Video.Uri) ?? new List<Comment>();

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

    }
}
