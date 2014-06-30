using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace VideaCesky
{
    public class VideoSource
    {
        public string YoutubeUri { get; set; }

        public string SubtitlesUri { get; set; }

        public string YoutubeId
        {
            get
            {
                var match = Regex.Match(YoutubeUri, @"watch\?v=(?<id>[a-zA-Z0-9]+)");
                if (match != null && match.Success)
                {
                    return match.Groups["id"].Value;
                }
                return YoutubeUri;
            }
        }

        public VideoSource(string youtubeUri, string subtitlesUri)
        {
            YoutubeUri = youtubeUri;
            SubtitlesUri = subtitlesUri;
        }
    }
}
