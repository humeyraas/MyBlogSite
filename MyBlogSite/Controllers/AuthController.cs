using Microsoft.AspNetCore.Mvc;
using MyBlogSite.Models; // User modelin bu namespace altında olmalı
using MyBlogSite.Data;   // DbContext bu namespace altındaysa
using System.Linq;

using System;

namespace MyBlogSite.Controllers
{
    public class AuthController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AuthController(ApplicationDbContext context)
        {
            _context = context;
            
        }

        [HttpGet]
        public IActionResult Register()
        {
            ViewBag.Categories = _context.Categories.ToList();

            return View();
            
        }

        [HttpPost]
        public IActionResult Register( string Username, string Password)
        {

            ViewBag.Categories = _context.Categories.ToList();

            if (string.IsNullOrWhiteSpace(Username) || string.IsNullOrWhiteSpace(Password))
                return View();

            
            var user = new User
            {
                
                Username = Username,
                Password = Password,
                Role = "User"
            };

            _context.Users.Add(user);
            _context.SaveChanges();

            return RedirectToAction("Login");
        }

        [HttpGet]
        public IActionResult Login()
        {
            ViewBag.Categories = _context.Categories.ToList();

            return View();
        }

        [HttpPost]
        public IActionResult Login(string Username, string Password)
        {
            ViewBag.Categories = _context.Categories.ToList();

            var user = _context.Users
                .FirstOrDefault(u => u.Username == Username && u.Password == Password);

            if (user == null)
            {
                ViewBag.Error = "Invalid username or password.";
                return View();
            }

            
            HttpContext.Session.SetString("username", user.Username);
            HttpContext.Session.SetString("role", user.Role);
            HttpContext.Session.SetInt32("userId", user.Id);


            return RedirectToAction("Index", "Home");
        }


        public IActionResult Logout()
        {
            ViewBag.Categories = _context.Categories.ToList();

            HttpContext.Session.Clear();
            return RedirectToAction("Index", "Home");
        }



    }

}
