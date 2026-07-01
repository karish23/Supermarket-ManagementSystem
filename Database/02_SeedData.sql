USE LocalSupermarketDB;
GO

INSERT INTO Categories (Name, Description, IsActive) VALUES
('Dairy', 'Milk, cheese, yogurt and chilled products', 1),
('Bakery', 'Bread, cakes and baked items', 1),
('Beverages', 'Water, juices and soft drinks', 1),
('Household', 'Cleaning and household items', 1),
('Snacks', 'Biscuits, chips and packaged snacks', 1);

INSERT INTO Suppliers (SupplierCode, Name, ContactPerson, Phone, Email, Address, IsActive) VALUES
('SUP-001', 'Fresh Dairy Ltd', 'Ali Khan', '0300-1111111', 'dairy@example.com', 'Industrial Area', 1),
('SUP-002', 'City Bakery Supplies', 'Sara Ahmed', '0300-2222222', 'bakery@example.com', 'Main Market', 1),
('SUP-003', 'Quick Wholesale', 'Usman Raza', '0300-3333333', 'wholesale@example.com', 'Wholesale Market', 1);

INSERT INTO Products
(ProductCode, Barcode, Title, Brand, ExpiryDate, RestockDate, Price, QuantityInStock, LowStockThreshold, IsActive, CategoryId, SupplierId)
VALUES
('PRD-001', '100000001', 'Milk 1 Litre', 'Fresh Dairy', DATEADD(DAY, 10, GETDATE()), DATEADD(DAY, 3, GETDATE()), 220, 35, 10, 1, 1, 1),
('PRD-002', '100000002', 'Cheddar Cheese', 'Fresh Dairy', DATEADD(DAY, 30, GETDATE()), DATEADD(DAY, 7, GETDATE()), 650, 8, 10, 1, 1, 1),
('PRD-003', '100000003', 'Bread Large', 'City Bakery', DATEADD(DAY, 5, GETDATE()), DATEADD(DAY, 1, GETDATE()), 180, 20, 6, 1, 2, 2),
('PRD-004', '100000004', 'Mineral Water 1.5L', 'Aqua', NULL, DATEADD(DAY, 5, GETDATE()), 120, 60, 15, 1, 3, 3),
('PRD-005', '100000005', 'Dishwashing Liquid', 'CleanPro', NULL, DATEADD(DAY, 15, GETDATE()), 350, 12, 5, 1, 4, 3),
('PRD-006', '100000006', 'Potato Chips', 'Snacky', DATEADD(MONTH, 5, GETDATE()), DATEADD(DAY, 8, GETDATE()), 90, 5, 12, 1, 5, 3);

INSERT INTO StockRecords (ProductId, QuantityChange, StockAction, Notes, CreatedAt)
SELECT Id, QuantityInStock, 'Initial Stock', 'Seed stock inserted', GETDATE()
FROM Products;
GO
