using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace VideaCesky
{
    public class VideoData : BindableBase
    {
        public static readonly string YoutubeIdPattern =
            @"https?(:|%3A)(\/|%2F)(\/|%2F)(www\.)?
            (
                youtube\.com(\/|%2F)
                (
	                embed(\/|%2F)|
	                watch(\?|%3F)
	                (
		                .*(&|%26)
	                )?
	                v(=|%3D)
                )|
                youtu\.be(\/|%2F)
            )
            (?<youtubeId>[A-Za-z0-9_\-]{11})";

        public static readonly string SubtitlesUriPattern =
            @"(https?(:|%3A)(\/|%2F)(\/|%2F)(www\.)?
                videacesky\.cz)?
                (\/|%2F)
                    autori(\/|%2F)
                    [A-Za-z0-9_\-]+(\/|%2F)
                        (?<type>[A-Za-z0-9_\-]+)(\/|%2F)
                    [A-Za-z0-9_\-]+\.
                        (?<format>srt|xml)";

        public string Title { get; set; }

        public string YoutubeId { get; set; }

        public Uri SubtitlesUri { get; set; }
    }
}
