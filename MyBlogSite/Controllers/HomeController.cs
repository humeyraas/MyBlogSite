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

        public IActionResult Index(int? categoryId, string tag)
        {
            ViewBag.Categories = _context.Categories.ToList();

            var blogsQuery = _context.Blogs
                .Include(b => b.User)
                .Include(b => b.Category)
                .OrderByDescending(b => b.CreatedAt)
                .AsQueryable();

            if (categoryId.HasValue)
            {
                blogsQuery = blogsQuery.Where(b => b.CategoryId == categoryId.Value);
            }

            var blogs = blogsQuery.ToList(); // Veritabanýndan veriler alýnýyor

            if (!string.IsNullOrEmpty(tag))
            {
                // Split iþlemi artýk bellekte yapýlabilir
                blogs = blogs.Where(b => b.Tags != null &&
                            b.Tags.Split(',', StringSplitOptions.RemoveEmptyEntries)
                                  .Any(t => t.Trim().Equals(tag, StringComparison.OrdinalIgnoreCase)))
                             .ToList();
            }

            ViewBag.PopularBlogs = _context.Blogs
                .OrderByDescending(b => b.Views)
                .Take(5)
                .ToList();

            return View(blogs);
        }



        public IActionResult Search(string query)
        {
            if (string.IsNullOrWhiteSpace(query))
            {
                return RedirectToAction("Index");
            }

            // Bloglarý baþlýk, içerik veya tag (string) içinde arama
            var matchedBlogs = _context.Blogs
                .Include(b => b.User)
                .Include(b => b.Category)
                .Where(b =>
                    b.Title.Contains(query) ||
                    b.Content.Contains(query) ||
                    (b.Tags != null && b.Tags.Contains(query)))
                .OrderByDescending(b => b.CreatedAt)
                .ToList();

            // Kullanýcý adýnda arama
            var matchedUsers = _context.Users
                .Where(u => u.Username.Contains(query))
                .ToList();

            var model = new SearchResultsViewModel
            {
                Blogs = matchedBlogs,
                Users = matchedUsers
            };

            ViewBag.Categories = _context.Categories.ToList();
            ViewBag.PopularBlogs = _context.Blogs
                .OrderByDescending(b => b.Views)
                .Take(5)
                .ToList();

            return View("SearchResults", model);
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
