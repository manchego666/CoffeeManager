using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using CoffeeManager.Models.Class;
using CoffeeManager.Front.Styles;

namespace CoffeeManager.Front
{
    public class WareHouseModule : UserControl
    {
        private readonly Store _store;

        private Panel panelRoot = null!;
        private CardPanel panelCard = null!;
        private DataGridView grid = null!;
        private TextBox txtSearch = null!;
        private ComboBox cbFilter = null!;
        private Panel overlay = null!;

        public WareHouseModule(Store store)
        {
            _store = store;

            DoubleBuffered = true;
            SetStyle(ControlStyles.AllPaintingInWmPaint |
                     ControlStyles.UserPaint |
                     ControlStyles.OptimizedDoubleBuffer, true);

            BackColor = Color.FromArgb(1, 0, 0, 0);

            InitializeLayout();
            LoadData();
        }

        private void InitializeLayout()
        {
            // ROOT (scroll + centrado)
            panelRoot = new Panel
            {
                Dock = DockStyle.Fill,
                AutoScroll = true,
                BackColor = Color.FromArgb(1, 0, 0, 0),
                Padding = new Padding(0, 20, 0, 20)
            };
            Controls.Add(panelRoot);

            // CARD CENTRADA
            panelCard = new CardPanel
            {
                Size = new Size(1100, 600),
                Padding = new Padding(20)
            };
            panelRoot.Controls.Add(panelCard);

            panelRoot.Resize += (s, e) =>
            {
                panelCard.Left = (panelRoot.ClientSize.Width - panelCard.Width) / 2;
            };

            panelCard.Left = (panelRoot.ClientSize.Width - panelCard.Width) / 2;
            panelCard.Top = 40;

            // TÍTULO
            var lblTitle = new Label
            {
                Text = "Inventario del almacén",
                Font = new Font("Segoe UI", 22, FontStyle.Bold),
                ForeColor = Color.FromArgb(40, 40, 40),
                AutoSize = true,
                Location = new Point(10, 10)
            };
            panelCard.Controls.Add(lblTitle);

            // BOTÓN AGREGAR
            var btnAdd = new Button
            {
                Text = "Agregar insumo",
                Size = new Size(160, 38),
                Location = new Point(10, 60),
                BackColor = Color.FromArgb(70, 180, 160),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };
            btnAdd.FlatAppearance.BorderSize = 0;
            btnAdd.Click += (s, e) => ShowItemModal(null);
            panelCard.Controls.Add(btnAdd);

            // BUSCADOR
            txtSearch = new TextBox
            {
                PlaceholderText = "Buscar...",
                Location = new Point(190, 65),
                Width = 250,
                Font = new Font("Segoe UI", 10)
            };
            txtSearch.TextChanged += (s, e) => LoadData();
            panelCard.Controls.Add(txtSearch);

            // FILTRO
            cbFilter = new ComboBox
            {
                Location = new Point(460, 65),
                Width = 180,
                DropDownStyle = ComboBoxStyle.DropDownList,
                Font = new Font("Segoe UI", 10)
            };
            cbFilter.Items.Add("Todos");
            cbFilter.Items.Add("Stock bajo");
            cbFilter.Items.Add("Agotados");
            cbFilter.SelectedIndex = 0;
            cbFilter.SelectedIndexChanged += (s, e) => LoadData();
            panelCard.Controls.Add(cbFilter);

            // GRID
            grid = new DataGridView
            {
                Location = new Point(10, 110),
                Size = new Size(1060, 440),
                ReadOnly = true,
                AllowUserToAddRows = false,
                AllowUserToResizeRows = false,
                RowHeadersVisible = false,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                MultiSelect = false,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
            };

            DataGridStyler.Apply(grid);
            panelCard.Controls.Add(grid);

            BuildColumns();
        }

        private void BuildColumns()
        {
            grid.Columns.Clear();

            grid.Columns.Add("Id", "ID");
            grid.Columns.Add("Name", "Nombre");
            grid.Columns.Add("Qty", "Cantidad");
            grid.Columns.Add("Unit", "Unidad");
            grid.Columns.Add("Cost", "Costo/U");
            grid.Columns.Add("UPP", "U. por paquete");
            grid.Columns.Add("PPB", "Paquetes/caja");
            grid.Columns.Add("Updated", "Actualizado");

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

            grid.CellClick += Grid_CellClick;
        }

        private void LoadData()
        {
            grid.Rows.Clear();

            var items = _store.Warehouse.Items.AsEnumerable();

            if (!string.IsNullOrWhiteSpace(txtSearch.Text))
            {
                string q = txtSearch.Text.ToLower();
                items = items.Where(i => i.Name.ToLower().Contains(q));
            }

            if (cbFilter.SelectedIndex == 1)
                items = items.Where(i => i.IsLowStock());
            else if (cbFilter.SelectedIndex == 2)
                items = items.Where(i => i.IsOutOfStock());

            foreach (var i in items)
            {
                grid.Rows.Add(
                    i.Id,
                    i.Name,
                    i.Quantity,
                    i.Unit,
                    i.CostPerUnit.ToString("F2"),
                    i.UnitsPerPackage,
                    i.PackagesPerBox,
                    i.LastUpdated.ToString("yyyy-MM-dd")
                );
            }
        }

        private void Grid_CellClick(object? sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;

            int id = Convert.ToInt32(grid.Rows[e.RowIndex].Cells[0].Value);

            if (e.ColumnIndex == 8) // EDITAR
            {
                var item = _store.Warehouse.GetItemById(id);
                if (item != null)
                    ShowItemModal(item);
            }
            else if (e.ColumnIndex == 9) // ELIMINAR
            {
                DeleteItem(id);
            }
        }

        private void ShowItemModal(InventoryItem? item)
        {
            overlay = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.FromArgb(120, 0, 0, 0)
            };
            Controls.Add(overlay);
            overlay.BringToFront();

            var modal = new EditWarehouseItemModal(_store.Warehouse, item);

            modal.Left = (overlay.Width - modal.Width) / 2;
            modal.Top = (overlay.Height - modal.Height) / 2;

            if (modal.Left < 0) modal.Left = 0;
            if (modal.Top < 0) modal.Top = 0;

            overlay.Controls.Add(modal);

            modal.OnClose += () =>
            {
                Controls.Remove(overlay);
                LoadData();
            };
        }

        private void DeleteItem(int id)
        {
            if (MessageBox.Show("¿Eliminar insumo?", "Confirmar",
                MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
            {
                _store.Warehouse.RemoveItem(id);
                LoadData();
            }
        }
    }
}
