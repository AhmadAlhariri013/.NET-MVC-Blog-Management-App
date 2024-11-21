using Blog_Management_App.ViewModels;

namespace Blog_Management_App.Models.Repositories;

public interface IBlogPostRepository
{
    Task<BlogPostsIndexViewModel> GetBlogPosts(string? searchTitle, int? searchCategoryId, int? pageNumber);
    Task<CategoryPostsViewModel> GetBlogPostsByCategory(Category category, int? pageNumber);
    Task<BlogPost?> GetBlogPostBySlug(string slug);
    Task<BlogPost?> GetBlogPostById(int? id);
    Task<BlogPost?> GetBlogPostByIdWithIncluding(int? id);
    string GetBlogPostSlugById(int? id);
    Task<BlogPost?> GetBlogPostByIdAsNoTracking(int? id);
    Task<BlogPost?> GetBlogPostByIdWithAuthor(int? id);
    Task<string> CreatePost(BlogPost blogPost, IFormFile? FeaturedImage);
    Task<string> UpdatePost(int? id, BlogPost blogPost, IFormFile? FeaturedImage);
    Task DeletePost(BlogPost blogPost);

}