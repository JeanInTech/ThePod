using System;
using System.Collections.Generic;

namespace ThePod.Models
{
    public partial class SavedPodcast
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public string EpisodeId { get; set; }
        public string PodcastName { get; set; }
        public string EpisodeName { get; set; }
        public string Publisher { get; set; }
        public string Description { get; set; }
        public string AudioPreviewUrl { get; set; }
        public string ExternalUrls { get; set; }
        public string ImageUrl { get; set; }
        public int? Duration { get; set; }
        public DateTime? ReleaseDate { get; set; }

        public virtual AspNetUsers User { get; set; }
    }
}
