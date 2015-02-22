using MyToolkit.Paging;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using VideaCesky.Controls;
using VideaCesky.Models;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace VideaCesky.Pages
{
    public sealed partial class SavedTagsPage : MtPage, INotifyPropertyChanged
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

        public SavedTagsPage()
        {
            this.InitializeComponent();
            DataContext = this;
        }

        protected override void OnNavigatedTo(MtNavigationEventArgs args)
        {
            base.OnNavigatedTo(args);
            TagsListView.ItemsSource = Settings.Current.SavedTags;

            SavedTags_CollectionChanged(null, null);
            Settings.Current.SavedTags.CollectionChanged -= SavedTags_CollectionChanged;
            Settings.Current.SavedTags.CollectionChanged += SavedTags_CollectionChanged;
        }

        protected override void OnNavigatedFrom(MtNavigationEventArgs args)
        {
            base.OnNavigatedFrom(args);
            Settings.Current.SavedTags.CollectionChanged -= SavedTags_CollectionChanged;
        }

        private Visibility _noTags = Visibility.Visible;
        public Visibility NoTags
        {
            get { return _noTags; }
            set { SetProperty(ref _noTags, value); }
        }

        void SavedTags_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            NoTags = (Settings.Current.SavedTags.Count == 0) ? Visibility.Visible : Visibility.Collapsed;
        }

        private async void DeleteAllAppBarButton_Click(object sender, RoutedEventArgs e)
        {
            await Settings.Current.ClearSavedTags();
        }

        #region Reorder
        private ListViewReorderMode _reorderMode = ListViewReorderMode.Disabled;
        public ListViewReorderMode ReorderMode
        {
            get { return _reorderMode; }
            set
            {
                if (SetProperty(ref _reorderMode, value))
                {
                    bool isEnabled = (TagsListView.ReorderMode == ListViewReorderMode.Disabled);
                    AppBar.ClosedDisplayMode = isEnabled ? AppBarClosedDisplayMode.Compact : AppBarClosedDisplayMode.Minimal;
                    foreach (Tag tag in TagsListView.Items)
                    {
                        tag.IsEnabled = isEnabled;
                    }
                }
            }
        }

        private void ReorderAppBarButton_Click(object sender, RoutedEventArgs e)
        {
            if (Settings.Current.SavedTags.Count > 1)
            {
                TagsListView.ReorderMode = ListViewReorderMode.Enabled;
            }
        }
        #endregion // end of Reorder
    }
}
