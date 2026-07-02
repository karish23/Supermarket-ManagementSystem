namespace LocalSupermarketManagementSystem.Services;

public class LowStockReportRow
{
    public string ProductCode { get; set; } = string.Empty;
    public string Barcode { get; set; } = string.Empty;
    public string Product { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public string Supplier { get; set; } = string.Empty;
    public int QuantityInStock { get; set; }
    public int LowStockThreshold { get; set; }
    public string Status { get; set; } = string.Empty;
}

public class SalesByProductReportRow
{
    public string Product { get; set; } = string.Empty;
    public int UnitsSold { get; set; }
    public decimal SalesValue { get; set; }
}

public class ProductsByCategoryReportRow
{
    public string Category { get; set; } = string.Empty;
    public int ProductCount { get; set; }
    public int TotalQuantity { get; set; }
    public decimal TotalStockValue { get; set; }
}

public class SupplierStockListReportRow
{
    public string Supplier { get; set; } = string.Empty;
    public string Product { get; set; } = string.Empty;
    public string Barcode { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public int QuantityInStock { get; set; }
    public decimal Price { get; set; }
    public DateTime? RestockDate { get; set; }
}
