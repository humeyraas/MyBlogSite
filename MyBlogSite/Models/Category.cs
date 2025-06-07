﻿using System.ComponentModel.DataAnnotations;

namespace MyBlogSite.Models
{
    public class Category
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; } = string.Empty;

        public ICollection<Blog> Blogs { get; set; } = new List<Blog>();
    }
}
