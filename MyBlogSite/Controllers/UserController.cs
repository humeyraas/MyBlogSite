using Microsoft.AspNetCore.Mvc;
using MyBlogSite.Models;
using Microsoft.AspNetCore.Http;
using System.Linq;
using MyBlogSite.Data;

namespace MyBlogSite.Controllers
{
    public class UserController : Controller
    {
        private readonly ApplicationDbContext _context;

        public UserController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Settings()
        {
            string? username = HttpContext.Session.GetString("username");
            if (username == null)
            {
                return RedirectToAction("Login", "Auth");
            }

            var user = _context.Users.FirstOrDefault(u => u.Username == username);
            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }

        [HttpPost]
        public IActionResult Settings(User updatedUser)
        {
            var user = _context.Users.FirstOrDefault(u => u.Id == updatedUser.Id);
            if (user == null)
            {
                return NotFound();
            }

            user.Username = updatedUser.Username;

            _context.SaveChanges();
            return RedirectToAction("Settings");
        }
    }
}
