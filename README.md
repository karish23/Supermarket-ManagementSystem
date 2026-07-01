# Local Supermarket Management System

Educational sample project for a small-shop supermarket management system.

## Technology Used

- C#
- WinForms
- .NET 10.0
- SQL Server LocalDB
- Entity Framework Core
- MSTest

## Main Features

- Product management
- Supplier management
- Category support
- Stock quantity management
- Low-stock alerts
- Sales recording
- Sale item handling
- Product search by name
- Product search by barcode
- Low-stock report
- Sales by product report
- Products by category report
- Supplier stock list report
- Custom linked list
- Custom hash table
- Unit tests for important operations

## Database Server

The project uses this LocalDB server:

```text
(localdb)\MSSQLLocalDB
```

Default connection string:

```text
Server=(localdb)\MSSQLLocalDB;Database=LocalSupermarketDB;Trusted_Connection=True;TrustServerCertificate=True;
```

## How To Run

1. Extract the ZIP file.
2. Open `LocalSupermarketManagementSystem.sln` in Visual Studio.
3. Restore NuGet packages.
4. Build the solution.
5. Run the `LocalSupermarketManagementSystem` project.

The application creates the database automatically using Entity Framework `EnsureCreated()` and inserts seed data if the tables are empty.

## Optional SQL Scripts

The `Database` folder contains:

- `01_CreateDatabase.sql`
- `02_SeedData.sql`

These scripts can be executed manually in SQL Server Management Studio if you want to create the database yourself.

## Running Tests

1. Open Test Explorer in Visual Studio.
2. Build the solution.
3. Run all tests in `LocalSupermarketManagementSystem.Tests`.

## Notes

This project is designed as a complete teaching sample with generic comments and clear separation of models, data access, services, custom data structures, UI, database scripts, and unit tests.
