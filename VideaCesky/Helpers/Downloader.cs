using HtmlAgilityPack;
using MyToolkit.Networking;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using VideaCesky.Models;

namespace VideaCesky.Helpers
{
    public static class Downloader
    {
        public static async Task<List<Video>> GetVideoList(Uri listUri)
        {
            List<Video> list = new List<Video>();
            try
            {
                HttpResponse response = await Http.GetAsync(listUri);
                HtmlDocument doc = new HtmlDocument();
                doc.LoadHtml(response.Response);

                HtmlNode contentArea = doc.GetElementbyId("contentArea");
                foreach (var node in contentArea.ChildNodes)
                {
                    if (node.NodeType == HtmlNodeType.Element && node.Id != "")
                    {
                        try
                        {
                            // Uri
                            Uri videoUri = new Uri(node.ChildNodes.FindFirst("a").Attributes["href"].Value);
                            if (videoUri.ToString().Contains("videacesky.cz/clanky-novinky-souteze"))
                            {
                                // toto nejsou videa...
                                continue;
                            }

                            // Title
                            string title = WebUtility.HtmlDecode(node.ChildNodes.FindFirst("span").InnerText);

                            // ImageUri
                            string imageUriString = node.ChildNodes.FindFirst("img").Attributes["src"].Value;
                            Uri imageUri = new Uri(imageUriString);

                            IEnumerable<HtmlNode> descendants = node.Descendants();
                            // Date
                            // Formát: 8.7.2014 v 08:00
                            HtmlNode dateNode = descendants.First(n => n.Attributes.Contains("class") && n.Attributes["class"].Value == "postDate");
                            DateTime date = DateTime.ParseExact(
                                dateNode.InnerText,
                                "d'.'M'.'yyyy' v 'HH':'mm",
                                CultureInfo.InvariantCulture);

                            // Detail
                            HtmlNode detailNode = descendants.First(n => n.Attributes.Contains("class") && n.Attributes["class"].Value == "obs");
                            string detail = WebUtility.HtmlDecode(Regex.Replace(detailNode.InnerText.Replace("(Celý příspěvek...)", ""), @"<!--[^>]*-->", "")).Trim();

                            // Tags
                            List<Tag> tags = new List<Tag>();
                            HtmlNode tagsNode = descendants.First(n => n.Attributes.Contains("class") && n.Attributes["class"].Value == "postTags");
                            foreach (var tagNode in tagsNode.ChildNodes)
                            {
                                if (tagNode.NodeType == HtmlNodeType.Element && tagNode.Name == "a")
                                {
                                    if (tagNode.Attributes.Contains("href"))
                                    {
                                        tags.Add(new Tag(tagNode.Attributes["href"].Value, tagNode.InnerText));
                                    }
                                }
                            }

                            // Rating
                            HtmlNode ratingNode = descendants.First(n => n.Attributes.Contains("class") && n.Attributes["class"].Value == "post-ratings");
                            string valueString = ratingNode.ChildNodes.First(n => n.Name == "strong").InnerText;
                            valueString = valueString.Replace(',', '.');
                            double rating = double.Parse(valueString, CultureInfo.InvariantCulture);

                            list.Add(new Video()
                            {
                                Uri = videoUri,
                                Title = title,
                                Detail = detail,
                                ImageUri = imageUri,
                                Date = date,
                                Tags = tags,
                                Rating = rating
                            });
                        }
                        catch
                        {
                            continue;
                        }
                    }
                }
                Debug.WriteLine("Loaded Page {0}", listUri.PathAndQuery);
            }
            catch (Exception e)
            {
                Debug.WriteLine("Error [{0}]: {1}", listUri, e.Message);
                return null;
            }
            return list;
        }

        public static async Task<Subtitles> GetSubtitles(Uri subtitlesUri)
        {
            string subtitlesText = "";
            try
            {
                HttpResponse response = await Http.GetAsync(subtitlesUri);
                subtitlesText = response.Response;
            }
            catch (OperationCanceledException)
            {
                return null;
            }
            catch (Exception)
            {
                return null;
            }

            string fileExt = subtitlesUri.OriginalString.Substring(subtitlesUri.OriginalString.Length - 3, 3);
            return Subtitles.Parse(fileExt, subtitlesText);
        }

        public static async Task<VideoDataCollection> GetVideoData(Uri pageUri)
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
                        string struri = subtitlesMatch.Value;
                        if (subtitlesMatch.Value.StartsWith("/autori"))
                        {
                            struri = "http://www.videacesky.cz" + struri;
                        }

                        Uri subtitlesUri = new Uri(struri);
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
                    dataCollection = await GetVideoDataFromPlaylist(playlistUri);
                    if (dataCollection != null)
                    {
                        dataCollection.Title = dataCollection.Title;
                    }
                    return dataCollection;
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

        private static async Task<VideoDataCollection> GetVideoDataFromPlaylist(Uri playlistUri)
        {
            VideoDataCollection dataCollection = new VideoDataCollection();
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

        public static async Task<Tuple<List<Comment>, int?>> GetComments(Uri pageUri)
        {
            try
            {
                HttpResponse response = await Http.GetAsync(pageUri);
                HtmlDocument doc = new HtmlDocument();
                doc.LoadHtml(response.Response);
                HtmlNode contentNode = doc.GetElementbyId("arjuna_comments");

                if (contentNode != null)
                {
                    int? currentPage = null;
                    HtmlNode paginationNode = contentNode.ChildNodes.FirstOrDefault(n => n.Attributes.Contains("class") && n.Attributes["class"].Value == "pagination commentPagination");
                    if (paginationNode != default(HtmlNode))
                    {
                        HtmlNode currentPageNode = paginationNode.Descendants().FirstOrDefault(n => n.Attributes.Contains("class") && n.Attributes["class"].Value == "page-numbers current");
                        if (currentPageNode != default(HtmlNode))
                        {
                            currentPage = int.Parse(currentPageNode.InnerText.Trim());
                        }
                    }

                    HtmlNode root = contentNode.ChildNodes.FirstOrDefault(n => n.NodeType == HtmlNodeType.Element && n.Name == "ul");
                    if (root != null)
                    {
                        List<Comment> comments = GetCommentsFromHtmlNode(root);
                        return new Tuple<List<Comment>, int?>(comments, currentPage);
                    }
                    else if (contentNode.Name == "p" && root.Attributes.Contains("class") && root.Attributes["class"].Value == "noComments")
                    {
                        return null;
                    }
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine("Error Comments: {0}", e.Message);
            }

            return null;
        }

        private static List<Comment> GetCommentsFromHtmlNode(HtmlNode root, int level = 0)
        {
            List<Comment> comments = new List<Comment>();
            foreach (HtmlNode node in root.ChildNodes.Where(n => n.NodeType == HtmlNodeType.Element && n.Name == "li"))
            {
                IEnumerable<HtmlNode> descendants = node.Descendants();

                // Id
                string id = node.Id.Replace("comment-", "");

                // Autor
                HtmlNode authorNode = descendants.Where(n => n.Attributes.Contains("href") && n.Attributes["href"].Value == "#comment-" + id).First();
                string author = authorNode.InnerText.Trim();

                // Date
                string dateString = authorNode.NextSibling.InnerText.Trim();
                DateTime date = DateTime.ParseExact(
                    dateString,
                    "HH':'mm' - 'd'.'M'.'yyyy",
                    CultureInfo.InvariantCulture);

                // Text
                List<string> texts = new List<string>();
                HtmlNode textRoot = descendants.First(n => n.Id == "commentbody-" + id);
                List<HtmlNode> textNodes;
                HtmlNode textDivNode = textRoot.ChildNodes.FirstOrDefault(n => n.NodeType == HtmlNodeType.Element && n.Name == "div");
                if (textDivNode != default(HtmlNode))
                {
                    textNodes = new List<HtmlNode>(textDivNode.ChildNodes.Where(n => n.NodeType == HtmlNodeType.Element && n.Name == "p"));
                }
                else
                {
                    textNodes = new List<HtmlNode>(textRoot.ChildNodes.Where(n => n.NodeType == HtmlNodeType.Element && n.Name == "p"));
                    textNodes.RemoveAt(textNodes.Count - 1);
                }
                foreach (HtmlNode textNode in textNodes)
                {
                    string part = "";
                    foreach (HtmlNode n in textNode.ChildNodes)
                    {
                        switch (n.NodeType)
                        {
                        case HtmlNodeType.Element:
                            if (n.Name == "img" && n.Attributes.Contains("alt"))
                            {
                                string emoticon = n.Attributes["alt"].Value;
                                part += emoticon;
                            }
                            else
                            {
                                part += n.InnerText;
                            }
                            break;
                        case HtmlNodeType.Text:
                            part += n.InnerText;
                            break;
                        default:
                            break;
                        }
                    }
                    texts.Add(WebUtility.HtmlDecode(part.Trim()));
                }
                string text = string.Join(Environment.NewLine, texts);

                // Karma + IsPopular
                HtmlNode karmaNode = textRoot.ChildNodes.Last(n => n.NodeType == HtmlNodeType.Element && n.Name == "p");
                bool isPopular = karmaNode.InnerText.Contains("Oblíbený komentář.");
                string karmaUp = karmaNode.ChildNodes.Where(n => n.Id == "karma-" + id + "-up").First().InnerText;
                string karmaDown = karmaNode.ChildNodes.Where(n => n.Id == "karma-" + id + "-down").First().InnerText;

                comments.Add(new Comment()
                {
                    Author = author,
                    Date = date,
                    Text = text,
                    KarmaUp = int.Parse(karmaUp),
                    KarmaDown = int.Parse(karmaDown),
                    IsPopular = isPopular,
                    Level = level
                });

                // Pod-komentáře
                HtmlNode childrenNode = node.ChildNodes.FirstOrDefault(n => n.NodeType == HtmlNodeType.Element && n.Name == "ul" && n.Attributes.Contains("class") && n.Attributes["class"].Value == "children");
                if (childrenNode != default(HtmlNode))
                {
                    comments.AddRange(GetCommentsFromHtmlNode(childrenNode, level + 1));
                }
            }
            return comments;
        }
    }
}
