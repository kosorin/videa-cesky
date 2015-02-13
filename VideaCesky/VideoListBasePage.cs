using MyToolkit.Paging;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
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
    public abstract class VideoListBasePage : MtPage, INotifyPropertyChanged
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

        public VideoList VideoList { get; private set; }

        protected async override void OnNavigatedTo(MtNavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            VideoList = GetVideListControl();
            AddAppBarButtons();

            DisplayInformation.AutoRotationPreferences = DisplayOrientations.Portrait | DisplayOrientations.PortraitFlipped;

            SetFeed(e.Parameter);
            if (e.NavigationMode == NavigationMode.New || e.NavigationMode == NavigationMode.Refresh
                || VideoList.List.Count == 0)
            {
                await VideoList.Refresh();
            }
            else
            {
                VideoList.RefreshScrollPosition();
            }
        }

        protected override void OnNavigatedFrom(MtNavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);
            DisplayInformation.AutoRotationPreferences = DisplayOrientations.None;

            VideoList.SaveScrollPosition();
        }

        protected abstract VideoList GetVideListControl();

        protected virtual void SetFeed(object parameter)
        {
            VideoList.Feed = "http://www.videacesky.cz";
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            if (VideoList != null)
            {
                VideoList.RefreshScrollPosition();
            }
            return base.ArrangeOverride(finalSize);
        }

        private void AddAppBarButtons()
        {
            if (BottomAppBar is CommandBar)
            {
                const string refreshTag = "REFRESH";
                const string searchTag = "SEARCH";
                const string homeTag = "HOME";

                CommandBar ab = ((CommandBar)BottomAppBar);
                if (ab.PrimaryCommands.Count > 0 && ab.PrimaryCommands[0] is AppBarButton)
                {
                    if (((AppBarButton)ab.PrimaryCommands[0]).Tag is string && (string)((AppBarButton)ab.PrimaryCommands[0]).Tag == refreshTag)
                    {
                        return;
                    }
                }

                bool isRefresh = false;
                bool isSearch = false;
                bool isHome = false;
                foreach (var b in ab.PrimaryCommands)
                {
                    if (b is AppBarButton)
                    {
                        AppBarButton button = (AppBarButton)b;
                        if ((button.Tag as string) == refreshTag)
                        {
                            isRefresh = true;
                            continue;
                        }
                        if ((button.Tag as string) == searchTag)
                        {
                            isSearch = true;
                            continue;
                        }
                        if ((button.Tag as string) == homeTag)
                        {
                            isHome = true;
                            continue;
                        }
                    }
                }

                if (!isRefresh)
                {
                    AppBarButton refreshButton = new AppBarButton();
                    refreshButton.Label = "obnovit";
                    refreshButton.Icon = new SymbolIcon(Symbol.Refresh);
                    refreshButton.Tag = refreshTag;
                    refreshButton.Click += async (s, e) => await VideoList.Refresh();
                    ((CommandBar)BottomAppBar).PrimaryCommands.Insert(0, refreshButton);
                }

                if (!isSearch)
                {
                    AppBarButton searchButton = new AppBarButton();
                    searchButton.Label = "vyhledávání";
                    searchButton.Icon = new SymbolIcon(Symbol.Find);
                    searchButton.Tag = searchTag;
                    searchButton.Click += async (s, e) => await new SearchDialog().ShowAsync();
                    ((CommandBar)BottomAppBar).PrimaryCommands.Insert(0, searchButton);
                }

                Type startPageType = ((MtApplication)App.Current).StartPageType;
                if (!isHome && this.GetType() != startPageType)
                {
                    AppBarButton searchButton = new AppBarButton();
                    searchButton.Label = "domů";
                    searchButton.Icon = new SymbolIcon(Symbol.Home);
                    searchButton.Tag = homeTag;
                    searchButton.Click += (s, e) => Frame.Navigate(startPageType);
                    ((CommandBar)BottomAppBar).PrimaryCommands.Insert(0, searchButton);
                }
            }
        }
    }
}
