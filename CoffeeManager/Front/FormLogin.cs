using System;
using System.Drawing;
using System.Windows.Forms;
using CoffeeManager.Services.Logic;

namespace CoffeeManager.Front
{
    public sealed class FormLogin : Form
    {
        private readonly LoginService _loginService;

        private Panel panelCard;
        private Label lblTitle;
        private Label lblUser;
        private Label lblPass;
        private TextBox txtUser;
        private TextBox txtPass;
        private Button btnLogin;

        public FormLogin(LoginService loginService)
        {
            _loginService = loginService;

            InitializeForm();
            InitializeControls();
        }

        private void InitializeForm()
        {
            Text = "CoffeeManager — Login";
            Size = new Size(480, 580);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            StartPosition = FormStartPosition.CenterScreen;

            BackgroundImage = Properties.Resources.bg_login;
            BackgroundImageLayout = ImageLayout.Stretch;

            DoubleBuffered = true;
        }

        private void InitializeControls()
        {
            // Panel estilo "glass" sin transparencia real (compatibilidad total)
            panelCard = new Panel
            {
                Width = 350,
                Height = 360,
                BackColor = Color.FromArgb(50, 50, 50), // gris oscuro suave
                Location = new Point((ClientSize.Width - 350) / 2, 150)
            };
            Controls.Add(panelCard);

            lblTitle = new Label
            {
                Text = "Iniciar sesión",
                Font = new Font("Segoe UI", 20, FontStyle.Bold),
                ForeColor = Color.FromArgb(255, 200, 220),
                AutoSize = false,
                TextAlign = ContentAlignment.MiddleCenter,
                Dock = DockStyle.Top,
                Height = 70
            };
            panelCard.Controls.Add(lblTitle);

            lblUser = new Label
            {
                Text = "ID",
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                ForeColor = Color.FromArgb(255, 180, 200),
                Location = new Point(30, 85),
                AutoSize = true
            };
            panelCard.Controls.Add(lblUser);

            txtUser = CreateTextBox(105);

            lblPass = new Label
            {
                Text = "PASS",
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                ForeColor = Color.FromArgb(255, 180, 200),
                Location = new Point(30, 165),
                AutoSize = true
            };
            panelCard.Controls.Add(lblPass);

            txtPass = CreateTextBox(185);
            txtPass.PasswordChar = '•';

            btnLogin = new Button
            {
                Text = "Ingresar",
                Width = 280,
                Height = 48,
                Location = new Point(35, 260),
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                BackColor = Color.FromArgb(255, 140, 160),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };

            btnLogin.FlatAppearance.BorderSize = 0;
            btnLogin.FlatAppearance.MouseOverBackColor = Color.FromArgb(255, 160, 180);
            btnLogin.FlatAppearance.MouseDownBackColor = Color.FromArgb(230, 120, 140);

            btnLogin.Click += (s, e) => Login();

            panelCard.Controls.Add(btnLogin);
        }

        private TextBox CreateTextBox(int y)
        {
            var txt = new TextBox
            {
                Width = 280,
                Height = 30,
                Location = new Point(30, y),
                Font = new Font("Segoe UI", 12),
                ForeColor = Color.White,
                BackColor = Color.FromArgb(70, 70, 70), // gris suave (simula transparencia)
                BorderStyle = BorderStyle.None
            };

            var underline = new Panel
            {
                Width = 280,
                Height = 2,
                Location = new Point(30, y + 30),
                BackColor = Color.FromArgb(255, 180, 200)
            };

            panelCard.Controls.Add(underline);
            return txt;
        }

        private void Login()
        {
            Hide();
            new FormMain().Show();
        }
    }
}
