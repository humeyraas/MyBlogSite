using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyBlogSite.Data;
using MyBlogSite.Models;
using System.Diagnostics;

namespace MyBlogSite.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;

        public HomeController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index(int? categoryId)
        {
            ViewBag.Categories = _context.Categories.ToList(); // Menü bar için

            var blogs = _context.Blogs
                .Include(b => b.User)
                .Include(b => b.Category)
                .OrderByDescending(b => b.CreatedAt)
                .AsQueryable();

            if (categoryId.HasValue)
            {
                blogs = blogs.Where(b => b.CategoryId == categoryId.Value);
            }

            // En çok görüntülenen 5 blog (Sidebar veya ana sayfa kýsmý için)
            ViewBag.PopularBlogs = _context.Blogs
                .OrderByDescending(b => b.Views)
                .Take(5)
                .ToList();

            return View(blogs.ToList());
        }

        public IActionResult Search(string query)
        {
            if (string.IsNullOrWhiteSpace(query))
            {
                return RedirectToAction("Index");
            }

            var results = _context.Blogs
                .Include(b => b.User)
                .Include(b => b.Category)
                .Where(b => b.Title.Contains(query) || b.Content.Contains(query))
                .OrderByDescending(b => b.CreatedAt)
                .ToList();

            ViewBag.Categories = _context.Categories.ToList();
            ViewBag.PopularBlogs = _context.Blogs
                .OrderByDescending(b => b.Views)
                .Take(5)
                .ToList();

            return View("SearchResults", results); // SearchResults.cshtml adýnda bir view gerektirir
        }

        public IActionResult Privacy()
        {
            ViewBag.Categories = _context.Categories.ToList();
            return View();
        }

        public IActionResult About()
        {
            ViewBag.Categories = _context.Categories.ToList();
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            ViewBag.Categories = _context.Categories.ToList();
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
