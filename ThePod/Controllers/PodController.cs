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
            if (searchType == "podcast")
            {
                var showResults = await _dal.SearchShowNameAsync(query); //this is where you can access the "next" property to see the next 20 Podcast-show results from your search
                List<Item> i = showResults.shows.items.ToList();
                List<string> showIds = new List<string>();
                foreach (Item p in i)
                {
                    if (p != null)
                    {
                        showIds.Add(p.id);
                    }
                }
                var shoId = String.Join(",", showIds);
                var eachShow = await _dal.SearchShowIdAsync(shoId);
                ViewBag.Podcast = query.ToLower();
                return View("PodcastDetails", eachShow);
            }
            else if (searchType == "episode")
            {
                var episodeResults = await _dal.SearchEpisodeNameAsync(query); //this is where you can access the "next" property to see the next 20 Episode results from your search
                List<EpisodeItem> s = episodeResults.episodes.items.ToList();
                List<string> episodeIds = new List<string>();
                foreach (EpisodeItem e in s)
                {
                    if (e != null)
                    {
                        episodeIds.Add(e.id);
                    }
                }
                var epId = String.Join(",", episodeIds);
                var eachEpisode = await _dal.SearchEpisodeIdAsync(epId);
                return View("EpisodeDetails", eachEpisode);
            }
            else
            {
                var episodeResults = await _dal.SearchEpisodeNameAsync(query); //this is where you can access the "next" property to see the next 20 Episode results from your search
                List<EpisodeItem> s = episodeResults.episodes.items.ToList();
                List<string> episodeIds = new List<string>();
                foreach (EpisodeItem e in s)
                {
                    if (e != null)
                    {
                        episodeIds.Add(e.id);
                    }
                }
                var epId = String.Join(",", episodeIds);
                var eachEpisode = await _dal.SearchEpisodeIdAsync(epId);
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