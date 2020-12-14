using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

#nullable disable

namespace ThePod.Models
{
    public partial class UserProfile
    {
        public int Id { get; set; }
        public int? UserFeedbackId { get; set; }
        [Required]
        public string UserId { get; set; }
        public string EpisodeId { get; set; }
        public string Tag { get; set; }
        public byte? Rating { get; set; }

        public virtual UserFeedback UserFeedback { get; set; }
    }
}
