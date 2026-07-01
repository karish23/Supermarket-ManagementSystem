IF DB_ID('LocalSupermarketDB') IS NULL
BEGIN
    CREATE DATABASE LocalSupermarketDB;
END
GO

USE LocalSupermarketDB;
GO

IF OBJECT_ID('SaleItems', 'U') IS NOT NULL DROP TABLE SaleItems;
IF OBJECT_ID('Sales', 'U') IS NOT NULL DROP TABLE Sales;
IF OBJECT_ID('StockRecords', 'U') IS NOT NULL DROP TABLE StockRecords;
IF OBJECT_ID('Products', 'U') IS NOT NULL DROP TABLE Products;
IF OBJECT_ID('Suppliers', 'U') IS NOT NULL DROP TABLE Suppliers;
IF OBJECT_ID('Categories', 'U') IS NOT NULL DROP TABLE Categories;
GO

CREATE TABLE Categories
(
    Id INT IDENTITY(1,1) PRIMARY KEY,
    Name NVARCHAR(100) NOT NULL UNIQUE,
    Description NVARCHAR(300) NOT NULL,
    IsActive BIT NOT NULL DEFAULT 1
);

CREATE TABLE Suppliers
(
    Id INT IDENTITY(1,1) PRIMARY KEY,
    SupplierCode NVARCHAR(50) NOT NULL UNIQUE,
    Name NVARCHAR(150) NOT NULL,
    ContactPerson NVARCHAR(150) NOT NULL,
    Phone NVARCHAR(50) NOT NULL,
    Email NVARCHAR(150) NOT NULL,
    Address NVARCHAR(300) NOT NULL,
    IsActive BIT NOT NULL DEFAULT 1
);

CREATE TABLE Products
(
    Id INT IDENTITY(1,1) PRIMARY KEY,
    ProductCode NVARCHAR(50) NOT NULL UNIQUE,
    Barcode NVARCHAR(80) NOT NULL UNIQUE,
    Title NVARCHAR(150) NOT NULL,
    Brand NVARCHAR(150) NOT NULL,
    ExpiryDate DATETIME2 NULL,
    RestockDate DATETIME2 NULL,
    Price DECIMAL(18,2) NOT NULL,
    QuantityInStock INT NOT NULL,
    LowStockThreshold INT NOT NULL,
    IsActive BIT NOT NULL DEFAULT 1,
    CategoryId INT NOT NULL,
    SupplierId INT NOT NULL,
    CONSTRAINT FK_Products_Categories FOREIGN KEY (CategoryId) REFERENCES Categories(Id),
    CONSTRAINT FK_Products_Suppliers FOREIGN KEY (SupplierId) REFERENCES Suppliers(Id),
    CONSTRAINT CK_Products_Price CHECK (Price > 0),
    CONSTRAINT CK_Products_Quantity CHECK (QuantityInStock >= 0),
    CONSTRAINT CK_Products_LowStock CHECK (LowStockThreshold >= 0)
);

CREATE TABLE StockRecords
(
    Id INT IDENTITY(1,1) PRIMARY KEY,
    ProductId INT NOT NULL,
    QuantityChange INT NOT NULL,
    StockAction NVARCHAR(100) NOT NULL,
    Notes NVARCHAR(300) NOT NULL,
    CreatedAt DATETIME2 NOT NULL DEFAULT GETDATE(),
    CONSTRAINT FK_StockRecords_Products FOREIGN KEY (ProductId) REFERENCES Products(Id)
);

CREATE TABLE Sales
(
    Id INT IDENTITY(1,1) PRIMARY KEY,
    SaleNumber NVARCHAR(80) NOT NULL UNIQUE,
    SaleDate DATETIME2 NOT NULL,
    TotalAmount DECIMAL(18,2) NOT NULL,
    PaymentMethod NVARCHAR(50) NOT NULL
);

CREATE TABLE SaleItems
(
    Id INT IDENTITY(1,1) PRIMARY KEY,
    SaleId INT NOT NULL,
    ProductId INT NOT NULL,
    Quantity INT NOT NULL,
    UnitPrice DECIMAL(18,2) NOT NULL,
    LineTotal DECIMAL(18,2) NOT NULL,
    CONSTRAINT FK_SaleItems_Sales FOREIGN KEY (SaleId) REFERENCES Sales(Id) ON DELETE CASCADE,
    CONSTRAINT FK_SaleItems_Products FOREIGN KEY (ProductId) REFERENCES Products(Id),
    CONSTRAINT CK_SaleItems_Quantity CHECK (Quantity > 0)
);
GO
