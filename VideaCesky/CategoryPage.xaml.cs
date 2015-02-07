using VideaCesky.Common;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Graphics.Display;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using System.ComponentModel;
using System.Runtime.CompilerServices;

// The Basic Page item template is documented at http://go.microsoft.com/fwlink/?LinkID=390556

namespace VideaCesky
{
    public sealed partial class CategoryPage : Page, INotifyPropertyChanged
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

        private Category _category = null;
        public Category Category
        {
            get { return _category; }
            set { SetProperty(ref _category, value); }
        }

        private NavigationHelper navigationHelper;
        private ObservableDictionary defaultViewModel = new ObservableDictionary();

        public CategoryPage()
        {
            this.InitializeComponent();

            this.navigationHelper = new NavigationHelper(this);
            this.navigationHelper.LoadState += this.NavigationHelper_LoadState;
            this.navigationHelper.SaveState += this.NavigationHelper_SaveState;

            DataContext = this;
        }

        public NavigationHelper NavigationHelper
        {
            get { return this.navigationHelper; }
        }

        private void NavigationHelper_LoadState(object sender, LoadStateEventArgs e)
        {
        }

        private void NavigationHelper_SaveState(object sender, SaveStateEventArgs e)
        {
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            this.navigationHelper.OnNavigatedTo(e);

            Category = e.Parameter as Category;
            CategoryVideoList.Feed = Category.Feed;

            DisplayProperties_OrientationChanged(null);
            DisplayProperties.OrientationChanged -= DisplayProperties_OrientationChanged;
            DisplayProperties.OrientationChanged += DisplayProperties_OrientationChanged;
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            this.navigationHelper.OnNavigatedFrom(e);
            DisplayProperties.OrientationChanged -= DisplayProperties_OrientationChanged;
        }

        public void DisplayProperties_OrientationChanged(object sender)
        {
            CategoryVideoList.Orientation = DisplayProperties.CurrentOrientation;
        }

        private async void RefreshButton_Click(object sender, RoutedEventArgs e)
        {
            await CategoryVideoList.Refresh();
        }

        private void CategoryVideoList_Click(object sender, VideoEventArgs e)
        {
            Frame.Navigate(typeof(VideoPage), e.Video.Uri.ToString());
        }
    }
}
