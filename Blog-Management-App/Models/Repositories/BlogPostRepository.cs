using Blog_Management_App.Models.Context;
using Blog_Management_App.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace Blog_Management_App.Models.Repositories;

public class BlogPostRepository:IBlogPostRepository
{
    public readonly AppDbContext _context;
    public readonly IConfiguration _configuration;
    public readonly IWebHostEnvironment _webHostEnvironment;
    public BlogPostRepository(AppDbContext context, IConfiguration configuration, IWebHostEnvironment webHostEnvironment)
    {
        _context = context;
        _configuration = configuration;
        _webHostEnvironment = webHostEnvironment;
    }

    public async Task<BlogPostsIndexViewModel> GetBlogPosts(string? searchTitle, int? searchCategoryId, int? pageNumber)
    {
        // 1. Fetch PageSize from appsettings.json, default to 10 if not set
        int pageSize = _configuration.GetValue<int>("Pagination");

        // 2. Initialize query
        var postsQuery = _context.BlogPosts
            .Include(b => b.Author) //Eager Loading
            .Include(b => b.Category) //Eager Loading
            .AsQueryable(); // The query is built but not executed yet

        // 3. Apply Title filter if provided
        if (!string.IsNullOrEmpty(searchTitle))
        {
            postsQuery = postsQuery.Where(p => p.Title.Contains(searchTitle));
        }
        
        // 4. Apply Category filter if provided
        if (searchCategoryId.HasValue && searchCategoryId != 0)
        {
            postsQuery = postsQuery.Where(p => p.CategoryId == searchCategoryId.Value);
        }
        
        // 5. Order by PublishedOn descending (recent first)
        postsQuery = postsQuery.OrderByDescending(p => p.PublishedOn);
        
        // 6. Fetch total count for pagination
        int totalPosts = await postsQuery.CountAsync();
        
        // 7. Calculate total pages
        int totalPages = (int)Math.Ceiling(totalPosts / (double)pageSize);
        totalPages = totalPages < 1 ? 1 : totalPages; // Ensure at least 1 page
        
        // 8. Ensure pageNumber is within valid range
        pageNumber = pageNumber.HasValue && pageNumber.Value > 0 ? pageNumber.Value : 1;
        pageNumber = pageNumber > totalPages ? totalPages : pageNumber;

        // 9. Fetch posts for the current page
        var posts = await postsQuery
            .Skip((pageNumber.Value - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(); //Execute the query to retrive only the records which required in the current page
        
        // 10. Prepare ViewModel for Pagination
        var viewModel = new BlogPostsIndexViewModel
        {
            BlogPosts = posts,
            CurrentPage = pageNumber.Value,
            TotalPages = totalPages,
            SearchTitle = searchTitle,
            SearchCategoryId = searchCategoryId
        };

        return viewModel;
    }

    public async Task<CategoryPostsViewModel> GetBlogPostsByCategory(Category category, int? pageNumber)
    {
        // 1. Fetch PageSize from appsettings.json, default to 10 if not set
        int pageSize = _configuration.GetValue<int?>("Pagination:PageSize") ?? 10;
        
        // 2. Initialize query
        var postsQuery = _context.BlogPosts
            .Where(b => b.CategoryId == category.Id)
            .Include(b => b.Author)
            .Include(b => b.Category)
            .OrderByDescending(b => b.PublishedOn)
            .AsQueryable();
        
        // 3. Fetch total count for pagination
        int totalPosts = await postsQuery.CountAsync();
        
        // 4. Calculate total pages
        int totalPages = (int)Math.Ceiling(totalPosts / (double)pageSize);
        totalPages = totalPages < 1 ? 1 : totalPages; // Ensure at least 1 page
        
        // 5. Ensure pageNumber is within valid range
        pageNumber = pageNumber.HasValue && pageNumber.Value > 0 ? pageNumber.Value : 1;
        pageNumber = pageNumber > totalPages ? totalPages : pageNumber;
        
        // 6. Fetch posts for the current page
        var posts = await postsQuery
            .Skip((pageNumber.Value - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(); //Execute the query to retrive only the records which required in the current page
        
        var viewModel = new CategoryPostsViewModel
        {
            BlogPosts = posts,
            CurrentPage = pageNumber.Value,
            TotalPages = totalPages,
            CategoryName = category.Name,
            CategoryId = category.Id
        };

        return viewModel;
    }

    public async Task<BlogPost?> GetBlogPostBySlug(string slug)
    {
        var result = await _context.BlogPosts
            .Include(b => b.Author)
            .Include(b => b.Category)
            .Include(b => b.Comments)
            .FirstOrDefaultAsync(b => b.Slug == slug);

        if (result == null) return result;

        result.Views += 1;
        await _context.SaveChangesAsync();

        return result;
    }

    public async Task<BlogPost?> GetBlogPostById(int? id)
    {
        return await _context.BlogPosts.FindAsync(id);
    }

    public async Task<BlogPost?> GetBlogPostByIdWithIncluding(int? id)
    {
        var blogPost = await _context.BlogPosts
            .Include(b => b.Author)
            .Include(b => b.Comments)
            .FirstOrDefaultAsync(b => b.Id == id);

        return blogPost;
    }

    public string GetBlogPostSlugById(int? id)
    {
        return _context.BlogPosts?.Find(id)?.Slug;
    }

    public async Task<BlogPost?> GetBlogPostByIdAsNoTracking(int? id)
    {
        return await _context.BlogPosts.AsNoTracking().FirstOrDefaultAsync(b => b.Id == id);
    }

    public async Task<BlogPost?> GetBlogPostByIdWithAuthor(int? id)
    {
        return await _context.BlogPosts
            .Include(b => b.Author)
            .FirstOrDefaultAsync(m => m.Id == id);
    }

    public async Task<string> CreatePost(BlogPost blogPost, IFormFile? FeaturedImage)
    {
        // Handle image upload (using a separate method to avoid duplication)
        blogPost.FeaturedImage = await UploadFeaturedImageAsync(FeaturedImage) ?? blogPost.FeaturedImage;
        
        // Generate or validate the slug
        blogPost.Slug = string.IsNullOrEmpty(blogPost.Slug)
            ? await GenerateSlugAsync(blogPost.Title)
            : blogPost.Slug;
        
        // Ensure the slug is unique
        if (await _context.BlogPosts.AnyAsync(b => b.Slug == blogPost.Slug))
        {
            return "Failed";
        }
        else
        {
            _context.Add(blogPost);
            await _context.SaveChangesAsync();
            return "Success";
        }
    }

    public async Task<string> UpdatePost(int? id, BlogPost blogPost, IFormFile? FeaturedImage)
    {
        var existingPost = await GetBlogPostByIdAsNoTracking(id);
        if (existingPost == null)
        {
            return "NotFound";
        }
        if (FeaturedImage != null && FeaturedImage.Length > 0)
        {
            blogPost.FeaturedImage = await UploadFeaturedImageAsync(FeaturedImage);
        }
        else
        {
            //If you to remove the Featured Image, then don't set this
            blogPost.FeaturedImage = existingPost.FeaturedImage;
        }
        
        // Generate or validate the slug
        blogPost.Slug = string.IsNullOrEmpty(blogPost.Slug)
            ? await GenerateSlugAsync(blogPost.Title)
            : blogPost.Slug;
        
        if (await _context.BlogPosts.AnyAsync(b => b.Slug == blogPost.Slug && b.Id != blogPost.Id))
        {
            return "Failed";
        }
        else
        {
            _context.Update(blogPost);
            await _context.SaveChangesAsync();
            return "Success";
        }
    }

    public async Task DeletePost(BlogPost blogPost)
    {
        _context.BlogPosts.Remove(blogPost);
        await _context.SaveChangesAsync();
    }


    private async Task<string> GenerateSlugAsync(string title)
    {
        // Slug generation with regex
        var slug = System.Text.RegularExpressions.Regex.Replace(title.ToLowerInvariant(), @"\s+", "-").Trim();
        // Ensure slug is unique by appending numbers if necessary
        var uniqueSlug = slug;
        int counter = 1;
        while (await _context.BlogPosts.AnyAsync(b => b.Slug == uniqueSlug))
        {
            uniqueSlug = $"{slug}-{counter++}";
        }
        return uniqueSlug;
    }
    
    private async Task<string> UploadFeaturedImageAsync(IFormFile? featuredImage)
    {
        if (featuredImage != null && featuredImage.Length > 0)
        {
            var uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "uploads");
            if (!Directory.Exists(uploadsFolder))
            {
                Directory.CreateDirectory(uploadsFolder);
            }
            var uniqueFileName = Guid.NewGuid().ToString() + "_" + featuredImage.FileName;
            var filePath = Path.Combine(uploadsFolder, uniqueFileName);
            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                await featuredImage.CopyToAsync(fileStream);
            }
            return "/uploads/" + uniqueFileName;
        }
        return null;
    }
}