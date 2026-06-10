using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using CoffeeManager.Models.Class;
using CoffeeManager.Models.Enums;

namespace CoffeeManager.Front
{
    internal class RecipeIngredientModal : UserControl
    {
        private readonly Store _store;
        private readonly RecipeItem? _original;

        public event Action<RecipeItem?>? OnClose;

        private Panel panelCard = null!;
        private ComboBox cmbIngredient = null!;
        private NumericUpDown numQty = null!;
        private ComboBox cmbUnit = null!;
        private CheckBox chkOptional = null!;
        private Button btnSave = null!;
        private Button btnCancel = null!;

        public RecipeIngredientModal(Store store, RecipeItem? item)
        {
            _store = store;
            _original = item;

            DoubleBuffered = true;
            Size = new Size(500, 350);
            BackColor = Color.Transparent;

            InitializeLayout();
            LoadData();
        }

        private void InitializeLayout()
        {
            panelCard = new Panel
            {
                Size = new Size(500, 350),
                BackColor = Color.FromArgb(235, 255, 255, 255),
                Padding = new Padding(20)
            };
            Controls.Add(panelCard);

            var lblTitle = new Label
            {
                Text = _original == null ? "Agregar ingrediente" : "Editar ingrediente",
                Font = new Font("Segoe UI", 16, FontStyle.Bold),
                ForeColor = Color.FromArgb(40, 40, 40),
                AutoSize = true,
                Location = new Point(10, 5)
            };
            panelCard.Controls.Add(lblTitle);

            panelCard.Controls.Add(new Label
            {
                Text = "Ingrediente",
                Location = new Point(10, 60),
                AutoSize = true
            });

            cmbIngredient = new ComboBox
            {
                Location = new Point(10, 80),
                Width = 300,
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            panelCard.Controls.Add(cmbIngredient);

            panelCard.Controls.Add(new Label
            {
                Text = "Cantidad",
                Location = new Point(10, 120),
                AutoSize = true
            });

            numQty = new NumericUpDown
            {
                Location = new Point(10, 140),
                Width = 120,
                DecimalPlaces = 2,
                Maximum = 9999
            };
            panelCard.Controls.Add(numQty);

            panelCard.Controls.Add(new Label
            {
                Text = "Unidad",
                Location = new Point(160, 120),
                AutoSize = true
            });

            cmbUnit = new ComboBox
            {
                Location = new Point(160, 140),
                Width = 150,
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            cmbUnit.Items.AddRange(Enum.GetNames(typeof(UnitType)));
            panelCard.Controls.Add(cmbUnit);

            chkOptional = new CheckBox
            {
                Text = "Opcional",
                Location = new Point(10, 180)
            };
            panelCard.Controls.Add(chkOptional);

            btnSave = new Button
            {
                Text = "Guardar",
                Size = new Size(150, 40),
                Location = new Point(200, 260),
                BackColor = Color.FromArgb(70, 180, 160),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };
            btnSave.Click += SaveIngredient;
            panelCard.Controls.Add(btnSave);

            btnCancel = new Button
            {
                Text = "Cancelar",
                Size = new Size(150, 40),
                Location = new Point(360, 260),
                BackColor = Color.FromArgb(220, 60, 60),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };
            btnCancel.Click += (s, e) => OnClose?.Invoke(null);
            panelCard.Controls.Add(btnCancel);
        }

        private void LoadData()
        {
            foreach (var i in _store.Warehouse.Items)
                cmbIngredient.Items.Add(i.Name);

            if (_original != null)
            {
                cmbIngredient.SelectedItem = _original.IngredientName;
                numQty.Value = _original.Quantity;
                cmbUnit.SelectedItem = _original.Unit.ToString();
                chkOptional.Checked = _original.IsOptional;
            }
            else
            {
                cmbUnit.SelectedIndex = 0;
            }
        }

        private void SaveIngredient(object? sender, EventArgs e)
        {
            if (cmbIngredient.SelectedItem == null)
            {
                MessageBox.Show("Seleccione un ingrediente.");
                return;
            }

            var ingredient = _store.Warehouse.Items
                .First(x => x.Name == cmbIngredient.SelectedItem.ToString());

            var item = new RecipeItem
            {
                IngredientId = ingredient.Id,
                IngredientName = ingredient.Name,
                Quantity = numQty.Value,
                Unit = Enum.Parse<UnitType>(cmbUnit.SelectedItem.ToString()),
                IsOptional = chkOptional.Checked
            };

            OnClose?.Invoke(item);
        }
    }
}
