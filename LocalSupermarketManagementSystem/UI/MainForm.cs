using System.Drawing;
using System.Windows.Forms;
using LocalSupermarketManagementSystem.Data;
using LocalSupermarketManagementSystem.Models;
using LocalSupermarketManagementSystem.Services;

namespace LocalSupermarketManagementSystem.UI;

public class MainForm : Form
{
    private readonly SupermarketDbContext _db = new();
    private readonly ProductService _productService;
    private readonly SupplierService _supplierService;
    private readonly CategoryService _categoryService;
    private readonly StockService _stockService;
    private readonly SaleService _saleService;
    private readonly ReportService _reportService;

    private DataGridView dgvProducts = new();
    private DataGridView dgvSuppliers = new();
    private DataGridView dgvCart = new();
    private DataGridView dgvSales = new();
    private DataGridView dgvReports = new();

    private TextBox txtProductCode = new();
    private TextBox txtBarcode = new();
    private TextBox txtTitle = new();
    private TextBox txtBrand = new();
    private NumericUpDown numPrice = new();
    private NumericUpDown numQuantity = new();
    private NumericUpDown numLowStock = new();
    private ComboBox cmbCategory = new();
    private ComboBox cmbSupplier = new();
    private DateTimePicker dtpExpiry = new();
    private DateTimePicker dtpRestock = new();
    private CheckBox chkExpiry = new();
    private CheckBox chkRestock = new();
    private TextBox txtProductSearch = new();
    private TextBox txtSupplierSearch = new();

    private TextBox txtSupplierCode = new();
    private TextBox txtSupplierName = new();
    private TextBox txtContactPerson = new();
    private TextBox txtSupplierPhone = new();
    private TextBox txtSupplierEmail = new();
    private TextBox txtSupplierAddress = new();

    private ComboBox cmbSaleProduct = new();
    private NumericUpDown numSaleQuantity = new();
    private ComboBox cmbPaymentMethod = new();
    private Label lblSaleTotal = new();
    private readonly List<CartLine> _cart = new();

    private int? _selectedProductId;
    private int? _selectedSupplierId;

    public MainForm()
    {
        _db.Database.EnsureCreated();
        DatabaseSeeder.Seed(_db);

        _productService = new ProductService(_db);
        _supplierService = new SupplierService(_db);
        _categoryService = new CategoryService(_db);
        _stockService = new StockService(_db);
        _saleService = new SaleService(_db);
        _reportService = new ReportService(_db);

        Text = "Local Supermarket Management System";
        Width = 1250;
        Height = 900;
        MinimumSize = new Size(1200, 850);
        StartPosition = FormStartPosition.CenterScreen;

        BuildLayout();
        LoadAllData();
    }

    private void BuildLayout()
    {
        var tabs = new TabControl { Dock = DockStyle.Fill };
        tabs.TabPages.Add(BuildProductsTab());
        tabs.TabPages.Add(BuildSuppliersTab());
        tabs.TabPages.Add(BuildSalesTab());
        tabs.TabPages.Add(BuildReportsTab());
        Controls.Add(tabs);
    }

    private TabPage BuildProductsTab()
    {
        var tab = new TabPage("Products and Stock");

        var pageLayout = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            RowCount = 4,
            ColumnCount = 1,
            Padding = new Padding(8)
        };
        pageLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 540));
        pageLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 48));
        pageLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
        pageLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 32));
        tab.Controls.Add(pageLayout);

        var form = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 2,
            Padding = new Padding(10),
            AutoScroll = true
        };
        form.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 130));
        form.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));

        AddLabeledControl(form, "Product ID", txtProductCode);
        AddLabeledControl(form, "Barcode", txtBarcode);
        AddLabeledControl(form, "Title", txtTitle);
        AddLabeledControl(form, "Brand", txtBrand);
        AddLabeledControl(form, "Category", cmbCategory);
        AddLabeledControl(form, "Supplier", cmbSupplier);
        numPrice.Maximum = 1000000; numPrice.DecimalPlaces = 2; numPrice.Minimum = 1;
        AddLabeledControl(form, "Price", numPrice);
        numQuantity.Maximum = 100000; numQuantity.Minimum = 0;
        AddLabeledControl(form, "Quantity", numQuantity);
        numLowStock.Maximum = 100000; numLowStock.Minimum = 0; numLowStock.Value = 10;
        AddLabeledControl(form, "Low Stock Limit", numLowStock);
        chkExpiry.Text = "Use expiry date";
        AddLabeledControl(form, "Expiry", chkExpiry);
        AddLabeledControl(form, "Expiry Date", dtpExpiry);
        chkRestock.Text = "Use restock date";
        AddLabeledControl(form, "Restock", chkRestock);
        AddLabeledControl(form, "Restock Date", dtpRestock);

        var buttons = new FlowLayoutPanel
        {
            Dock = DockStyle.Fill,
            AutoSize = false,
            Height = 42,
            Padding = new Padding(0, 4, 0, 4),
            WrapContents = false
        };
        buttons.Controls.Add(MakeButton("Add", AddProduct));
        buttons.Controls.Add(MakeButton("Update", UpdateProduct));
        buttons.Controls.Add(MakeButton("Delete", DeleteProduct));
        buttons.Controls.Add(MakeButton("Clear", ClearProductForm));
        buttons.Controls.Add(MakeButton("Refresh", LoadProducts));
        form.Controls.Add(new Label());
        form.Controls.Add(buttons);
        pageLayout.Controls.Add(form, 0, 0);

        var searchPanel = new FlowLayoutPanel
        {
            Dock = DockStyle.Fill,
            Padding = new Padding(0, 5, 0, 0),
            WrapContents = false
        };
        txtProductSearch.Width = 520;
        searchPanel.Controls.Add(new Label { Text = "Search", Width = 70, TextAlign = ContentAlignment.MiddleLeft });
        searchPanel.Controls.Add(txtProductSearch);
        searchPanel.Controls.Add(MakeButton("By Name", SearchProductByName));
        searchPanel.Controls.Add(MakeButton("By Barcode", SearchProductByBarcode));
        searchPanel.Controls.Add(MakeButton("All", LoadProducts));
        pageLayout.Controls.Add(searchPanel, 0, 1);

        dgvProducts.Dock = DockStyle.Fill;
        dgvProducts.ReadOnly = true;
        dgvProducts.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
        dgvProducts.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        dgvProducts.CellClick += (_, _) => SelectProductFromGrid();
        pageLayout.Controls.Add(dgvProducts, 0, 2);

        var note = new Label
        {
            Text = "Search uses linear search for product names and a custom hash table for barcode lookup.",
            Dock = DockStyle.Fill,
            TextAlign = ContentAlignment.MiddleLeft
        };
        pageLayout.Controls.Add(note, 0, 3);

        return tab;
    }

    private TabPage BuildSuppliersTab()
    {
        var tab = new TabPage("Suppliers");

        var pageLayout = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            RowCount = 3,
            ColumnCount = 1,
            Padding = new Padding(8)
        };
        pageLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 270));
        pageLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 42));
        pageLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
        tab.Controls.Add(pageLayout);

        var form = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 2,
            Padding = new Padding(10),
            AutoScroll = true
        };
        form.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 130));
        form.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));

        AddLabeledControl(form, "Supplier Code", txtSupplierCode);
        AddLabeledControl(form, "Name", txtSupplierName);
        AddLabeledControl(form, "Contact Person", txtContactPerson);
        AddLabeledControl(form, "Phone", txtSupplierPhone);
        AddLabeledControl(form, "Email", txtSupplierEmail);
        AddLabeledControl(form, "Address", txtSupplierAddress);

        var buttons = new FlowLayoutPanel
        {
            Dock = DockStyle.Fill,
            AutoSize = false,
            Height = 42,
            Padding = new Padding(0, 4, 0, 4),
            WrapContents = false
        };
        buttons.Controls.Add(MakeButton("Add", AddSupplier));
        buttons.Controls.Add(MakeButton("Update", UpdateSupplier));
        buttons.Controls.Add(MakeButton("Delete", DeleteSupplier));
        buttons.Controls.Add(MakeButton("Clear", ClearSupplierForm));
        buttons.Controls.Add(MakeButton("Refresh", LoadSuppliers));
        form.Controls.Add(new Label());
        form.Controls.Add(buttons);
        pageLayout.Controls.Add(form, 0, 0);

        var searchPanel = new FlowLayoutPanel
        {
            Dock = DockStyle.Fill,
            Padding = new Padding(0, 5, 0, 0),
            WrapContents = false
        };
        txtSupplierSearch.Width = 520;
        searchPanel.Controls.Add(new Label { Text = "Search", Width = 70, TextAlign = ContentAlignment.MiddleLeft });
        searchPanel.Controls.Add(txtSupplierSearch);
        searchPanel.Controls.Add(MakeButton("Search", SearchSupplier));
        searchPanel.Controls.Add(MakeButton("All", LoadSuppliers));
        pageLayout.Controls.Add(searchPanel, 0, 1);

        dgvSuppliers.Dock = DockStyle.Fill;
        dgvSuppliers.ReadOnly = true;
        dgvSuppliers.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
        dgvSuppliers.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        dgvSuppliers.CellClick += (_, _) => SelectSupplierFromGrid();
        pageLayout.Controls.Add(dgvSuppliers, 0, 2);

        return tab;
    }

    private TabPage BuildSalesTab()
    {
        var tab = new TabPage("Sales");
        var split = new SplitContainer { Dock = DockStyle.Fill, Orientation = Orientation.Horizontal, SplitterDistance = 260 };
        tab.Controls.Add(split);

        var top = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 2, Padding = new Padding(10) };
        top.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 55));
        top.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 45));

        var saleForm = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 2 };
        saleForm.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 120));
        saleForm.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
        AddLabeledControl(saleForm, "Product", cmbSaleProduct);
        numSaleQuantity.Minimum = 1; numSaleQuantity.Maximum = 10000; numSaleQuantity.Value = 1;
        AddLabeledControl(saleForm, "Quantity", numSaleQuantity);
        cmbPaymentMethod.Items.AddRange(new object[] { "Cash", "Card", "Mobile Payment" });
        cmbPaymentMethod.SelectedIndex = 0;
        AddLabeledControl(saleForm, "Payment", cmbPaymentMethod);
        lblSaleTotal.Text = "Total: 0.00";
        lblSaleTotal.Font = new Font(Font, FontStyle.Bold);
        AddLabeledControl(saleForm, "Sale Total", lblSaleTotal);

        var saleButtons = new FlowLayoutPanel { Dock = DockStyle.Fill };
        saleButtons.Controls.Add(MakeButton("Add To Cart", AddToCart));
        saleButtons.Controls.Add(MakeButton("Remove Item", RemoveCartItem));
        saleButtons.Controls.Add(MakeButton("Complete Sale", CompleteSale));
        saleButtons.Controls.Add(MakeButton("Clear Cart", ClearCart));
        saleForm.Controls.Add(new Label());
        saleForm.Controls.Add(saleButtons);
        top.Controls.Add(saleForm);

        dgvCart.Dock = DockStyle.Fill;
        dgvCart.ReadOnly = true;
        dgvCart.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
        dgvCart.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        top.Controls.Add(dgvCart);
        split.Panel1.Controls.Add(top);

        dgvSales.Dock = DockStyle.Fill;
        dgvSales.ReadOnly = true;
        dgvSales.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        split.Panel2.Controls.Add(dgvSales);

        return tab;
    }

    private TabPage BuildReportsTab()
    {
        var tab = new TabPage("Reports");
        var layout = new TableLayoutPanel { Dock = DockStyle.Fill, RowCount = 2, Padding = new Padding(10) };
        layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 50));
        layout.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
        tab.Controls.Add(layout);

        var buttons = new FlowLayoutPanel { Dock = DockStyle.Fill };
        buttons.Controls.Add(MakeButton("Low Stock", () => ShowReport(_reportService.GetLowStockReport())));
        buttons.Controls.Add(MakeButton("Sales By Product", () => ShowReport(_reportService.GetSalesByProductReport())));
        buttons.Controls.Add(MakeButton("Products By Category", () => ShowReport(_reportService.GetProductsByCategoryReport())));
        buttons.Controls.Add(MakeButton("Supplier Stock List", () => ShowReport(_reportService.GetSupplierStockListReport())));
        layout.Controls.Add(buttons);

        dgvReports.Dock = DockStyle.Fill;
        dgvReports.ReadOnly = true;
        dgvReports.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        layout.Controls.Add(dgvReports);

        return tab;
    }

    private static void AddLabeledControl(TableLayoutPanel panel, string label, Control control)
    {
        panel.RowStyles.Add(new RowStyle(SizeType.Absolute, 34));
        var lbl = new Label { Text = label, Dock = DockStyle.Fill, TextAlign = ContentAlignment.MiddleLeft };
        control.Dock = DockStyle.Fill;
        panel.Controls.Add(lbl);
        panel.Controls.Add(control);
    }

    private static Button MakeButton(string text, Action action)
    {
        var button = new Button { Text = text, Width = 110, Height = 30, Margin = new Padding(4) };
        button.Click += (_, _) => action();
        return button;
    }

    private void LoadAllData()
    {
        LoadCategoriesAndSuppliers();
        LoadProducts();
        LoadSuppliers();
        LoadSaleProducts();
        LoadSales();
        ShowReport(_reportService.GetLowStockReport());
    }

    private void LoadCategoriesAndSuppliers()
    {
        cmbCategory.DataSource = _categoryService.GetActiveCategories();
        cmbCategory.DisplayMember = "Name";
        cmbCategory.ValueMember = "Id";

        cmbSupplier.DataSource = _supplierService.GetActiveSuppliers();
        cmbSupplier.DisplayMember = "Name";
        cmbSupplier.ValueMember = "Id";
    }

    private void LoadProducts()
    {
        dgvProducts.DataSource = _productService.GetAllProducts().Select(p => new
        {
            p.Id,
            ProductID = p.ProductCode,
            p.Barcode,
            Product = p.Title,
            p.Brand,
            Category = p.Category?.Name,
            Supplier = p.Supplier?.Name,
            p.Price,
            p.QuantityInStock,
            p.LowStockThreshold,
            Status = p.StockAvailabilityStatus,
            p.ExpiryDate,
            p.RestockDate
        }).ToList();
        LoadSaleProducts();
    }

    private void LoadSuppliers()
    {
        dgvSuppliers.DataSource = _supplierService.GetActiveSuppliers().Select(s => new
        {
            s.Id,
            s.SupplierCode,
            s.Name,
            s.ContactPerson,
            s.Phone,
            s.Email,
            s.Address
        }).ToList();
        LoadCategoriesAndSuppliers();
    }


    private void SearchSupplier()
    {
        var keyword = txtSupplierSearch.Text.Trim().ToLowerInvariant();

        dgvSuppliers.DataSource = _supplierService.GetActiveSuppliers()
            .Where(s =>
                string.IsNullOrWhiteSpace(keyword) ||
                s.SupplierCode.ToLower().Contains(keyword) ||
                s.Name.ToLower().Contains(keyword) ||
                s.ContactPerson.ToLower().Contains(keyword) ||
                s.Phone.ToLower().Contains(keyword) ||
                s.Email.ToLower().Contains(keyword) ||
                s.Address.ToLower().Contains(keyword))
            .Select(s => new
            {
                s.Id,
                s.SupplierCode,
                s.Name,
                s.ContactPerson,
                s.Phone,
                s.Email,
                s.Address
            })
            .ToList();
    }

    private void LoadSaleProducts()
    {
        var products = _productService.GetAllProducts().Where(p => p.QuantityInStock > 0).ToList();
        cmbSaleProduct.DataSource = products;
        cmbSaleProduct.DisplayMember = "Title";
        cmbSaleProduct.ValueMember = "Id";
    }

    private void LoadSales()
    {
        dgvSales.DataSource = _saleService.GetRecentSales().Select(s => new
        {
            s.SaleNumber,
            s.SaleDate,
            Items = s.SaleItems.Count,
            s.PaymentMethod,
            s.TotalAmount
        }).ToList();
    }

    private void AddProduct()
    {
        var product = ReadProductForm();
        var result = _productService.AddProduct(product);
        ShowResult(result);
        if (result.Success)
        {
            ClearProductForm();
            LoadProducts();
        }
    }

    private void UpdateProduct()
    {
        if (_selectedProductId == null) { MessageBox.Show("Select a product first."); return; }
        var product = ReadProductForm();
        product.Id = _selectedProductId.Value;
        var result = _productService.UpdateProduct(product);
        ShowResult(result);
        if (result.Success)
        {
            ClearProductForm();
            LoadProducts();
        }
    }

    private void DeleteProduct()
    {
        if (_selectedProductId == null) { MessageBox.Show("Select a product first."); return; }
        var result = _productService.DeleteProduct(_selectedProductId.Value);
        ShowResult(result);
        if (result.Success)
        {
            ClearProductForm();
            LoadProducts();
        }
    }

    private Product ReadProductForm()
    {
        return new Product
        {
            ProductCode = txtProductCode.Text.Trim(),
            Barcode = txtBarcode.Text.Trim(),
            Title = txtTitle.Text.Trim(),
            Brand = txtBrand.Text.Trim(),
            CategoryId = cmbCategory.SelectedValue is int c ? c : 0,
            SupplierId = cmbSupplier.SelectedValue is int s ? s : 0,
            Price = numPrice.Value,
            QuantityInStock = (int)numQuantity.Value,
            LowStockThreshold = (int)numLowStock.Value,
            ExpiryDate = chkExpiry.Checked ? dtpExpiry.Value.Date : null,
            RestockDate = chkRestock.Checked ? dtpRestock.Value.Date : null
        };
    }

    private void SelectProductFromGrid()
    {
        if (dgvProducts.CurrentRow?.Cells["Id"].Value == null) return;
        _selectedProductId = Convert.ToInt32(dgvProducts.CurrentRow.Cells["Id"].Value);
        var product = _productService.GetById(_selectedProductId.Value);
        if (product == null) return;

        txtProductCode.Text = product.ProductCode;
        txtBarcode.Text = product.Barcode;
        txtTitle.Text = product.Title;
        txtBrand.Text = product.Brand;
        cmbCategory.SelectedValue = product.CategoryId;
        cmbSupplier.SelectedValue = product.SupplierId;
        numPrice.Value = product.Price;
        numQuantity.Value = product.QuantityInStock;
        numLowStock.Value = product.LowStockThreshold;
        chkExpiry.Checked = product.ExpiryDate.HasValue;
        if (product.ExpiryDate.HasValue) dtpExpiry.Value = product.ExpiryDate.Value;
        chkRestock.Checked = product.RestockDate.HasValue;
        if (product.RestockDate.HasValue) dtpRestock.Value = product.RestockDate.Value;
    }

    private void ClearProductForm()
    {
        _selectedProductId = null;
        txtProductCode.Clear();
        txtBarcode.Clear();
        txtTitle.Clear();
        txtBrand.Clear();
        numPrice.Value = 1;
        numQuantity.Value = 0;
        numLowStock.Value = 10;
        chkExpiry.Checked = false;
        chkRestock.Checked = false;
    }

    private void SearchProductByName()
    {
        var products = _productService.SearchByNameLinear(txtProductSearch.Text);
        dgvProducts.DataSource = products.Select(p => new
        {
            p.Id,
            ProductID = p.ProductCode,
            p.Barcode,
            Product = p.Title,
            p.Brand,
            Category = p.Category?.Name,
            Supplier = p.Supplier?.Name,
            p.Price,
            p.QuantityInStock,
            Status = p.StockAvailabilityStatus
        }).ToList();
    }

    private void SearchProductByBarcode()
    {
        var product = _productService.SearchByBarcodeHash(txtProductSearch.Text);
        var products = product == null ? new List<Product>() : new List<Product> { product };
        dgvProducts.DataSource = products.Select(p => new
        {
            p.Id,
            ProductID = p.ProductCode,
            p.Barcode,
            Product = p.Title,
            p.Brand,
            Category = p.Category?.Name,
            Supplier = p.Supplier?.Name,
            p.Price,
            p.QuantityInStock,
            Status = p.StockAvailabilityStatus
        }).ToList();
    }

    private void AddSupplier()
    {
        var result = _supplierService.AddSupplier(ReadSupplierForm());
        ShowResult(result);
        if (result.Success)
        {
            ClearSupplierForm();
            LoadSuppliers();
        }
    }

    private void UpdateSupplier()
    {
        if (_selectedSupplierId == null) { MessageBox.Show("Select a supplier first."); return; }
        var supplier = ReadSupplierForm();
        supplier.Id = _selectedSupplierId.Value;
        var result = _supplierService.UpdateSupplier(supplier);
        ShowResult(result);
        if (result.Success)
        {
            ClearSupplierForm();
            LoadSuppliers();
        }
    }

    private void DeleteSupplier()
    {
        if (_selectedSupplierId == null) { MessageBox.Show("Select a supplier first."); return; }
        var result = _supplierService.DeleteSupplier(_selectedSupplierId.Value);
        ShowResult(result);
        if (result.Success)
        {
            ClearSupplierForm();
            LoadSuppliers();
        }
    }

    private Supplier ReadSupplierForm()
    {
        return new Supplier
        {
            SupplierCode = txtSupplierCode.Text.Trim(),
            Name = txtSupplierName.Text.Trim(),
            ContactPerson = txtContactPerson.Text.Trim(),
            Phone = txtSupplierPhone.Text.Trim(),
            Email = txtSupplierEmail.Text.Trim(),
            Address = txtSupplierAddress.Text.Trim()
        };
    }

    private void SelectSupplierFromGrid()
    {
        if (dgvSuppliers.CurrentRow?.Cells["Id"].Value == null) return;
        _selectedSupplierId = Convert.ToInt32(dgvSuppliers.CurrentRow.Cells["Id"].Value);
        var supplier = _supplierService.GetById(_selectedSupplierId.Value);
        if (supplier == null) return;

        txtSupplierCode.Text = supplier.SupplierCode;
        txtSupplierName.Text = supplier.Name;
        txtContactPerson.Text = supplier.ContactPerson;
        txtSupplierPhone.Text = supplier.Phone;
        txtSupplierEmail.Text = supplier.Email;
        txtSupplierAddress.Text = supplier.Address;
    }

    private void ClearSupplierForm()
    {
        _selectedSupplierId = null;
        txtSupplierCode.Clear();
        txtSupplierName.Clear();
        txtContactPerson.Clear();
        txtSupplierPhone.Clear();
        txtSupplierEmail.Clear();
        txtSupplierAddress.Clear();
    }

    private void AddToCart()
    {
        if (cmbSaleProduct.SelectedItem is not Product product) return;
        int quantity = (int)numSaleQuantity.Value;
        var existing = _cart.FirstOrDefault(c => c.ProductId == product.Id);
        if (existing == null)
        {
            _cart.Add(new CartLine(product.Id, product.Title, product.Price, quantity));
        }
        else
        {
            existing.Quantity += quantity;
        }
        RefreshCart();
    }

    private void RemoveCartItem()
    {
        if (dgvCart.CurrentRow?.Cells["ProductId"].Value == null) return;
        int productId = Convert.ToInt32(dgvCart.CurrentRow.Cells["ProductId"].Value);
        var item = _cart.FirstOrDefault(c => c.ProductId == productId);
        if (item != null) _cart.Remove(item);
        RefreshCart();
    }

    private void CompleteSale()
    {
        var requests = _cart.Select(c => new SaleItemRequest { ProductId = c.ProductId, Quantity = c.Quantity }).ToList();
        var result = _saleService.RecordSale(requests, cmbPaymentMethod.Text);
        ShowResult(result);
        if (result.Success)
        {
            _cart.Clear();
            RefreshCart();
            LoadProducts();
            LoadSales();
        }
    }

    private void ClearCart()
    {
        _cart.Clear();
        RefreshCart();
    }

    private void RefreshCart()
    {
        dgvCart.DataSource = _cart.Select(c => new
        {
            c.ProductId,
            c.Product,
            c.UnitPrice,
            c.Quantity,
            LineTotal = c.LineTotal
        }).ToList();
        lblSaleTotal.Text = $"Total: {_cart.Sum(c => c.LineTotal):0.00}";
    }

    private void ShowReport(object reportData)
    {
        dgvReports.DataSource = reportData;
    }

    private static void ShowResult(OperationResult result)
    {
        MessageBox.Show(result.Message, result.Success ? "Success" : "Validation", MessageBoxButtons.OK,
            result.Success ? MessageBoxIcon.Information : MessageBoxIcon.Warning);
    }

    protected override void OnFormClosed(FormClosedEventArgs e)
    {
        _db.Dispose();
        base.OnFormClosed(e);
    }

    private sealed class CartLine
    {
        public int ProductId { get; }
        public string Product { get; }
        public decimal UnitPrice { get; }
        public int Quantity { get; set; }
        public decimal LineTotal => UnitPrice * Quantity;

        public CartLine(int productId, string product, decimal unitPrice, int quantity)
        {
            ProductId = productId;
            Product = product;
            UnitPrice = unitPrice;
            Quantity = quantity;
        }
    }
}
