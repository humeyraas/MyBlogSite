
using System.ComponentModel.DataAnnotations;

namespace MyBlogSite.Models
{
    public class User 

    {
        public int Id { get; set; }

        [Required]
        public string Username { get; set; } = null!;

        [Required]
        public string Password { get; set; }

        public string Role { get; set; } = "User"; // Default: "User", alternatif: "Admin"

        public string? ProfileImagePath { get; set; } 
        public string? About { get; set; } 

        public ICollection<Blog>? Blogs { get; set; }
        public ICollection<Comment>? Comments { get; set; }
    }
}
