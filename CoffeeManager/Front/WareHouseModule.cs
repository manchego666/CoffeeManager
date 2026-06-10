using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using CoffeeManager.Models.Class;
using CoffeeManager.Front.Styles;

namespace CoffeeManager.Front
{
    public class WarehouseModule : Panel
    {
        private readonly Store _store;
        private readonly DataGridView grid;

        public WarehouseModule(Store store)
        {
            _store = store;

            Dock = DockStyle.Fill;
            BackgroundImage = Properties.Resources.bg_warehouse;
            BackgroundImageLayout = ImageLayout.Stretch;

            // CARD PANEL PREMIUM
            var card = new CardPanel
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(20)
            };
            Controls.Add(card);

            // TÍTULO
            var lblTitle = new Label
            {
                Text = "Inventario del Almacén",
                Font = new Font("Segoe UI", 28, FontStyle.Bold),
                ForeColor = Color.FromArgb(40, 40, 40),
                AutoSize = true,
                Location = new Point(10, 10)
            };
            card.Controls.Add(lblTitle);

            // GRID
            grid = new DataGridView
            {
                Location = new Point(10, 80),
                Size = new Size(1100, 550),
                ReadOnly = true,
                AllowUserToAddRows = false
            };
            DataGridStyler.Apply(grid);
            card.Controls.Add(grid);

            BuildColumns();
            LoadData();
        }

        private void BuildColumns()
        {
            grid.Columns.Clear();

            grid.Columns.Add("Id", "ID");
            grid.Columns.Add("Name", "Nombre");
            grid.Columns.Add("Qty", "Cantidad");
            grid.Columns.Add("Unit", "Unidad");
            grid.Columns.Add("Cost", "Costo/U");
            grid.Columns.Add("UPP", "Unidades/Paquete");
            grid.Columns.Add("PPB", "Paquetes/Caja");
            grid.Columns.Add("Updated", "Actualizado");
        }

        private void LoadData()
        {
            grid.Rows.Clear();

            foreach (var i in _store.Warehouse.Items)
            {
                grid.Rows.Add(
                    i.Id,
                    i.Name,
                    i.Quantity,
                    i.Unit,
                    i.CostPerUnit.ToString("F2"),
                    i.UnitsPerPackage,
                    i.PackagesPerBox,
                    i.LastUpdated.ToString("yyyy-MM-dd HH:mm")
                );
            }
        }
    }
}
