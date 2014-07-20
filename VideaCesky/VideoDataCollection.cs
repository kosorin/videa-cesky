using HtmlAgilityPack;
using MyToolkit.Networking;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;

namespace VideaCesky
{
    public class VideoDataCollection : ObservableCollection<VideoData>, INotifyPropertyChanged
    {
        #region INotifyPropertyChanged
        public new event PropertyChangedEventHandler PropertyChanged;

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

        public string Title { get; set; }

        public VideoDataCollection(string title = "")
        {
            Title = title;

            CollectionChanged += OnCollectionChanged;
        }

        private void OnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            OnPropertyChanged("Count");
        }

        public static async Task<VideoDataCollection> ParsePage(Uri pageUri)
        {
            VideoDataCollection dataCollection = new VideoDataCollection();
            try
            {
                HttpResponse response = await Http.GetAsync(pageUri);
                HtmlDocument doc = new HtmlDocument();
                doc.LoadHtml(response.Response);
                HtmlNode contentNode = doc.GetElementbyId("contentArea");

                // Název videa
                HtmlNode titleNode = contentNode.ChildNodes.FindFirst("span");
                if (titleNode != null)
                {
                    dataCollection.Title = WebUtility.HtmlDecode(titleNode.InnerText);
                    Debug.WriteLine("Title: " + dataCollection.Title);
                }

                // Odkaz na video a titulky
                List<string> youtubeIdList = new List<string>();
                List<Uri> subtitlesList = new List<Uri>();
                Uri playlistUri = null;

                foreach (HtmlNode node in contentNode.Descendants("div")
                    .Where(n => n.Attributes.Contains("class") && n.Attributes["class"].Value == "postContent"))
                {
                    string postContent = node.InnerHtml;

                    // Video
                    MatchCollection youtubeMatches = Regex.Matches(postContent, VideoData.YoutubeIdPattern, RegexOptions.IgnorePatternWhitespace | RegexOptions.IgnoreCase);
                    foreach (Match youtubeMatch in youtubeMatches)
                    {
                        youtubeIdList.Add(WebUtility.UrlDecode(youtubeMatch.Groups["youtubeId"].Value));
                        Debug.WriteLine("Youtube ID [{0}]: {1}", youtubeIdList.Count, youtubeIdList[youtubeIdList.Count - 1]);
                    }

                    // Titulky
                    MatchCollection subtitlesMatches = Regex.Matches(postContent, VideoData.SubtitlesUriPattern, RegexOptions.IgnorePatternWhitespace | RegexOptions.IgnoreCase);
                    foreach (Match subtitlesMatch in subtitlesMatches)
                    {
                        Uri subtitlesUri = new Uri(subtitlesMatch.Value);
                        if (subtitlesMatch.Groups["type"].Value == "playlisty")
                        {
                            playlistUri = subtitlesUri;
                            break;
                        }
                        subtitlesList.Add(subtitlesUri);
                        Debug.WriteLine("Subtitles URI [{0}]: {1}", subtitlesList.Count, subtitlesList[subtitlesList.Count - 1]);
                    }
                }

                if (playlistUri != null)
                {
                    Debug.WriteLine("Playlist");
                    return await ParsePlaylist(dataCollection.Title, playlistUri);
                }
                else
                {
                    youtubeIdList = youtubeIdList.Distinct().ToList();
                    subtitlesList = subtitlesList.Distinct().ToList();

                    int min = Math.Min(youtubeIdList.Count, subtitlesList.Count);
                    for (int i = 0; i < min; ++i)
                    {
                        VideoData data = new VideoData();

                        data.Title = string.Format("{0}. část", i + 1);
                        data.YoutubeId = youtubeIdList[i];
                        data.SubtitlesUri = subtitlesList[i];

                        dataCollection.Add(data);
                    }
                }
            }
            catch (VideoException)
            {
                throw;
            }
            catch (Exception)
            {
                throw new VideoException();
            }

            return dataCollection;
        }

        private static async Task<VideoDataCollection> ParsePlaylist(string title, Uri playlistUri)
        {
            VideoDataCollection dataCollection = new VideoDataCollection(title);
            try
            {
                HttpResponse response = await Http.GetAsync(playlistUri);
                string playlistXml = response.Response;
                int startIndex = playlistXml.IndexOf('<');
                if (startIndex > 0)
                {
                    playlistXml = playlistXml.Remove(0, startIndex);
                }
                XDocument doc = XDocument.Parse(playlistXml);

                XNamespace ns = XNamespace.None;
                if (doc.Root.Attribute(XNamespace.Xmlns + "jwplayer") != null)
                {
                    ns = doc.Root.Attribute(XNamespace.Xmlns + "jwplayer").Value;
                }

                foreach (XElement node in doc.DescendantNodes()
                    .Where(n => n.NodeType == XmlNodeType.Element && ((XElement)n).Name == "item"))
                {
                    Debug.WriteLine("Video");
                    VideoData data = new VideoData();

                    // Title
                    data.Title = node.Element("title").Value.Trim();
                    Debug.WriteLine("  Title: {0}", data.Title);

                    // YoutubeID
                    Match youtubeMatch = Regex.Match(node.Element(ns + "file").Value.Trim(), VideoData.YoutubeIdPattern, RegexOptions.IgnorePatternWhitespace | RegexOptions.IgnoreCase);
                    if (youtubeMatch != null && youtubeMatch.Success)
                    {
                        data.YoutubeId = WebUtility.UrlDecode(youtubeMatch.Groups["youtubeId"].Value);
                        Debug.WriteLine("  Youtube ID: {0}", data.YoutubeId);
                    }
                    else
                    {
                        throw new VideoException("Špatný odkaz videa.");
                    }

                    // Subtitles
                    data.SubtitlesUri = new Uri("http://www.videacesky.cz" + (node.Element(ns + "captions.file").Value.Trim()));
                    Debug.WriteLine("  Subtitles URI: {0}", data.SubtitlesUri);

                    dataCollection.Add(data);
                }
            }
            catch (VideoException)
            {
                throw;
            }
            catch (Exception)
            {
                throw new VideoException();
            }

            return dataCollection;
        }
    }
}
