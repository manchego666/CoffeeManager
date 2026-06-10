using System;
using System.Drawing;
using System.Windows.Forms;
using CoffeeManager.Models.Class;
using CoffeeManager.Models.Enums;

namespace CoffeeManager.Front
{
    internal class EditWarehouseItemModal : UserControl
    {
        private readonly Warehouse _warehouse;
        private readonly InventoryItem? _original;

        public event Action? OnClose;

        private Panel panelCard = null!;
        private TextBox txtName = null!;
        private NumericUpDown numQuantity = null!;
        private ComboBox cmbUnit = null!;
        private NumericUpDown numCost = null!;
        private NumericUpDown numUnitsPerPackage = null!;
        private NumericUpDown numPackagesPerBox = null!;
        private Button btnSave = null!;
        private Button btnCancel = null!;

        public EditWarehouseItemModal(Warehouse warehouse, InventoryItem? item)
        {
            _warehouse = warehouse;
            _original = item;

            DoubleBuffered = true;
            Size = new Size(600, 380);
            BackColor = Color.Transparent;

            InitializeLayout();
            LoadData();
        }

        private void InitializeLayout()
        {
            panelCard = new Panel
            {
                Size = new Size(580, 360),
                BackColor = Color.FromArgb(235, 255, 255, 255),
                Padding = new Padding(20)
            };
            Controls.Add(panelCard);

            var lblTitle = new Label
            {
                Text = _original == null ? "Agregar insumo" : "Editar insumo",
                Font = new Font("Segoe UI", 18, FontStyle.Bold),
                ForeColor = Color.FromArgb(40, 40, 40),
                AutoSize = true,
                Location = new Point(10, 5)
            };
            panelCard.Controls.Add(lblTitle);

            int top = 60;
            int col1 = 20;
            int col2 = 300;
            int rowH = 45;

            // Nombre
            panelCard.Controls.Add(new Label
            {
                Text = "Nombre",
                Location = new Point(col1, top),
                AutoSize = true
            });
            txtName = new TextBox
            {
                Location = new Point(col1, top + 18),
                Width = 240,
                Font = new Font("Segoe UI", 10)
            };
            panelCard.Controls.Add(txtName);

            // Cantidad
            panelCard.Controls.Add(new Label
            {
                Text = "Cantidad (unidad base)",
                Location = new Point(col1, top + rowH),
                AutoSize = true
            });
            numQuantity = new NumericUpDown
            {
                Location = new Point(col1, top + rowH + 18),
                Width = 120,
                DecimalPlaces = 2,
                Maximum = 999999
            };
            panelCard.Controls.Add(numQuantity);

            // Unidad
            panelCard.Controls.Add(new Label
            {
                Text = "Unidad",
                Location = new Point(col2, top),
                AutoSize = true
            });
            cmbUnit = new ComboBox
            {
                Location = new Point(col2, top + 18),
                Width = 200,
                DropDownStyle = ComboBoxStyle.DropDownList,
                Font = new Font("Segoe UI", 10)
            };
            cmbUnit.Items.AddRange(Enum.GetNames(typeof(UnitType)));
            panelCard.Controls.Add(cmbUnit);

            // Costo/U
            panelCard.Controls.Add(new Label
            {
                Text = "Costo por unidad",
                Location = new Point(col2, top + rowH),
                AutoSize = true
            });
            numCost = new NumericUpDown
            {
                Location = new Point(col2, top + rowH + 18),
                Width = 120,
                DecimalPlaces = 2,
                Maximum = 999999
            };
            panelCard.Controls.Add(numCost);

            // U. por paquete
            panelCard.Controls.Add(new Label
            {
                Text = "Unidades por paquete",
                Location = new Point(col1, top + rowH * 2),
                AutoSize = true
            });
            numUnitsPerPackage = new NumericUpDown
            {
                Location = new Point(col1, top + rowH * 2 + 18),
                Width = 120,
                Minimum = 1,
                Maximum = 10000,
                Value = 1
            };
            panelCard.Controls.Add(numUnitsPerPackage);

            // Paquetes por caja
            panelCard.Controls.Add(new Label
            {
                Text = "Paquetes por caja",
                Location = new Point(col2, top + rowH * 2),
                AutoSize = true
            });
            numPackagesPerBox = new NumericUpDown
            {
                Location = new Point(col2, top + rowH * 2 + 18),
                Width = 120,
                Minimum = 1,
                Maximum = 10000,
                Value = 1
            };
            panelCard.Controls.Add(numPackagesPerBox);

            // Botones
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
            btnSave.Click += SaveItem;
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

        private void LoadData()
        {
            if (_original == null)
            {
                cmbUnit.SelectedIndex = 0;
                return;
            }

            txtName.Text = _original.Name;
            numQuantity.Value = _original.Quantity;
            cmbUnit.SelectedItem = _original.Unit.ToString();
            numCost.Value = _original.CostPerUnit;
            numUnitsPerPackage.Value = _original.UnitsPerPackage;
            numPackagesPerBox.Value = _original.PackagesPerBox;
        }

        private void SaveItem(object? sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtName.Text))
            {
                MessageBox.Show("El nombre es obligatorio.");
                return;
            }

            if (cmbUnit.SelectedItem == null)
            {
                MessageBox.Show("Seleccione una unidad.");
                return;
            }

            var unit = Enum.Parse<UnitType>(cmbUnit.SelectedItem.ToString()!);

            if (_original == null)
            {
                // Nuevo insumo
                var item = new InventoryItem
                {
                    Id = _warehouse.GetNextId(),
                    Name = txtName.Text.Trim(),
                    Quantity = numQuantity.Value,
                    Unit = unit,
                    CostPerUnit = numCost.Value,
                    UnitsPerPackage = (int)numUnitsPerPackage.Value,
                    PackagesPerBox = (int)numPackagesPerBox.Value,
                    LastUpdated = DateTime.Now
                };

                _warehouse.Items.Add(item);
            }
            else
            {
                // Editar existente
                _original.Name = txtName.Text.Trim();
                _original.Quantity = numQuantity.Value;
                _original.Unit = unit;
                _original.CostPerUnit = numCost.Value;
                _original.UnitsPerPackage = (int)numUnitsPerPackage.Value;
                _original.PackagesPerBox = (int)numPackagesPerBox.Value;
                _original.LastUpdated = DateTime.Now;

                _warehouse.UpdateItem(_original);
            }

            OnClose?.Invoke();
        }
    }
}
