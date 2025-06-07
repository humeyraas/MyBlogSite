using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using MyBlogSite.Data;
using MyBlogSite.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;

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
            ViewBag.Categories = categories;
            ViewBag.CategorySelectList = new SelectList(categories, "Id", "Name");
            return View();
        }

        public IActionResult Detail(int id)
        {
            var blog = _context.Blogs
                .Include(b => b.User)
                .Include(b => b.Comments)
                    .ThenInclude(c => c.User)
                .Include(b => b.Category)
               .Include(b => b.LikesList)
                .FirstOrDefault(b => b.Id == id);

            if (blog == null)
                return NotFound();

            blog.Views += 1;
            _context.SaveChanges();

         
            var categories = _context.Categories.ToList();
            ViewBag.Categories = categories;
            ViewBag.CategorySelectList = new SelectList(categories, "Id", "Name");

            var userId = HttpContext.Session.GetInt32("userId"); 
            ViewBag.CurrentUserId = userId;
            ViewBag.IsLikedByCurrentUser = blog.LikesList.Any(l => l.UserId == userId);
            ViewBag.LikeCount = _context.BlogLikes.Count(l => l.BlogId == id);

          //  ViewBag.LikeCount = blog.LikesList.Count();

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
            if (!userId.HasValue)
            {
                return RedirectToAction("Login", "Auth");
            }

            blog.UserId = userId.Value;

            _context.Blogs.Add(blog);
            await _context.SaveChangesAsync();

            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        public IActionResult AddComment(int blogId, string content)
        {
            var userId = HttpContext.Session.GetInt32("userId");
            if (userId == null)
            {
                return RedirectToAction("Login", "Auth");
            }

            var comment = new Comment
            {
                BlogId = blogId,
                Content = content,
                UserId = userId.Value,
                CreatedAt = DateTime.Now
            };

            _context.Comments.Add(comment);
            _context.SaveChanges();

            return RedirectToAction("Detail", new { id = blogId });
        }

       
        [HttpPost]
        public IActionResult DeleteComment(int id)
        {
            var comment = _context.Comments.FirstOrDefault(c => c.Id == id);
            var sessionUserId = HttpContext.Session.GetInt32("userId");

            if (comment == null)
                return NotFound();

            // Yalnızca yorumu yazan kişi silebilsin
            if (comment.UserId != sessionUserId)
                return Unauthorized();

            int blogId = comment.BlogId;

            _context.Comments.Remove(comment);
            _context.SaveChanges();

            return RedirectToAction("Detail", new { id = blogId });
        }

        public IActionResult Index()
        {
            var blogs = _context.Blogs
                .Include(b => b.User)
                .Include(b => b.Category)
                .Include(b => b.Comments)
                 .Include(b => b.LikesList)
                .ToList();

            return View(blogs);
        }
        [HttpPost]
        public IActionResult ToggleLike([FromBody] int blogId)
        {
            var userId = HttpContext.Session.GetInt32("userId");

            if (userId == null)
                return Unauthorized();

            var existingLike = _context.BlogLikes
                .FirstOrDefault(l => l.BlogId == blogId && l.UserId == userId);

            if (existingLike != null)
            {
                _context.BlogLikes.Remove(existingLike);
            }
            else
            {
                _context.BlogLikes.Add(new BlogLike
                {
                    BlogId = blogId,
                    UserId = userId.Value
                });
            }

            _context.SaveChanges();

            var newLikeCount = _context.BlogLikes.Count(l => l.BlogId == blogId);

            return Json(new
            {
                liked = existingLike == null,
                count = newLikeCount
            });
        }

    }
}
