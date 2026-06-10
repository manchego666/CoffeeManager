using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using CoffeeManager.Models.Class;
using CoffeeManager.Front.Styles;

namespace CoffeeManager.Front
{
    public class ProductModule : Panel
    {
        private readonly Store _store;
        private readonly DataGridView grid;
        private Panel overlay = null!;

        public ProductModule(Store store)
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
                Text = "Productos",
                Font = new Font("Segoe UI", 28, FontStyle.Bold),
                ForeColor = Color.FromArgb(40, 40, 40),
                AutoSize = true,
                Location = new Point(10, 10)
            };
            card.Controls.Add(lblTitle);

            var btnAdd = new Button
            {
                Text = "Agregar producto",
                Size = new Size(180, 40),
                Location = new Point(10, 60),
                BackColor = Color.FromArgb(240, 128, 148),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };
            btnAdd.FlatAppearance.BorderSize = 0;
            btnAdd.Click += (s, e) => ShowProductModal(null);
            card.Controls.Add(btnAdd);

            grid = new DataGridView
            {
                Location = new Point(10, 120),
                Size = new Size(1100, 520),
                ReadOnly = true,
                AllowUserToAddRows = false
            };
            DataGridStyler.Apply(grid);
            card.Controls.Add(grid);

            BuildColumns();
            LoadData();

            grid.CellClick += Grid_CellClick;
        }

        private void BuildColumns()
        {
            grid.Columns.Clear();

            grid.Columns.Add("Id", "ID");
            grid.Columns.Add("Name", "Nombre");
            grid.Columns.Add("Price", "Precio");
            grid.Columns.Add("Stock", "Stock");
            grid.Columns.Add("Type", "Tipo");
            grid.Columns.Add("Ingredients", "Ingredientes");

            var editBtn = new DataGridViewButtonColumn
            {
                HeaderText = "Editar",
                Text = "✎",
                UseColumnTextForButtonValue = true,
                Width = 60
            };
            grid.Columns.Add(editBtn);

            var deleteBtn = new DataGridViewButtonColumn
            {
                HeaderText = "Eliminar",
                Text = "🗑",
                UseColumnTextForButtonValue = true,
                Width = 60
            };
            grid.Columns.Add(deleteBtn);
        }

        private void LoadData()
        {
            grid.Rows.Clear();

            foreach (var p in _store.Products)
            {
                string tipo = p.UsesWarehouse ? "Receta" : "Stock";
                string ingredientes = p.UsesWarehouse
                    ? string.Join(", ", p.Recipe.Select(r => $"{r.IngredientName} x{r.Quantity}"))
                    : "No aplica";

                grid.Rows.Add(
                    p.Id,
                    p.Name,
                    p.Price.ToString("F2"),
                    p.Stock,
                    tipo,
                    ingredientes
                );
            }
        }

        private void Grid_CellClick(object? sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;

            int id = Convert.ToInt32(grid.Rows[e.RowIndex].Cells[0].Value);
            var product = _store.GetProduct(id);

            if (e.ColumnIndex == 6)
                ShowProductModal(product);

            else if (e.ColumnIndex == 7)
            {
                if (MessageBox.Show("¿Eliminar producto?", "Confirmar",
                    MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
                {
                    _store.Products.Remove(product);
                    LoadData();
                }
            }
        }

        private void ShowProductModal(Product? product)
        {
            overlay = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.FromArgb(120, 0, 0, 0)
            };
            Controls.Add(overlay);
            overlay.BringToFront();

            var modal = new ProductModal(_store, product);

            modal.Left = (overlay.Width - modal.Width) / 2;
            modal.Top = (overlay.Height - modal.Height) / 2;

            overlay.Controls.Add(modal);

            modal.OnClose += () =>
            {
                Controls.Remove(overlay);
                LoadData();
            };
        }
    }
}
    