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
            @"https?:\/\/(www\.)?
            (
                youtube\.com\/
                (
	                embed\/|
	                watch\?
	                (
		                .*&
	                )?
	                v=
                )|
                youtu\.be\/
            )
            (?<youtubeId>[A-Za-z0-9_\-]{11})";

        public static string SubtitlesPattern = @"http:\/\/www\.videacesky\.cz(?<uriPart>[A-Za-z0-9_\-\/]*\.srt)";

        public static string SubtitlesUriFormat = "http://www.videacesky.cz{0}";

        public Uri VideoUri { get; set; }

        public string Title { get; set; }

        public string YoutubeId { get; set; }

        public string SubtitlesUriPart { get; set; }
    }
}
