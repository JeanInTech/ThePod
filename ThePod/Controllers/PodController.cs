using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ThePod.DataAccess;
using ThePod.Models;

namespace ThePod.Controllers
{
    public class PodController : Controller
    {
        private readonly PodDAL _dal;
        private readonly IConfiguration _config;
        public PodController(PodDAL dal, IConfiguration config)
        {
            _dal = dal;
            _config = config;
        }
        public async Task<IActionResult> SearchResults(string query)
        {
            var results = await _dal.SearchShowsAsync(query);
            List<Show> shows = results.shows.ToList();
            return View(shows);
        }
    }
}
