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
        public static string YoutubePattern =
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

        public static string SubtitlesPattern = @"http(:|%3A)(\/|%2F)(\/|%2F)(www\.)?videacesky\.cz(?<uriPart>([A-Za-z0-9_\-\/]|%2F)*\.srt)";

        public static string SubtitlesUriFormat = "http://www.videacesky.cz{0}";

        public Uri VideoUri { get; set; }

        public string Title { get; set; }

        public string YoutubeId { get; set; }

        public string SubtitlesUriPart { get; set; }
    }
}
