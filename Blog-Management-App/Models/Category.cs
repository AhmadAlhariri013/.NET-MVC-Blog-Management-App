using System.ComponentModel.DataAnnotations;

namespace Blog_Management_App.Models;

/*
 * The Category Model defines the categories under which blog posts are classified.
 * This model organizes blog posts into meaningful groups, enabling users to filter and browse posts by category.
 */
public class Category
{
    public int Id { get; set; }
    
    [Required,StringLength(100)]
    public string Name { get; set; }
    
    // Navigation property
    public ICollection<BlogPost> BlogPosts { get; set; }
    
}