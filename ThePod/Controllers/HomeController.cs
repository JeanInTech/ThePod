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

        //public IActionResult Index()
        //{
        //    return View();
        //}
        public IActionResult Recommendations()
        {
            return View(_context.UserFeedbacks.ToList());
        }

        public IActionResult AboutUs()
        {
            return View();
        }

        public IActionResult Privacy()
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
            List<string> usersTopTags = GetProfile(); //get a list of the users top tags (the tag they used most frequently on episodes rated 3+)
            if (usersTopTags.Count < 3)
            {
                return View("userrecommendations");
            }
            string firstPreferred = usersTopTags[0]; //1st place tag (this is just the single word(tag) so this can be used later in a viewbag
            string secondPreferred = usersTopTags[1]; //2nd place tag
            string thirdPreferred = usersTopTags[2]; //3rd place tag
            ViewBag.FirstTag = firstPreferred;
            ViewBag.SecondTag = " - " + secondPreferred;
            ViewBag.ThirdTag = " - " + thirdPreferred;

            List<UserProfile> bestProfiles = GetBestEpisodesRawData(); //list of every tag in UserProfile table with a rating of 3+, that the logged in user has not reviewed, organized by highest rated first

            List<string> firstTagEpisodeRec = new List<string>();
            List<string> secondTagEpisodeRec = new List<string>();
            List<string> thirdTagEpisodeRec = new List<string>();

            foreach (UserProfile u in bestProfiles)
            {
                if (firstPreferred == u.Tag)
                {
                    firstTagEpisodeRec.Add(u.EpisodeId);
                }
                if (secondPreferred == u.Tag)
                {
                    secondTagEpisodeRec.Add(u.EpisodeId);
                }
                if (thirdPreferred == u.Tag)
                {
                    thirdTagEpisodeRec.Add(u.EpisodeId);
                }
            }
            List<List<string>> topThreeEpisdodeLists = new List<List<string>>(); //these are just lists of strings (episode Ids)
            {
                topThreeEpisdodeLists.Add(firstTagEpisodeRec);
                topThreeEpisdodeLists.Add(secondTagEpisodeRec);
                topThreeEpisdodeLists.Add(thirdTagEpisodeRec);
            }
            List<string> episodeIds = new List<string>();
            foreach (var e in firstTagEpisodeRec)
            {
                if (e != null)
                {
                    episodeIds.Add(e);
                }
            }
            foreach (var e in secondTagEpisodeRec)
            {
                if (e != null)
                {
                    episodeIds.Add(e);
                }
            }
            foreach (var e in thirdTagEpisodeRec)
            {
                if (e != null)
                {
                    episodeIds.Add(e);
                }
            }
            var epId = String.Join(",", episodeIds);

            var recommendedEpisodes = await _dal.SearchEpisodeIdAsync(epId);

            return View("Index", recommendedEpisodes);
        }

        // ==============================================================
        // Methods
        // ==============================================================
        public string FindUser()
        {
            var claimsIdentity = (ClaimsIdentity)this.User.Identity;
            var claim = claimsIdentity.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);
            var userId = claim.Value;
            return userId;
        }
        public List<string> GetProfile()
        {
            var user = FindUser();
            var userSpecific = _context.UserProfiles.Where(x => x.UserId == user);
            var qualifiedRatings = userSpecific.Where(x => x.Rating >= 3);
            var countPerTag = from z in qualifiedRatings
                              group z by z.Tag into taggedList
                              select new
                              {
                                  TagGroup = taggedList.Key,
                                  CountTag = taggedList.Count(),
                              };
            var topTags = countPerTag.OrderByDescending(countPerTag => countPerTag.CountTag).ToList();

            List<string> usersTopTags = new List<string>();
            foreach (var t in topTags)
            {
                usersTopTags.Add(t.TagGroup);
            }

            return (usersTopTags);
        }
        public List<UserProfile> GetBestEpisodesRawData()
        {
            string user = FindUser();
            List<UserProfile> globalProfiles = _context.UserProfiles.ToList();
            List<UserProfile> filteredProfiles = globalProfiles.Where(x => x.UserId != user).ToList(); //filtering out reviews that belong to the logged in user
            List<UserProfile> qualifiedProfiles = filteredProfiles.Where(x => x.Rating >= 3).ToList(); //filtering out review that are less than rating of 3
            List<UserProfile> descOrderedProfiles = qualifiedProfiles.OrderByDescending(x => x.Rating).ToList(); //orders everything on the list based on highest-rated episdoes first

            return descOrderedProfiles;
        }


    }
}
