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
        private PictureBox picProduct = null!;
        private DataGridView grid = null!;
        private Button btnAdd = null!;
        private Button btnEdit = null!;
        private Button btnDelete = null!;
        private Button btnClose = null!;

        public RecipeModal(Product product, Store store)
        {
            _product = product;
            _store = store;

            DoubleBuffered = true;
            Size = new Size(750, 500);
            BackColor = Color.Transparent;

            InitializeLayout();
            LoadData();
        }

        private void InitializeLayout()
        {
            panelCard = new Panel
            {
                Size = new Size(750, 500),
                BackColor = Color.FromArgb(235, 255, 255, 255),
                Padding = new Padding(20)
            };
            Controls.Add(panelCard);

            var lblTitle = new Label
            {
                Text = $"Receta: {_product.Name}",
                Font = new Font("Segoe UI", 18, FontStyle.Bold),
                ForeColor = Color.FromArgb(40, 40, 40),
                AutoSize = true,
                Location = new Point(10, 5)
            };
            panelCard.Controls.Add(lblTitle);

            // FOTO DEL PRODUCTO
            picProduct = new PictureBox
            {
                Size = new Size(150, 150),
                Location = new Point(560, 40),
                SizeMode = PictureBoxSizeMode.Zoom,
                BackColor = Color.LightGray
            };
            panelCard.Controls.Add(picProduct);

            if (!string.IsNullOrWhiteSpace(_product.ImagePath) &&
                System.IO.File.Exists(_product.ImagePath))
            {
                picProduct.Image = Image.FromFile(_product.ImagePath);
            }

            // GRID DE INGREDIENTES
            grid = new DataGridView
            {
                Location = new Point(10, 60),
                Size = new Size(530, 330),
                BackgroundColor = Color.White,
                BorderStyle = BorderStyle.None,
                AllowUserToAddRows = false,
                ReadOnly = true,
                RowHeadersVisible = false,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect
            };

            grid.Columns.Add("Ingredient", "Ingrediente");
            grid.Columns.Add("Qty", "Cantidad");
            grid.Columns.Add("Unit", "Unidad");
            grid.Columns.Add("Optional", "Opcional");

            panelCard.Controls.Add(grid);

            // BOTÓN AGREGAR
            btnAdd = new Button
            {
                Text = "Agregar",
                Size = new Size(120, 35),
                Location = new Point(10, 410),
                BackColor = Color.FromArgb(70, 180, 160),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };
            btnAdd.FlatAppearance.BorderSize = 0;
            btnAdd.Click += AddIngredient;
            panelCard.Controls.Add(btnAdd);

            // BOTÓN EDITAR
            btnEdit = new Button
            {
                Text = "Editar",
                Size = new Size(120, 35),
                Location = new Point(140, 410),
                BackColor = Color.FromArgb(240, 180, 80),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };
            btnEdit.FlatAppearance.BorderSize = 0;
            btnEdit.Click += EditIngredient;
            panelCard.Controls.Add(btnEdit);

            // BOTÓN ELIMINAR
            btnDelete = new Button
            {
                Text = "Eliminar",
                Size = new Size(120, 35),
                Location = new Point(270, 410),
                BackColor = Color.FromArgb(220, 60, 60),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };
            btnDelete.FlatAppearance.BorderSize = 0;
            btnDelete.Click += DeleteIngredient;
            panelCard.Controls.Add(btnDelete);

            // BOTÓN CERRAR
            btnClose = new Button
            {
                Text = "Cerrar",
                Size = new Size(120, 35),
                Location = new Point(600, 410),
                BackColor = Color.FromArgb(120, 120, 120),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };
            btnClose.FlatAppearance.BorderSize = 0;
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
            ShowIngredientModal(null);
        }

        private void EditIngredient(object? sender, EventArgs e)
        {
            if (grid.SelectedRows.Count == 0) return;

            int index = grid.SelectedRows[0].Index;
            var item = _product.Recipe[index];

            ShowIngredientModal(item);
        }

        private void DeleteIngredient(object? sender, EventArgs e)
        {
            if (grid.SelectedRows.Count == 0) return;

            int index = grid.SelectedRows[0].Index;

            if (MessageBox.Show("¿Eliminar ingrediente?", "Confirmar",
                MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
            {
                _product.Recipe.RemoveAt(index);
                LoadData();
            }
        }

        private void ShowIngredientModal(RecipeItem? item)
        {
            var overlay = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.FromArgb(120, 0, 0, 0)
            };
            Parent.Controls.Add(overlay);
            overlay.BringToFront();

            var modal = new RecipeIngredientModal(_store, item);

            modal.Left = (overlay.Width - modal.Width) / 2;
            modal.Top = (overlay.Height - modal.Height) / 2;

            overlay.Controls.Add(modal);

            modal.OnClose += (result) =>
            {
                Parent.Controls.Remove(overlay);

                if (result != null)
                {
                    if (item == null)
                    {
                        // AGREGAR
                        _product.Recipe.Add(result);
                    }
                    else
                    {
                        // EDITAR
                        item.IngredientId = result.IngredientId;
                        item.IngredientName = result.IngredientName;
                        item.Quantity = result.Quantity;
                        item.Unit = result.Unit;
                        item.IsOptional = result.IsOptional;
                    }
                }

                LoadData();
            };
        }
    }
}
