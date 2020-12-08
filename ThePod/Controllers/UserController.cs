using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ThePod.Models;

namespace ThePod.Controllers
{
    [Authorize]
    public class UserController : Controller
    {

        private readonly thepodContext _context;
        public UserController(thepodContext context)
        {
            _context = context;
        }
        public async Task<IActionResult> Index()
        {
            var thepodContext = _context.Favorites.Include(f => f.User);

            return View(await thepodContext.ToListAsync());
        }

        //public async Task<IActionResult> AddToFavorites(UserFavoritesViewModel userFavorites)
        //{
        //    Favorite f = new Favorite();
        //    List<Favorite> favorites = new List<Favorite>();

        //    if(ModelState.IsValid)
        //    {
        //        _context.Add(favorites);
        //        await _context.SaveChangesAsync();
        //        return RedirectToAction(nameof(Index));
        //    }

        //}

        //public IActionResult GetFavorites(string )
       
        
    }
}
