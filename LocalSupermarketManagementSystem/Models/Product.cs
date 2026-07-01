using System.ComponentModel.DataAnnotations.Schema;

namespace LocalSupermarketManagementSystem.Models;

public class Product
{
    public int Id { get; set; }
    public string ProductCode { get; set; } = string.Empty;
    public string Barcode { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Brand { get; set; } = string.Empty;
    public DateTime? ExpiryDate { get; set; }
    public DateTime? RestockDate { get; set; }
    public decimal Price { get; set; }
    public int QuantityInStock { get; set; }
    public int LowStockThreshold { get; set; } = 10;
    public bool IsActive { get; set; } = true;

    public int CategoryId { get; set; }
    public Category? Category { get; set; }

    public int SupplierId { get; set; }
    public Supplier? Supplier { get; set; }

    public ICollection<SaleItem> SaleItems { get; set; } = new List<SaleItem>();

    [NotMapped]
    public string StockAvailabilityStatus
    {
        get
        {
            if (QuantityInStock <= 0) return "Out of Stock";
            if (QuantityInStock <= LowStockThreshold) return "Low Stock";
            return "Available";
        }
    }

    [NotMapped]
    public decimal StockValue => Price * QuantityInStock;

    public override string ToString() => $"{Title} ({Barcode})";
}
