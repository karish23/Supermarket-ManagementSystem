using LocalSupermarketManagementSystem.Data;
using Microsoft.EntityFrameworkCore;

namespace LocalSupermarketManagementSystem.Services;

public class ReportService
{
    private readonly SupermarketDbContext _db;

    public ReportService(SupermarketDbContext db)
    {
        _db = db;
    }

    public List<LowStockReportRow> GetLowStockReport()
    {
        return _db.Products
            .Include(p => p.Category)
            .Include(p => p.Supplier)
            .Where(p => p.IsActive && p.QuantityInStock <= p.LowStockThreshold)
            .OrderBy(p => p.QuantityInStock)
            .AsEnumerable()
            .Select(p => new LowStockReportRow
            {
                ProductCode = p.ProductCode,
                Barcode = p.Barcode,
                Product = p.Title,
                Category = p.Category?.Name ?? string.Empty,
                Supplier = p.Supplier?.Name ?? string.Empty,
                QuantityInStock = p.QuantityInStock,
                LowStockThreshold = p.LowStockThreshold,
                Status = p.StockAvailabilityStatus
            })
            .ToList();
    }

    public List<SalesByProductReportRow> GetSalesByProductReport()
    {
        return _db.SaleItems
            .Include(si => si.Product)
            .AsEnumerable()
            .GroupBy(si => si.Product?.Title ?? "Unknown Product")
            .Select(g => new SalesByProductReportRow
            {
                Product = g.Key,
                UnitsSold = g.Sum(x => x.Quantity),
                SalesValue = g.Sum(x => x.LineTotal)
            })
            .OrderByDescending(x => x.SalesValue)
            .ToList();
    }

    public List<ProductsByCategoryReportRow> GetProductsByCategoryReport()
    {
        return _db.Products
            .Include(p => p.Category)
            .Where(p => p.IsActive)
            .AsEnumerable()
            .GroupBy(p => p.Category?.Name ?? "Uncategorised")
            .Select(g => new ProductsByCategoryReportRow
            {
                Category = g.Key,
                ProductCount = g.Count(),
                TotalQuantity = g.Sum(x => x.QuantityInStock),
                TotalStockValue = g.Sum(x => x.Price * x.QuantityInStock)
            })
            .OrderBy(x => x.Category)
            .ToList();
    }

    public List<SupplierStockListReportRow> GetSupplierStockListReport()
    {
        return _db.Products
            .Include(p => p.Supplier)
            .Include(p => p.Category)
            .Where(p => p.IsActive)
            .OrderBy(p => p.Supplier!.Name)
            .ThenBy(p => p.Title)
            .AsEnumerable()
            .Select(p => new SupplierStockListReportRow
            {
                Supplier = p.Supplier?.Name ?? string.Empty,
                Product = p.Title,
                Barcode = p.Barcode,
                Category = p.Category?.Name ?? string.Empty,
                QuantityInStock = p.QuantityInStock,
                Price = p.Price,
                RestockDate = p.RestockDate
            })
            .ToList();
    }
}
