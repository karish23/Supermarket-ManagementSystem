using LocalSupermarketManagementSystem.Data;
using LocalSupermarketManagementSystem.Models;
using Microsoft.EntityFrameworkCore;

namespace LocalSupermarketManagementSystem.Tests;

internal static class TestDbFactory
{
    public static SupermarketDbContext Create()
    {
        var options = new DbContextOptionsBuilder<SupermarketDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        var db = new SupermarketDbContext(options);
        db.Categories.Add(new Category { Id = 1, Name = "Test Category", Description = "For tests" });
        db.Suppliers.Add(new Supplier { Id = 1, SupplierCode = "SUP-T", Name = "Test Supplier", Phone = "000" });
        db.SaveChanges();
        return db;
    }
}
