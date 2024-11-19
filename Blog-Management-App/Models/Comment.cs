using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Blog_Management_App.Models;
/*
 * It represents user comments on blog posts.
 * This model allows users to engage with blog posts by adding comments.
 * Enhances interactivity and community engagement within the blog.
 */
public class Comment
{
      public int Id { get; set; }
      
      [Required, StringLength(100)]
      public string Name { get; set; }
      
      [Required, EmailAddress]
      public string Email { get; set; }
      
      [Required, StringLength(1000)]
      //[CommentTextValidator] //We will create this Custom Validator
      public string Text { get; set; }
      
      public DateTime PostedOn { get; set; }
      
      // Foreign Key
      [ForeignKey("BlogPostId")]
      public int BlogPostId { get; set; }
      // Navigation Property
      public BlogPost? BlogPost { get; set; }
}

