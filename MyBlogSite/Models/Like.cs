using MyBlogSite.Models;
using System.ComponentModel.DataAnnotations.Schema;

public class Like
{
    public int Id { get; set; }
    public int BlogId { get; set; }
    public Blog Blog { get; set; }

    public int UserId { get; set; }
    [ForeignKey("UserId")]
    public User User { get; set; }
}
