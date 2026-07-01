using LocalSupermarketManagementSystem.Models;

namespace LocalSupermarketManagementSystem.Data;

public static class DatabaseSeeder
{
    public static void Seed(SupermarketDbContext db)
    {
        if (!db.Categories.Any())
        {
            db.Categories.AddRange(
                new Category { Name = "Dairy", Description = "Milk, cheese, yogurt and chilled products" },
                new Category { Name = "Bakery", Description = "Bread, cakes and baked items" },
                new Category { Name = "Beverages", Description = "Water, juices and soft drinks" },
                new Category { Name = "Household", Description = "Cleaning and household items" },
                new Category { Name = "Snacks", Description = "Biscuits, chips and packaged snacks" }
            );
            db.SaveChanges();
        }

        if (!db.Suppliers.Any())
        {
            db.Suppliers.AddRange(
                new Supplier { SupplierCode = "SUP-001", Name = "Fresh Dairy Ltd", ContactPerson = "Ali Khan", Phone = "0300-1111111", Email = "dairy@example.com", Address = "Industrial Area" },
                new Supplier { SupplierCode = "SUP-002", Name = "City Bakery Supplies", ContactPerson = "Sara Ahmed", Phone = "0300-2222222", Email = "bakery@example.com", Address = "Main Market" },
                new Supplier { SupplierCode = "SUP-003", Name = "Quick Wholesale", ContactPerson = "Usman Raza", Phone = "0300-3333333", Email = "wholesale@example.com", Address = "Wholesale Market" }
            );
            db.SaveChanges();
        }

        if (!db.Products.Any())
        {
            var dairy = db.Categories.First(c => c.Name == "Dairy");
            var bakery = db.Categories.First(c => c.Name == "Bakery");
            var beverages = db.Categories.First(c => c.Name == "Beverages");
            var household = db.Categories.First(c => c.Name == "Household");
            var snacks = db.Categories.First(c => c.Name == "Snacks");

            var freshDairy = db.Suppliers.First(s => s.SupplierCode == "SUP-001");
            var cityBakery = db.Suppliers.First(s => s.SupplierCode == "SUP-002");
            var quickWholesale = db.Suppliers.First(s => s.SupplierCode == "SUP-003");

            db.Products.AddRange(
                new Product { ProductCode = "PRD-001", Barcode = "100000001", Title = "Milk 1 Litre", Brand = "Fresh Dairy", CategoryId = dairy.Id, SupplierId = freshDairy.Id, ExpiryDate = DateTime.Today.AddDays(10), RestockDate = DateTime.Today.AddDays(3), Price = 220, QuantityInStock = 35, LowStockThreshold = 10 },
                new Product { ProductCode = "PRD-002", Barcode = "100000002", Title = "Cheddar Cheese", Brand = "Fresh Dairy", CategoryId = dairy.Id, SupplierId = freshDairy.Id, ExpiryDate = DateTime.Today.AddDays(30), RestockDate = DateTime.Today.AddDays(7), Price = 650, QuantityInStock = 8, LowStockThreshold = 10 },
                new Product { ProductCode = "PRD-003", Barcode = "100000003", Title = "Bread Large", Brand = "City Bakery", CategoryId = bakery.Id, SupplierId = cityBakery.Id, ExpiryDate = DateTime.Today.AddDays(5), RestockDate = DateTime.Today.AddDays(1), Price = 180, QuantityInStock = 20, LowStockThreshold = 6 },
                new Product { ProductCode = "PRD-004", Barcode = "100000004", Title = "Mineral Water 1.5L", Brand = "Aqua", CategoryId = beverages.Id, SupplierId = quickWholesale.Id, RestockDate = DateTime.Today.AddDays(5), Price = 120, QuantityInStock = 60, LowStockThreshold = 15 },
                new Product { ProductCode = "PRD-005", Barcode = "100000005", Title = "Dishwashing Liquid", Brand = "CleanPro", CategoryId = household.Id, SupplierId = quickWholesale.Id, RestockDate = DateTime.Today.AddDays(15), Price = 350, QuantityInStock = 12, LowStockThreshold = 5 },
                new Product { ProductCode = "PRD-006", Barcode = "100000006", Title = "Potato Chips", Brand = "Snacky", CategoryId = snacks.Id, SupplierId = quickWholesale.Id, ExpiryDate = DateTime.Today.AddMonths(5), RestockDate = DateTime.Today.AddDays(8), Price = 90, QuantityInStock = 5, LowStockThreshold = 12 }
            );
            db.SaveChanges();

            foreach (var product in db.Products.ToList())
            {
                db.StockRecords.Add(new StockRecord
                {
                    ProductId = product.Id,
                    QuantityChange = product.QuantityInStock,
                    StockAction = "Initial Stock",
                    Notes = "Seed stock added for demonstration"
                });
            }
            db.SaveChanges();
        }
    }
}
