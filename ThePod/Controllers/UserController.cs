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

        public async Task<IActionResult> Index()
        {
            return View();
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


        [HttpPost]
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
            await _context.SavedPodcasts.AddAsync(favorite);
            await _context.SaveChangesAsync();

            return RedirectToAction("Index", "Favorite");
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
