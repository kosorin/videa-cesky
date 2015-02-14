using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace VideaCesky.Models
{
    public partial class Subtitles : List<Subtitle>
    {
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
