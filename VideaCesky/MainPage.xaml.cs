using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using System;
using Windows.System;
using Windows.Phone.UI.Input;

namespace VideaCesky
{
    public sealed partial class MainPage : VideoListBasePage
    {
        public MainPage()
        {
            this.InitializeComponent();
            DataContext = this;
            IsSuspendable = false;
        }

        protected override VideoList GetVideListControl()
        {
            return VideoListControl;
        }

        protected override async void OnNavigatedTo(MyToolkit.Paging.MtNavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            HardwareButtons.BackPressed -= HardwareButtons_BackPressed;
            HardwareButtons.BackPressed += HardwareButtons_BackPressed;
        }

        protected override void OnNavigatedFrom(MyToolkit.Paging.MtNavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);
            HardwareButtons.BackPressed -= HardwareButtons_BackPressed;
        }

        void HardwareButtons_BackPressed(object sender, BackPressedEventArgs e)
        {
            e.Handled = true;
            App.Current.Exit();
        }

        #region Kategorie
        private ObservableCollection<Category> _categories = null;
        public ObservableCollection<Category> Categories
        {
            get
            {
                if (_categories == null)
                {
                    _categories = new ObservableCollection<Category>();
                    //_categories.Add(new Category("Články", "Novinky, články a soutěže o ceny na webu VideaCesky.cz", "http://www.videacesky.cz/category/clanky-novinky-souteze"));
                    _categories.Add(new Category("Krátké filmy", "Krátké filmy s českými titulky pro vás zdarma. Pohádky online zdarma", "http://www.videacesky.cz/category/kratke-filmy-online-zdarma"));
                    _categories.Add(new Category("Legendární videa", "Do této kategorie je video zařazeno, jakmile je na našich stránkách déle než 3 měsíce, má více než 1500 hodnocení a známku aspoň 9,20 z 10.", "http://www.videacesky.cz/category/legendarni-videa"));
                    _categories.Add(new Category("Naučná", "Dokumentární videa, návody, pokusy a mnoho dalšího.", "http://www.videacesky.cz/category/navody-dokumenty-pokusy"));
                    _categories.Add(new Category("Ostatní", "Nezařaditelná cizojazyčná videa ze serveru VideaČesky s českými titulky.", "http://www.videacesky.cz/category/ostatni-zabavna-videa"));
                    _categories.Add(new Category("Parodie", "Parodie na seriály, filmy a populární hudební videoklipy. Funny parody song", "http://www.videacesky.cz/category/parodie-parody-youtube"));
                    _categories.Add(new Category("Reklamy", "Zábavné reklamní spoty. Reklamní slogany", "http://www.videacesky.cz/category/reklamy-reklamni-spot-video"));
                    _categories.Add(new Category("Rozhovory", "Talkshow a rozhovory se slavnými hvězdami.", "http://www.videacesky.cz/category/talk-show-rozhovory"));
                    _categories.Add(new Category("Seriály", "VideaČesky přináší krátké online seriály zdarma.", "http://www.videacesky.cz/category/serialy-online-zdarma"));
                    _categories.Add(new Category("Skeče", "Filmové zábavné scénky. Vtipné skeče na serveru VideaCesky.cz", "http://www.videacesky.cz/category/skece"));
                    _categories.Add(new Category("Trailery", "Trailery k filmům. Recenze populárních filmů.", "http://www.videacesky.cz/category/trailery-recenze-filmy"));
                    _categories.Add(new Category("Videoklipy", "Videoklipy zahraničních skupin z youtube. Parodie na nejznámější hudební klipy.", "http://www.videacesky.cz/category/hudebni-klipy-videoklipy-hudba"));
                }
                return _categories;
            }
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
