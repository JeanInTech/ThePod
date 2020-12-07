using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ThePod.Models
{

    public class RootobjectEpisodes
    {
        public Episodes episodes { get; set; }
    }

    public class Episodes
    {
        public string href { get; set; }
        public EpisodeItem[] items { get; set; }
        public int limit { get; set; }
        public string next { get; set; }
        public int offset { get; set; }
        public object previous { get; set; }
        public int total { get; set; }
    }

    public class EpisodeItem
    {
        public string audio_preview_url { get; set; }
        public string description { get; set; }
        public int duration_ms { get; set; }
        public bool _explicit { get; set; }
        public External_Urls external_urls { get; set; }
        public string href { get; set; }
        public string id { get; set; }
        public Image[] images { get; set; }
        public bool is_externally_hosted { get; set; }
        public bool is_playable { get; set; }
        public string language { get; set; }
        public string[] languages { get; set; }
        public string name { get; set; }
        public string release_date { get; set; }
        public string release_date_precision { get; set; }
        public string type { get; set; }
        public string uri { get; set; }
        public Restriction restriction { get; set; }
    }

    public class Episode_External_Urls
    {
        public string spotify { get; set; }
    }

    public class Restriction
    {
        public string reason { get; set; }
    }

    public class EpisodeImage
    {
        public int height { get; set; }
        public string url { get; set; }
        public int width { get; set; }
    }

}
