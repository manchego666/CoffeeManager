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
        private Panel panelCard = null!;
        private DataGridView grid = null!;
        private Panel overlay = null!;

        public RecipeModule(Store store)
        {
            _store = store;

            DoubleBuffered = true;
            InitializeLayout();
        }

        private void InitializeLayout()
        {
            BackColor = Color.Transparent;

            panelCard = new Panel
            {
                Size = new Size(900, 500),
                BackColor = Color.FromArgb(235, 255, 255, 255),
                Padding = new Padding(25)
            };
            Controls.Add(panelCard);

            var lblTitle = new Label
            {
                Text = "Recetas",
                Font = new Font("Segoe UI", 20, FontStyle.Bold),
                ForeColor = Color.FromArgb(40, 40, 40),
                AutoSize = true,
                Location = new Point(10, 5)
            };
            panelCard.Controls.Add(lblTitle);

            grid = new DataGridView
            {
                Location = new Point(10, 60),
                Size = new Size(860, 400),
                ReadOnly = true,
                AllowUserToAddRows = false,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
            };
            panelCard.Controls.Add(grid);

            grid.Columns.Add("Id", "ID");
            grid.Columns.Add("Name", "Producto");
            grid.Columns.Add("Ingredients", "Ingredientes");

            var editBtn = new DataGridViewButtonColumn
            {
                HeaderText = "Editar",
                Text = "✎",
                UseColumnTextForButtonValue = true
            };
            grid.Columns.Add(editBtn);

            grid.CellClick += Grid_CellClick;

            LoadRecipes();
        }

        private void LoadRecipes()
        {
            grid.Rows.Clear();

            foreach (var p in _store.Products.Where(p => p.UsesWarehouse))
            {
                string ing = string.Join(", ",
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
    }
}
