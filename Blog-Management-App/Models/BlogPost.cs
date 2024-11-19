using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Blog_Management_App.Models;

/*
 * This model represents individual blog posts created by authors.
 * It serves as the primary entity for creating, displaying, editing, and managing blog content.
 * It facilitates relationships with authors, categories, and comments, enabling comprehensive blog management features.
 */
public class BlogPost
{
      public int Id { get; set; }
      
      [Required, StringLength(200)]
      public string Title { get; set; }
      
      [Required]
      public string Body { get; set; }
      
      public string? FeaturedImage { get; set; }
      
      [StringLength(150)]
      public string? MetaTitle { get; set; }
      
      [StringLength(300)]
      public string? MetaDescription { get; set; }
      
      [StringLength(250)]
      public string? MetaKeywords { get; set; }
      
      [StringLength(200)]
      public string? Slug { get; set; }
      
      public int Views { get; set; } = 0; // Initialize to 0
      
      public DateTime PublishedOn { get; set; } = DateTime.UtcNow;
      public DateTime? ModifiedOn { get; set; } = DateTime.UtcNow;
             
      [Required]
      [ForeignKey("AuthorId")]
      public int? AuthorId { get; set; }  // Foreign Key
      public Author? Author { get; set; } // Navigation Property
            
      [Required]
      [ForeignKey("CategoryId")]
      public int? CategoryId { get; set; } // Foreign Key
      public Category? Category { get; set; } // Navigation Property
      
      public ICollection<Comment>? Comments { get; set; }
}