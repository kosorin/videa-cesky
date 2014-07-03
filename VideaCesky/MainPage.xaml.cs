using HtmlAgilityPack;
using MyToolkit.Networking;
using System;
using System.Diagnostics;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using System.Collections.Specialized;
using System.Collections;
using System.Collections.ObjectModel;
using Windows.UI.Popups;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Collections.Generic;


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

        public MainPage()
        {
            this.InitializeComponent();

            this.NavigationCacheMode = NavigationCacheMode.Required;

            Page = 1;
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            Button_Click_Html(null, null);
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


        public ObservableCollection<VideoSource> VideoList { get; set; }

        public ObservableCollection<int> Pagination { get; set; }

        public static int Page { get; set; }

        private async void Button_Click_Html(object sender, RoutedEventArgs e)
        {
            VideoList = new ObservableCollection<VideoSource>();
            Pagination = new ObservableCollection<int>();
            DataContext = this;
            try
            {
                HttpResponse response = await Http.GetAsync("http://www.videacesky.cz/page/" + Page.ToString());

                HtmlDocument doc = new HtmlDocument();
                doc.LoadHtml(response.Response);

                var contentNode = doc.GetElementbyId("contentArea");
                foreach (var n in contentNode.ChildNodes)
                {
                    if (n.NodeType == HtmlNodeType.Element && n.Id != "")
                    {
                        string title = n.ChildNodes.FindFirst("span").InnerText;
                        string uri = n.ChildNodes.FindFirst("a").Attributes["href"].Value;
                        string imageUri = n.ChildNodes.FindFirst("img").Attributes["src"].Value;

                        VideoList.Add(new VideoSource(title, "", "", "", uri, imageUri));

                        Debug.WriteLine("[{0}] {1} (odkaz: {2}) (obrázek: {3})", n.Id, title, uri, imageUri);
                    }
                }

                int minPage = 1;
                int currentPage = 1;
                int maxPage = 1;
                var pagination = contentNode.LastChild.PreviousSibling.ChildNodes.FindFirst("ol");
                foreach (var p in pagination.ChildNodes)
                {
                    if (p.Attributes.Contains("class") && p.Attributes["class"].Value == "current")
                    {
                        currentPage = Convert.ToInt32(p.ChildNodes.FindFirst("span").InnerText);
                    }
                    if (p.Attributes.Contains("class") && p.Attributes["class"].Value == "gap")
                    {
                        maxPage = Convert.ToInt32(p.NextSibling.ChildNodes.FindFirst("span").InnerText);
                    }
                }

                Pagination.Add(minPage);
                for (int i = currentPage - 10; i <= currentPage + 10; ++i)
                {
                    if (i > minPage && i < maxPage)
                    {
                        Pagination.Add(i);
                    }
                }
                Pagination.Add(maxPage);
                Debug.WriteLine("{0}..{1}..{2}", minPage, currentPage, maxPage);
            }
            catch (Exception ex)
            {
                Debug.WriteLine("CHYBA: " + ex.Message);
            }
        }

        private async void VideoListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (VideoListBox.SelectedItem != null)
            {
                VideoSource source = (VideoSource)VideoListBox.SelectedItem;
                bool stop = false;
                bool isPlaylist = false;
                try
                {
                    HttpResponse response = await Http.GetAsync(source.VideoUri);
                    HtmlDocument doc = new HtmlDocument();
                    doc.LoadHtml(response.Response);

                    var contentNode = doc.GetElementbyId("contentArea");
                    foreach (var ob in contentNode.ChildNodes.Descendants("object"))
                    {
                        if (stop)
                            break;

                        Debug.WriteLine("=======================================");
                        Debug.WriteLine("OBJECT: " + ob.InnerHtml);
                        Debug.WriteLine("=======================================");

                        foreach (var p in ob.Descendants())
                        {
                            if (p.NodeType == HtmlNodeType.Element && p.Name == "param" && p.Attributes.Contains("name") && p.Attributes["name"].Value == "flashvars")
                            {
                                string value = p.Attributes["value"].Value;
                                string[] values = value.Split(new string[] { "&amp;" }, StringSplitOptions.None);
                                Debug.WriteLine("PARAM: " + value);
                                foreach (string v in values)
                                {
                                    if (v.Contains("="))
                                    {
                                        if (v.Substring(0, v.IndexOf('=')) == "captions.file")
                                        {
                                            source.SubtitlesUri = v.Substring(v.IndexOf('=') + 1);
                                        }
                                        if (v.Substring(0, v.IndexOf('=')) == "file")
                                        {
                                            source.YoutubeUri = v.Substring(v.IndexOf('=') + 1);
                                        }
                                    }
                                }
                                stop = true;
                                break;
                            }
                        }
                        Debug.WriteLine("YOUTUBE: " + source.YoutubeUri);
                        Debug.WriteLine("SUBTITLES: " + source.SubtitlesUri);
                    }

                    if (!stop)
                    {
                        var ob = contentNode.ChildNodes.FindFirst("embed");

                        Debug.WriteLine("=======================================");
                        Debug.WriteLine("EMBED: " + ob.InnerHtml);
                        Debug.WriteLine("=======================================");

                        if (ob.Attributes.Contains("flashvars"))
                        {
                            string value = ob.Attributes["flashvars"].Value;
                            string[] values = value.Split(new string[] { "&amp;" }, StringSplitOptions.None);
                            Debug.WriteLine("PARAM: " + value);
                            foreach (string v in values)
                            {
                                if (v.Contains("="))
                                {
                                    if (v.Substring(0, v.IndexOf('=')) == "captions.file")
                                    {
                                        source.SubtitlesUri = v.Substring(v.IndexOf('=') + 1);
                                    }
                                    else if (v.Substring(0, v.IndexOf('=')) == "file")
                                    {
                                        source.YoutubeUri = v.Substring(v.IndexOf('=') + 1);
                                    }
                                    else if (v.Substring(0, v.IndexOf('=')) == "playlistfile")
                                    {
                                        Debug.WriteLine("PLAYLIST -----------------");
                                        isPlaylist = true;
                                    }
                                }
                            }
                            stop = true;
                        }
                        Debug.WriteLine("YOUTUBE: " + source.YoutubeUri);
                        Debug.WriteLine("SUBTITLES: " + source.SubtitlesUri);
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine("CHYBA: " + ex.Message);
                }

                VideoListBox.SelectedItem = null;
                if (stop)
                {
                    Frame.Navigate(typeof(VideoPage), source);
                }
                else if (isPlaylist)
                {
                    await (new MessageDialog("playlist")).ShowAsync();
                }
                else
                {
                    await (new MessageDialog("bohužel nic :(")).ShowAsync();
                }
            }
        }

        private void ListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (((ListBox)sender).SelectedItem != null)
            {
                Page = (int)((ListBox)sender).SelectedItem;
                Button_Click_Html(null, null);
                ((ListBox)sender).SelectedItem = null;
                OnPropertyChanged("VideoList");
                OnPropertyChanged("Pagination");
                OnPropertyChanged("Page");
            }
        }
    }
}
