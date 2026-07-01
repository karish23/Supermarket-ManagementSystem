using LocalSupermarketManagementSystem.Data;
using LocalSupermarketManagementSystem.Models;
using Microsoft.EntityFrameworkCore;

namespace LocalSupermarketManagementSystem.Services;

public class SupplierService
{
    private readonly SupermarketDbContext _db;

    public SupplierService(SupermarketDbContext db)
    {
        _db = db;
    }

    public List<Supplier> GetActiveSuppliers()
    {
        return _db.Suppliers.Where(s => s.IsActive).OrderBy(s => s.Name).ToList();
    }

    public Supplier? GetById(int id)
    {
        return _db.Suppliers.FirstOrDefault(s => s.Id == id && s.IsActive);
    }

    public OperationResult AddSupplier(Supplier supplier)
    {
        var validation = ValidateSupplier(supplier);
        if (!validation.Success) return validation;

        if (_db.Suppliers.Any(s => s.SupplierCode == supplier.SupplierCode))
            return OperationResult.Fail("Supplier code must be unique.");

        supplier.IsActive = true;
        _db.Suppliers.Add(supplier);
        _db.SaveChanges();
        return OperationResult.Ok("Supplier added successfully.");
    }

    public OperationResult UpdateSupplier(Supplier supplier)
    {
        var existing = _db.Suppliers.FirstOrDefault(s => s.Id == supplier.Id);
        if (existing == null) return OperationResult.Fail("Supplier not found.");

        var validation = ValidateSupplier(supplier);
        if (!validation.Success) return validation;

        if (_db.Suppliers.Any(s => s.SupplierCode == supplier.SupplierCode && s.Id != supplier.Id))
            return OperationResult.Fail("Supplier code must be unique.");

        existing.SupplierCode = supplier.SupplierCode;
        existing.Name = supplier.Name;
        existing.ContactPerson = supplier.ContactPerson;
        existing.Phone = supplier.Phone;
        existing.Email = supplier.Email;
        existing.Address = supplier.Address;
        _db.SaveChanges();
        return OperationResult.Ok("Supplier updated successfully.");
    }

    public OperationResult DeleteSupplier(int id)
    {
        var supplier = _db.Suppliers.Include(s => s.Products).FirstOrDefault(s => s.Id == id);
        if (supplier == null) return OperationResult.Fail("Supplier not found.");

        supplier.IsActive = false;
        _db.SaveChanges();
        return OperationResult.Ok("Supplier removed successfully.");
    }

    private static OperationResult ValidateSupplier(Supplier supplier)
    {
        if (string.IsNullOrWhiteSpace(supplier.SupplierCode)) return OperationResult.Fail("Supplier code is required.");
        if (string.IsNullOrWhiteSpace(supplier.Name)) return OperationResult.Fail("Supplier name is required.");
        if (string.IsNullOrWhiteSpace(supplier.Phone)) return OperationResult.Fail("Supplier phone is required.");
        if (!string.IsNullOrWhiteSpace(supplier.Email) && !supplier.Email.Contains('@')) return OperationResult.Fail("Supplier email is not valid.");
        return OperationResult.Ok("Supplier data is valid.");
    }
}
