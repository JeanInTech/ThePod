using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ThePod.Models
{
    public class PopularEpisodes
    {
        public UserProfile userProfile { get; set; }
        public UserFeedback userFeedback { get; set; }
        public string EpisodeName { get; set; }
    }
   
}
