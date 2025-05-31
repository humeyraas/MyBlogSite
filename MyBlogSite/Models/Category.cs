using System.ComponentModel.DataAnnotations;
using System.Reflection.Metadata;

namespace MyBlogSite.Models
{
    public class Category
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        public ICollection<Blog>? Blogs { get; set; }
    }
}
