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
        public async Task<IActionResult> SearchResults(string query, string type) //this is searching using "search by shows" endpoint
        {
            TempData["query"] = query;
            TempData["type"] = type;
            var results = await _dal.SearchShowsAsync(query);
            List<Item> s = results.shows.items.ToList();

            return View(s);
        }
        public async Task<IActionResult> SearchEpisodeNameResults(string query) //this is searching using "search by episodes"
        {
            var results = await _dal.SearchEpisodeNameAsync(query);
            List<EpisodeItem> s = results.episodes.items.ToList();

            return View("episodesearchresults", s);
        }
        public async Task<IActionResult> SearchByEpisodeId(string query) //this takes in a search term and gets the episode id to feed that into the "episode" endpoint
        {
            var results = await _dal.SearchEpisodeNameAsync(query);
            List<EpisodeItem> s = results.episodes.items.ToList();

            List<string> episodeIds = new List<string>();

            foreach (EpisodeItem e in s)
            {
                episodeIds.Add(e.id);

            }
            var str = String.Join("," , episodeIds);
            var eachEpisode = await _dal.SearchEpisodeIdAsync(str);

            //var eachEpisode = await _dal.SearchEpisodeIdAsync(query);

            return View("episodeidresults", eachEpisode);
        }
    }
}