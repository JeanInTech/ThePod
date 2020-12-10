using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ThePod.Models
{
    /*This is a helper class and is called in the SearchShowIdAsync in the DAL 
     * Helper class exists for the sole purpose of obtaining an ID to feed into the
     * SearchShowIdAsync to retrieve Show data that will result in more accurate search results.
     * This class is also required to get us the "next" property for episodes so that 
     * We can obtain the next set of 20 results*/
    public class Rootobject
        {
            public Shows shows { get; set; }
        }
        public class Shows
        {
            public string href { get; set; }
            public Item[] items { get; set; }
            public int limit { get; set; }
            public string next { get; set; }
            public int offset { get; set; }
            public string previous { get; set; }
            public int total { get; set; }
        }

        public class Item
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
            public int total_episodes { get; set; }
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


