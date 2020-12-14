using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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
            var podcasts = from r in _context.SavedPodcasts
                           where r.UserId.Equals(user)
                           select r;

            return View(podcasts.ToList());
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

            if(ModelState.IsValid)
            {
                if(_context.SavedPodcasts.Any(id=>id.EpisodeId.Equals(ep.id)))
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
                    podcast = podcast.OrderBy(p =>p.PodcastName);
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
            List<UserFeedback> feedbackList = _context.UserFeedbacks.ToList();
            List<UserFeedback> feedbackMatch = feedbackList.Where(x => x.UserId == user && x.EpisodeId == id).ToList();
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
        public async Task<IActionResult> ReviewEpisode(string EpisodeId, int Rating, string[] Tags, string Review, string EpisodeName, string PodcastName, string Description, string AudioPreviewURL, string ImageUrl, DateTime ReleaseDate, string ExternalURLS)
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
            await _context.SaveChangesAsync();

            return RedirectToAction("Index", "Home");
        }

        public IActionResult ViewFeedBack()
        {
            string user = FindUser();
            List<UserFeedback> feedback = _context.UserFeedbacks.ToList();
            List<UserFeedback> usersFeedback = feedback.Where(x => x.UserId == user).ToList(); //used LINQ to show only user's feedback

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
            if(!String.IsNullOrEmpty(searchString))
            {
                feedback = feedback.Where(f => f.Tags.Contains(searchString));
            }
            switch(sortOrder)
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

        public async Task<IActionResult> DeleteReview(int Id)
        {
            var userReview = await _context.UserFeedbacks.FindAsync(Id);
            _context.UserFeedbacks.Remove(userReview);
            await _context.SaveChangesAsync();

            return RedirectToAction("ViewFeedBack");
        }

        [HttpGet]
        public async Task<IActionResult> EditReview(int Id)
        {
            var userReview = await _context.UserFeedbacks.FindAsync(Id);
            return View(userReview);
        }

        [HttpPost]
        public async Task<IActionResult> EditReview(int Id, int UserId, string EpisodeId, int Rating, string[] Tags, string Review, string EpisodeName, string PodcastName, string Description, string AudioPreviewURL, string ImageUrl, DateTime ReleaseDate, string ExternalURLS)
        {
            UserFeedback feedback1 = await _context.UserFeedbacks.FindAsync(Id);
            //feedback.Id = Id;
            //feedback.UserId = 
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
    }
}
