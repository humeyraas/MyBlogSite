using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyBlogSite.Data;
using MyBlogSite.Models;

public class UserProfileController : Controller
{
    private readonly ApplicationDbContext _context;

    public UserProfileController(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index(int id)
    {
        var user = await _context.Users.FindAsync(id);
        if (user == null) return NotFound();

        var userPosts = await _context.Blogs
            .Where(b => b.UserId == id)
            .Include(b => b.User)
            .ToListAsync();

        var model = new ProfileViewModel
        {
            OwnPosts = userPosts,
            User = user
        };

        ViewBag.IsOtherUser = true; // kendi profilimiz değilse flag atalım
        ViewBag.Categories = _context.Categories.ToList();

        return View(model);
    }
}
