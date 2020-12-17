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
        // ==============================================================
        // Favorites table 
        // ==============================================================
        public IActionResult UserFavorites()
        {
            var user = FindUser();
            var spod = from all in _context.SavedPodcast
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
                fvm.Duration = (int)p.Duration;
                fvm.ReleaseDate = (DateTime)p.ReleaseDate;

                //user needs to rate otherwise this shows up as null
                var f = _context.UserFeedback.Where(x => x.EpisodeId == p.EpisodeId & x.UserId == p.UserId).FirstOrDefault();
                if (f != null)
                {
                    fvm.DatePosted = (DateTime)f.DatePosted;
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

            ViewBag.EpName = ep.name;

            if (ModelState.IsValid)
            {
                if (_context.SavedPodcast.Any(id => id.EpisodeId.Equals(ep.id) & id.UserId.Equals(user)))
                {
                    return View("DuplicateFavorite");
                }
                else
                {
                    await _context.SavedPodcast.AddAsync(favorite);
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

            var podcast = from p in _context.SavedPodcast
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
            var favorites = await _context.SavedPodcast.FindAsync(id);
            _context.SavedPodcast.Remove(favorites);
            await _context.SaveChangesAsync();
            if (favorites == null)
            {
                return NotFound();
            }
            return RedirectToAction("UserFavorites");
        }
        // ==============================================================
        // Reviews/UserFeedback
        // ==============================================================
        [HttpGet]
        public async Task<IActionResult> ReviewEpisode(string id)
        {
            string user = FindUser();
            List<UserFeedback> feedbackMatch = _context.UserFeedback.Where(x => x.UserId == user && x.EpisodeId == id).ToList();
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

            await _context.UserFeedback.AddAsync(feedback);
            await _context.SaveChangesAsync(); //saving the review to the UserFeedback table

            for (int i = 0; i < Tags.Length; i++) //looping through each item in the Tags array so that we can add a new entry for each tag
            {
                UserProfile profile = new UserProfile();
                profile.UserFeedbackId = feedback.Id;
                profile.UserId = user;
                profile.EpisodeId = EpisodeId;
                profile.Tag = Tags[i];
                profile.Rating = Rating;
                await _context.UserProfile.AddAsync(profile);
                await _context.SaveChangesAsync(); //saving the entries to the UserProfile table
            }
            return RedirectToAction("ViewFeedback", "User");
        }
        public IActionResult ViewFeedBack()
        {
            string user = FindUser();
            var feedback = _context.UserFeedback.Where(x => x.UserId == user);
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

            var feedback = from f in _context.UserFeedback
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
        [HttpGet]
        public async Task<IActionResult> EditReview(int fbId)
        {
            var userReview = await _context.UserFeedback.FindAsync(fbId);


            return View("EditReview", userReview);
        }

        [HttpPost]
        public async Task<IActionResult> EditReview(int Id, string EpisodeId, int Rating, string[] Tags, string Review)
        {
            string user = FindUser();
            UserFeedback feedback1 = await _context.UserFeedback.FindAsync(Id);
            string tag = string.Join(", ", Tags);

            List<UserProfile> up = _context.UserProfile.Where(x => x.UserId == user && x.EpisodeId == EpisodeId).ToList();
            foreach (UserProfile u in up)
            {
                _context.UserProfile.Remove(u);
                await _context.SaveChangesAsync();
            }

            for (int i = 0; i < Tags.Length; i++)
            {
                UserProfile profile = new UserProfile();
                profile.UserFeedbackId = feedback1.Id;
                profile.UserId = user;
                profile.EpisodeId = EpisodeId;
                profile.Tag = Tags[i];
                profile.Rating = (byte)Rating;
                await _context.UserProfile.AddAsync(profile);
                await _context.SaveChangesAsync();
            }

            feedback1.Tags = tag;
            feedback1.Rating = (byte)Rating;
            feedback1.Review = Review;
            feedback1.DatePosted = DateTime.Now;
            await _context.SaveChangesAsync();
            return RedirectToAction("ViewFeedBack");
        }
        public async Task<IActionResult> DeleteReview(string epId, int fbId)
        {
            var userProfile = _context.UserProfile.Where(x => x.EpisodeId == epId).ToList();
            foreach (var up in userProfile)
            {
                _context.UserProfile.Remove(up);
                await _context.SaveChangesAsync();
            }

            var userReview = await _context.UserFeedback.FindAsync(fbId);
            _context.UserFeedback.Remove(userReview);
            await _context.SaveChangesAsync();

            return RedirectToAction("ViewFeedBack");
        }

        // ==============================================================
        // Recommendations
        // ==============================================================
        public IActionResult GetGlobalBestOf()
        {
            List<UserProfile> bestOf = GetBestEpisodesRawData();

            return View("topPicks", bestOf);
        }
        public async Task<IActionResult> GetRecommendations()
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

            List<UserProfile> topTagEpisodes = new List<UserProfile>();

            foreach (UserProfile u in bestProfiles)
            {
                if (firstPreferred == u.Tag)
                {
                    topTagEpisodes.Add(u);
                }
                if (secondPreferred == u.Tag)
                {
                    topTagEpisodes.Add(u);
                }
                if (thirdPreferred == u.Tag)
                {
                    topTagEpisodes.Add(u);
                }
            }
           
            return View("recommended", topTagEpisodes);

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
            var userSpecific = _context.UserProfile.Where(x => x.UserId == user);
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
            List<UserProfile> globalProfiles = _context.UserProfile.Include(m => m.UserFeedback.User).ToList();
            List<UserProfile> filteredProfiles = globalProfiles.Where(x => x.UserId != user).ToList(); //filtering out reviews that belong to the logged in user
            List<UserProfile> qualifiedProfiles = filteredProfiles.Where(x => x.Rating >= 3).ToList(); //filtering out review that are less than rating of 3
            List<UserProfile> descOrderedProfiles = qualifiedProfiles.OrderByDescending(x => x.Rating).ToList(); //orders everything on the list based on highest-rated episdoes first

            return descOrderedProfiles;
        }
    }
}