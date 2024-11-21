using Blog_Management_App.Models;
using Blog_Management_App.Models.Repositories;
using Blog_Management_App.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace Blog_Management_App.Controllers;

public class CommentsController:Controller
{
    public readonly ICommentRepository _commentRepository;
    public readonly IBlogPostRepository _blogPostRepository;

    public CommentsController(ICommentRepository commentRepository)
    {
        _commentRepository = commentRepository;
    }

    // POST: Comments/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(int blogPostId, Comment comment)
    {
        if (ModelState.IsValid)
        {
            comment.BlogPostId = blogPostId;
            comment.PostedOn = DateTime.UtcNow;
            await _commentRepository.AddComment(comment);
            return RedirectToAction("Details", "BlogPosts", new { slug = _blogPostRepository.GetBlogPostSlugById(blogPostId) });
        }
        // If validation fails, reload the blog post details with errors
        var blogPost = await _blogPostRepository.GetBlogPostByIdWithIncluding(blogPostId);
        if (blogPost == null)
        {
            ViewBag.ErrorMessage = "Blog Post Not Found";
            return View("Error");
        }
        var viewModel = new BlogPostDetailsViewModel
        {
            BlogPost = blogPost,
            Comment = comment
        };
        return View("../BlogPosts/Details", viewModel);
    }
}