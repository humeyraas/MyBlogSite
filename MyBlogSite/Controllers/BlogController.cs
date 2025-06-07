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
        public async Task<IActionResult> Add(Blog blog, IFormFile? file, string? Tags)
        {
            if (!ModelState.IsValid)
            {
                var categories = _context.Categories.ToList();
                ViewBag.Categories = categories;
                ViewBag.CategorySelectList = new SelectList(categories, "Id", "Name");
                return View(blog);
            }

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
                return RedirectToAction("Login", "Auth");

            blog.UserId = userId.Value;

            // Tagleri ata
            blog.Tags = Tags;

            _context.Blogs.Add(blog);
            await _context.SaveChangesAsync();

            return RedirectToAction("Index", "Home");
        }



        [HttpPost]
        public async Task<IActionResult> UploadImage(IFormFile upload)
        {
            if (upload != null && upload.Length > 0)
            {
                var uploadsFolder = Path.Combine(_env.WebRootPath, "uploads");
                Directory.CreateDirectory(uploadsFolder);

                var fileName = Guid.NewGuid().ToString() + Path.GetExtension(upload.FileName);
                var filePath = Path.Combine(uploadsFolder, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await upload.CopyToAsync(stream);
                }

                var imageUrl = Url.Content("~/uploads/" + fileName);

                return Json(new { url = imageUrl });
            }

            return BadRequest();
        }

    }
}