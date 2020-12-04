using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ThePod.Models
{
    public class Shows
    {
        public Show[] shows { get; set; }
    }

    public class Show
    {
        public string[] available_markets { get; set; }
        public object[] copyrights { get; set; }
        public string description { get; set; }
        public bool _explicit { get; set; }
        public External_Urls external_urls { get; set; }
        public string href { get; set; }
        public string id { get; set; }
        public Image[] images { get; set; }
        public bool is_externally_hosted { get; set; }
        public string[] languages { get; set; }
        public string media_type { get; set; }
        public string name { get; set; }
        public string publisher { get; set; }
        public string type { get; set; }
        public string uri { get; set; }
    }

    public class External_Urls
    {
        public string spotify { get; set; }
    }

    public class Image
    {
        public int height { get; set; }
        public string url { get; set; }
        public int width { get; set; }
    }

}
