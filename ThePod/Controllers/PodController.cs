﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ThePod.DataAccess;
using ThePod.Models;

namespace ThePod.Controllers
{
    public class PodController : Controller
    {
        private readonly PodDAL _dal;
        private readonly IConfiguration _config;
        private readonly thepodContext _context;
        public PodController(PodDAL dal, IConfiguration config, thepodContext context)
        {
            _dal = dal;
            _config = config;
            _context = context;
        }
        // ==============================================================
        // Search Results
        // ==============================================================
        public async Task<IActionResult> SearchResults(string query, string searchType) //this takes in a search term and gets the episode id to feed that into the "episode" endpoint
        {
            TempData["SearchType"] = searchType;
            if (searchType == "podcast")
            {
                TempData["UserQuery"] = query;
                var showResults = await _dal.SearchShowNameAsync(query); //this is where you can access the "next" property to see the next 20 Podcast-show results from your search
                List<Item> i = showResults.shows.items.ToList();

                TempData["TotalResults"] = showResults.shows.total;
                TempData["NextPage"] = showResults.shows.next;
                TempData["PreviousPage"] = showResults.shows.previous;
                TempData["Offset"] = showResults.shows.offset;

                var showId = ConvertToIdString(i).ToString();

                var eachShow = await _dal.SearchShowIdAsync(showId);

                if (eachShow.shows == null)
                {
                    var episodeResults = await _dal.SearchEpisodeNameAsync(query);
                    List<EpisodeItem> s = episodeResults.episodes.items.ToList();

                    TempData["TotalResults"] = episodeResults.episodes.total;
                    TempData["NextPage"] = episodeResults.episodes.next;
                    TempData["PreviousPage"] = episodeResults.episodes.previous;
                    TempData["Offset"] = episodeResults.episodes.offset;

                    var epId = ConvertToIdString(s).ToString();
                    var eachEpisode = await _dal.SearchEpisodeIdAsync(epId);

                    return View("EpisodeDetails", eachEpisode);
                }
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

                var epId = ConvertToIdString(s).ToString();
                var eachEpisode = await _dal.SearchEpisodeIdAsync(epId);

                return View("EpisodeDetails", eachEpisode);
            }
        }
        // ==============================================================
        // Navigate (Next/Previous) Search Results For Podcast Episodes
        // ==============================================================
        [HttpPost]
        public async Task<IActionResult> GetNextEpisode(string query, int offset, string type)
        {
            int increment = offset + 20;
            var episodeResults = await _dal.MoreSearchEpisodeAsync(query, increment);
            List<EpisodeItem> s = episodeResults.episodes.items.ToList();

            TempData["TotalResults"] = episodeResults.episodes.total;
            TempData["NextPage"] = episodeResults.episodes.next;
            TempData["PreviousPage"] = episodeResults.episodes.previous;
            TempData["Offset"] = increment;

            var epId = ConvertToIdString(s);
            var nextEpisodes = await _dal.SearchEpisodeIdAsync(epId);

            if (type == "episode")
            {
                return View("EpisodeDetails", nextEpisodes);
            }
            else
            {
                return View("EpisodeDetails", nextEpisodes);
            }
        }
        [HttpPost]
        public async Task<IActionResult> GetPreviousEpisode(string query, int offset, string type)
        {
            int decrement = offset - 20;
            var episodeResults = await _dal.MoreSearchEpisodeAsync(query, decrement);
            List<EpisodeItem> s = episodeResults.episodes.items.ToList();

            TempData["TotalResults"] = episodeResults.episodes.total;
            TempData["NextPage"] = episodeResults.episodes.next;
            TempData["PreviousPage"] = episodeResults.episodes.previous;
            TempData["Offset"] = decrement;

            var epId = ConvertToIdString(s);
            var previousEpisodes = await _dal.SearchEpisodeIdAsync(epId);

            if (type == "episode")
            {
                return View("EpisodeDetails", previousEpisodes);
            }
            else
            {
                return View("EpisodeDetails", previousEpisodes);
            }
        }
        // ==============================================================
        // Navigate (Next/Previous) Search Results For Podcast Shows
        // ==============================================================
        [HttpPost]
        public async Task<IActionResult> GetNextPodcast(string query, int offset)
        {
            int increment = offset + 20;
            var showResults = await _dal.MoreSearchShowAsync(query, increment);
            List<Item> i = showResults.shows.items.ToList();

            TempData["TotalResults"] = showResults.shows.total;
            TempData["NextPage"] = showResults.shows.next;
            TempData["PreviousPage"] = showResults.shows.previous;
            TempData["Offset"] = increment;

            var epId = ConvertToIdString(i);
            var nextShows = await _dal.SearchShowIdAsync(epId);

            return View("PodcastDetails", nextShows);
        }
        [HttpPost]
        public async Task<IActionResult> GetPreviousPodcast(string query, int offset)
        {
            int decrement = offset - 20;
            var showResults = await _dal.MoreSearchShowAsync(query, decrement);
            List<Item> i = showResults.shows.items.ToList();

            TempData["TotalResults"] = showResults.shows.total;
            TempData["NextPage"] = showResults.shows.next;
            TempData["PreviousPage"] = showResults.shows.previous;
            TempData["Offset"] = decrement;

            var epId = ConvertToIdString(i);
            var previousShows = await _dal.SearchShowIdAsync(epId);

            return View("PodcastDetails", previousShows);
        }
        // ==============================================================
        // Convert Simplified Objects to ID String to obtain Full Objects
        // ==============================================================
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
        // ==============================================================
        // Get Episode by Podcast Id and Page Navigation
        // ==============================================================
        public async Task<IActionResult> GetEpisodeByPodcast(string query)
        {
            var showResults = await _dal.SearchShowNameAsync(query);
            List<Item> i = showResults.shows.items.ToList();

            TempData["showName"] = query;
            TempData["ShowId"] = i.First().id;
            TempData["TotalResults"] = showResults.shows.total;
            TempData["NextPage"] = showResults.shows.next;
            TempData["PreviousPage"] = showResults.shows.previous;
            TempData["Offset"] = showResults.shows.offset;

            foreach (Item x in i)
            {
                if (x.name == query)
                {
                    string shoId = x.id;

                    var episodesByPodcast = await _dal.SearchEpbyPodIdAsync(shoId);
                    TempData["shoId"] = shoId;
                    return View("EpisodesByPodcast", episodesByPodcast);
                }
            }

            return View(); 
        }
        public async Task<IActionResult> GetPodcastFromCarosel(string id, string query)
        {
            var showResults = await _dal.SearchShowNameAsync(query);
            List<Item> i = showResults.shows.items.ToList();

            var episodesByPodcast = await _dal.SearchEpbyPodIdAsync(id);
            TempData["showName"] = query;
            TempData["ShowId"] = i.First().id; ;
            TempData["TotalResults"] = episodesByPodcast.total;
            TempData["NextPage"] = episodesByPodcast.next;
            TempData["PreviousPage"] = episodesByPodcast.previous;
            TempData["Offset"] = episodesByPodcast.offset;
            TempData["shoId"] = id;

            return View("EpisodesByPodcast", episodesByPodcast);
        }
        [HttpPost]
        public async Task<IActionResult> GetNextEpByPod(string shoId, int offset)
        {
            int increment = offset + 20;
            var showResults = await _dal.MoreEpbyPodIdAsync(shoId, increment);

            TempData["ShowId"] = shoId;
            TempData["TotalResults"] = showResults.total;
            TempData["NextPage"] = showResults.next;
            TempData["PreviousPage"] = showResults.previous;
            TempData["Offset"] = increment;

            return View("EpisodesByPodcast", showResults);
        }
        [HttpPost]
        public async Task<IActionResult> GetPreviousEpByPod(string shoId, int offset)
        {
            int decrement = offset - 20;
            var showResults = await _dal.MoreEpbyPodIdAsync(shoId, decrement);

            TempData["ShowId"] = shoId;
            TempData["TotalResults"] = showResults.total;
            TempData["NextPage"] = showResults.next;
            TempData["PreviousPage"] = showResults.previous;
            TempData["Offset"] = decrement;

            return View("EpisodesByPodcast", showResults);
        }
    }
}