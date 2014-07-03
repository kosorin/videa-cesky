using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace VideaCesky
{
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();

            this.NavigationCacheMode = NavigationCacheMode.Required;
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(VideoPage), new VideoSource(
                "Který X-Man je nejlepší?",
                "Člen komediální skupiny Suricate Julien Josselin a známý francouzský vlogger Cyprien o tom podiskutují v následujícím videu.",
                "https://www.youtube.com/watch?v=pVMoi5weypI",
                "http://www.videacesky.cz/autori/qetu/titulky/KteryX-Man.srt"));
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(VideoPage), new VideoSource(
                "Odvážní válečníci – 2×05 – Želátko navždy",
                "Mít doma želé skřítka, který tvoří toasty, musí být skvělé. Co se ale stane, když ho potká Catbug?",
                "https://www.youtube.com/watch?v=839ptAhRI9I",
                "http://www.videacesky.cz/autori/Erzika/titulky/OdvazniValecnici205.srt"));
        }

        private void Button_Click_3(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(VideoPage), new VideoSource(
                "Je možná telepatie?",
                "Je v dnešní době možné číst lidem myšlenky? A jak to bude vypadat v budoucnosti? Přesně na to se v následujícím videu zaměří americký fyzik Michio Kaku.",
                "https://www.youtube.com/watch?v=OjcgT_oj3jQ",
                "http://www.videacesky.cz/autori/qetu/titulky/Jemoznatelepatie.srt"));
        }

        private void Button_Click_4(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(VideoPage), new VideoSource(
                "Google vyděrač",
                "Google si pro veřejnost připravil zbrusu novou službu, Google vyděrač. Také se toužíte dozvědět, co s sebou přináší?",
                "https://www.youtube.com/watch?v=ymkA1N3oFwg",
                "http://www.videacesky.cz/autori/qetu/titulky/GooglevyderacEN.srt"));
        }

        private void Button_Click_5(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(VideoPage), new VideoSource(
                "Whose Line Is It Anyway?: Zpravodajské panoptikum #5",
                "V dnešním Zpravodajském panoptiku zazáří zejména Wayne jako zrychlená a zpomalená kazeta. Sekunduje mu Greg jako kapitán Kirk a Ryan jako rocková hvězda.",
                "http://www.youtube.com/watch?v=O1YNH6ogm2k",
                "http://www.videacesky.cz/autori/Jackolo/titulky/WLIIAZpravodajskePanoptikum5.srt"));
        }

        private void Button_Click_Video(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(VideoPage), new VideoSource(
                "chyba při načítání videa",
                "",
                "adsfasdfasdfasdfasdfd",
                "http://www.videacesky.cz/autori/Jackolo/titulky/WLIIAZpravodajskePanoptikum5.srt"));
        }

        private void Button_Click_Subtitles(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(VideoPage), new VideoSource(
                "Happy Tree Friends - All Work And No Play (Ep #76)",
                "",
                "https://www.youtube.com/watch?v=d4NmZRXa8zo",
                "asdqweasdfasd"));
        }
    }
}
