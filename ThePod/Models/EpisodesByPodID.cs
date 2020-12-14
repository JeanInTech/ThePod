using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ThePod.Models
{
    /* This class is used to obtain episode data when you are using the GetEpisodeByPodcast method
     * That method calls the API at a new endpoint using SearchEpbyPodIdAsync in the DAL
     * and this is what it returns*/
    public class EpisodesByPodId
    {
        public string href { get; set; }
        public EpByPodItem[] items { get; set; }
        public int limit { get; set; }
        public string next { get; set; }
        public int offset { get; set; }
        public string previous { get; set; }
        public int total { get; set; }
    }

    public class EpByPodItem
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
        public DateTime release_date { get; set; }
        public string release_date_precision { get; set; }
        public Resume_Point resume_point { get; set; }
        public string type { get; set; }
        public string uri { get; set; }
    }

    public class EpByPodExternal_Urls
    {
        public string spotify { get; set; }
    }

    public class Resume_Point
    {
        public bool fully_played { get; set; }
        public int resume_position_ms { get; set; }
    }

    public class EpByPodImage
    {
        public int height { get; set; }
        public string url { get; set; }
        public int width { get; set; }
    }

}
