using LocalSupermarketManagementSystem.Data;
using LocalSupermarketManagementSystem.Models;
using Microsoft.EntityFrameworkCore;

namespace LocalSupermarketManagementSystem.Services;

public class SaleService
{
    private readonly SupermarketDbContext _db;

    public SaleService(SupermarketDbContext db)
    {
        _db = db;
    }

    public OperationResult<Sale> RecordSale(List<SaleItemRequest> items, string paymentMethod)
    {
        if (items.Count == 0) return OperationResult<Sale>.Fail("At least one sale item is required.");

        var productIds = items.Select(i => i.ProductId).Distinct().ToList();
        var products = _db.Products.Where(p => productIds.Contains(p.Id) && p.IsActive).ToList();

        foreach (var item in items)
        {
            if (item.Quantity <= 0) return OperationResult<Sale>.Fail("Sale quantity must be greater than zero.");

            var product = products.FirstOrDefault(p => p.Id == item.ProductId);
            if (product == null) return OperationResult<Sale>.Fail("One or more products were not found.");
            if (product.QuantityInStock < item.Quantity)
                return OperationResult<Sale>.Fail($"Insufficient stock for {product.Title}.");
        }

        var sale = new Sale
        {
            SaleNumber = GenerateSaleNumber(),
            SaleDate = DateTime.Now,
            PaymentMethod = string.IsNullOrWhiteSpace(paymentMethod) ? "Cash" : paymentMethod.Trim()
        };

        foreach (var item in items)
        {
            var product = products.First(p => p.Id == item.ProductId);
            var lineTotal = product.Price * item.Quantity;

            sale.SaleItems.Add(new SaleItem
            {
                ProductId = product.Id,
                Quantity = item.Quantity,
                UnitPrice = product.Price,
                LineTotal = lineTotal
            });

            sale.TotalAmount += lineTotal;
            product.QuantityInStock -= item.Quantity;

            _db.StockRecords.Add(new StockRecord
            {
                ProductId = product.Id,
                QuantityChange = -item.Quantity,
                StockAction = "Sale",
                Notes = $"Sale transaction {sale.SaleNumber}"
            });
        }

        _db.Sales.Add(sale);
        _db.SaveChanges();
        return OperationResult<Sale>.Ok(sale, "Sale recorded successfully.");
    }

    public List<Sale> GetRecentSales()
    {
        return _db.Sales
            .Include(s => s.SaleItems)
            .ThenInclude(si => si.Product)
            .OrderByDescending(s => s.SaleDate)
            .Take(50)
            .ToList();
    }

    private static string GenerateSaleNumber()
    {
        return $"S-{DateTime.Now:yyyyMMdd-HHmmss-fff}";
    }
}
