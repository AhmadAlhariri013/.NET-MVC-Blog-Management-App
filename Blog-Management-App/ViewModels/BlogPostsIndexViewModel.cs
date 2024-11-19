using Blog_Management_App.Models;

namespace Blog_Management_App.ViewModels;
/*
 * encapsulates data required to display the blog post lists,
 * including pagination and search/filtering options.
 * This model provides the necessary data for listing blog posts with pagination and search capabilities.
 * It enhances user experience by allowing efficient navigation and filtering of blog content.
 */
public class BlogPostsIndexViewModel
{
    public List<BlogPost?> BlogPosts { get; set; }
    
    // Pagination Properties
    public int? CurrentPage { get; set; }
    public int? TotalPages { get; set; }
    
    // Search Filter Properties
    public string? SearchTitle { get; set; }
    public int? SearchCategoryId { get; set; }
}