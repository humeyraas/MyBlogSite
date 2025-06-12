using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Xml.Linq;
using MyBlogSite.Models; // veya senin model namespace’in


namespace MyBlogSite.Models
{
    public class Blog
    {
        public int Id { get; set; }

        [Required]
        public string Title { get; set; }

        [Required]
        public string Content { get; set; }

        public string? ImageUrl { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        // Foreign Keys
        public int UserId { get; set; }
        [ForeignKey("UserId")]
        public User? User { get; set; }

        public int CategoryId { get; set; }
        [ForeignKey("CategoryId")]
        public Category? Category { get; set; }

        public ICollection<Comment>? Comments { get; set; }
    }
}
