using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ThePod.Models
{
    public class FavoriteViewModel
    {
        public int SavedPodcastId { get; set; }
        public string EpisodeId { get; set; }
        public string PodcastName { get; set; }
        public string EpisodeName { get; set; }
        public string Publisher { get; set; }
        public string Description { get; set; }
        public string AudioPreviewUrl { get; set; }
        public string ExternalUrls { get; set; }
        public string ImageUrl { get; set; }
        public int Duration { get; set; }
        public DateTime ReleaseDate { get; set; }
        public int UserFeedbackId { get; set; }
        public byte? Rating { get; set; }
        public string Review { get; set; }
        public string Tags { get; set; }
        public DateTime DatePosted { get; set; }
    }
}
