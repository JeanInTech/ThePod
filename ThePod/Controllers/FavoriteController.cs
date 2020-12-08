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
    public class FavoriteController : Controller
    {
        private readonly thepodContext _context;
        private readonly PodDAL _dal;
        public FavoriteController(thepodContext context, PodDAL dal)
        {
            _context = context;
            _dal = dal;
        }
        public IActionResult Index()
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


    }
}
