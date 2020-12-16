using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using ThePod.DataAccess;
using ThePod.Models;

namespace ThePod.Controllers
{
    public class HomeController : Controller
    {
        private readonly thepodContext _context;
        private readonly PodDAL _dal;
        public HomeController(PodDAL dal, thepodContext context)
        {
            _dal = dal;
            _context = context;
        }
      
        public IActionResult Recommendations()
        {
            return View(_context.UserFeedback.ToList());
        }

        public IActionResult AboutUs()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        public async Task<IActionResult> Index()
        {
            List<UserProfile> bestProfiles = GetBestEpisodesRawData(); //list of every tag in UserProfile table with a rating of 3+, that the logged in user has not reviewed, organized by highest rated first
           
            List<string> episodeIds = new List<string>();

            int epIdCount = 0;

            foreach (UserProfile e in bestProfiles)
            {
                if (e != null && !episodeIds.Contains(e.EpisodeId) && epIdCount < 21)
                {
                    episodeIds.Add(e.EpisodeId);
                    epIdCount++;
                }
              
            }
          
            var epId = String.Join(",", episodeIds);

            var recommendedEpisodes = await _dal.SearchEpisodeIdAsync(epId);

            return View("Index", recommendedEpisodes);
        }

        // ==============================================================
        // Methods
        // ==============================================================
      
        public List<UserProfile> GetBestEpisodesRawData()
        {

            List<UserProfile> globalProfiles = _context.UserProfile.ToList();
           // List<UserProfile> filteredProfiles = globalProfiles.Where(x => x.UserId != user).ToList(); //filtering out reviews that belong to the logged in user
            List<UserProfile> qualifiedProfiles = globalProfiles.Where(x => x.Rating >= 3).ToList(); //filtering out review that are less than rating of 3
            List<UserProfile> descOrderedProfiles = qualifiedProfiles.OrderByDescending(x => x.Rating).ToList(); //orders everything on the list based on highest-rated episdoes first

            return descOrderedProfiles;
        }
        public List<UserFeedback> GetPopularEpisodesRawData()
        {
            List<UserFeedback> globalProfiles = _context.UserFeedback.ToList();
            List<UserFeedback> qualifiedProfiles = globalProfiles.Where(x => x.Rating >= 3).ToList(); //filtering out review that are less than rating of 3
            List<UserFeedback> descOrderedProfiles = qualifiedProfiles.OrderByDescending(x => x.Rating).ToList(); //orders everything on the list based on highest-rated episdoes first
                                                                                                                 //SELECT * FROM UserProfile JOIN UserFeedback ON UserProfile.UserFeedbackId=UserFeedback.Id
            return descOrderedProfiles;
        }
        public async Task<IActionResult> GetPopularResults(string id)
        {
             TempData["UserQuery"]=id;
            var getPopular = await _dal.SearchEpisodeIdAsync(id);
            return View("../Pod/EpisodeDetails", getPopular);
        }

        public async Task<IActionResult> Popular()
        {
            List<UserFeedback> bestProfiles = GetPopularEpisodesRawData(); //list of every tag in UserProfile table with a rating of 3+, that the logged in user has not reviewed, organized by highest rated first
            return View("test", bestProfiles);
        }

    }
}
