using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using CoffeeManager.Models.Class;
using CoffeeManager.Front.Styles;

namespace CoffeeManager.Front
{
    public class SalesHistoryModule : Panel
    {
        private readonly Store _store;
        private readonly DataGridView grid;

        public SalesHistoryModule(Store store)
        {
            _store = store;

            Dock = DockStyle.Fill;
            BackgroundImage = Properties.Resources.bg_main;
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
                Text = "Historial de Ventas",
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
            grid.Columns.Add("Date", "Fecha");
            grid.Columns.Add("Employee", "Empleado");
            grid.Columns.Add("Products", "Productos Vendidos");
            grid.Columns.Add("Ingredients", "Ingredientes Usados");
            grid.Columns.Add("Cost", "Costo Total");
            grid.Columns.Add("Profit", "Ganancia Total");
            grid.Columns.Add("Total", "Total Venta");
        }

        private void LoadData()
        {
            grid.Rows.Clear();

            foreach (var sale in _store.Sales.OrderByDescending(x => x.Date))
            {
                // PRODUCTOS VENDIDOS
                string productos = string.Join(", ",
                    sale.Details.Select(d => $"{d.Product.Name} x{d.Quantity}"));

                // INGREDIENTES USADOS (solo productos con receta)
                var ingredientes = sale.Details
                    .Where(d => d.Product.UsesWarehouse)
                    .SelectMany(d => d.Product.Recipe.Select(r =>
                        new
                        {
                            r.IngredientName,
                            Cantidad = r.Quantity * d.Quantity
                        }))
                    .GroupBy(x => x.IngredientName)
                    .Select(g => $"{g.Key} x{g.Sum(x => x.Cantidad)}")
                    .ToList();

                string ingredientesTexto = ingredientes.Count > 0
                    ? string.Join(", ", ingredientes)
                    : "No aplica";

                // COSTO TOTAL
                decimal costoTotal = sale.Details.Sum(d =>
                    d.Product.GetUnitCost(_store.Warehouse) * d.Quantity);

                // GANANCIA TOTAL
                decimal gananciaTotal = sale.Details.Sum(d =>
                    d.Product.GetEstimatedProfit(_store.Warehouse) * d.Quantity);

                grid.Rows.Add(
                    sale.Id,
                    sale.Date.ToString("yyyy-MM-dd HH:mm"),
                    $"{sale.Employee?.FirstName} {sale.Employee?.LastName}",
                    productos,
                    ingredientesTexto,
                    costoTotal.ToString("F2"),
                    gananciaTotal.ToString("F2"),
                    sale.Total.ToString("F2")
                );
            }
        }
    }
}
