using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using System;
using Windows.System;

namespace VideaCesky
{
    public sealed partial class MainPage : VideoListBasePage
    {
        public MainPage()
        {
            this.InitializeComponent();
            DataContext = this;
            InitCategories();
        }

        protected override VideoList GetVideListControl()
        {
            return VideoListControl;
        }

        #region AppBar
        private void HelpButton_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(GuidePage));
        }

        private async void SearchButton_Click(object sender, RoutedEventArgs e)
        {
            await new SearchDialog().ShowAsync();
        }
        #endregion // end of AppBar

        #region Kategorie
        private ObservableCollection<Category> _categories = new ObservableCollection<Category>();
        public ObservableCollection<Category> Categories
        {
            get { return _categories; }
            set { SetProperty(ref _categories, value); }
        }

        private void InitCategories()
        {
            Categories.Add(new Category("Články", "Novinky, články a soutěže o ceny na webu VideaCesky.cz", "http://www.videacesky.cz/category/clanky-novinky-souteze"));
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
        }

        private void CategoriesListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ListView lw = (ListView)sender;
            if (lw.SelectedItem != null)
            {
                Category category = (Category)lw.SelectedItem;
                lw.SelectedItem = null;
                Frame.NavigateAsync(typeof(CategoryPage), category);
            }
        }
        #endregion // end of Kategorie
    }
}
