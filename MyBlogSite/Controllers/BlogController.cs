using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using MyBlogSite.Data;
using MyBlogSite.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;


namespace MyBlogSite.Controllers
{
    public class BlogController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _env;

        public BlogController(ApplicationDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }

        [HttpGet]
        public IActionResult Add()
        {
            var categories = _context.Categories.ToList();
            ViewBag.Categories = new SelectList(categories, "Id", "Name");
            return View();
        }

        public IActionResult Detail(int id)
        {
            var blog = _context.Blogs
                .Include(b => b.User) 
                .Include(b => b.Category) 
                .FirstOrDefault(b => b.Id == id);

            if (blog == null)
                return NotFound();

            return View(blog);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Add(Blog blog, IFormFile? file)
        {
            if (!ModelState.IsValid)
                return View(blog);

            if (file != null && file.Length > 0)
            {
                var uploads = Path.Combine(_env.WebRootPath, "uploads");
                Directory.CreateDirectory(uploads);

                var fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
                var filePath = Path.Combine(uploads, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                blog.ImageUrl = "/uploads/" + fileName;
            }

            blog.CreatedAt = DateTime.Now;

            var userId = HttpContext.Session.GetInt32("userId");
            if (userId == null)
            {
                return RedirectToAction("Login", "Auth");
            }
            blog.UserId = userId.Value;


            _context.Blogs.Add(blog);
            await _context.SaveChangesAsync();

            return RedirectToAction("Index", "Home");
        }
    }
}