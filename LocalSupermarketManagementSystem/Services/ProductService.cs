using LocalSupermarketManagementSystem.Data;
using LocalSupermarketManagementSystem.DataStructures;
using LocalSupermarketManagementSystem.Models;
using Microsoft.EntityFrameworkCore;

namespace LocalSupermarketManagementSystem.Services;

public class ProductService
{
    private readonly SupermarketDbContext _db;

    public ProductService(SupermarketDbContext db)
    {
        _db = db;
    }

    public List<Product> GetAllProducts()
    {
        return _db.Products
            .Include(p => p.Category)
            .Include(p => p.Supplier)
            .Where(p => p.IsActive)
            .OrderBy(p => p.Title)
            .ToList();
    }

    public Product? GetById(int id)
    {
        return _db.Products
            .Include(p => p.Category)
            .Include(p => p.Supplier)
            .FirstOrDefault(p => p.Id == id && p.IsActive);
    }

    public OperationResult AddProduct(Product product)
    {
        var validation = ValidateProduct(product);
        if (!validation.Success) return validation;

        if (_db.Products.Any(p => p.ProductCode == product.ProductCode))
            return OperationResult.Fail("Product ID/code must be unique.");

        if (_db.Products.Any(p => p.Barcode == product.Barcode))
            return OperationResult.Fail("Barcode must be unique.");

        product.IsActive = true;
        _db.Products.Add(product);
        _db.SaveChanges();

        _db.StockRecords.Add(new StockRecord
        {
            ProductId = product.Id,
            QuantityChange = product.QuantityInStock,
            StockAction = "Product Added",
            Notes = "Initial product quantity added"
        });
        _db.SaveChanges();

        return OperationResult.Ok("Product added successfully.");
    }

    public OperationResult UpdateProduct(Product product)
    {
        var existing = _db.Products.FirstOrDefault(p => p.Id == product.Id && p.IsActive);
        if (existing == null) return OperationResult.Fail("Product not found.");

        var validation = ValidateProduct(product);
        if (!validation.Success) return validation;

        if (_db.Products.Any(p => p.ProductCode == product.ProductCode && p.Id != product.Id))
            return OperationResult.Fail("Product ID/code must be unique.");

        if (_db.Products.Any(p => p.Barcode == product.Barcode && p.Id != product.Id))
            return OperationResult.Fail("Barcode must be unique.");

        int quantityChange = product.QuantityInStock - existing.QuantityInStock;

        existing.ProductCode = product.ProductCode;
        existing.Barcode = product.Barcode;
        existing.Title = product.Title;
        existing.Brand = product.Brand;
        existing.CategoryId = product.CategoryId;
        existing.SupplierId = product.SupplierId;
        existing.ExpiryDate = product.ExpiryDate;
        existing.RestockDate = product.RestockDate;
        existing.Price = product.Price;
        existing.QuantityInStock = product.QuantityInStock;
        existing.LowStockThreshold = product.LowStockThreshold;

        if (quantityChange != 0)
        {
            _db.StockRecords.Add(new StockRecord
            {
                ProductId = existing.Id,
                QuantityChange = quantityChange,
                StockAction = "Manual Stock Update",
                Notes = "Stock changed during product update"
            });
        }

        _db.SaveChanges();
        return OperationResult.Ok("Product updated successfully.");
    }

    public OperationResult DeleteProduct(int id)
    {
        var product = _db.Products.FirstOrDefault(p => p.Id == id && p.IsActive);
        if (product == null) return OperationResult.Fail("Product not found.");

        product.IsActive = false;
        _db.SaveChanges();
        return OperationResult.Ok("Product removed successfully.");
    }

    public List<Product> SearchByNameLinear(string searchText)
    {
        var list = BuildProductLinkedList();
        if (string.IsNullOrWhiteSpace(searchText)) return list.ToList();

        return list.FindAll(p => p.Title.Contains(searchText.Trim(), StringComparison.OrdinalIgnoreCase)).ToList();
    }

    public Product? SearchByBarcodeHash(string barcode)
    {
        var table = BuildBarcodeHashTable();
        return table.TryGetValue(barcode.Trim(), out var product) ? product : null;
    }

    public List<Product> SearchByCategory(string categoryName)
    {
        var list = BuildProductLinkedList();
        return list.FindAll(p => p.Category != null && p.Category.Name.Contains(categoryName.Trim(), StringComparison.OrdinalIgnoreCase)).ToList();
    }

    public List<Product> GetLowStockProducts()
    {
        return GetAllProducts().Where(p => p.QuantityInStock <= p.LowStockThreshold).ToList();
    }

    private CustomLinkedList<Product> BuildProductLinkedList()
    {
        var linkedList = new CustomLinkedList<Product>();
        foreach (var product in GetAllProducts())
        {
            linkedList.AddLast(product);
        }
        return linkedList;
    }

    private CustomHashTable<string, Product> BuildBarcodeHashTable()
    {
        var hashTable = new CustomHashTable<string, Product>();
        foreach (var product in GetAllProducts())
        {
            hashTable.AddOrUpdate(product.Barcode, product);
        }
        return hashTable;
    }

    private OperationResult ValidateProduct(Product product)
    {
        if (string.IsNullOrWhiteSpace(product.ProductCode)) return OperationResult.Fail("Product ID/code is required.");
        if (string.IsNullOrWhiteSpace(product.Barcode)) return OperationResult.Fail("Barcode is required.");
        if (string.IsNullOrWhiteSpace(product.Title)) return OperationResult.Fail("Product title is required.");
        if (product.CategoryId <= 0) return OperationResult.Fail("Product category is required.");
        if (product.SupplierId <= 0) return OperationResult.Fail("Supplier is required.");
        if (product.Price <= 0) return OperationResult.Fail("Price must be greater than zero.");
        if (product.QuantityInStock < 0) return OperationResult.Fail("Stock quantity cannot be negative.");
        if (product.LowStockThreshold < 0) return OperationResult.Fail("Low stock threshold cannot be negative.");
        if (product.ExpiryDate.HasValue && product.ExpiryDate.Value.Date < DateTime.Today) return OperationResult.Fail("Expiry date cannot be in the past.");
        if (product.RestockDate.HasValue && product.RestockDate.Value.Date < DateTime.Today) return OperationResult.Fail("Restock date cannot be in the past.");
        if (!_db.Categories.Any(c => c.Id == product.CategoryId && c.IsActive)) return OperationResult.Fail("Selected category does not exist.");
        if (!_db.Suppliers.Any(s => s.Id == product.SupplierId && s.IsActive)) return OperationResult.Fail("Selected supplier does not exist.");
        return OperationResult.Ok("Product data is valid.");
    }
}
