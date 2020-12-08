using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ThePod.DataAccess;
using ThePod.Models;

namespace ThePod.Controllers
{
    public class FavoriteController : Controller
    {
        private readonly thepodContext _db;
        private readonly thepodContext _context;
        private readonly PodDAL _dal;
        private thepodContext db = new thepodContext();
        public FavoriteController(thepodContext context, PodDAL dal, thepodContext db)
        {
            _db = db;
            _context = context;
            _dal = dal;
        }

        public async Task<IActionResult> IndexAsync()
        {

            var thepodContext = _context.Favorites.Include(f => f.User);
            return View(await thepodContext.ToListAsync());
        }

        public async Task<IActionResult> Details(int? id)
        {
            if(id == null)
            {
                return NotFound();
            }

            var favorites = await _context.Favorites.
                Include(f => f.User).FirstOrDefaultAsync(m => m.Id == id);
            if(favorites==null)
            {
                return NotFound();
            }
            return View(favorites);
        }

        public IActionResult Favorite()
        {
            return View(_db.Favorites.ToList());
        }

        public IActionResult AddFavorite(int id/*, int? UserId, int? episodeId, string publisher, string podcastName, string episodeName, string audioPreviewUrl, string description, string durationMs, string externalUrls, string images, string releaseDatePrecision*/)
        {
            Favorite add = new Favorite(id/*, UserId, episodeId, publisher, podcastName, episodeName, audioPreviewUrl, description, durationMs, externalUrls, images, releaseDatePrecision*/);
            _db.Favorites.Add(add);
            _db.SaveChanges();
            return RedirectToAction("Index", "Favorite");
        }
    }
} 
