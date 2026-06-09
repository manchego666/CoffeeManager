using System;
using System.Drawing;
using System.Windows.Forms;

namespace CoffeeManager.Front
{
    /// <summary>
    /// Settings module for user configuration such as password change.
    /// </summary>
    public class SettingsModule : Panel
    {
        #region Controls
        private Label lblTitle = null!;
        private Label lblOldPass = null!;
        private Label lblNewPass = null!;
        private Label lblConfirm = null!;
        private TextBox txtOldPass = null!;
        private TextBox txtNewPass = null!;
        private TextBox txtConfirm = null!;
        private Button btnSave = null!;
        #endregion

        #region Constructor
        public SettingsModule()
        {
            InitializeModule();
        }
        #endregion

        #region Initialize
        /// <summary>
        /// Initializes the visual structure of the Settings module.
        /// </summary>
        private void InitializeModule()
        {
            Dock = DockStyle.Fill;
            BackColor = Color.FromArgb(40, 40, 40, 180);

            lblTitle = new Label
            {
                Text = "Configuración — Cambiar contraseña",
                Font = new Font("Segoe UI", 18, FontStyle.Bold),
                ForeColor = Color.White,
                AutoSize = true,
                Location = new Point(40, 40)
            };
            Controls.Add(lblTitle);

            int y = 120;

            lblOldPass = CreateLabel("Contraseña actual:", ref y);
            txtOldPass = CreateTextBox(ref y);

            lblNewPass = CreateLabel("Nueva contraseña:", ref y);
            txtNewPass = CreateTextBox(ref y);

            lblConfirm = CreateLabel("Confirmar contraseña:", ref y);
            txtConfirm = CreateTextBox(ref y);

            btnSave = new Button
            {
                Text = "Guardar cambios",
                Width = 250,
                Height = 45,
                Location = new Point(40, y + 20),
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                BackColor = Color.FromArgb(255, 140, 160),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };
            btnSave.FlatAppearance.BorderSize = 0;
            btnSave.Click += SavePassword;
            Controls.Add(btnSave);
        }
        #endregion

        #region Helpers
        private Label CreateLabel(string text, ref int y)
        {
            var lbl = new Label
            {
                Text = text,
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                ForeColor = Color.White,
                AutoSize = true,
                Location = new Point(40, y)
            };
            Controls.Add(lbl);
            y += 35;
            return lbl;
        }

        private TextBox CreateTextBox(ref int y)
        {
            var txt = new TextBox
            {
                Width = 300,
                Location = new Point(40, y),
                Font = new Font("Segoe UI", 12),
                BackColor = Color.FromArgb(60, 60, 60),
                ForeColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle,
                UseSystemPasswordChar = true
            };
            Controls.Add(txt);
            y += 45;
            return txt;
        }
        #endregion

        #region Events
        private void SavePassword(object? sender, EventArgs e)
        {
            // TODO: Add password validation and saving logic
            MessageBox.Show("Contraseña actualizada correctamente.", "Éxito",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        #endregion
    }
}
