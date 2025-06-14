using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using MyBlogSite.Data;
using MyBlogSite.Models;
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

            return View(blog);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Add(Blog blog, IFormFile? file, string? Tags)
        {
            var categories = _context.Categories.ToList();

            if (!ModelState.IsValid)
            {
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
            if (!userId.HasValue)
                return RedirectToAction("Login", "Auth");

            blog.UserId = userId.Value;

            // Tags özelliğini ekle
            blog.Tags = Tags;

            _context.Blogs.Add(blog);
            await _context.SaveChangesAsync();

            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        public IActionResult AddComment(int blogId, string content)
        {
            var userId = HttpContext.Session.GetInt32("userId");
            if (!userId.HasValue)
                return RedirectToAction("Login", "Auth");

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

            if (comment.UserId != sessionUserId)
                return Unauthorized();

            int blogId = comment.BlogId;

            _context.Comments.Remove(comment);
            _context.SaveChanges();

            return RedirectToAction("Detail", new { id = blogId });
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var blog = await _context.Blogs.FindAsync(id);
            if (blog == null)
                return NotFound();

            // Silmeden önce oturum kontrolü (güvenlik için önerilir)
            var userId = HttpContext.Session.GetInt32("userId");
            if (userId != blog.UserId)
                return Unauthorized();

            _context.Blogs.Remove(blog);
            await _context.SaveChangesAsync();

            return RedirectToAction("Index", "Profile"); //  PROFİL SAYFASINA YÖNLENDİR
        }

        [HttpGet]
        public IActionResult Edit(int id)
        {
            var blog = _context.Blogs.FirstOrDefault(b => b.Id == id);
            if (blog == null)
                return NotFound();

            var userId = HttpContext.Session.GetInt32("userId");
            if (blog.UserId != userId)
                return Unauthorized();

            ViewBag.Categories = _context.Categories.ToList();
            ViewBag.CategorySelectList = new SelectList(_context.Categories, "Id", "Name");

            return View(blog);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Blog blog, IFormFile? file)
        {
            var existingBlog = await _context.Blogs.FindAsync(blog.Id);
            if (existingBlog == null)
                return NotFound();

            var userId = HttpContext.Session.GetInt32("userId");
            if (existingBlog.UserId != userId)
                return Unauthorized();

            existingBlog.Title = blog.Title;
            existingBlog.Content = blog.Content;
            existingBlog.CategoryId = blog.CategoryId;
            existingBlog.Tags = blog.Tags;

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

                existingBlog.ImageUrl = "/uploads/" + fileName;
            }

            await _context.SaveChangesAsync();
            return RedirectToAction("Detail", new { id = blog.Id }); // İsteğe göre: "Profile" da olabilir
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

            if (!userId.HasValue)
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
        [HttpGet]
        public IActionResult Repost(int id)
        {
            var username = HttpContext.Session.GetString("username");
            if (username == null)
                return RedirectToAction("Login", "Auth");

            var user = _context.Users.FirstOrDefault(u => u.Username == username);
            if (user == null)
                return NotFound();

            var existing = _context.Reposts.FirstOrDefault(r => r.UserId == user.Id && r.BlogId == id);
            if (existing != null)
                return RedirectToAction("Index", "Profile"); // Zaten repostlanmışsa tekrar eklemesin

            var repost = new Repost
            {
                UserId = user.Id,
                BlogId = id,
            };

            _context.Reposts.Add(repost);
            _context.SaveChanges();

            return RedirectToAction("Index", "Profile"); // Başarılıysa profile dön
        }
        [HttpGet]
        public IActionResult ToggleRepost(int id, string tab = "own")
        {
            var username = HttpContext.Session.GetString("username");
            if (username == null)
                return RedirectToAction("Login", "Auth");

            var user = _context.Users.FirstOrDefault(u => u.Username == username);
            if (user == null)
                return NotFound();

            var existingRepost = _context.Reposts.FirstOrDefault(r => r.UserId == user.Id && r.BlogId == id);

            if (existingRepost != null)
            {
                // Zaten repostlanmışsa kaldır
                _context.Reposts.Remove(existingRepost);
            }
            else
            {
                // Repostla
                var repost = new Repost { BlogId = id, UserId = user.Id };
                _context.Reposts.Add(repost);
            }

            _context.SaveChanges();
            return RedirectToAction("Index", "Profile", new { tab });
        }


    }
}
