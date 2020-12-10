﻿using Microsoft.AspNetCore.Authorization;
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

        public async Task<IActionResult> Index()
        {
            return View(_context.SavedPodcasts.ToList());
        }

        //public async Task<IActionResult> IndexAsync()
        //{
        //    var thepodContext = _context.Favorites.Include(f => f.User);
        //    return View(await thepodContext.ToListAsync());
        //}

        //public async Task<IActionResult> Details(int? id)
        //{
        //    if(id == null)
        //    {
        //        return NotFound();
        //    }

        //    var favorites = await _context.Favorites.
        //        Include(f => f.User).FirstOrDefaultAsync(m => m.Id == id);
        //    if(favorites==null)
        //    {
        //        return NotFound();
        //    }
        //    return View(favorites);
        //}

        //public IActionResult Favorite()
        //{
        //    return View();
        //    return View(_context.SavedPodcasts.ToList());
        //}


        //[HttpPost]
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

            return RedirectToAction("Index", "User");
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteFromFavorites(int id)
        {
            var favoriteItem = await _context.SavedPodcasts.FindAsync(id);
            _context.SavedPodcasts.Remove(favoriteItem);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var favorites = await _context.SavedPodcasts
                .Include(f => f.User)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (favorites == null)
            {
                return NotFound();
            }

            return View(favorites);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var favorites = await _context.SavedPodcasts.FindAsync(id);
            _context.SavedPodcasts.Remove(favorites);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        public string FindUser()
        {
            var claimsIdentity = (ClaimsIdentity)this.User.Identity;
            var claim = claimsIdentity.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);
            var userId = claim.Value;
            return userId;
        }
        [HttpGet]
        public async Task<IActionResult> ReviewEpisode(string id)
        {
            var results = await _dal.SearchEpisodeIdAsync(id);
            var ep = results.episodes.First();
            return View(ep);
        }

        [HttpPost]
        public async Task<IActionResult> ReviewEpisode(string EpisodeId, int Rating, string Tags, string Review, string EpisodeName, string PodcastName, string Description, string AudioPreviewURL, string ImageUrl, DateTime ReleaseDate, string ExternalURLS)
        {
            string user = FindUser();
            UserFeedback feedback = new UserFeedback();
            feedback.UserId = user;
            feedback.EpisodeId = EpisodeId;
            feedback.Rating = (byte)Rating;
            feedback.Tags = Tags;
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

        public async Task<IActionResult> DeleteReview(int Id)
        {
            var userReview = await _context.UserFeedbacks.FindAsync(Id);
            _context.UserFeedbacks.Remove(userReview);
            await _context.SaveChangesAsync();

            return RedirectToAction("ViewFeedBack");
        }

        public async Task<IActionResult> EditReview(int Id)
        {
            var userReview = await _context.UserFeedbacks.FindAsync(Id);
            return View(userReview);
        }

        [HttpPost]
        public async Task<IActionResult> EditReview(int Id, int UserId, string EpisodeId, int Rating, string Tags, string Review, string EpisodeName, string PodcastName, string Description, string AudioPreviewURL, string ImageUrl, DateTime ReleaseDate, string ExternalURLS)
        {
            UserFeedback feedback1 = await _context.UserFeedbacks.FindAsync(Id);
            //feedback.Id = Id;
            //feedback.UserId = 
            feedback1.Tags = Tags;
            feedback1.Rating = (byte?)Rating;
            feedback1.Review = Review;
            feedback1.DatePosted = DateTime.Now;
            await _context.SaveChangesAsync();
            return RedirectToAction("ViewFeedBack");
        }

    }
}
