using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using CoffeeManager.Models.Class;

namespace CoffeeManager.Front
{
    internal class SelectProductForRecipeModal : UserControl
    {
        private readonly Store _store;

        public event Action<Product?>? OnProductSelected;

        private Panel panelCard = null!;
        private ComboBox cmbProducts = null!;
        private Button btnOk = null!;
        private Button btnCancel = null!;

        public SelectProductForRecipeModal(Store store)
        {
            _store = store;

            DoubleBuffered = true;
            Size = new Size(500, 220);
            BackColor = Color.Transparent;

            InitializeLayout();
            LoadProducts();
        }

        private void InitializeLayout()
        {
            panelCard = new Panel
            {
                Size = new Size(480, 200),
                BackColor = Color.FromArgb(235, 255, 255, 255),
                Padding = new Padding(20)
            };
            Controls.Add(panelCard);

            var lblTitle = new Label
            {
                Text = "Crear / editar receta",
                Font = new Font("Segoe UI", 16, FontStyle.Bold),
                ForeColor = Color.FromArgb(40, 40, 40),
                AutoSize = true,
                Location = new Point(10, 5)
            };
            panelCard.Controls.Add(lblTitle);

            var lblProduct = new Label
            {
                Text = "Seleccione el producto:",
                Location = new Point(10, 55),
                AutoSize = true
            };
            panelCard.Controls.Add(lblProduct);

            cmbProducts = new ComboBox
            {
                Location = new Point(10, 80),
                Width = 440,
                DropDownStyle = ComboBoxStyle.DropDownList,
                Font = new Font("Segoe UI", 10)
            };
            panelCard.Controls.Add(cmbProducts);

            btnOk = new Button
            {
                Text = "Continuar",
                Size = new Size(140, 35),
                Location = new Point(190, 130),
                BackColor = Color.FromArgb(70, 180, 160),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };
            btnOk.FlatAppearance.BorderSize = 0;
            btnOk.Click += Confirm;
            panelCard.Controls.Add(btnOk);

            btnCancel = new Button
            {
                Text = "Cancelar",
                Size = new Size(140, 35),
                Location = new Point(330, 130),
                BackColor = Color.FromArgb(220, 60, 60),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };
            btnCancel.FlatAppearance.BorderSize = 0;
            btnCancel.Click += (s, e) => OnProductSelected?.Invoke(null);
            panelCard.Controls.Add(btnCancel);
        }

        private void LoadProducts()
        {
            // Solo productos que pueden tener receta (UsesWarehouse true)
            var products = _store.Products
                .Where(p => p.UsesWarehouse)
                .OrderBy(p => p.Name)
                .ToList();

            cmbProducts.Items.Clear();

            foreach (var p in products)
            {
                cmbProducts.Items.Add(new ProductComboItem(p));
            }

            if (cmbProducts.Items.Count > 0)
                cmbProducts.SelectedIndex = 0;
        }

        private void Confirm(object? sender, EventArgs e)
        {
            if (cmbProducts.SelectedItem is not ProductComboItem item)
            {
                MessageBox.Show("Seleccione un producto.");
                return;
            }

            OnProductSelected?.Invoke(item.Product);
        }

        private class ProductComboItem
        {
            public Product Product { get; }

            public ProductComboItem(Product product)
            {
                Product = product;
            }

            public override string ToString()
            {
                return $"{Product.Name} — {Product.Price:C}";
            }
        }
    }
}
