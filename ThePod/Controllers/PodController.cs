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
            var results = await _dal.SearchEpisodeNameAsync(query);
            List<EpisodeItem> s = results.episodes.items.ToList();
            
            List<string> episodeIds = new List<string>();
                foreach (EpisodeItem e in s)
            {

                if(e != null)
                { 
                    episodeIds.Add(e.id);
                }
            }
            var str = String.Join(",", episodeIds);
            var eachEpisode = await _dal.SearchEpisodeIdAsync(str);

            if (searchType == "podcast")
            {

             ViewBag.Podcast = query.ToLower();
            
                return View("PodcastDetails", eachEpisode);
            }
            else if (searchType == "episode")
            {
                ViewBag.Episode = query.ToLower();
                return View("EpisodeDetails", eachEpisode);
            }
            else
            {
                return View("AllContent", eachEpisode);
            }
        }
        [HttpPost]
        public async Task<IActionResult> GetNext(string query)
        {
            var nextResults = await _dal.GetNextEpisode(query);
                return View("NextResults", nextResults);
        }
    }
}