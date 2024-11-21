using Blog_Management_App.Models.Context;
using Microsoft.EntityFrameworkCore;

namespace Blog_Management_App.Models.Repositories;

public class AuthorsRepository:IAuthorsRepository
{
    public readonly AppDbContext _context;

    public AuthorsRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Author>> GetAllAuthors()
    {
       return await _context.Authors.ToListAsync();
    }
}