using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ThePod.Models
{
    public class SpotifyViewModel
    {
        public List<Show> Shows { get; set; }
        public List<RootEpisodes> Episodes { get; set; }
        public List<SavedPodcast> SavedPodcast { get; set; }
        public List<UserFeedback> UserFeedback { get; set; }
        public List<EpByPodItem> EpisodesByPodIds { get; set; }

        public List<AspNetUser> AspNetUsers { get; set; }



    }
}
