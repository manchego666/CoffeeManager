using System;
using System.Drawing;
using System.Linq;
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
        private PictureBox picProduct = null!;
        private TextBox txtName = null!;
        private TextBox txtPrice = null!;
        private TextBox txtDescription = null!;
        private CheckBox chkUsesWarehouse = null!;
        private TextBox txtStock = null!;
        private ComboBox cmbCategories = null!;
        private FlowLayoutPanel panelChips = null!;
        private Button btnAddCategory = null!;
        private Button btnEditRecipe = null!;
        private Button btnSave = null!;
        private Button btnClose = null!;

        public ProductModal(Store store, Product product)
        {
            _store = store;
            _product = product;

            DoubleBuffered = true;
            Size = new Size(750, 600);
            BackColor = Color.Transparent;

            InitializeLayout();
            LoadData();
        }

        private void InitializeLayout()
        {
            panelCard = new Panel
            {
                Size = new Size(750, 600),
                BackColor = Color.FromArgb(235, 255, 255, 255),
                Padding = new Padding(20)
            };
            Controls.Add(panelCard);

            var lblTitle = new Label
            {
                Text = $"Producto: {_product.Name}",
                Font = new Font("Segoe UI", 18, FontStyle.Bold),
                ForeColor = Color.FromArgb(40, 40, 40),
                AutoSize = true,
                Location = new Point(10, 5)
            };
            panelCard.Controls.Add(lblTitle);

            // FOTO
            picProduct = new PictureBox
            {
                Size = new Size(150, 150),
                Location = new Point(560, 40),
                SizeMode = PictureBoxSizeMode.Zoom,
                BackColor = Color.LightGray
            };
            panelCard.Controls.Add(picProduct);

            var btnChangePhoto = new Button
            {
                Text = "Cambiar foto",
                Size = new Size(150, 30),
                Location = new Point(560, 200),
                BackColor = Color.FromArgb(70, 180, 160),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };
            btnChangePhoto.FlatAppearance.BorderSize = 0;
            btnChangePhoto.Click += ChangePhoto;
            panelCard.Controls.Add(btnChangePhoto);

            // NOMBRE
            var lblName = new Label
            {
                Text = "Nombre:",
                Location = new Point(10, 60),
                AutoSize = true
            };
            panelCard.Controls.Add(lblName);

            txtName = new TextBox
            {
                Location = new Point(10, 85),
                Width = 500
            };
            panelCard.Controls.Add(txtName);

            // PRECIO
            var lblPrice = new Label
            {
                Text = "Precio:",
                Location = new Point(10, 130),
                AutoSize = true
            };
            panelCard.Controls.Add(lblPrice);

            txtPrice = new TextBox
            {
                Location = new Point(10, 155),
                Width = 200
            };
            panelCard.Controls.Add(txtPrice);

            // DESCRIPCIÓN
            var lblDesc = new Label
            {
                Text = "Descripción:",
                Location = new Point(10, 200),
                AutoSize = true
            };
            panelCard.Controls.Add(lblDesc);

            txtDescription = new TextBox
            {
                Location = new Point(10, 225),
                Width = 500,
                Height = 80,
                Multiline = true
            };
            panelCard.Controls.Add(txtDescription);

            // USA ALMACÉN
            chkUsesWarehouse = new CheckBox
            {
                Text = "Usa almacén",
                Location = new Point(10, 320),
                AutoSize = true
            };
            chkUsesWarehouse.CheckedChanged += (s, e) => UpdateWarehouseFields();
            panelCard.Controls.Add(chkUsesWarehouse);

            // STOCK
            var lblStock = new Label
            {
                Text = "Stock:",
                Location = new Point(10, 350),
                AutoSize = true
            };
            panelCard.Controls.Add(lblStock);

            txtStock = new TextBox
            {
                Location = new Point(10, 375),
                Width = 200
            };
            panelCard.Controls.Add(txtStock);

            // BOTÓN EDITAR RECETA
            btnEditRecipe = new Button
            {
                Text = "Editar receta",
                Size = new Size(150, 35),
                Location = new Point(230, 370),
                BackColor = Color.FromArgb(240, 180, 80),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };
            btnEditRecipe.FlatAppearance.BorderSize = 0;
            btnEditRecipe.Click += EditRecipe;
            panelCard.Controls.Add(btnEditRecipe);

            // CATEGORÍAS
            var lblCat = new Label
            {
                Text = "Categorías:",
                Location = new Point(10, 420),
                AutoSize = true
            };
            panelCard.Controls.Add(lblCat);

            cmbCategories = new ComboBox
            {
                Location = new Point(10, 445),
                Width = 300,
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            panelCard.Controls.Add(cmbCategories);

            btnAddCategory = new Button
            {
                Text = "+",
                Size = new Size(40, 30),
                Location = new Point(320, 445),
                BackColor = Color.FromArgb(70, 180, 160),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };
            btnAddCategory.FlatAppearance.BorderSize = 0;
            btnAddCategory.Click += AddCategoryModal;
            panelCard.Controls.Add(btnAddCategory);

            panelChips = new FlowLayoutPanel
            {
                Location = new Point(10, 485),
                Size = new Size(700, 60),
                AutoScroll = true
            };
            panelCard.Controls.Add(panelChips);

            // BOTÓN GUARDAR
            btnSave = new Button
            {
                Text = "Guardar",
                Size = new Size(120, 35),
                Location = new Point(500, 540),
                BackColor = Color.FromArgb(70, 180, 160),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };
            btnSave.FlatAppearance.BorderSize = 0;
            btnSave.Click += SaveProduct;
            panelCard.Controls.Add(btnSave);

            // BOTÓN CERRAR
            btnClose = new Button
            {
                Text = "Cerrar",
                Size = new Size(120, 35),
                Location = new Point(630, 540),
                BackColor = Color.FromArgb(220, 60, 60),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };
            btnClose.FlatAppearance.BorderSize = 0;
            btnClose.Click += (s, e) => OnClose?.Invoke();
            panelCard.Controls.Add(btnClose);
        }

        private void LoadData()
        {
            if (!string.IsNullOrWhiteSpace(_product.ImagePath) &&
                System.IO.File.Exists(_product.ImagePath))
                picProduct.Image = Image.FromFile(_product.ImagePath);

            txtName.Text = _product.Name;
            txtPrice.Text = _product.Price.ToString();
            txtDescription.Text = _product.Description;
            chkUsesWarehouse.Checked = _product.UsesWarehouse;
            txtStock.Text = _product.Stock.ToString();

            cmbCategories.Items.Clear();
            foreach (var c in _store.Categories)
                cmbCategories.Items.Add(c);

            UpdateChips();
            UpdateWarehouseFields();
        }

        private void UpdateWarehouseFields()
        {
            bool uses = chkUsesWarehouse.Checked;

            txtStock.Enabled = !uses;
            btnEditRecipe.Enabled = uses;
        }

        private void UpdateChips()
        {
            panelChips.Controls.Clear();

            foreach (var cat in _product.Categories)
            {
                var chip = CreateChip(cat);
                panelChips.Controls.Add(chip);
            }
        }

        private Panel CreateChip(string text)
        {
            var panel = new Panel
            {
                BackColor = Color.FromArgb(230, 230, 230),
                Padding = new Padding(5),
                AutoSize = true
            };

            var lbl = new Label
            {
                Text = text,
                AutoSize = true
            };
            panel.Controls.Add(lbl);

            var btnX = new Button
            {
                Text = "X",
                Size = new Size(25, 25),
                BackColor = Color.FromArgb(220, 60, 60),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };
            btnX.FlatAppearance.BorderSize = 0;
            btnX.Click += (s, e) =>
            {
                _product.Categories.Remove(text);
                UpdateChips();
            };
            panel.Controls.Add(btnX);

            return panel;
        }

        private void AddCategoryModal(object? sender, EventArgs e)
        {
            string input = Microsoft.VisualBasic.Interaction.InputBox(
                "Nueva categoría:", "Agregar categoría", "");

            if (string.IsNullOrWhiteSpace(input)) return;

            if (!_store.Categories.Contains(input))
                _store.Categories.Add(input);

            if (!_product.Categories.Contains(input))
                _product.Categories.Add(input);

            LoadData();
        }

        private void ChangePhoto(object? sender, EventArgs e)
        {
            var dlg = new OpenFileDialog
            {
                Filter = "Imágenes|*.jpg;*.png;*.jpeg"
            };

            if (dlg.ShowDialog() == DialogResult.OK)
            {
                _product.ImagePath = dlg.FileName;
                picProduct.Image = Image.FromFile(dlg.FileName);
            }
        }

        private void EditRecipe(object? sender, EventArgs e)
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
            _product.Description = txtDescription.Text;
            _product.UsesWarehouse = chkUsesWarehouse.Checked;

            if (decimal.TryParse(txtPrice.Text, out var price))
                _product.Price = price;

            if (int.TryParse(txtStock.Text, out var stock))
                _product.Stock = stock;

            OnClose?.Invoke();
        }
    }
}
