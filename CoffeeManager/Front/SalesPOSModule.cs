using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using CoffeeManager.Models.Class;
using CoffeeManager.Models.Enums;
using CoffeeManager.Front.Styles;
using CoffeeManager.Services;

namespace CoffeeManager.Front
{
    public class SalesPOSModule : Panel
    {
        private readonly Store _store;
        private readonly JsonService _json = new();

        private ComboBox cmbProducts;
        private NumericUpDown numQty;
        private Button btnAdd;
        private Button btnPay;

        private DataGridView grid;
        private Label lblTotal;

        private readonly List<SaleDetail> _ticket = new();

        public SalesPOSModule(Store store)
        {
            _store = store;

            Dock = DockStyle.Fill;
            BackgroundImage = Properties.Resources.bg_main;
            BackgroundImageLayout = ImageLayout.Stretch;

            var card = new CardPanel
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(20)
            };
            Controls.Add(card);

            var lblTitle = new Label
            {
                Text = "Punto de Venta",
                Font = new Font("Segoe UI", 28, FontStyle.Bold),
                ForeColor = Color.FromArgb(40, 40, 40),
                AutoSize = true,
                Location = new Point(10, 10)
            };
            card.Controls.Add(lblTitle);

            var top = new Panel
            {
                Location = new Point(10, 80),
                Size = new Size(1100, 80),
                BackColor = Color.Transparent
            };
            card.Controls.Add(top);

            cmbProducts = new ComboBox
            {
                Width = 300,
                DropDownStyle = ComboBoxStyle.DropDownList,
                Font = new Font("Segoe UI", 12)
            };
            top.Controls.Add(cmbProducts);

            numQty = new NumericUpDown
            {
                Width = 80,
                Minimum = 1,
                Maximum = 99,
                Font = new Font("Segoe UI", 12),
                Location = new Point(320, 0)
            };
            top.Controls.Add(numQty);

            btnAdd = new Button
            {
                Text = "Agregar",
                Width = 150,
                Height = 40,
                Location = new Point(420, 0),
                BackColor = Color.FromArgb(70, 130, 180),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 11, FontStyle.Bold)
            };
            btnAdd.FlatAppearance.BorderSize = 0;
            btnAdd.Click += BtnAdd_Click;
            top.Controls.Add(btnAdd);

            grid = new DataGridView
            {
                Location = new Point(10, 170),
                Size = new Size(1100, 420),
                ReadOnly = true,
                AllowUserToAddRows = false
            };
            DataGridStyler.Apply(grid);
            card.Controls.Add(grid);

            BuildColumns();

            lblTotal = new Label
            {
                Text = "Total: $0.00",
                Font = new Font("Segoe UI", 26, FontStyle.Bold),
                ForeColor = Color.FromArgb(40, 40, 40),
                AutoSize = true,
                Location = new Point(10, 600)
            };
            card.Controls.Add(lblTotal);

            btnPay = new Button
            {
                Text = "Cobrar",
                Width = 200,
                Height = 50,
                Location = new Point(900, 600),
                BackColor = Color.FromArgb(70, 180, 120),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 14, FontStyle.Bold)
            };
            btnPay.FlatAppearance.BorderSize = 0;
            btnPay.Click += BtnPay_Click;
            card.Controls.Add(btnPay);

            LoadProducts();
        }

        private void BuildColumns()
        {
            grid.Columns.Clear();

            grid.Columns.Add("Name", "Producto");
            grid.Columns.Add("Qty", "Cantidad");
            grid.Columns.Add("Price", "Precio");
            grid.Columns.Add("Total", "Subtotal");
        }

        private void LoadProducts()
        {
            cmbProducts.Items.Clear();
            foreach (var p in _store.Products)
                cmbProducts.Items.Add(p.Name);
        }

        private void BtnAdd_Click(object? sender, EventArgs e)
        {
            if (cmbProducts.SelectedItem == null) return;

            var product = _store.Products.First(x => x.Name == cmbProducts.SelectedItem.ToString());
            int qty = (int)numQty.Value;

            //  VALIDACIÓN 1: NO VENDER SIN RECETA
            if (product.UsesWarehouse && product.Recipe.Count == 0)
            {
                MessageBox.Show(
                    $"El producto '{product.Name}' no tiene receta definida. No se puede vender hasta configurarla. (╥﹏╥)",
                    "Receta faltante",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                return;
            }

            //  VALIDACIÓN 2: INGREDIENTES EXISTEN Y HAY STOCK
            foreach (var ing in product.Recipe)
            {
                var item = _store.Warehouse.GetItemById(ing.IngredientId);

                if (item == null && !ing.IsOptional)
                {
                    MessageBox.Show(
                        $"El ingrediente '{ing.IngredientName}' no existe en el almacén. (╥﹏╥)",
                        "Ingrediente faltante",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning);
                    return;
                }

                if (item != null)
                {
                    decimal needed = UnitConverter.Convert(
                        ing.Unit,
                        item.Unit,
                        ing.Quantity * qty
                    );

                    if (item.Quantity < needed && !ing.IsOptional)
                    {
                        MessageBox.Show(
                            $"No hay suficiente '{ing.IngredientName}' para preparar {qty} unidades. (╥﹏╥)",
                            "Stock insuficiente",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Warning);
                        return;
                    }
                }
            }

            var detail = new SaleDetail
            {
                Product = product,
                Quantity = qty
            };

            _ticket.Add(detail);

            grid.Rows.Add(
                detail.Product.Name,
                detail.Quantity,
                detail.Product.Price.ToString("F2"),
                detail.CalculateSubtotal().ToString("F2")
            );

            UpdateTotal();
        }

        private void UpdateTotal()
        {
            decimal total = _ticket.Sum(x => x.CalculateSubtotal());
            lblTotal.Text = $"Total: ${total:F2}";
        }

        private void BtnPay_Click(object? sender, EventArgs e)
        {
            if (_ticket.Count == 0) return;

            var sale = new Sale
            {
                Id = new Random().Next(100000, 999999),
                Date = DateTime.Now,
                Employee = _store.Employees.FirstOrDefault() ?? new Employee(),
                Details = _ticket.ToList()
            };

            sale.ApplyToStore(_store);

            _store.Sales.Add(sale);

            _json.Save<Sale>(PathService.Sales, _store.Sales);
            _json.Save<Product>(PathService.Products, _store.Products);
            _json.SaveObject<Warehouse>(PathService.Warehouse, _store.Warehouse);

            MessageBox.Show("Venta registrada correctamente (≧◡≦)", "Éxito",
                MessageBoxButtons.OK, MessageBoxIcon.Information);

            _ticket.Clear();
            grid.Rows.Clear();
            UpdateTotal();
        }
    }
}
