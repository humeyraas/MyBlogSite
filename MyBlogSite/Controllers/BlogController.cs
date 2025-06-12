using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyBlogSite.Data;
using MyBlogSite.Models;

namespace MyBlogSite.Controllers
{
    public class BlogController : Controller
    {
        private readonly ApplicationDbContext _context;

        public BlogController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult Edit(int id)
        {
            var blog = _context.Blogs.FirstOrDefault(b => b.Id == id);
            if (blog == null) return NotFound();
            return View(blog);
        }

        [HttpPost]
        public IActionResult Edit(Blog blog)
        {
            _context.Blogs.Update(blog);
            _context.SaveChanges();
            return RedirectToAction("Index", "Profile");
        }

        [HttpGet]
        public IActionResult Delete(int id)
        {
            var blog = _context.Blogs.FirstOrDefault(b => b.Id == id);
            if (blog == null) return NotFound();

            _context.Blogs.Remove(blog);
            _context.SaveChanges();
            return RedirectToAction("Index", "Profile");
        }

        [HttpGet]
        public IActionResult Repost(int id)
        {
            var username = HttpContext.Session.GetString("username");
            var user = _context.Users.FirstOrDefault(u => u.Username == username);
            if (user == null) return RedirectToAction("Login", "Auth");

            var blog = _context.Blogs.FirstOrDefault(b => b.Id == id);
            if (blog == null) return NotFound();

            var alreadyReposted = _context.Reposts.Any(r => r.BlogId == id && r.UserId == user.Id);
            if (!alreadyReposted)
            {
                _context.Reposts.Add(new Repost { BlogId = blog.Id, UserId = user.Id });
                _context.SaveChanges();
            }

            return RedirectToAction("Index", "Profile");
        }
    }
}
