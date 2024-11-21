namespace Blog_Management_App.Models.Repositories;

public interface IAuthorsRepository
{
    Task<IEnumerable<Author>> GetAllAuthors();
}