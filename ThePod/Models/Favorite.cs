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
