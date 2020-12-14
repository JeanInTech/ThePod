using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
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

        public IActionResult Index()
        {
            return View(GetTagCloud());
        }
        public IActionResult Recommendations()
        {
            return View(_context.UserFeedbacks.ToList());
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
        // ==============================================================
        // Tag Cloud
        // ==============================================================
        public string GetTagCloud()
        {
            var allTags = _context.UserProfiles.ToList();
            var tags = TagCloud.MakeTagCloud(allTags);

            return tags;
        }
    }
}
