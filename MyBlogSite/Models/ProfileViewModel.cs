using MyBlogSite.Models;

namespace MyBlogSite.Models
{
    public class ProfileViewModel
    {
        public List<Blog> OwnPosts { get; set; }
        public List<Blog> LikedPosts { get; set; }
        public List<Blog> RepostedPosts { get; set; }
    }
}
