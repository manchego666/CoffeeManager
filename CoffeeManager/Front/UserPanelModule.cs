using System;
using System.Drawing;
using System.Windows.Forms;
using CoffeeManager.Models.Class;
using CoffeeManager.Models.Enums;

namespace CoffeeManager.Front
{
    internal class UsersPanelModule : UserControl
    {
        private readonly Store _store;

        private Panel panelRoot = null!;
        private Panel panelCard = null!;
        private DataGridView grid = null!;
        private Panel overlay = null!;

        public UsersPanelModule(Store store)
        {
            _store = store;

            DoubleBuffered = true;
            SetStyle(ControlStyles.AllPaintingInWmPaint |
                     ControlStyles.UserPaint |
                     ControlStyles.OptimizedDoubleBuffer, true);

            InitializeLayout();
        }

        private void InitializeLayout()
        {
            BackColor = Color.FromArgb(1, 0, 0, 0);

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
                Size = new Size(900, 500),
                BackColor = Color.FromArgb(235, 255, 255, 255),
                Padding = new Padding(25)
            };
            panelCard.Paint += PanelCard_Paint;
            panelRoot.Controls.Add(panelCard);

            panelRoot.Resize += (s, e) =>
            {
                panelCard.Left = (panelRoot.ClientSize.Width - panelCard.Width) / 2;
            };

            panelCard.Left = (panelRoot.ClientSize.Width - panelCard.Width) / 2;
            panelCard.Top = 40;

            var lblTitle = new Label
            {
                Text = "Administrar empleados",
                Font = new Font("Segoe UI", 20, FontStyle.Bold),
                ForeColor = Color.FromArgb(40, 40, 40),
                AutoSize = true,
                Location = new Point(10, 5)
            };
            panelCard.Controls.Add(lblTitle);

            CreateGrid();
            LoadEmployees();
        }

        private void CreateGrid()
        {
            grid = new DataGridView
            {
                Location = new Point(10, 60),
                Size = new Size(860, 400),
                BackgroundColor = Color.White,
                BorderStyle = BorderStyle.None,
                AllowUserToAddRows = false,
                AllowUserToResizeRows = false,
                ReadOnly = true,
                RowHeadersVisible = false,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                MultiSelect = false,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
            };

            grid.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(240, 240, 240);
            grid.ColumnHeadersDefaultCellStyle.ForeColor = Color.Black;
            grid.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            grid.EnableHeadersVisualStyles = false;

            grid.RowsDefaultCellStyle.BackColor = Color.White;
            grid.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(245, 245, 245);

            grid.CellClick += Grid_CellClick;

            grid.Columns.Add("Id", "ID");
            grid.Columns.Add("Name", "Nombre");
            grid.Columns.Add("Type", "Tipo");
            grid.Columns.Add("Phone", "Teléfono");
            grid.Columns.Add("Active", "Activo");

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

            panelCard.Controls.Add(grid);
        }

        private void LoadEmployees()
        {
            grid.Rows.Clear();

            foreach (var e in _store.Employees)
            {
                grid.Rows.Add(
                    e.Id,
                    e.GetFullName(),
                    e.Type.ToString(),
                    e.Phone,
                    e.Active ? "Sí" : "No"
                );
            }
        }

        private void Grid_CellClick(object? sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;

            int id = Convert.ToInt32(grid.Rows[e.RowIndex].Cells[0].Value);

            if (e.ColumnIndex == 5) // EDITAR
            {
                ShowEditModal(id);
            }
            else if (e.ColumnIndex == 6) // ELIMINAR
            {
                if (MessageBox.Show("¿Eliminar empleado?", "Confirmar",
                    MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
                {
                    _store.RemoveEmployee(id);
                    LoadEmployees();
                }
            }
        }

        private void ShowEditModal(int id)
        {
            var emp = _store.GetEmployee(id);
            if (emp == null) return;

            overlay = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.FromArgb(120, 0, 0, 0)
            };
            Controls.Add(overlay);
            overlay.BringToFront();

            var modal = new EditEmployeeModal(emp, _store);

            modal.Left = (overlay.Width - modal.Width) / 2;
            modal.Top = (overlay.Height - modal.Height) / 2;

            if (modal.Left < 0) modal.Left = 0;
            if (modal.Top < 0) modal.Top = 0;


            overlay.Controls.Add(modal);

            modal.OnClose += () =>
            {
                Controls.Remove(overlay);
                LoadEmployees();
            };
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
