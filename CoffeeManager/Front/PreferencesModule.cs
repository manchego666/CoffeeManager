using System;
using System.Drawing;
using System.Windows.Forms;

namespace CoffeeManager.Front
{
    public class PreferencesModule : UserControl
    {
        private CheckBox chkDarkMode = null!;
        private CheckBox chkConfirmDelete = null!;
        private NumericUpDown numLowStock = null!;
        private Button btnSave = null!;

        public PreferencesModule()
        {
            Dock = DockStyle.Fill;
            BackColor = Color.White;
            InitializeLayout();
        }

        private void InitializeLayout()
        {
            var title = new Label
            {
                Text = "Preferencias",
                Font = new Font("Segoe UI", 26, FontStyle.Bold),
                Location = new Point(20, 20),
                AutoSize = true
            };
            Controls.Add(title);

            int top = 100;

            chkDarkMode = new CheckBox
            {
                Text = "Modo oscuro",
                Font = new Font("Segoe UI", 10),
                Location = new Point(20, top),
                AutoSize = true
            };
            Controls.Add(chkDarkMode);

            chkConfirmDelete = new CheckBox
            {
                Text = "Confirmar antes de eliminar",
                Font = new Font("Segoe UI", 10),
                Location = new Point(20, top + 40),
                AutoSize = true
            };
            Controls.Add(chkConfirmDelete);

            var lblLowStock = new Label
            {
                Text = "Nivel de stock bajo:",
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                Location = new Point(20, top + 90),
                AutoSize = true
            };
            Controls.Add(lblLowStock);

            numLowStock = new NumericUpDown
            {
                Location = new Point(20, top + 115),
                Width = 80,
                Minimum = 1,
                Maximum = 100,
                Value = 5
            };
            Controls.Add(numLowStock);

            btnSave = new Button
            {
                Text = "Guardar",
                Size = new Size(150, 40),
                Location = new Point(20, top + 180),
                BackColor = Color.FromArgb(70, 180, 160),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };
            btnSave.Click += SavePreferences;
            Controls.Add(btnSave);
        }

        private void SavePreferences(object? sender, EventArgs e)
        {
            MessageBox.Show("Preferencias guardadas.", "Éxito",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
    }
}
