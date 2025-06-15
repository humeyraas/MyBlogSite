using MyBlogSite.Models;
namespace MyBlogSite.Models
{
    public class SearchResultsViewModel
    {
        public List<Blog> Blogs { get; set; }
        public List<User> Users { get; set; }
    }
}