using HtmlAgilityPack;
using MyToolkit.Networking;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;

namespace VideaCesky
{
    public class VideoDataCollection : List<VideoData>
    {
        public string Title { get; set; }

        public VideoDataCollection(string title = "")
        {
            Title = title;
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

                        data.Title = string.Format("{0}. část", i);
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

                foreach (XNode node in doc.DescendantNodes()
                    .Where(n => n.NodeType == XmlNodeType.Element && ((XElement)n).Name == "item"))
                {
                    Debug.WriteLine("Video");
                    VideoData data = new VideoData();

                    // Title
                    XElement current = (XElement)((XElement)node).FirstNode;
                    data.Title = current.Value;
                    Debug.WriteLine("  Title: {0}", data.Title);

                    // YoutubeID
                    current = (XElement)current.NextNode;
                    Match youtubeMatch = Regex.Match(current.Value, VideoData.YoutubeIdPattern, RegexOptions.IgnorePatternWhitespace | RegexOptions.IgnoreCase);
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
                    current = (XElement)((XElement)current.NextNode).NextNode; // jde se ještě přes duration
                    data.SubtitlesUri = new Uri("http://www.videacesky.cz" + current.Value);
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
