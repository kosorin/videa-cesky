using MyToolkit.Multimedia;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Xml.Linq;
using Windows.Graphics.Display;
using Windows.Phone.UI.Input;
using Windows.System.Display;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Documents;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

namespace VideaCesky
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class VideoPage : Page, INotifyPropertyChanged
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

        #region Page
        private DisplayRequest displayRequest = null;

        public VideoPage()
        {
            this.InitializeComponent();

            SliderPosition = TimeSpan.Zero;
            Duration = TimeSpan.Zero;

            updateSliderPositionTimer.Interval = TimeSpan.FromMilliseconds(1000F / 60F);
            updateSliderPositionTimer.Tick += updateSliderPositionTimer_Tick;

            autoHideSliderTimer.Interval = TimeSpan.FromSeconds(2.5);
            autoHideSliderTimer.Tick += autoHideSliderTimer_Tick;

            VideoMediaElement.MarkerReached += VideoMediaElement_MarkerReached;
            subtitleTimer.Tick += subtitleTimer_Tick;
        }

        private async Task VideoRequest(VideoSource videoSource)
        {
            Title = videoSource.Title;
            if (!await AttachVideo(videoSource.YoutubeId))
            {
                ShowError("Něco se pokazilo během nahrávání videa. Video není možné přehrát.");
            }
            else if (!await AttachSubtitles(videoSource.SubtitlesUri))
            {
                ShowError("Titulky se nepodařilo stáhnout. Video můžete přesto přehrát.", true);
            }
        }

        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            #region Tlačítko zpět, orientace displeje, atd...
            HardwareButtons.BackPressed += BackButtonPress;
            DisplayInformation.AutoRotationPreferences = DisplayOrientations.Landscape | DisplayOrientations.LandscapeFlipped;
            StatusBar statusBar = StatusBar.GetForCurrentView();
            await statusBar.HideAsync();
            #endregion

            DataContext = this;

            // Zpracování požadavku na video
            await VideoRequest((VideoSource)e.Parameter);
        }

        protected async override void OnNavigatedFrom(NavigationEventArgs e)
        {
            #region Tlačítko zpět, orientace displeje, atd...
            HardwareButtons.BackPressed -= BackButtonPress;
            DisplayInformation.AutoRotationPreferences = DisplayOrientations.None;
            StatusBar statusBar = StatusBar.GetForCurrentView();
            await statusBar.ShowAsync();
            #endregion

            base.OnNavigatedFrom(e);
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

        #region Properties
        private string _title;
        public string Title
        {
            get { return _title; }
            set { SetProperty(ref _title, value); }
        }

        private TimeSpan _duration;
        public TimeSpan Duration
        {
            get { return _duration; }
            set { SetProperty(ref _duration, value); }
        }

        private TimeSpan _position;
        public TimeSpan SliderPosition
        {
            get { return _position; }
            set
            {
                if (SetProperty(ref _position, value))
                {
                    OnPropertyChanged("SliderPositionWidth");
                }
            }
        }

        public int SliderPositionWidth
        {
            get
            {
                double maxWidth = ControlsGrid.ActualWidth;
                double videoPosition = SliderPosition.TotalSeconds / Duration.TotalSeconds;
                return (int)(videoPosition * maxWidth);
            }
        }

        private bool _isLoaded = false;
        public bool IsLoaded
        {
            get { return _isLoaded; }
            private set { SetProperty(ref _isLoaded, value); }
        }

        private bool _isVisibleSlider = true;
        public bool IsVisibleSlider
        {
            get { return _isVisibleSlider; }
            set { SetProperty(ref _isVisibleSlider, value); }
        }

        private string _subtitle;
        public string Subtitle
        {
            get { return _subtitle; }
            set { SetProperty(ref _subtitle, value); }
        }

        private bool _isError = false;
        public bool IsError
        {
            get { return _isError; }
            private set { SetProperty(ref _isError, value); }
        }

        private string _errorMessage;
        public string ErrorMessage
        {
            get { return _errorMessage; }
            private set { SetProperty(ref _errorMessage, value); }
        }

        private bool _canPlayAnyway = false;
        public bool CanPlayAnyway
        {
            get { return _canPlayAnyway; }
            private set { SetProperty(ref _canPlayAnyway, value); }
        }

        private bool _isPlaying = false;
        public bool IsPlaying
        {
            get { return _isPlaying; }
            private set { SetProperty(ref _isPlaying, value); }
        }
        #endregion

        #region Video
        private void PlayVideoPlayback()
        {
            Debug.WriteLine("PLAY");
            if (displayRequest == null)
            {
                displayRequest = new DisplayRequest();
                displayRequest.RequestActive();
            }

            VideoMediaElement.Play();

            updateSliderPositionTimer.Start();
            UpdateSliderPosition();
            autoHideSliderTimer.Start();

            IsPlaying = true;
        }

        private void PauseVideoPlayback()
        {
            Debug.WriteLine("PAUSE");
            VideoMediaElement.Pause();

            updateSliderPositionTimer.Stop();
            UpdateSliderPosition();
            ShowSlider();
            autoHideSliderTimer.Stop();

            IsPlaying = false;

            if (displayRequest != null)
            {
                displayRequest.RequestRelease();
                displayRequest = null;
            }
        }

        private void StopVideoPlayback()
        {
            Debug.WriteLine("STOP");
            VideoMediaElement.Stop();

            updateSliderPositionTimer.Stop();
            UpdateSliderPosition();
            ShowSlider();
            autoHideSliderTimer.Stop();

            IsPlaying = false;

            if (displayRequest != null)
            {
                displayRequest.RequestRelease();
                displayRequest = null;
            }
        }
        #endregion

        #region MediaElement ======================================================================
        private async Task<bool> AttachVideo(string youtubeId, YouTubeQuality quality = YouTubeQuality.Quality360P)
        {
            YouTubeUri video = await YouTube.GetVideoUriAsync(youtubeId, quality);
            if (video != null)
            {
                VideoMediaElement.Source = video.Uri;
                return true;
            }
            else
            {
                return false;
            }
        }

        private void UpdateDuration()
        {
            Duration = VideoMediaElement.NaturalDuration.TimeSpan;
        }

        private void VideoMediaElement_MediaOpened(object sender, RoutedEventArgs e)
        {
            if (!IsError)
            {
                IsLoaded = true;

                UpdateDuration();
                ResetSliderPosition();
                ShowSlider();

                PlayVideoPlayback();
            }
        }

        private void VideoMediaElement_MediaEnded(object sender, RoutedEventArgs e)
        {
            StopVideoPlayback();
        }

        private void VideoMediaElement_MediaFailed(object sender, ExceptionRoutedEventArgs e)
        {
            ShowError("Neznámá chyba");
        }

        private void VideoMediaElement_Tapped(object sender, TappedRoutedEventArgs e)
        {
            if (IsLoaded)
            {
                if (IsVisibleSlider)
                {
                    HideSlider();
                }
                else
                {
                    ShowSlider();
                    if (IsPlaying)
                    {
                        autoHideSliderTimer.Start(); // TODO
                    }
                }
            }
        }

        private void VideoMediaElement_MarkerReached(object sender, TimelineMarkerRoutedEventArgs e)
        {
            if (e.Marker.Type == "subtitle")
            {
                try
                {
                    SetSubtitle(Convert.ToInt32(e.Marker.Text));
                }
                catch (FormatException)
                {
                }
                catch (OverflowException)
                {
                }
            }
        }
        #endregion

        #region PlayPause =========================================================================
        private void PlayPauseButton_Tapped(object sender, TappedRoutedEventArgs e)
        {
            if (IsPlaying)
            {
                PauseVideoPlayback();
            }
            else
            {
                PlayVideoPlayback();
            }
        }
        #endregion

        #region Slider ============================================================================
        DispatcherTimer updateSliderPositionTimer = new DispatcherTimer();

        DispatcherTimer autoHideSliderTimer = new DispatcherTimer();

        private bool canAutoUpdateSliderPosition = true;

        private TimeSpan sliderDragPosition;

        private bool playAfterDrag;

        private void updateSliderPositionTimer_Tick(object sender, object e)
        {
            UpdateSliderPosition();
        }

        private void autoHideSliderTimer_Tick(object sender, object e)
        {
            autoHideSliderTimer.Stop();
            HideSlider();
        }

        private void UpdateSliderPosition()
        {
            if (canAutoUpdateSliderPosition)
            {
                UpdateSliderPosition(VideoMediaElement.Position);
            }
        }

        private void UpdateSliderPosition(TimeSpan position)
        {
            SliderPosition = position;
        }

        private void ResetSliderPosition()
        {
            SliderPosition = VideoMediaElement.Position;
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
            if (IsError)
                return;

            ShowSlider();

            canAutoUpdateSliderPosition = false;
            sliderDragPosition = VideoMediaElement.Position;
            playAfterDrag = IsPlaying;

            PauseVideoPlayback();
        }

        private void ControlsGrid_ManipulationCompleted(object sender, ManipulationCompletedRoutedEventArgs e)
        {
            if (IsError)
                return;

            canAutoUpdateSliderPosition = true;
            VideoMediaElement.Position = sliderDragPosition;
            UpdateSliderPosition(sliderDragPosition);

            if (playAfterDrag)
            {
                PlayVideoPlayback();
            }
        }

        private void ControlsGrid_ManipulationDelta(object sender, ManipulationDeltaRoutedEventArgs e)
        {
            if (IsError)
                return;

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
            UpdateSliderPosition(sliderDragPosition);
        }
        #endregion

        #region Subtitles =========================================================================
        private Subtitles subtitles = null;

        DispatcherTimer subtitleTimer = new DispatcherTimer();

        private void subtitleTimer_Tick(object sender, object e)
        {
            Subtitle = null;
        }

        private async Task<bool> AttachSubtitles(string uri)
        {
            subtitles = await Subtitles.Download(uri);
            if (subtitles != null)
            {
                int i = 0;
                foreach (Subtitle sub in subtitles)
                {
                    VideoMediaElement.Markers.Add(new TimelineMarker()
                    {
                        Time = sub.Start,
                        Type = "subtitle",
                        Text = (i++).ToString()
                    });
                }
                return true;
            }
            else
            {
                return false;
            }
        }

        private void SetSubtitle(int i)
        {
            subtitleTimer.Stop();

            if (i >= 0 && i < subtitles.Count)
            {
                Subtitle sub = subtitles[i];
                Subtitle = sub.Text;

                subtitleTimer.Interval = sub.End - sub.Start;
                subtitleTimer.Start();
            }
        }
        #endregion

        #region Chyba =============================================================================
        private void ShowError(string message, bool canPlayAnyway = false)
        {
            IsError = true;
            ErrorMessage = message;
            CanPlayAnyway = canPlayAnyway;

            StopVideoPlayback(); // projistotu, aby tam nic nehrálo
            HideSlider();

            ShowErrorStoryboard.Begin();
        }
        #endregion
    }
}
