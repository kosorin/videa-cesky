using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VideaCesky
{
    public class VideoSource
    {
        public string YoutubeUri { get; set; }

        public string SubtitleUri { get; set; }

        public string YoutubeId
        {
            get
            {
                return YoutubeUri;
            }
        }

        public VideoSource(string youtubeUri, string subtitleUri)
        {
            YoutubeUri = youtubeUri;
            SubtitleUri = subtitleUri;
        }
    }
}
