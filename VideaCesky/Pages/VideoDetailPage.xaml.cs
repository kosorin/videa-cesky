﻿using MyToolkit.Paging;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using VideaCesky.Helpers;
using VideaCesky.Models;
using Windows.Graphics.Display;
using Windows.Storage;
using Windows.System;
using Windows.UI.Input;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;

namespace VideaCesky.Pages
{
    public sealed partial class VideoDetailPage : MtPage, INotifyPropertyChanged
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

        public Video Video { get; set; }

        public VideoDetailPage()
        {
            this.InitializeComponent();
        }

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

            UpdateWatchLaterButton(null);
        }

        #region AppBar
        private void UpdateWatchLaterButton(Action action)
        {
            if (Video != null)
            {
                WatchLaterAppBarButton.Click -= AddWatchLater_Click;
                WatchLaterAppBarButton.Click -= RemoveWatchLater_Click;

                if (action != null)
                {
                    action();
                }

                if (Settings.Current.WatchLaterList.Contains(Video))
                {
                    WatchLaterAppBarButton.Icon = new BitmapIcon() { UriSource = new Uri("ms-appx:///Assets/AppBar/ClockRemove.png") };
                    WatchLaterAppBarButton.Click += RemoveWatchLater_Click;
                }
                else
                {
                    WatchLaterAppBarButton.Icon = new BitmapIcon() { UriSource = new Uri("ms-appx:///Assets/AppBar/ClockAdd.png") };
                    WatchLaterAppBarButton.Click += AddWatchLater_Click;
                }

                AppBarButton button = WatchLaterAppBarButton;
                ((CommandBar)BottomAppBar).PrimaryCommands.Remove(button);
                ((CommandBar)BottomAppBar).PrimaryCommands.Insert(1, button);
            }
        }
        private void AddWatchLater_Click(object sender, RoutedEventArgs e)
        {
            UpdateWatchLaterButton(async () =>
            {
                if (!Settings.Current.WatchLaterList.Contains(Video))
                {
                    await Settings.Current.AddVideo(Video);
                }
            });
        }

        private void RemoveWatchLater_Click(object sender, RoutedEventArgs e)
        {
            UpdateWatchLaterButton(async () =>
            {
                if (Settings.Current.WatchLaterList.Contains(Video))
                {
                    await Settings.Current.RemoveVideo(Video);
                }
            });
        }

        private async void PlayButton_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            if (Video != null)
            {
                await Frame.NavigateAsync(typeof(VideoPage), Video.Uri.ToString());
            }
        }

        private async void CommentsButton_Click(object sender, RoutedEventArgs e)
        {
            if (Video != null)
            {
                await Frame.NavigateAsync(typeof(CommentsPage), Video.Uri);
            }
        }

        private async void WebButton_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            if (Video != null)
            {
                await Launcher.LaunchUriAsync(Video.Uri);
            }
        }
        #endregion // end of AppBar
    }
}
