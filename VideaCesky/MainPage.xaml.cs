using HtmlAgilityPack;
using MyToolkit.Networking;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Graphics.Display;
using Windows.System;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

namespace VideaCesky
{
    public sealed partial class MainPage : Page, INotifyPropertyChanged
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

        private ObservableCollection<Category> _categories = new ObservableCollection<Category>();
        public ObservableCollection<Category> Categories
        {
            get { return _categories; }
            set { SetProperty(ref _categories, value); }
        }

        public MainPage()
        {
            this.InitializeComponent();
            DataContext = this;

            //Categories.Add(new Category("Články", "Novinky, články a soutěže o ceny na webu VideaCesky.cz", "http://www.videacesky.cz/category/clanky-novinky-souteze"));
            Categories.Add(new Category("Krátké filmy", "Krátké filmy s českými titulky pro vás zdarma. Pohádky online zdarma", "http://www.videacesky.cz/category/kratke-filmy-online-zdarma"));
            Categories.Add(new Category("Legendární videa", "Do této kategorie je video zařazeno, jakmile je na našich stránkách déle než 3 měsíce, má více než 1500 hodnocení a známku aspoň 9,20 z 10.", "http://www.videacesky.cz/category/legendarni-videa"));
            Categories.Add(new Category("Naučná", "Dokumentární videa, návody, pokusy a mnoho dalšího.", "http://www.videacesky.cz/category/navody-dokumenty-pokusy"));
            Categories.Add(new Category("Ostatní", "Nezařaditelná cizojazyčná videa ze serveru VideaČesky s českými titulky.", "http://www.videacesky.cz/category/ostatni-zabavna-videa"));
            Categories.Add(new Category("Parodie", "Parodie na seriály, filmy a populární hudební videoklipy. Funny parody song", "http://www.videacesky.cz/category/parodie-parody-youtube"));
            Categories.Add(new Category("Reklamy", "Zábavné reklamní spoty. Reklamní slogany", "http://www.videacesky.cz/category/reklamy-reklamni-spot-video"));
            Categories.Add(new Category("Rozhovory", "Talkshow a rozhovory se slavnými hvězdami.", "http://www.videacesky.cz/category/talk-show-rozhovory"));
            Categories.Add(new Category("Seriály", "VideaČesky přináší krátké online seriály zdarma.", "http://www.videacesky.cz/category/serialy-online-zdarma"));
            Categories.Add(new Category("Skeče", "Filmové zábavné scénky. Vtipné skeče na serveru VideaCesky.cz", "http://www.videacesky.cz/category/skece"));
            Categories.Add(new Category("Trailery", "Trailery k filmům. Recenze populárních filmů.", "http://www.videacesky.cz/category/trailery-recenze-filmy"));
            Categories.Add(new Category("Videoklipy", "Videoklipy zahraničních skupin z youtube. Parodie na nejznámější hudební klipy.", "http://www.videacesky.cz/category/hudebni-klipy-videoklipy-hudba"));

            VideoList.Refresh();

            NavigationCacheMode = NavigationCacheMode.Required;
#if DEBUG
            //Loaded += async (s, e) =>
            //{
            //    ListPickerFlyout fo = new ListPickerFlyout();
            //    fo.ItemsSource = new Dictionary<string, string>() 
            //    { 
            //        {"normální", "http://www.videacesky.cz/serialy-online-zdarma/odvazni-valecnici-2x06-loutkovy-horor"},
            //        {"playlist 1", "http://www.videacesky.cz/ostatni-zabavna-videa/conan-policejnim-straznikem"},
            //        {"playlist 2", "http://www.videacesky.cz/talk-show-rozhovory/arnold-schwarzenegger-u-jimmyho-fallona"},
            //        {"více videí", "http://www.videacesky.cz/talk-show-rozhovory/russell-brand-u-conana-obriena"},
            //        {"xml", "http://www.videacesky.cz/reklamy-reklamni-spot-video/nekompromisni-maskot"},
            //    };
            //    fo.ItemsPicked += (s2, e2) =>
            //    {
            //        if (e2.AddedItems.Count > 0)
            //        {
            //            Frame.Navigate(typeof(VideoPage), ((KeyValuePair<string, string>)e2.AddedItems[0]).Value);
            //        }
            //    };

            //    await fo.ShowAtAsync(ContentRoot);
            //};
#endif
        }

        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            DisplayProperties_OrientationChanged(null);
            DisplayProperties.OrientationChanged -= DisplayProperties_OrientationChanged;
            DisplayProperties.OrientationChanged += DisplayProperties_OrientationChanged;
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);
            DisplayProperties.OrientationChanged -= DisplayProperties_OrientationChanged;
        }

        public void DisplayProperties_OrientationChanged(object sender)
        {
            VideoList.Orientation = DisplayProperties.CurrentOrientation;
        }

        private void HelpButton_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(Guide));
        }

        private async void RefreshButton_Click(object sender, RoutedEventArgs e)
        {
            await VideoList.Refresh();
        }

        private void VideoList_Click(object sender, VideoEventArgs e)
        {
            Frame.Navigate(typeof(VideoPage), e.Video.Uri.ToString());
        }

        private void CategoriesListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ListView lw = (ListView)sender;
            if (lw.SelectedItem != null)
            {
                Category category = (Category)lw.SelectedItem;
                lw.SelectedItem = null;
                Frame.Navigate(typeof(CategoryPage), category);
            }
        }
    }
}
