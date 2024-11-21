using Blog_Management_App.Models.Context;
using Microsoft.EntityFrameworkCore;

namespace Blog_Management_App.Models.Repositories;

public class CategoryRepository:ICategoryRepository
{
    public readonly AppDbContext _context;

    public CategoryRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Category>> GetAllCategories()
    {
        return await _context.Categories.OrderBy(c => c.Name).ToListAsync();
    }

    public async Task<Category?> GetCategoryById(int id)
    {
        return await _context.Categories.FindAsync(id);
    }
}