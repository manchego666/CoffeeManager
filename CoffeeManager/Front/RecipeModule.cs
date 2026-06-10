using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using CoffeeManager.Models.Class;

namespace CoffeeManager.Front
{
    internal class RecipeModule : UserControl
    {
        private readonly Store _store;
        private Panel panelRoot = null!;
        private Panel panelCard = null!;
        private DataGridView grid = null!;
        private Panel overlay = null!;

        public RecipeModule(Store store)
        {
            _store = store;

            DoubleBuffered = true;
            BackColor = Color.FromArgb(1, 0, 0, 0);

            InitializeLayout();
            LoadRecipes();
        }

        private void InitializeLayout()
        {
            panelRoot = new Panel
            {
                Dock = DockStyle.Fill,
                AutoScroll = true,
                BackColor = Color.FromArgb(1, 0, 0, 0),
                Padding = new Padding(0, 20, 0, 20)
            };
            Controls.Add(panelRoot);

            panelCard = new Panel
            {
                Size = new Size(900, 500),
                BackColor = Color.FromArgb(235, 255, 255, 255),
                Padding = new Padding(25)
            };
            panelRoot.Controls.Add(panelCard);

            panelRoot.Resize += (s, e) =>
            {
                panelCard.Left = (panelRoot.ClientSize.Width - panelCard.Width) / 2;
            };
            panelCard.Left = (panelRoot.ClientSize.Width - panelCard.Width) / 2;
            panelCard.Top = 40;

            var lblTitle = new Label
            {
                Text = "Recetas",
                Font = new Font("Segoe UI", 20, FontStyle.Bold),
                ForeColor = Color.FromArgb(40, 40, 40),
                AutoSize = true,
                Location = new Point(10, 5)
            };
            panelCard.Controls.Add(lblTitle);

            var btnAdd = new Button
            {
                Text = "Agregar receta",
                Size = new Size(160, 35),
                Location = new Point(10, 50),
                BackColor = Color.FromArgb(70, 180, 160),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };
            btnAdd.FlatAppearance.BorderSize = 0;
            btnAdd.Click += (s, e) => ShowSelectProductModal();
            panelCard.Controls.Add(btnAdd);

            grid = new DataGridView
            {
                Location = new Point(10, 95),
                Size = new Size(850, 370),
                ReadOnly = true,
                AllowUserToAddRows = false,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                RowHeadersVisible = false,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect
            };
            panelCard.Controls.Add(grid);

            grid.Columns.Add("Id", "ID");
            grid.Columns.Add("Name", "Producto");
            grid.Columns.Add("Ingredients", "Ingredientes");

            var editBtn = new DataGridViewButtonColumn
            {
                HeaderText = "Editar",
                Text = "✎",
                UseColumnTextForButtonValue = true,
                Width = 60
            };
            grid.Columns.Add(editBtn);

            grid.CellClick += Grid_CellClick;
        }

        private void LoadRecipes()
        {
            grid.Rows.Clear();

            foreach (var p in _store.Products.Where(p => p.UsesWarehouse))
            {
                string ing = p.Recipe.Count == 0
                    ? "(Sin ingredientes)"
                    : string.Join(", ",
                        p.Recipe.Select(r => $"{r.IngredientName} ({r.Quantity}{r.Unit})"));

                grid.Rows.Add(p.Id, p.Name, ing);
            }
        }

        private void Grid_CellClick(object? sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;

            if (e.ColumnIndex == 3)
            {
                int id = Convert.ToInt32(grid.Rows[e.RowIndex].Cells[0].Value);
                ShowEditModal(id);
            }
        }

        private void ShowEditModal(int id)
        {
            var product = _store.GetProduct(id);
            if (product == null) return;

            ShowRecipeModal(product);
        }

        private void ShowRecipeModal(Product product)
        {
            overlay = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.FromArgb(120, 0, 0, 0)
            };
            Controls.Add(overlay);
            overlay.BringToFront();

            var modal = new RecipeModal(product, _store);

            modal.Left = (overlay.Width - modal.Width) / 2;
            modal.Top = (overlay.Height - modal.Height) / 2;

            overlay.Controls.Add(modal);

            modal.OnClose += () =>
            {
                Controls.Remove(overlay);
                LoadRecipes();
            };
        }

        private void ShowSelectProductModal()
        {
            overlay = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.FromArgb(120, 0, 0, 0)
            };
            Controls.Add(overlay);
            overlay.BringToFront();

            var modal = new SelectProductForRecipeModal(_store);

            modal.Left = (overlay.Width - modal.Width) / 2;
            modal.Top = (overlay.Height - modal.Height) / 2;

            overlay.Controls.Add(modal);

            modal.OnProductSelected += (product) =>
            {
                Controls.Remove(overlay);
                if (product == null) return;

                if (!product.UsesWarehouse)
                {
                    MessageBox.Show(
                        "Este producto no usa almacén. Activa 'UsesWarehouse' para poder definir receta.",
                        "Aviso",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information);
                    return;
                }

                ShowRecipeModal(product);
            };
        }
    }
}
