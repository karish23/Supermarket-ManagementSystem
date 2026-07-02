using LocalSupermarketManagementSystem.Models;
using LocalSupermarketManagementSystem.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace LocalSupermarketManagementSystem.Tests;

[TestClass]
public class ProductServiceTests
{
    [TestMethod]
    public void AddProduct_ShouldSaveProduct_WhenDataIsValid()
    {
        using var db = TestDbFactory.Create();
        var service = new ProductService(db);

        var result = service.AddProduct(CreateProduct("P-001", "B-001"));

        Assert.IsTrue(result.Success);
        Assert.AreEqual(1, db.Products.Count());
    }

    [TestMethod]
    public void AddProduct_ShouldRejectDuplicateBarcode()
    {
        using var db = TestDbFactory.Create();
        var service = new ProductService(db);

        service.AddProduct(CreateProduct("P-001", "B-001"));
        var result = service.AddProduct(CreateProduct("P-002", "B-001"));

        Assert.IsFalse(result.Success);
        Assert.AreEqual(1, db.Products.Count());
    }

    [TestMethod]
    public void SearchByBarcodeHash_ShouldFindCorrectProduct()
    {
        using var db = TestDbFactory.Create();
        var service = new ProductService(db);
        service.AddProduct(CreateProduct("P-001", "BARCODE-99", "Coffee Jar"));

        var product = service.SearchByBarcodeHash("BARCODE-99");

        Assert.IsNotNull(product);
        Assert.AreEqual("Coffee Jar", product.Title);
    }

    [TestMethod]
    public void SearchByNameLinear_ShouldReturnMatchingProducts()
    {
        using var db = TestDbFactory.Create();
        var service = new ProductService(db);
        service.AddProduct(CreateProduct("P-001", "B-001", "Green Tea"));
        service.AddProduct(CreateProduct("P-002", "B-002", "Black Tea"));
        service.AddProduct(CreateProduct("P-003", "B-003", "Sugar"));

        var results = service.SearchByNameLinear("Tea");

        Assert.AreEqual(2, results.Count);
    }

    private static Product CreateProduct(string code, string barcode, string title = "Test Product")
    {
        return new Product
        {
            ProductCode = code,
            Barcode = barcode,
            Title = title,
            Brand = "Test Brand",
            CategoryId = 1,
            SupplierId = 1,
            Price = 100,
            QuantityInStock = 20,
            LowStockThreshold = 5,
            RestockDate = DateTime.Today.AddDays(3)
        };
    }
}
