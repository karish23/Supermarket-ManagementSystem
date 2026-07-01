using LocalSupermarketManagementSystem.Data;
using LocalSupermarketManagementSystem.Models;
using Microsoft.EntityFrameworkCore;

namespace LocalSupermarketManagementSystem.Services;

public class StockService
{
    private readonly SupermarketDbContext _db;

    public StockService(SupermarketDbContext db)
    {
        _db = db;
    }

    public OperationResult UpdateStock(int productId, int newQuantity, string notes)
    {
        if (newQuantity < 0) return OperationResult.Fail("Stock quantity cannot be negative.");

        var product = _db.Products.FirstOrDefault(p => p.Id == productId && p.IsActive);
        if (product == null) return OperationResult.Fail("Product not found.");

        int change = newQuantity - product.QuantityInStock;
        product.QuantityInStock = newQuantity;

        _db.StockRecords.Add(new StockRecord
        {
            ProductId = productId,
            QuantityChange = change,
            StockAction = "Stock Update",
            Notes = notes
        });

        _db.SaveChanges();
        return OperationResult.Ok("Stock updated successfully.");
    }

    public List<StockRecord> GetStockHistory(int productId)
    {
        return _db.StockRecords
            .Include(sr => sr.Product)
            .Where(sr => sr.ProductId == productId)
            .OrderByDescending(sr => sr.CreatedAt)
            .ToList();
    }
}
