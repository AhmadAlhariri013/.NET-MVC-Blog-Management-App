namespace Blog_Management_App.Models.Repositories;

public interface ICommentRepository
{
    Task AddComment(Comment comment);
}