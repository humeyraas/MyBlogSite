using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyBlogSite.Models
{
    public class Blog
    {
        public int Id { get; set; }

        [Required]
        public string Title { get; set; } = string.Empty;

        [Required]
        public string Content { get; set; } = string.Empty;

        public string? ImageUrl { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        // Foreign Keys
        public int UserId { get; set; }

        [ForeignKey("UserId")]
        public User? User { get; set; }

        public int CategoryId { get; set; }

        [ForeignKey("CategoryId")]
        public Category? Category { get; set; }

        public ICollection<Comment> Comments { get; set; } = new List<Comment>();

        public int Views { get; set; } = 0;
        public ICollection<BlogLike> LikesList { get; set; } = new List<BlogLike>();

        [NotMapped]
        public int LikeCount => LikesList?.Count ?? 0;

        public string? Tags { get; set; }
    }
}
