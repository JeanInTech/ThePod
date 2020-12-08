using System;
using System.Collections.Generic;

#nullable disable

namespace ThePod.Models
{
    public partial class Favorite
    {
        public Favorite()
        {
            InverseUser = new HashSet<Favorite>();
        }

        public Favorite(int id /*,int? userId, int? episodeId, string publisher, string podcastName, string episodeName, string audioPreviewUrl, string description, string durationMs, string externalUrls, string images, string releaseDatePrecision*/)
        {
            Id = id;
            /*UserId = userId;
            EpisodeId = episodeId;
            Publisher = publisher;
            PodCastName = podcastName;
            EpisodeName = episodeName;
            AudioPreviewUrl = audioPreviewUrl;
            Description = description;
            DurationMs = durationMs;
            ExternalUrls = externalUrls;
            Images = images;
            ReleaseDatePrecision = releaseDatePrecision;*/
        }

        public int Id { get; set; }
        public int? UserId { get; set; }
        public int? EpisodeId { get; set; }
        public string Publisher { get; set; }
        public string PodCastName { get; set; }
        public string EpisodeName { get; set; }
        public string AudioPreviewUrl { get; set; }
        public string Description { get; set; }
        public string DurationMs { get; set; }
        public string ExternalUrls { get; set; }
        public string Images { get; set; }
        public string ReleaseDatePrecision { get; set; }

        public virtual Favorite User { get; set; }
        public virtual ICollection<Favorite> InverseUser { get; set; }

       
    }
}
