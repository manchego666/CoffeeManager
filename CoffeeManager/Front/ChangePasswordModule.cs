using CoffeeManager.Services;
using CoffeeManager.Services.Logic;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace CoffeeManager.Front
{
    public class ChangePasswordModule : UserControl
    {
        private readonly LoginService _loginService = new();

        private Panel panelCard = null!;
        private TextBox txtCurrent = null!;
        private TextBox txtNew = null!;
        private TextBox txtConfirm = null!;
        private Button btnSave = null!;

        public ChangePasswordModule()
        {
            DoubleBuffered = true;
            Dock = DockStyle.Fill;
            BackColor = Color.Transparent;

            InitializeLayout();
        }

        private void InitializeLayout()
        {
            panelCard = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.FromArgb(235, 255, 255, 255),
                Padding = new Padding(25)
            };
            panelCard.Paint += PanelCard_Paint;
            Controls.Add(panelCard);

            var lblTitle = new Label
            {
                Text = "Cambiar contraseña",
                Font = new Font("Segoe UI", 26, FontStyle.Bold),
                ForeColor = Color.FromArgb(40, 40, 40),
                AutoSize = true,
                Location = new Point(10, 10)
            };
            panelCard.Controls.Add(lblTitle);

            int top = 80;

            txtCurrent = CreatePassword(panelCard, "Contraseña actual", 20, top);
            txtNew = CreatePassword(panelCard, "Nueva contraseña", 20, top + 60);
            txtConfirm = CreatePassword(panelCard, "Confirmar nueva contraseña", 20, top + 120);

            btnSave = new Button
            {
                Text = "Guardar cambios",
                Size = new Size(200, 45),
                Location = new Point(20, top + 200),
                BackColor = Color.FromArgb(70, 180, 160),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };
            btnSave.FlatAppearance.BorderSize = 0;
            btnSave.Click += SavePassword;
            panelCard.Controls.Add(btnSave);
        }

        private TextBox CreatePassword(Control parent, string label, int x, int y)
        {
            var lbl = new Label
            {
                Text = label,
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                ForeColor = Color.FromArgb(60, 60, 60),
                Location = new Point(x, y),
                AutoSize = true
            };
            parent.Controls.Add(lbl);

            var txt = new TextBox
            {
                Location = new Point(x, y + 20),
                Width = 260,
                Font = new Font("Segoe UI", 10),
                BorderStyle = BorderStyle.FixedSingle,
                UseSystemPasswordChar = true
            };
            parent.Controls.Add(txt);

            return txt;
        }

        private void SavePassword(object? sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtCurrent.Text) ||
                string.IsNullOrWhiteSpace(txtNew.Text) ||
                string.IsNullOrWhiteSpace(txtConfirm.Text))
            {
                MessageBox.Show("Completa todos los campos.", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (!_loginService.ValidatePassword(txtCurrent.Text))
            {
                MessageBox.Show("La contraseña actual es incorrecta.", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (txtNew.Text != txtConfirm.Text)
            {
                MessageBox.Show("Las contraseñas no coinciden.", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            _loginService.ChangePassword(txtNew.Text);

            MessageBox.Show("Contraseña actualizada correctamente.", "Éxito",
                MessageBoxButtons.OK, MessageBoxIcon.Information);

            txtCurrent.Clear();
            txtNew.Clear();
            txtConfirm.Clear();
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
