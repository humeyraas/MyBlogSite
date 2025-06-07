using System.ComponentModel.DataAnnotations.Schema;

namespace MyBlogSite.Models
{
    public class BlogLike
    {
        public int Id { get; set; }

        public int BlogId { get; set; }
        [ForeignKey("BlogId")]
        public Blog Blog { get; set; }

        public int UserId { get; set; }
        [ForeignKey("UserId")]
        public User User { get; set; }
    }
}
