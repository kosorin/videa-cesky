using HtmlAgilityPack;
using MyToolkit.Networking;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml.Linq;
using Windows.UI.Popups;

namespace VideaCesky.Models
{
    public class Subtitles : List<Subtitle>
    {
        private static readonly string srtPattern = @"\d+\r\n(?<start>\S+)\s-->\s(?<end>\S+)\r\n(?<text>(.|[\r\n])+?)\r\n\r\n";

        public Subtitle At(TimeSpan position)
        {
            foreach (Subtitle s in this)
            {
                if (s.Start <= position && s.End >= position)
                {
                    return s;
                }
            }
            return null;
        }

        public new Subtitle this[int i]
        {
            get
            {
                if (i >= 0 && i < Count)
                {
                    return this.ElementAt(i);
                }
                return null;
            }
        }

        public async static Task<Subtitles> Download(Uri uri)
        {
            string subtitlesText = "";
            try
            {
                HttpResponse response = await Http.GetAsync(uri);
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

            string fileExt = uri.OriginalString.Substring(uri.OriginalString.Length - 3, 3);
            if (fileExt == "srt")
                return SubRipSubtitles.Parse(subtitlesText);
            else
                return XmlSubtitles.Parse(subtitlesText);
        }

        private class SubRipSubtitles
        {
            public static Subtitles Parse(string srt)
            {
                srt += "\r\n\r\n";
                Subtitles subtitles = new Subtitles();

                var matches = Regex.Matches(srt, srtPattern);
                foreach (Match match in matches)
                {
                    GroupCollection groups = match.Groups;

                    Subtitle subtitle = new Subtitle();
                    subtitle.Start = TimeSpan.Parse(groups["start"].Value.Replace(',', '.'));
                    subtitle.End = TimeSpan.Parse(groups["end"].Value.Replace(',', '.'));
                    subtitle.Text = groups["text"].Value;

                    subtitles.Add(subtitle);
                }

                return subtitles;
            }
        }

        private class XmlSubtitles
        {
            public static Subtitles Parse(string xml)
            {
                Subtitles subtitles = new Subtitles();

                try
                {
                    XDocument doc = XDocument.Parse(xml);
                    XNamespace ns = XNamespace.None;
                    if (doc.Root.Attribute("xmlns") != null)
                    {
                        ns = doc.Root.Attribute("xmlns").Value;
                    }
                    XElement body = doc.Root.Element(ns + "body");

                    foreach (XElement s in ((XElement)body.FirstNode).Elements())
                    {
                        if (s.Name == ns + "p")
                        {
                            Subtitle subtitle = new Subtitle();

                            subtitle.Start = TimeSpan.Parse(s.Attribute("begin").Value);
                            subtitle.End = TimeSpan.Parse(s.Attribute("end").Value);
                            subtitle.Text = Regex.Replace(s.Value, @"<br\s*\/>", Environment.NewLine);

                            subtitles.Add(subtitle);
                        }
                    }
                }
                catch (Exception e)
                {
                    Debug.WriteLine(e.Message);
                }

                return subtitles;
            }
        }
    }
}
