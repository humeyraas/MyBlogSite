using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyBlogSite.Data; // Projene göre namespace değişebilir
using MyBlogSite.Models; // Model yolun buysa

public class ProfileController : Controller
{
    private readonly ApplicationDbContext _context;

    public ProfileController(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        ViewBag.Categories = _context.Categories.ToList();
        var userName = HttpContext.Session.GetString("username");
        if (userName == null) return RedirectToAction("Login", "Auth");

        var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == userName);
        if (user == null) return NotFound();

        var ownPosts = await _context.Blogs
            .Where(b => b.UserId == user.Id)
            .ToListAsync();

        var likedPosts = await _context.Likes
            .Where(l => l.UserId == user.Id)
            .Include(l => l.Blog)
            .Select(l => l.Blog)
            .ToListAsync();

        var repostedPosts = await _context.Reposts
            .Where(r => r.UserId == user.Id)
            .Include(r => r.Blog)
            .Select(r => r.Blog)
            .ToListAsync();

        var model = new ProfileViewModel
        {
            OwnPosts = ownPosts,
            LikedPosts = likedPosts,
            RepostedPosts = repostedPosts
        };

        return View(model);
    }
}
