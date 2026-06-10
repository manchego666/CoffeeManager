using System;
using System.Drawing;
using System.Windows.Forms;

namespace CoffeeManager.Front
{
    public class BusinessInfoModule : UserControl
    {
        private TextBox txtName = null!;
        private TextBox txtPhone = null!;
        private TextBox txtAddress = null!;
        private Button btnSave = null!;

        public BusinessInfoModule()
        {
            Dock = DockStyle.Fill;
            BackColor = Color.White;
            InitializeLayout();
        }

        private void InitializeLayout()
        {
            var title = new Label
            {
                Text = "Información del negocio",
                Font = new Font("Segoe UI", 26, FontStyle.Bold),
                Location = new Point(20, 20),
                AutoSize = true
            };
            Controls.Add(title);

            int top = 100;

            txtName = CreateField("Nombre del negocio", 20, top);
            txtPhone = CreateField("Teléfono", 20, top + 70);
            txtAddress = CreateField("Dirección", 20, top + 140);

            btnSave = new Button
            {
                Text = "Guardar",
                Size = new Size(150, 40),
                Location = new Point(20, top + 220),
                BackColor = Color.FromArgb(70, 180, 160),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };
            btnSave.Click += SaveInfo;
            Controls.Add(btnSave);
        }

        private TextBox CreateField(string label, int x, int y)
        {
            var lbl = new Label
            {
                Text = label,
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                Location = new Point(x, y),
                AutoSize = true
            };
            Controls.Add(lbl);

            var txt = new TextBox
            {
                Location = new Point(x, y + 22),
                Width = 300,
                Font = new Font("Segoe UI", 10)
            };
            Controls.Add(txt);

            return txt;
        }

        private void SaveInfo(object? sender, EventArgs e)
        {
            MessageBox.Show("Información guardada correctamente.", "Éxito",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
    }
}
