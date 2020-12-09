using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ThePod.DataAccess;
using ThePod.Models;

namespace ThePod.Controllers
{
    public class PodController : Controller
    {
        private readonly PodDAL _dal;
        private readonly IConfiguration _config;
        public PodController(PodDAL dal, IConfiguration config)
        {
            _dal = dal;
            _config = config;
        }
        public async Task<IActionResult> SearchResults(string query, string searchType) //this takes in a search term and gets the episode id to feed that into the "episode" endpoint
        { 
            TempData["SearchType"] = searchType;
            if (searchType == "podcast")
            {
                ViewBag.Podcast = query.ToLower();
                var showResults = await _dal.SearchShowNameAsync(query); //this is where you can access the "next" property to see the next 20 Podcast-show results from your search
                List<Item> i = showResults.shows.items.ToList();

                TempData["TotalResults"] = showResults.shows.total;
                TempData["NextPage"] = showResults.shows.next;
                TempData["PreviousPage"] = showResults.shows.previous;
                TempData["Offset"] = showResults.shows.offset;
                
                List<string> showIds = new List<string>();
                var showId = ConvertToIdString(i).ToString();

                var eachShow = await _dal.SearchShowIdAsync(showId);

                return View("PodcastDetails", eachShow);
            }
            else
            {

                TempData["UserQuery"] = query;
                var episodeResults = await _dal.SearchEpisodeNameAsync(query); //this is where you can access the "next" property to see the next 20 Episode results from your search
                List<EpisodeItem> s = episodeResults.episodes.items.ToList();

                TempData["TotalResults"] = episodeResults.episodes.total;
                TempData["NextPage"] = episodeResults.episodes.next;
                TempData["PreviousPage"] = episodeResults.episodes.previous;
                TempData["Offset"] = episodeResults.episodes.offset;
                ViewBag.OS = episodeResults.episodes.offset;

                List<string> episodeIds = new List<string>();
                var epId = ConvertToIdString(s).ToString();
                var eachEpisode = await _dal.SearchEpisodeIdAsync(epId);

                if(searchType == "episode")
                {
                    return View("EpisodeDetails", eachEpisode);
                }
                else
                {
                    return View("AllContent", eachEpisode);
                }
            }
        }
        [HttpPost]
        public async Task<IActionResult> GetNextEpisode(string query, int offset)
        {
            TempData["UserQuery"] = query;
            int increment = offset + 20;
            var episodeResults = await _dal.MoreSearchEpisodeAsync(query, increment);
            List<EpisodeItem> s = episodeResults.episodes.items.ToList();

            TempData["TotalResults"] = episodeResults.episodes.total;
            TempData["NextPage"] = episodeResults.episodes.next;
            TempData["PreviousPage"] = episodeResults.episodes.previous;
            TempData["Offset"] = increment;

            var epId = ConvertToIdString(s);
            var nextEpisodes = await _dal.SearchEpisodeIdAsync(epId);

            return View(nextEpisodes);
        }
        //[HttpPost]
        //public async Task<IActionResult> GetPreviousEpisode(string query, int offset)
        //{2
        //    //

        //}
        public static string ConvertToIdString(List<EpisodeItem> s)
        {
            List<string> episodeIds = new List<string>();
            foreach (EpisodeItem e in s)
            {
                if (e != null)
                {
                    episodeIds.Add(e.id);
                }
            }
            var epId = String.Join(",", episodeIds);
            
            return epId;
        }
        public static string ConvertToIdString(List<Item> i)
        {
            List<string> showIds = new List<string>();

            foreach (Item p in i)
            {
                if (p != null)
                {
                    showIds.Add(p.id);
                }
            }
            var shId = String.Join(",", showIds);

            return shId;
        }
    }
}