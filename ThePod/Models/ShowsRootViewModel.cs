using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ThePod.Models
{
    public class ShowsRootViewModel
    {
        public List<Show> Shows { get; set; }
        public List<RootEpisodes> Episodes { get; set; }
        public SavedPodcast SavedPodcast { get; set; }
    }
}
