using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Graphics.Display;
using Windows.Phone.UI.Input;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace VideaCesky
{
    public abstract class VideoListBasePage : Page, INotifyPropertyChanged
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

        public VideoList VideoList { get; protected set; }

        public VideoListBasePage()
        {
            DataContext = this;
            NavigationCacheMode = NavigationCacheMode.Required;
        }

        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            VideoList = GetVideListControl();
            AddRefreshAppBarButton();

            HardwareButtons.BackPressed -= HardwareButtons_BackPressed;
            HardwareButtons.BackPressed += HardwareButtons_BackPressed;
            DisplayProperties.OrientationChanged -= VideoList.OrientationChanged;
            DisplayProperties.OrientationChanged += VideoList.OrientationChanged;


            VideoList.StartRefreshing -= VideoList_StartRefreshing;
            VideoList.StartRefreshing += VideoList_StartRefreshing;
            VideoList.Refreshed -= VideoList_Refreshed;
            VideoList.Refreshed += VideoList_Refreshed;
            VideoList.UpdateOrientation();
            VideoList.PageFrame = Frame;

            SetFeed(e.Parameter);

            if (e.NavigationMode == NavigationMode.New || e.NavigationMode == NavigationMode.Refresh)
            {
                await VideoList.Refresh();
            }
            else
            {
                VideoList.RefreshScrollPosition();
            }
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);
            DisplayProperties.OrientationChanged -= VideoList.OrientationChanged;
            HardwareButtons.BackPressed -= HardwareButtons_BackPressed;

            VideoList.StartRefreshing -= VideoList_StartRefreshing;
            VideoList.Refreshed -= VideoList_Refreshed;

            VideoList.SaveScrollPosition();
        }

        protected abstract VideoList GetVideListControl();

        protected virtual void SetFeed(object parameter)
        {
            VideoList.Feed = "http://www.videacesky.cz";
        }

        protected void HardwareButtons_BackPressed(object sender, BackPressedEventArgs e)
        {
            if (Frame.CanGoBack)
            {
                Frame.GoBack();
                e.Handled = true;
            }
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            if (VideoList != null)
            {
                VideoList.RefreshScrollPosition();
            }
            return base.ArrangeOverride(finalSize);
        }

        private void AddRefreshAppBarButton()
        {
            if (BottomAppBar is CommandBar)
            {
                const string refreshTag = "REFRESH";

                CommandBar ab = ((CommandBar)BottomAppBar);
                if (ab.PrimaryCommands.Count > 0 && ab.PrimaryCommands[0] is AppBarButton)
                {
                    if (((AppBarButton)ab.PrimaryCommands[0]).Tag is string && (string)((AppBarButton)ab.PrimaryCommands[0]).Tag == refreshTag)
                    {
                        return;
                    }
                }

                AppBarButton refreshButton = new AppBarButton();
                refreshButton.Label = "obnovit";
                refreshButton.Icon = new SymbolIcon(Symbol.Refresh);
                refreshButton.Tag = refreshTag;
                refreshButton.Click += async (s, e) => await VideoList.Refresh();
                ((CommandBar)BottomAppBar).PrimaryCommands.Insert(0, refreshButton);
            }
        }

        private void VideoList_StartRefreshing(object sender, EventArgs e)
        {
            this.OnStartRefreshing();
        }

        private void VideoList_Refreshed(object sender, EventArgs e)
        {
            this.OnRefresh();
        }

        protected virtual void OnStartRefreshing()
        {
        }

        protected virtual void OnRefresh()
        {
        }
    }
}
