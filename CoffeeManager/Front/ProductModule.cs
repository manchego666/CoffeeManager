using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using CoffeeManager.Models.Class;

namespace CoffeeManager.Front
{
    internal class ProductModule : UserControl
    {
        private readonly Store _store;

        private Panel panelRoot = null!;
        private Panel panelCard = null!;
        private DataGridView grid = null!;
        private Button btnAdd = null!;
        private Panel overlay = null!;

        public ProductModule(Store store)
        {
            _store = store;

            DoubleBuffered = true;
            BackColor = Color.FromArgb(1, 0, 0, 0);

            InitializeLayout();
            LoadProducts();
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
                Size = new Size(900, 520),
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
                Text = "Productos",
                Font = new Font("Segoe UI", 20, FontStyle.Bold),
                ForeColor = Color.FromArgb(40, 40, 40),
                AutoSize = true,
                Location = new Point(10, 5)
            };
            panelCard.Controls.Add(lblTitle);

            btnAdd = new Button
            {
                Text = "Agregar producto",
                Size = new Size(180, 35),
                Location = new Point(10, 50),
                BackColor = Color.FromArgb(70, 180, 160),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };
            btnAdd.FlatAppearance.BorderSize = 0;
            btnAdd.Click += (s, e) => ShowProductModal(null);
            panelCard.Controls.Add(btnAdd);

            grid = new DataGridView
            {
                Location = new Point(10, 95),
                Size = new Size(850, 380),
                ReadOnly = true,
                AllowUserToAddRows = false,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                RowHeadersVisible = false,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect
            };
            panelCard.Controls.Add(grid);

            grid.Columns.Add("Id", "ID");

            var imgCol = new DataGridViewImageColumn
            {
                HeaderText = "Foto",
                Width = 60,
                ImageLayout = DataGridViewImageCellLayout.Zoom
            };
            grid.Columns.Add(imgCol);

            grid.Columns.Add("Name", "Nombre");
            grid.Columns.Add("Price", "Precio");
            grid.Columns.Add("Categories", "Categorías");

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

        private void LoadProducts()
        {
            grid.Rows.Clear();

            foreach (var p in _store.Products)
            {
                Image? img = null;

                if (!string.IsNullOrWhiteSpace(p.ImagePath) &&
                    System.IO.File.Exists(p.ImagePath))
                {
                    using var temp = Image.FromFile(p.ImagePath);
                    img = new Bitmap(temp, new Size(50, 50));
                }

                string cats = p.Categories.Count == 0
                    ? "(Sin categorías)"
                    : string.Join(", ", p.Categories);

                grid.Rows.Add(
                    p.Id,
                    img,
                    p.Name,
                    $"${p.Price}",
                    cats
                );
            }
        }

        private void Grid_CellClick(object? sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;

            if (e.ColumnIndex == 5)
            {
                int id = Convert.ToInt32(grid.Rows[e.RowIndex].Cells[0].Value);
                var product = _store.GetProduct(id);
                if (product != null)
                    ShowProductModal(product);
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

            var modal = new ProductModal(_store, product ?? new Product());

            modal.Left = (overlay.Width - modal.Width) / 2;
            modal.Top = (overlay.Height - modal.Height) / 2;

            overlay.Controls.Add(modal);

            modal.OnClose += () =>
            {
                if (product == null)
                    _store.Products.Add(modal.Product);

                Controls.Remove(overlay);
                LoadProducts();
            };
        }
    }
}
