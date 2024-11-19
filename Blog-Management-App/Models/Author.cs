using System.ComponentModel.DataAnnotations;

namespace Blog_Management_App.Models;

/*
 * represents the authors who create blog posts within the application.
 * We will use this model to store and manage information about authors.
 * Enables associating blog posts with their respective authors, facilitating author-specific functionalities like listing all posts by an author.
 */
public class Author
{
      public int Id { get; set; }
      
      [Required, StringLength(100)]
      public string Name { get; set; }
      
      [EmailAddress,Required]
      public string Email { get; set; }
      
      // Navigation property
      public ICollection<BlogPost> BlogPosts { get; set; }
}