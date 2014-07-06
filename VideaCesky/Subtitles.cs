using MyToolkit.Networking;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Windows.UI.Popups;

namespace VideaCesky
{
    public class Subtitles : List<Subtitle>
    {
        public static string srtPattern = @"\d+\r\n(?<start>\S+)\s-->\s(?<end>\S+)\r\n(?<text>(.|[\r\n])+?)\r\n\r\n";

        public async static Task<Subtitles> Download(Uri uri)
        {
            string srt = "";
            try
            {
                HttpResponse response = await Http.GetAsync(uri);
                srt = response.Response;
            }
            catch (OperationCanceledException)
            {
                return null;
            }
            catch (Exception)
            {
                return null;
            }
            return Parse(srt);
        }

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
}
