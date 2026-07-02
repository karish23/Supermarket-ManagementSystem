using LocalSupermarketManagementSystem.Models;
using LocalSupermarketManagementSystem.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace LocalSupermarketManagementSystem.Tests;

[TestClass]
public class StockAndSaleTests
{
    [TestMethod]
    public void UpdateStock_ShouldChangeProductQuantity()
    {
        using var db = TestDbFactory.Create();
        var productService = new ProductService(db);
        productService.AddProduct(CreateProduct("P-001", "B-001", 20));
        var product = db.Products.First();

        var stockService = new StockService(db);
        var result = stockService.UpdateStock(product.Id, 7, "Test update");

        Assert.IsTrue(result.Success);
        Assert.AreEqual(7, db.Products.First().QuantityInStock);
    }

    [TestMethod]
    public void LowStockReport_ShouldShowProductBelowThreshold()
    {
        using var db = TestDbFactory.Create();
        var productService = new ProductService(db);
        productService.AddProduct(CreateProduct("P-001", "B-001", 3));

        var lowStock = productService.GetLowStockProducts();

        Assert.AreEqual(1, lowStock.Count);
        Assert.AreEqual("Low Stock", lowStock.First().StockAvailabilityStatus);
    }

    [TestMethod]
    public void RecordSale_ShouldReduceStockAndCreateSale()
    {
        using var db = TestDbFactory.Create();
        var productService = new ProductService(db);
        productService.AddProduct(CreateProduct("P-001", "B-001", 20));
        var product = db.Products.First();

        var saleService = new SaleService(db);
        var result = saleService.RecordSale(new List<SaleItemRequest>
        {
            new SaleItemRequest { ProductId = product.Id, Quantity = 4 }
        }, "Cash");

        Assert.IsTrue(result.Success);
        Assert.AreEqual(16, db.Products.First().QuantityInStock);
        Assert.AreEqual(1, db.Sales.Count());
        Assert.AreEqual(400, db.Sales.First().TotalAmount);
    }

    [TestMethod]
    public void RecordSale_ShouldRejectInsufficientStock()
    {
        using var db = TestDbFactory.Create();
        var productService = new ProductService(db);
        productService.AddProduct(CreateProduct("P-001", "B-001", 2));
        var product = db.Products.First();

        var saleService = new SaleService(db);
        var result = saleService.RecordSale(new List<SaleItemRequest>
        {
            new SaleItemRequest { ProductId = product.Id, Quantity = 5 }
        }, "Cash");

        Assert.IsFalse(result.Success);
        Assert.AreEqual(0, db.Sales.Count());
    }

    private static Product CreateProduct(string code, string barcode, int quantity)
    {
        return new Product
        {
            ProductCode = code,
            Barcode = barcode,
            Title = "Test Product",
            Brand = "Test Brand",
            CategoryId = 1,
            SupplierId = 1,
            Price = 100,
            QuantityInStock = quantity,
            LowStockThreshold = 5,
            RestockDate = DateTime.Today.AddDays(3)
        };
    }
}
