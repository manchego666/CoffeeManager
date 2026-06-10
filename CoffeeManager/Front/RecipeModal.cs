using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using CoffeeManager.Models.Class;

namespace CoffeeManager.Front
{
    internal class RecipeModal : UserControl
    {
        private readonly Product _product;
        private readonly Store _store;

        public event Action? OnClose;

        private Panel panelCard = null!;
        private DataGridView grid = null!;
        private Button btnAdd = null!;
        private Button btnClose = null!;

        public RecipeModal(Product product, Store store)
        {
            _product = product;
            _store = store;

            DoubleBuffered = true;
            Size = new Size(700, 450);
            BackColor = Color.Transparent;

            InitializeLayout();
            LoadData();
        }

        private void InitializeLayout()
        {
            panelCard = new Panel
            {
                Size = new Size(700, 450),
                BackColor = Color.FromArgb(235, 255, 255, 255),
                Padding = new Padding(20)
            };
            Controls.Add(panelCard);

            var lblTitle = new Label
            {
                Text = $"Receta: {_product.Name}",
                Font = new Font("Segoe UI", 16, FontStyle.Bold),
                ForeColor = Color.FromArgb(40, 40, 40),
                AutoSize = true,
                Location = new Point(10, 5)
            };
            panelCard.Controls.Add(lblTitle);

            grid = new DataGridView
            {
                Location = new Point(10, 50),
                Size = new Size(660, 300),
                BackgroundColor = Color.White,
                BorderStyle = BorderStyle.None,
                AllowUserToAddRows = false,
                ReadOnly = true,
                RowHeadersVisible = false,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
            };

            grid.Columns.Add("Ingredient", "Ingrediente");
            grid.Columns.Add("Qty", "Cantidad");
            grid.Columns.Add("Unit", "Unidad");
            grid.Columns.Add("Optional", "Opcional");

            panelCard.Controls.Add(grid);

            btnAdd = new Button
            {
                Text = "Agregar ingrediente",
                Size = new Size(160, 35),
                Location = new Point(10, 370),
                BackColor = Color.FromArgb(240, 128, 148),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };
            btnAdd.Click += AddIngredient;
            panelCard.Controls.Add(btnAdd);

            btnClose = new Button
            {
                Text = "Cerrar",
                Size = new Size(120, 35),
                Location = new Point(550, 370),
                BackColor = Color.FromArgb(220, 60, 60),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };
            btnClose.Click += (s, e) => OnClose?.Invoke();
            panelCard.Controls.Add(btnClose);
        }

        private void LoadData()
        {
            grid.Rows.Clear();

            foreach (var r in _product.Recipe)
            {
                grid.Rows.Add(
                    r.IngredientName,
                    r.Quantity,
                    r.Unit.ToString(),
                    r.IsOptional ? "Sí" : "No"
                );
            }
        }

        private void AddIngredient(object? sender, EventArgs e)
        {
            var overlay = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.FromArgb(120, 0, 0, 0)
            };
            Parent.Controls.Add(overlay);
            overlay.BringToFront();

            var modal = new RecipeIngredientModal(_store, null);

            modal.Left = (overlay.Width - modal.Width) / 2;
            modal.Top = (overlay.Height - modal.Height) / 2;

            overlay.Controls.Add(modal);

            modal.OnClose += (item) =>
            {
                Parent.Controls.Remove(overlay);

                if (item != null)
                    _product.Recipe.Add(item);

                LoadData();
            };
        }
    }
}
