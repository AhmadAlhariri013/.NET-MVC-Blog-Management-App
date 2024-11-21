using Blog_Management_App.Models;
using Blog_Management_App.Models.Repositories;
using Blog_Management_App.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Blog_Management_App.Controllers;
/*
 * The BlogPostsController manages all operations related to blog posts, including listing, viewing details, creating, editing, and deleting posts.
 * It is the central hub for all blog post-related functionalities, coordinating data retrieval, processing, and view rendering.
 */
public class BlogPostsController:Controller
{
    public readonly IBlogPostRepository _blogPostRepository;
    public readonly ICategoryRepository _categoryRepository;
    public readonly IAuthorsRepository _authorsRepository;

    public BlogPostsController(IBlogPostRepository blogPostRepository, ICategoryRepository categoryRepository, IAuthorsRepository authorsRepository)
    {
        _blogPostRepository = blogPostRepository;
        _categoryRepository = categoryRepository;
        _authorsRepository = authorsRepository;
    }

    public async Task<IActionResult> Index(string? searchTitle, int? searchCategoryId, int? pageNumber)
    {
        try
        {
            // 1. Fetch Categories for the Dropdown
            var categories = await _categoryRepository.GetAllCategories();
            ViewBag.Categories = new SelectList(categories, "Id", "Name");

            // 2. Fetch posts for the current page
            var viewModel =await _blogPostRepository.GetBlogPosts(searchTitle, searchCategoryId, pageNumber);
            return View(viewModel);
            
        }catch (Exception ex)
        {
            ViewBag.ErrorMessage = "Unable to load blog posts. Please try again later.";
            return View("Error");
        }
    }
    
    // GET: Blog Post by Slug
    [HttpGet]
    [Route("/blog/{slug}")]
    public async Task<IActionResult> Details(string slug)
    {
        if (string.IsNullOrEmpty(slug))
        {
            ViewBag.ErrorMessage = "Slug not provided.";
            return View("Error");
        }

        try
        {
            var blogPost = await _blogPostRepository.GetBlogPostBySlug(slug);

            if (blogPost == null)
            {
                ViewBag.ErrorMessage = "Slug not provided.";
                return View("Error");
            }
            
            // Set SEO meta tags
            ViewBag.MetaDescription = blogPost.MetaDescription;
            ViewBag.MetaKeywords = blogPost.MetaKeywords;
            ViewBag.Title = blogPost.MetaTitle ?? blogPost.Title;

            var viewModel = new BlogPostDetailsViewModel
            {
                BlogPost = blogPost,
                Comment = new Comment()
            };

            return View(viewModel);

        }
        catch (Exception ex)
        {
            ViewBag.ErrorMessage = "An error occurred while loading the blog post details.";
            return View("Error");
        }
    }
    // GET: Categories/{id}/Posts
    [HttpGet("/categories/{id}/posts")]
    public async Task<IActionResult> PostsByCategory(int id, int? pageNumber)
    {
        var category = await _categoryRepository.GetCategoryById(id);
        if (category == null)
        {
            ViewBag.ErrorMessage = "Invalid Category";
            return View("Error");
        }

        var result = await _blogPostRepository.GetBlogPostsByCategory(category, pageNumber);
        
        return View("CategoryPosts", result);
    }
    
    // GET: BlogPosts/Create
    public async Task<IActionResult> Create()
    {
        try
        {
            // Fetch authors and Categories for dropdown
            ViewBag.Authors = await _authorsRepository.GetAllAuthors();
            ViewBag.Categories = await _categoryRepository.GetAllCategories();
            return View();
        }
        catch (Exception ex)
        {
            ViewBag.ErrorMessage = "Unable to load the create blog post form. Please try again later.";
            return View("Error");
        }
    }
    // POST: BlogPosts/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(BlogPost blogPost, IFormFile? FeaturedImage)
    {
        if (ModelState.IsValid)
        {
            try
            {
                var result = await _blogPostRepository.CreatePost(blogPost,FeaturedImage);
                // Ensure the slug is unique
                if (result != "Success")
                {
                    ModelState.AddModelError("Slug", "The slug must be unique.");
                }
                else
                {
                    TempData["SuccessMessage"] = "Blog post added successfully.";
                    return RedirectToAction(nameof(Index));
                }
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = "An error occurred while creating the blog post.";
                return View("Error");
            }
        }
        // If validation fails, reload authors and Categories for dropdown
        ViewBag.Authors = await _authorsRepository.GetAllAuthors();
        ViewBag.Categories = await _categoryRepository.GetAllCategories();
        return View();
    }
    
    // GET: BlogPosts/Edit/5
    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null)
        {
            ViewBag.ErrorMessage = "Blog post ID is missing.";
            return View("Error");
        }
        try
        {
            var blogPost = await _blogPostRepository.GetBlogPostById(id);
            if (blogPost == null)
            {
                ViewBag.ErrorMessage = "Blog post not found.";
                return View("Error");
            }
            ViewBag.Authors = await _authorsRepository.GetAllAuthors();
            ViewBag.Categories = await _categoryRepository.GetAllCategories();
            return View(blogPost);
        }
        catch (Exception ex)
        {
            ViewBag.ErrorMessage = "An error occurred while loading the edit blog post form.";
            return View("Error");
        }
    }
    // POST: BlogPosts/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, BlogPost blogPost, IFormFile? FeaturedImage)
    {
        if (id != blogPost.Id)
        {
            return NotFound("Blog post ID mismatch.");
        }
        if (ModelState.IsValid)
        {
            try
            {
                
                var result = await _blogPostRepository.UpdatePost(id, blogPost, FeaturedImage);
                if (result == "NotFound")
                {
                    return NotFound("Blog post not found.");
                }
                
                
                if (result == "Failed")
                {
                    ModelState.AddModelError("Slug", "The slug must be unique.");
                }
                else
                {
                    // Set success message
                    TempData["SuccessMessage"] = "Blog post updated successfully.";
                    return RedirectToAction(nameof(Index));
                }
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = "An error occurred while updating the blog post.";
                return View("Error");
            }
        }
        ViewBag.Authors = await _authorsRepository.GetAllAuthors();
        ViewBag.Categories = await _categoryRepository.GetAllCategories();
        return View(blogPost);
    }

    // GET: BlogPosts/Delete/5
    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null)
        {
            ViewBag.ErrorMessage = "Blog post ID is missing.";
            return View("Error");
        }
        try
        {
            var blogPost = await _blogPostRepository.GetBlogPostByIdWithAuthor(id);
            if (blogPost == null)
            {
                ViewBag.ErrorMessage = "Blog post not found.";
                return View("Error");
            }
            return View(blogPost);
        }
        catch (Exception ex)
        {
            ViewBag.ErrorMessage = "An error occurred while loading the blog post for deletion.";
            return View("Error");
        }
    }
    // POST: BlogPosts/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        try
        {
            var blogPost = await _blogPostRepository.GetBlogPostById(id);
            if (blogPost != null)
            {
                await _blogPostRepository.DeletePost(blogPost);
                // Set success message
                TempData["SuccessMessage"] = "Blog post deleted successfully.";
            }
            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            ViewBag.ErrorMessage = "An error occurred while deleting the blog post.";
            return View("Error");
        }
    }
    
    
    
    
    

    
}