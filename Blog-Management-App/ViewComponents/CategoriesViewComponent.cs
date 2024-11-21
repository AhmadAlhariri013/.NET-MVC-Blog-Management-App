using Blog_Management_App.Models.Context;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Blog_Management_App.ViewComponents;

public class CategoriesViewComponent:ViewComponent
{
    private readonly AppDbContext _dbContext;

    public CategoriesViewComponent(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IViewComponentResult> InvokeAsync()
    {
        var categories = await _dbContext.Categories.OrderBy(c => c.Name).ToListAsync();
        //View Location shuld be : Views/Shared/Components/Categories/Default.cshtml
        return View(categories); 
    }
}