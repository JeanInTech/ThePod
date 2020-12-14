using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using ThePod.DataAccess;
using ThePod.Models;


namespace ThePod.Controllers
{
    [Authorize]
    public class UserController : Controller
    {
        private readonly thepodContext _context;
        private readonly PodDAL _dal;
        public UserController(PodDAL dal, thepodContext context)
        {
            _dal = dal;
            _context = context;
        }
        public IActionResult UserFavorites()
        {
            var user = FindUser();
            var spod = from all in _context.SavedPodcasts
                       where all.UserId.Equals(user)
                       select all;
            List<SavedPodcast> podcasts = spod.ToList();
            List<FavoriteViewModel> fvmList = new List<FavoriteViewModel>();

            foreach (var p in podcasts)
            {
                FavoriteViewModel fvm = new FavoriteViewModel();
                fvm.SavedPodcastId = p.Id;
                fvm.EpisodeId = p.EpisodeId;
                fvm.PodcastName = p.PodcastName;
                fvm.EpisodeName = p.EpisodeName;
                fvm.Publisher = p.Publisher;
                fvm.Description = p.Description;
                fvm.AudioPreviewUrl = p.AudioPreviewUrl;
                fvm.ExternalUrls = p.ExternalUrls;
                fvm.ImageUrl = p.ImageUrl;
                fvm.Duration = p.Duration;
                fvm.ReleaseDate = p.ReleaseDate;

                //user needs to rate otherwise this shows up as null
                var f = _context.UserFeedbacks.Where(x => x.EpisodeId == p.EpisodeId & x.UserId == p.UserId).FirstOrDefault();
                if (f != null)
                {
                    fvm.DatePosted = f.DatePosted;
                    fvm.UserFeedbackId = f.Id;
                    fvm.Rating = f.Rating;
                    fvm.Review = f.Review;
                    fvm.Tags = f.Tags;
                }
                fvmList.Add(fvm);
            }
            return View(fvmList);
        }
        public async Task<IActionResult> AddFavorite(string id)
        {
            string user = FindUser();
            var results = await _dal.SearchEpisodeIdAsync(id);
            var ep = results.episodes.ToList().First();
            SavedPodcast favorite = new SavedPodcast();

            favorite.UserId = user;
            favorite.EpisodeId = ep.id;
            favorite.PodcastName = ep.show.name;
            favorite.EpisodeName = ep.name;
            favorite.Publisher = ep.show.publisher;
            favorite.Description = ep.description;
            favorite.AudioPreviewUrl = ep.audio_preview_url;
            favorite.ExternalUrls = ep.external_urls.spotify;
            List<Image1> img = new List<Image1>(ep.images);
            var firstPic = img.First();
            favorite.ImageUrl = firstPic.url;
            favorite.Duration = ep.duration_ms;
            favorite.ReleaseDate = DateTime.Parse(ep.release_date);

            if (ModelState.IsValid)
            {
                if (_context.SavedPodcasts.Any(id => id.EpisodeId.Equals(ep.id)))
                {
                    return View("Error");
                }
                else
                {
                    await _context.SavedPodcasts.AddAsync(favorite);
                    await _context.SaveChangesAsync();
                }
            }
            return RedirectToAction("UserFavorites");
        }
        public async Task<IActionResult> SortFavorites(string sortOrder, string searchString)
        {
            string user = FindUser();
            ViewData["EpNameSortParm"] = String.IsNullOrEmpty(sortOrder) ? "epname_desc" : "";
            ViewData["PodNameSortParm"] = String.IsNullOrEmpty(sortOrder) ? "podname_desc" : "";
            ViewData["PublisherSortParm"] = String.IsNullOrEmpty(sortOrder) ? "publisher_sort" : "";
            ViewData["DateSortParm"] = sortOrder == "Date" ? "date_desc" : "Date";
            ViewData["CurrentFilter"] = searchString;

            var podcast = from p in _context.SavedPodcasts
                          select p;
            if (!String.IsNullOrEmpty(searchString))
            {
                podcast = podcast.Where(p => p.Description.Contains(searchString));
            }
            switch (sortOrder)
            {
                case "Date":
                    podcast = podcast.OrderBy(p => p.ReleaseDate);
                    break;
                case "date_desc":
                    podcast = podcast.OrderByDescending(p => p.ReleaseDate);
                    break;
                case "epname_desc":
                    podcast = podcast.OrderBy(p => p.EpisodeName);
                    break;
                case "podname_desc":
                    podcast = podcast.OrderBy(p => p.PodcastName);
                    break;
                case "publisher_sort":
                    podcast = podcast.OrderByDescending(p => p.Publisher);
                    break;
                default:
                    podcast = podcast.OrderBy(p => p.PodcastName);
                    break;
            }
            return View("Index", await podcast.Where(x => x.UserId == user).AsNoTracking().ToListAsync());
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var favorites = await _context.SavedPodcasts.FindAsync(id);
            _context.SavedPodcasts.Remove(favorites);
            await _context.SaveChangesAsync();
            if (favorites == null)
            {
                return NotFound();
            }
            return RedirectToAction("UserFavorites");
        }

        [HttpGet]
        public async Task<IActionResult> ReviewEpisode(string id)
        {
            string user = FindUser();
            List<UserFeedback> feedbackMatch = _context.UserFeedbacks.Where(x => x.UserId == user && x.EpisodeId == id).ToList();
            if (feedbackMatch.Count > 0)
            {
                UserFeedback duplicateReview = feedbackMatch.First();
                return View("DuplicateReview", duplicateReview);
            }
            else
            {
                var results = await _dal.SearchEpisodeIdAsync(id);
                var ep = results.episodes.First();
                return View(ep);
            }
        }

        [HttpPost]
        public async Task<IActionResult> ReviewEpisode(string EpisodeId, byte Rating, string[] Tags, string Review, string EpisodeName, string PodcastName, string Description, string AudioPreviewURL, string ImageUrl, DateTime ReleaseDate, string ExternalURLS)
        {
            string user = FindUser();
            UserFeedback feedback = new UserFeedback();
            feedback.UserId = user;
            feedback.EpisodeId = EpisodeId;
            feedback.Rating = (byte)Rating;
            string tag = string.Join(", ", Tags);
            feedback.Tags = tag;
            feedback.Review = Review;
            feedback.EpisodeName = EpisodeName;
            feedback.PodcastName = PodcastName;
            feedback.Description = Description;
            feedback.AudioPreviewUrl = AudioPreviewURL;
            feedback.ImageUrl = ImageUrl;
            feedback.ReleaseDate = ReleaseDate;
            feedback.ExternalUrls = ExternalURLS;
            feedback.DatePosted = DateTime.Now;

            await _context.UserFeedbacks.AddAsync(feedback);
            await _context.SaveChangesAsync(); //saving the review to the UserFeedback table

            for (int i = 0; i < Tags.Length; i++) //looping through each item in the Tags array so that we can add a new entry for each tag
            {
                UserProfile profile = new UserProfile();
                profile.UserFeedbackId = feedback.Id;
                profile.UserId = user;
                profile.EpisodeId = EpisodeId;
                profile.Tag = Tags[i];
                profile.Rating = Rating;
                await _context.UserProfiles.AddAsync(profile);
                await _context.SaveChangesAsync(); //saving the entries to the UserProfile table
            }
            return RedirectToAction("ViewFeedback", "User");

        }
       
        public IActionResult ViewFeedBack()
        {
            string user = FindUser();
            var feedback = _context.UserFeedbacks.Where(x => x.UserId == user);
            List<UserFeedback> usersFeedback = feedback.ToList();

            return View(usersFeedback);
        }

        public async Task<IActionResult> SortFeedback(string sortOrder, string searchString)
        {
            string user = FindUser();
            ViewData["EpNameSortParm"] = String.IsNullOrEmpty(sortOrder) ? "epname_desc" : "";
            ViewData["PodNameSortParm"] = String.IsNullOrEmpty(sortOrder) ? "podname_desc" : "";
            ViewData["DateSortParm"] = sortOrder == "Date" ? "date_desc" : "Date";
            ViewData["RatingSortParm"] = String.IsNullOrEmpty(sortOrder) ? "sort_rating" : "";
            ViewData["CurrentFilter"] = searchString;

            var feedback = from f in _context.UserFeedbacks
                           select f;
            if (!String.IsNullOrEmpty(searchString))
            {
                feedback = feedback.Where(f => f.Tags.Contains(searchString));
            }
            switch (sortOrder)
            {
                case "Date":
                    feedback = feedback.OrderBy(f => f.DatePosted);
                    break;
                case "date_desc":
                    feedback = feedback.OrderByDescending(f => f.DatePosted);
                    break;
                case "epname_desc":
                    feedback = feedback.OrderBy(f => f.EpisodeName);
                    break;
                case "podname_desc":
                    feedback = feedback.OrderBy(f => f.PodcastName);
                    break;
                case "sort_rating":
                    feedback = feedback.OrderByDescending(f => f.Rating);
                    break;
                default:
                    feedback = feedback.OrderBy(f => f.DatePosted);
                    break;
            }
            return View("ViewFeedback", await feedback.Where(x => x.UserId == user).AsNoTracking().ToListAsync());
        }

        public async Task<IActionResult> DeleteReview(string epId, int fbId)
        {
            var userProfile = _context.UserProfiles.Where(x => x.EpisodeId == epId).ToList();
            foreach(var up in userProfile)
            {
                _context.UserProfiles.Remove(up);
                await _context.SaveChangesAsync();
            }

            var userReview = await _context.UserFeedbacks.FindAsync(fbId);
            _context.UserFeedbacks.Remove(userReview);
            await _context.SaveChangesAsync();

            return RedirectToAction("ViewFeedBack");
        }

        [HttpGet]
        public async Task<IActionResult> EditReview(int Id)
        {
            var userReview = await _context.UserFeedbacks.FindAsync(Id);

            return View("EditReview", userReview);
        }

        [HttpPost]
        public async Task<IActionResult> EditReview(int Id, int UserId, string EpisodeId, int Rating, string[] Tags, string Review, string EpisodeName, string PodcastName, string Description, string AudioPreviewURL, string ImageUrl, DateTime ReleaseDate, string ExternalURLS)
        {
            UserFeedback feedback1 = await _context.UserFeedbacks.FindAsync(Id);
            string tag = string.Join(", ", Tags);
            feedback1.Tags = tag;
            feedback1.Rating = (byte)Rating;
            feedback1.Review = Review;
            feedback1.DatePosted = DateTime.Now;
            await _context.SaveChangesAsync();
            return RedirectToAction("ViewFeedBack");
        }
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
            var userSpecific = from x in _context.UserProfiles
                               where x.UserId.Equals(user)
                               select x;
            var qualifiedRatings = from y in userSpecific
                                   where y.Rating >= 3
                                   select y;
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

            //var groupedRatings = qualifiedRatings.AsEnumerable().GroupBy(x => x.Tag).ToList();


            return (usersTopTags);
        }
        public async Task<IActionResult> GetRecommendations()
        {
            List<string> usersTopTags = GetProfile(); //get a list of the users top tags (the tag they used most frequently on episodes rated 3+)
            if(usersTopTags.Count < 3)
            {
                return View("userrecommendations");
            }
            string firstPreferred = usersTopTags[0]; //1st place tag (this is just the single word(tag) so this can be used later in a viewbag
            string secondPreferred = usersTopTags[1]; //2nd place tag
            string thirdPreferred = usersTopTags[2]; //3rd place tag
            ViewBag.FirstTag = firstPreferred;
            ViewBag.SecondTag = " - "+secondPreferred;
            ViewBag.ThirdTag = " - " +thirdPreferred;

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

            return View("userrecommendations", recommendedEpisodes);
        }
    public IActionResult GetGlobalBestOf()
        {
            List<UserProfile> bestOf = GetBestEpisodesRawData();

            return View("topPicks", bestOf);
        }
        public List<UserProfile> GetBestEpisodesRawData()
        {
            string user = FindUser();
            List<UserProfile> globalProfiles = _context.UserProfiles.ToList();
            List<UserProfile> filteredProfiles = globalProfiles.Where(x => x.UserId != user).ToList(); //filtering out reviews that belong to the logged in user
            List<UserProfile> qualifiedProfiles = filteredProfiles.Where(x => x.Rating >= 3).ToList(); //filtering out review that are less than rating of 3
            List<UserProfile> descOrderedProfiles = qualifiedProfiles.OrderByDescending(x => x.Rating).ToList(); //orderes everything on the list based on highest-rated episdoes first

            return descOrderedProfiles;
            
        }
    }


}

