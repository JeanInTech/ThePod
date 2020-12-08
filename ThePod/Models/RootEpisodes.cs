using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ThePod.Models
{
    public class RootEpisodes
    {
        public Episode[] episodes { get; set; }
    }

    public class Episode
    {
        public string audio_preview_url { get; set; }
        public string description { get; set; } //description of the specific episode 
        public int duration_ms { get; set; }
        public bool _explicit { get; set; }
        public External_Urls external_urls { get; set; } 
        public string href { get; set; }
        public string id { get; set; } //episode ID
        public Image1[] images { get; set; }
        public bool is_externally_hosted { get; set; }
        public bool is_playable { get; set; }
        public string language { get; set; }
        public string[] languages { get; set; }
        public string name { get; set; } //name of the specific episode
        public string release_date { get; set; }
        public string release_date_precision { get; set; }
        public Show show { get; set; } //this gets me into the show data
        public string type { get; set; }
        public string uri { get; set; }
    }

    public class Root_Episode_External_Urls
    {
        public string spotify { get; set; }
    }

    public class Show
    {
        public string[] available_markets { get; set; }
        public object[] copyrights { get; set; }
        public string description { get; set; } //podcast show info
        public bool _explicit { get; set; }
        public External_Urls1 external_urls { get; set; }
        public string href { get; set; }
        public string id { get; set; }
        public Image[] images { get; set; }
        public bool is_externally_hosted { get; set; }
        public string[] languages { get; set; }
        public string media_type { get; set; }
        public string name { get; set; } //Series name of podcast
        public string publisher { get; set; }
        public int total_episodes { get; set; }
        public string type { get; set; }
        public string uri { get; set; }
    }

    public class External_Urls1
    {
        public string spotify { get; set; }
    }

    public class Root_episode_Image
    {
        public int height { get; set; }
        public string url { get; set; }
        public int width { get; set; }
    }

    public class Image1
    {
        public int height { get; set; }
        public string url { get; set; }
        public int width { get; set; }
    }

}
