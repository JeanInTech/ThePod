using System;
using System.Collections.Generic;

#nullable disable

namespace ThePod.Models
{
    public partial class UserFeedback
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public string EpisodeId { get; set; }
        public string Tags { get; set; }
        public byte? Rating { get; set; }
        public string Review { get; set; }

        public virtual AspNetUser User { get; set; }
    }
}
