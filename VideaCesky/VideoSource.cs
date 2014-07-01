using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace VideaCesky
{
    public class VideoSource : BindableBase
    {
        private string _title;
        public string Title
        {
            get { return _title; }
            set { SetProperty(ref _title, value); }
        }

        public string Description { get; set; }

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

        public VideoSource(string title, string description, string youtubeUri, string subtitlesUri)
        {
            Title = title;
            Description = description;
            YoutubeUri = youtubeUri;
            SubtitlesUri = subtitlesUri;
        }
    }
}
