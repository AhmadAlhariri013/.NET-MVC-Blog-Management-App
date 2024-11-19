using Blog_Management_App.Models;

namespace Blog_Management_App.ViewModels;
/*
 * contains data needed to display blog posts filtered by a specific category and pagination details.
 * It facilitates the display of blog posts filtered by category, enabling users to browse content within specific topics.
 * Enhances content organization and user navigation within the application.
 */
public class CategoryPostsViewModel
{
    public List<BlogPost>? BlogPosts { get; set; }
    
    public int CurrentPage { get; set; }
    public int TotalPages { get; set; }
    
    public int CategoryId { get; set; }
    public string CategoryName { get; set; }
}