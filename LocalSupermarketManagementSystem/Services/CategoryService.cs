using LocalSupermarketManagementSystem.Data;
using LocalSupermarketManagementSystem.Models;

namespace LocalSupermarketManagementSystem.Services;

public class CategoryService
{
    private readonly SupermarketDbContext _db;

    public CategoryService(SupermarketDbContext db)
    {
        _db = db;
    }

    public List<Category> GetActiveCategories()
    {
        return _db.Categories.Where(c => c.IsActive).OrderBy(c => c.Name).ToList();
    }

    public OperationResult AddCategory(Category category)
    {
        if (string.IsNullOrWhiteSpace(category.Name)) return OperationResult.Fail("Category name is required.");
        if (_db.Categories.Any(c => c.Name == category.Name)) return OperationResult.Fail("Category name must be unique.");

        _db.Categories.Add(category);
        _db.SaveChanges();
        return OperationResult.Ok("Category added successfully.");
    }
}
