using System;
using System.Drawing;
using System.Windows.Forms;
using CoffeeManager.Models.Class;

namespace CoffeeManager.Front
{
    internal class ProductModal : UserControl
    {
        private readonly Store _store;
        private readonly Product _product;
        public Product Product => _product;
        public event Action? OnClose;

        private Panel panelCard = null!;
        private TextBox txtName = null!;
        private NumericUpDown numPrice = null!;
        private NumericUpDown numStock = null!;
        private CheckBox chkUsesWarehouse = null!;
        private Button btnRecipe = null!;
        private Button btnSave = null!;
        private Button btnCancel = null!;

        


        public ProductModal(Store store, Product? product)
        {
            _store = store;
            _product = product ?? new Product
            {
                Id = new Random().Next(100000, 999999),
                Recipe = new System.Collections.Generic.List<RecipeItem>()
            };

            DoubleBuffered = true;
            SetStyle(ControlStyles.AllPaintingInWmPaint |
                     ControlStyles.UserPaint |
                     ControlStyles.OptimizedDoubleBuffer, true);

            Size = new Size(720, 520);
            BackColor = Color.Transparent;

            InitializeLayout();
            LoadProduct();
        }

        private void InitializeLayout()
        {
            panelCard = new Panel
            {
                Size = new Size(760, 540),
                BackColor = Color.FromArgb(235, 255, 255, 255),
                Padding = new Padding(25)
            };
            panelCard.Paint += PanelCard_Paint;
            Controls.Add(panelCard);

            var lblTitle = new Label
            {
                Text = _product.Id == 0 ? "Agregar producto" : "Editar producto",
                Font = new Font("Segoe UI", 20, FontStyle.Bold),
                ForeColor = Color.FromArgb(40, 40, 40),
                AutoSize = true,
                Location = new Point(10, 5)
            };
            panelCard.Controls.Add(lblTitle);

            int top = 60;
            int col1 = 20;
            int col2 = 350;
            int rowH = 45;

            txtName = CreateText(panelCard, "Nombre", col1, top);
            numPrice = CreateNumeric(panelCard, "Precio", col1, top + rowH);
            numStock = CreateNumeric(panelCard, "Stock", col1, top + rowH * 2);

            chkUsesWarehouse = new CheckBox
            {
                Text = "Usa ingredientes del almacén",
                Location = new Point(col1, top + rowH * 3 + 10),
                Font = new Font("Segoe UI", 10)
            };
            panelCard.Controls.Add(chkUsesWarehouse);

            btnRecipe = new Button
            {
                Text = "Editar receta",
                Size = new Size(200, 40),
                Location = new Point(col1, top + rowH * 4 + 10),
                BackColor = Color.FromArgb(240, 128, 148),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };
            btnRecipe.FlatAppearance.BorderSize = 0;
            btnRecipe.Click += OpenRecipeModal;
            panelCard.Controls.Add(btnRecipe);

            btnSave = new Button
            {
                Text = "Guardar",
                Size = new Size(150, 40),
                Location = new Point(panelCard.Width - 330, panelCard.Height - 70),
                BackColor = Color.FromArgb(70, 180, 160),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };
            btnSave.FlatAppearance.BorderSize = 0;
            btnSave.Click += SaveProduct;
            panelCard.Controls.Add(btnSave);

            btnCancel = new Button
            {
                Text = "Cancelar",
                Size = new Size(150, 40),
                Location = new Point(panelCard.Width - 170, panelCard.Height - 70),
                BackColor = Color.FromArgb(220, 60, 60),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };
            btnCancel.FlatAppearance.BorderSize = 0;
            btnCancel.Click += (s, e) => OnClose?.Invoke();
            panelCard.Controls.Add(btnCancel);
        }

        private TextBox CreateText(Control parent, string label, int x, int y)
        {
            parent.Controls.Add(new Label
            {
                Text = label,
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                ForeColor = Color.FromArgb(60, 60, 60),
                Location = new Point(x, y),
                AutoSize = true
            });

            var txt = new TextBox
            {
                Location = new Point(x, y + 18),
                Width = 260,
                Font = new Font("Segoe UI", 10),
                BorderStyle = BorderStyle.FixedSingle
            };
            parent.Controls.Add(txt);
            return txt;
        }

        private NumericUpDown CreateNumeric(Control parent, string label, int x, int y)
        {
            parent.Controls.Add(new Label
            {
                Text = label,
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                ForeColor = Color.FromArgb(60, 60, 60),
                Location = new Point(x, y),
                AutoSize = true
            });

            var num = new NumericUpDown
            {
                Location = new Point(x, y + 18),
                Width = 260,
                DecimalPlaces = 2,
                Maximum = 999999,
                Font = new Font("Segoe UI", 10)
            };
            parent.Controls.Add(num);
            return num;
        }

        private void LoadProduct()
        {
            txtName.Text = _product.Name;
            numPrice.Value = _product.Price;
            numStock.Value = _product.Stock;
            chkUsesWarehouse.Checked = _product.UsesWarehouse;
        }

        private void OpenRecipeModal(object? sender, EventArgs e)
        {
            var overlay = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.FromArgb(120, 0, 0, 0)
            };
            Parent.Controls.Add(overlay);
            overlay.BringToFront();

            var modal = new RecipeModal(_product, _store);

            modal.Left = (overlay.Width - modal.Width) / 2;
            modal.Top = (overlay.Height - modal.Height) / 2;

            overlay.Controls.Add(modal);

            modal.OnClose += () =>
            {
                Parent.Controls.Remove(overlay);
            };
        }

        private void SaveProduct(object? sender, EventArgs e)
        {
            _product.Name = txtName.Text;
            _product.Price = numPrice.Value;
            _product.Stock = (int)numStock.Value;
            _product.UsesWarehouse = chkUsesWarehouse.Checked;

            OnClose?.Invoke();
        }

        private void PanelCard_Paint(object? sender, PaintEventArgs e)
        {
            var g = e.Graphics;
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

            var rect = panelCard.ClientRectangle;
            rect.Inflate(-1, -1);

            using var path = RoundedRect(rect, 18);
            using var brush = new SolidBrush(Color.FromArgb(235, 255, 255, 255));
            using var pen = new Pen(Color.FromArgb(200, 200, 200), 1);

            g.FillPath(brush, path);
            g.DrawPath(pen, path);
        }

        private static System.Drawing.Drawing2D.GraphicsPath RoundedRect(Rectangle bounds, int radius)
        {
            int d = radius * 2;
            var path = new System.Drawing.Drawing2D.GraphicsPath();
            path.AddArc(bounds.X, bounds.Y, d, d, 180, 90);
            path.AddArc(bounds.Right - d, bounds.Y, d, d, 270, 90);
            path.AddArc(bounds.Right - d, bounds.Bottom - d, d, d, 0, 90);
            path.AddArc(bounds.X, bounds.Bottom - d, d, d, 90, 90);
            path.CloseFigure();
            return path;
        }
    }
}
