using MyToolkit.Multimedia;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Graphics.Display;
using Windows.Phone.UI.Input;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkID=390556

namespace VideaCesky
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class VideoPage : Page, INotifyPropertyChanged
    {
        #region Page
        public VideoPage()
        {
            this.InitializeComponent();

            Position = TimeSpan.Zero;
            Duration = TimeSpan.Zero;

            updateSliderTimer.Interval = TimeSpan.FromMilliseconds(1000F / 60F);
            updateSliderTimer.Tick += updateSliderTimer_Tick;

            autoHideSliderTimer.Interval = TimeSpan.FromSeconds(2.5);
            autoHideSliderTimer.Tick += autoHideSliderTimer_Tick;
        }

        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            HardwareButtons.BackPressed += BackButtonPress;
            DisplayInformation.AutoRotationPreferences = DisplayOrientations.Landscape | DisplayOrientations.LandscapeFlipped;
            StatusBar statusBar = StatusBar.GetForCurrentView();
            await statusBar.HideAsync();

            DataContext = this;

            videoSource = (VideoSource)e.Parameter;
            youtubeUri = await LoadVideo(videoSource.YoutubeId);
            AttachToMediaElement(youtubeUri);
        }

        protected async override void OnNavigatedFrom(NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);

            HardwareButtons.BackPressed -= BackButtonPress;
            DisplayInformation.AutoRotationPreferences = DisplayOrientations.None;
            StatusBar statusBar = StatusBar.GetForCurrentView();
            await statusBar.ShowAsync();
        }

        private void BackButtonPress(object sender, Windows.Phone.UI.Input.BackPressedEventArgs e)
        {
            e.Handled = true;
            if (Frame.CanGoBack)
            {
                Frame.GoBack();
            }
            else
            {
                Frame.Navigate(typeof(MainPage));
            }
        }
        #endregion

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


        private TimeSpan _duration;
        public TimeSpan Duration
        {
            get { return _duration; }
            set { SetProperty(ref _duration, value); }
        }

        private TimeSpan _position;
        public TimeSpan Position
        {
            get { return _position; }
            set { SetProperty(ref _position, value); }
        }

        private VideoSource videoSource;

        private YouTubeUri youtubeUri;


        #region MediaElement ======================================================================
        private async Task<YouTubeUri> LoadVideo(string id, YouTubeQuality quality = YouTubeQuality.Quality360P)
        {
            YouTubeUri source = await YouTube.GetVideoUriAsync(id, quality);
            if (source == null)
            {
                Debug.WriteLine("Chyba youtubu");
            }
            return source;
        }

        private void AttachToMediaElement(YouTubeUri video)
        {
            VideoMediaElement.Source = video.Uri;
            VideoMediaElement.Position = TimeSpan.Zero;
        }

        private void VideoMediaElement_MediaOpened(object sender, RoutedEventArgs e)
        {
            ResetSlider();
            Duration = VideoMediaElement.NaturalDuration.TimeSpan;

            ShowSlider();
            autoHideSliderTimer.Stop();

            LoadProgressRing.IsActive = false;
            PlayPauseButton.IsEnabled = true;
        }

        private void VideoMediaElement_MediaEnded(object sender, RoutedEventArgs e)
        {
            VideoMediaElement.Stop();
            updateSliderTimer.Stop();

            PlayPauseButton.IsChecked = false;
            ResetSlider();

            ShowSlider();
            autoHideSliderTimer.Stop();
        }

        private void VideoMediaElement_Tapped(object sender, TappedRoutedEventArgs e)
        {
            if (!LoadProgressRing.IsActive)
            {
                if (IsVisibleSlider)
                {
                    HideSlider();
                }
                else
                {
                    ShowSlider();
                    if (PlayPauseButton.IsChecked ?? false)
                    {
                        autoHideSliderTimer.Start();
                    }
                }
            }
        }
        #endregion

        #region PlayPause ======================================================================
        private void PlayPauseButton_Pause(object sender, RoutedEventArgs e)
        {
            VideoMediaElement.Pause();
            updateSliderTimer.Stop();

            UpdateSlider();

            ShowSlider();
            autoHideSliderTimer.Stop();
        }

        private void PlayPauseButton_Play(object sender, RoutedEventArgs e)
        {
            VideoMediaElement.Play();
            updateSliderTimer.Start();

            UpdateSlider();

            autoHideSliderTimer.Start();
        }
        #endregion

        #region Slider ============================================================================
        DispatcherTimer updateSliderTimer = new DispatcherTimer();
        DispatcherTimer autoHideSliderTimer = new DispatcherTimer();

        private bool canAutoUpdateSlider = true;

        private TimeSpan sliderDragPosition;

        private bool playAfterDrag;

        private bool _isVisibleSlider = true;
        public bool IsVisibleSlider
        {
            get { return _isVisibleSlider; }
            set { SetProperty(ref _isVisibleSlider, value); }
        }

        void updateSliderTimer_Tick(object sender, object e)
        {
            UpdateSlider();
        }

        void autoHideSliderTimer_Tick(object sender, object e)
        {
            autoHideSliderTimer.Stop();
            HideSlider();
        }

        private void UpdateSlider()
        {
            if (canAutoUpdateSlider)
            {
                UpdateSlider(VideoMediaElement.Position);
            }
        }

        private void UpdateSlider(TimeSpan position)
        {
            double maxWidth = ControlsGrid.ActualWidth;
            double videoPosition = position.TotalSeconds / VideoMediaElement.NaturalDuration.TimeSpan.TotalSeconds;
            VideoSlider.Width = videoPosition * maxWidth;
            Position = position;
        }

        private void ResetSlider()
        {
            VideoSlider.Width = 0;
            Position = VideoMediaElement.Position;
        }

        private void ShowSlider()
        {
            IsVisibleSlider = true;
            ShowSliderStoryboard.Begin();
        }

        private void HideSlider()
        {
            IsVisibleSlider = false;
            HideSliderStoryboard.Begin();
        }

        private void ControlsGrid_ManipulationStarted(object sender, ManipulationStartedRoutedEventArgs e)
        {
            ShowSlider();

            sliderDragPosition = VideoMediaElement.Position;
            canAutoUpdateSlider = false;

            playAfterDrag = PlayPauseButton.IsChecked ?? false;
            PlayPauseButton.IsChecked = false;

            autoHideSliderTimer.Start();
        }

        private void ControlsGrid_ManipulationCompleted(object sender, ManipulationCompletedRoutedEventArgs e)
        {
            VideoMediaElement.Position = sliderDragPosition;
            UpdateSlider(sliderDragPosition);
            canAutoUpdateSlider = true;

            if (playAfterDrag)
            {
                PlayPauseButton.IsChecked = true;
            }

            autoHideSliderTimer.Start();
        }

        private void ControlsGrid_ManipulationDelta(object sender, ManipulationDeltaRoutedEventArgs e)
        {
            double ratio = Duration.TotalSeconds / e.Container.DesiredSize.Width;
            sliderDragPosition = sliderDragPosition.Add(TimeSpan.FromSeconds(e.Delta.Translation.X * ratio));
            if (sliderDragPosition < TimeSpan.Zero)
            {
                sliderDragPosition = TimeSpan.Zero;
            }
            else if (sliderDragPosition > Duration)
            {
                sliderDragPosition = Duration;
            }
            UpdateSlider(sliderDragPosition);

            autoHideSliderTimer.Start();
        }
        #endregion
    }
}
