using Microsoft.AspNetCore.Mvc;
using MyBlogSite.Data;
using MyBlogSite.Models;
using System.Diagnostics;
using Microsoft.EntityFrameworkCore;

namespace MyBlogSite.Controllers
{
    public class HomeController : Controller
    {
        
        
        private readonly ApplicationDbContext _context;

        public HomeController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var blogs = _context.Blogs
                .Include(b => b.User)
                .Include(b => b.Category)
                .OrderByDescending(b => b.CreatedAt)
                .ToList();

            return View(blogs);
        }

        public IActionResult Privacy()
        {
            return View();
        }
        public IActionResult About()
        {
            return View(); 
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
