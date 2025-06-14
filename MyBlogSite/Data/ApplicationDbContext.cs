using Microsoft.EntityFrameworkCore;
using MyBlogSite.Models;
using System.Collections.Generic;
using System.Reflection.Metadata;
using System.Xml.Linq;

namespace MyBlogSite.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }


        public DbSet<User> Users { get; set; }
        public DbSet<Blog> Blogs { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public DbSet<BlogLike> BlogLikes { get; set; }
        public DbSet<Repost> Reposts { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Comment>()
                .HasOne(c => c.User)
                .WithMany()
                .HasForeignKey(c => c.UserId)
                .OnDelete(DeleteBehavior.Restrict); 

            modelBuilder.Entity<Comment>()
                .HasOne(c => c.Blog)
                .WithMany(b => b.Comments)
                .HasForeignKey(c => c.BlogId)
                .OnDelete(DeleteBehavior.Cascade); 

            modelBuilder.Entity<BlogLike>()
                .HasOne(bl => bl.User)
                .WithMany()
                .HasForeignKey(bl => bl.UserId)
                .OnDelete(DeleteBehavior.Restrict); 

            modelBuilder.Entity<BlogLike>()
                .HasOne(bl => bl.Blog)
                .WithMany()
                .HasForeignKey(bl => bl.BlogId)
                .OnDelete(DeleteBehavior.Cascade); 
        }


    }


}
