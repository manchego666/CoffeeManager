using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using CoffeeManager.Models.Class;

namespace CoffeeManager.Front
{
    public class WareHouseModule : UserControl
    {
        private readonly Store _store;
        private DataGridView grid = null!;
        private Panel panelCard = null!;

        public WareHouseModule(Store store)
        {
            _store = store;

            DoubleBuffered = true;
            Dock = DockStyle.Fill;
            BackColor = Color.Transparent;

            InitializeLayout();
            LoadData();
        }

        private void InitializeLayout()
        {
            panelCard = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.FromArgb(235, 255, 255, 255),
                Padding = new Padding(20)
            };
            Controls.Add(panelCard);

            var lblTitle = new Label
            {
                Text = "Inventario del Almacén",
                Font = new Font("Segoe UI", 24, FontStyle.Bold),
                ForeColor = Color.FromArgb(40, 40, 40),
                AutoSize = true,
                Location = new Point(10, 10)
            };
            panelCard.Controls.Add(lblTitle);

            grid = new DataGridView
            {
                Location = new Point(10, 70),
                Size = new Size(900, 500),
                ReadOnly = true,
                AllowUserToAddRows = false,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                BackgroundColor = Color.White,
                BorderStyle = BorderStyle.None
            };

            grid.Columns.Add("Id", "ID");
            grid.Columns.Add("Name", "Nombre");
            grid.Columns.Add("Qty", "Cantidad");
            grid.Columns.Add("Unit", "Unidad");
            grid.Columns.Add("Low", "¿Bajo?");
            grid.Columns.Add("Out", "¿Agotado?");

            panelCard.Controls.Add(grid);
        }

        private void LoadData()
        {
            grid.Rows.Clear();

            foreach (var item in _store.Warehouse.Items)
            {
                grid.Rows.Add(
                    item.Id,
                    item.Name,
                    item.Quantity,
                    item.Unit.ToString(),
                    item.IsLowStock() ? "Sí" : "No",
                    item.IsOutOfStock() ? "Sí" : "No"
                );
            }
        }
    }
}
