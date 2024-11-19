using Blog_Management_App.Models;

namespace Blog_Management_App.ViewModels;
/*
 * aggregates data needed to display the details of a single blog post along with a comment form.
 * This model facilitates the display of a blog post’s details and provides the necessary structure for users to submit comments.
 * Enhances the separation of concerns by isolating the data required for the details view.
 */
public class BlogPostDetailsViewModel
{
    public BlogPost BlogPost { get; set; }
    public Comment Comment { get; set; }
}