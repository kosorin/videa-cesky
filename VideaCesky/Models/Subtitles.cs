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
    public partial class Subtitles : List<Subtitle>
    {
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

        public static Subtitles Parse(string type, string data)
        {
            if (type == "srt")
                return SubRipSubtitles.Parse(data);
            else
                return XmlSubtitles.Parse(data);
        }
    }
}
