using Microsoft.AspNetCore.Mvc;
using MyBlogSite.Models;
using Microsoft.AspNetCore.Http;
using System.Linq;
using System.IO;
using MyBlogSite.Data;

namespace MyBlogSite.Controllers
{
    public class UserController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _environment;

        public UserController(ApplicationDbContext context, IWebHostEnvironment environment)
        {
            _context = context;
            _environment = environment;
        }

        [HttpGet]
        public IActionResult Settings()

        {
            ViewBag.Categories = _context.Categories.ToList();
            var username = HttpContext.Session.GetString("username");
            if (username == null)
                return RedirectToAction("Login", "Auth");

            var user = _context.Users.FirstOrDefault(u => u.Username == username);
            if (user == null)
                return NotFound();

            return View(user);
        }

        
        [HttpPost]
        public IActionResult Settings(User updatedUser, IFormFile? ProfileImage)
        {
            var username = HttpContext.Session.GetString("username");
            if (username == null)
                return RedirectToAction("Login", "Auth");

            var user = _context.Users.FirstOrDefault(u => u.Id == updatedUser.Id);
            if (user == null)
                return NotFound();

            user.Username = updatedUser.Username;

            
            if (!string.IsNullOrEmpty(updatedUser.Password))
            {
                user.Password = updatedUser.Password;
            }

            
            user.About = updatedUser.About;

            
            if (ProfileImage != null && ProfileImage.Length > 0)
            {
                var uploadsFolder = Path.Combine(_environment.WebRootPath, "img/user");
                var uniqueFileName = Guid.NewGuid().ToString() + "_" + ProfileImage.FileName;
                var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    ProfileImage.CopyTo(stream);
                }

                user.ProfileImagePath = "/img/user/" + uniqueFileName;
            }

            _context.SaveChanges();
            HttpContext.Session.SetString("profileImagePath", user.ProfileImagePath ?? "/img/user/user-1.png");

            return RedirectToAction("Settings");
        }

        
        [HttpPost]
        public IActionResult DeleteAccount()
        {
            var username = HttpContext.Session.GetString("username");
            if (username == null)
                return RedirectToAction("Login", "Auth");

            var user = _context.Users.FirstOrDefault(u => u.Username == username);
            if (user == null)
                return NotFound();

            _context.Users.Remove(user);
            _context.SaveChanges();
            HttpContext.Session.Clear();

            return RedirectToAction("Index", "Home");
        }
    }
}
