using Blog_Management_App.Models.Context;

namespace Blog_Management_App.Models.Repositories;

public class CommentRepository:ICommentRepository
{
    public readonly AppDbContext _context;

    public CommentRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task AddComment(Comment comment)
    {
        _context.Comments.Add(comment);
        await _context.SaveChangesAsync();
    }
}