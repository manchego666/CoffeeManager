using CoffeeManager.Services.Logic;
using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace CoffeeManager.Front
{
    /// <summary>
    /// CoffeeManager Professional Login Screen.
    /// Premium UX Edition: Features modern minimalist text inputs, organic soft-rounded card corners,
    /// and integrated brand iconography scaled dynamically for high-end displays.
    /// </summary>
    public class FormLogin : Form
    {
        #region === Private UI Fields ===
        private readonly LoginService _loginService;

        private Panel leftPanel;
        private Panel rightPanel;
        private Panel card;

        private TextBox txtUser;
        private TextBox txtPass;
        private Label lblError;
        private Label btnEye;
        private Button btnLogin;

        private Label btnClose;
        private Label btnMin;
        private PictureBox pbLogo; // Dedicated node for branding assets

        #endregion

        #region === Constructors ===

        public FormLogin()
        {
            InitializeForm();
            BuildUI();
            EnableDragRecursive(this);
        }

        public FormLogin(object parameter)
        {
            InitializeForm();
            BuildUI();
            EnableDragRecursive(this);
        }


        public FormLogin(LoginService loginService)
        {
            _loginService = loginService;

            InitializeForm();
            BuildUI();
            EnableDragRecursive(this);
        }

        #endregion

        #region === Form Configuration ===

        private void InitializeForm()
        {
            Text = "CoffeeManager Login";
            StartPosition = FormStartPosition.CenterScreen;
            FormBorderStyle = FormBorderStyle.None;
            Size = new Size(1200, 700);
            MinimumSize = new Size(1100, 650);
            DoubleBuffered = true;

            if (Properties.Resources.bg_login != null)
            {
                BackgroundImage = Properties.Resources.bg_login;
                BackgroundImageLayout = ImageLayout.Stretch;
            }
            else
            {
                BackColor = Color.FromArgb(30, 30, 45);
            }
        }

        #endregion

        #region === UI Construction ===

        private void BuildUI()
        {
            // 1. RIGHT PANEL: Standard workspace partition
            rightPanel = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.Transparent
            };
            Controls.Add(rightPanel);

            // 2. LEFT PANEL: Dynamic Dark Glass overlay 
            int calculatedLeftWidth = (int)(Width * 0.40);
            if (calculatedLeftWidth > 450) calculatedLeftWidth = 450;

            leftPanel = new Panel
            {
                Width = calculatedLeftWidth,
                Dock = DockStyle.Left,
                BackColor = Color.FromArgb(165, 22, 18, 30) // Softened deep dark chocolate overlay
            };
            Controls.Add(leftPanel);

            // --- Integrated Branding (Icon + Title) ---
            pbLogo = new PictureBox
            {
                Size = new Size(96, 96),
                Location = new Point(35, 75),
                SizeMode = PictureBoxSizeMode.Zoom,
                BackColor = Color.Transparent
            };

            // Safe Icon Assignment Loop
            if (Properties.Resources.logo_icon != null)
                pbLogo.Image = Properties.Resources.logo_icon;
            else if (System.IO.File.Exists("logo.icon"))
                pbLogo.Image = Image.FromFile("logo.icon");

            leftPanel.Controls.Add(pbLogo);

            var title = new Label
            {
                Text = "CoffeeManager",
                Font = new Font("Segoe UI", 30, FontStyle.Bold),
                ForeColor = Color.White,
                AutoSize = true,
                Location = new Point(145, 98), // Pushed to the right of the logo smoothly
                BackColor = Color.Transparent
            };
            leftPanel.Controls.Add(title);

            var subtitle = new Label
            {
                Text = "Administra tu cafetería de forma sencilla y profesional.",
                Font = new Font("Segoe UI", 13.5f, FontStyle.Regular),
                ForeColor = Color.FromArgb(235, 235, 245),
                Size = new Size(leftPanel.Width - 60, 65),
                Location = new Point(42, 175),
                BackColor = Color.Transparent
            };
            leftPanel.Controls.Add(subtitle);

            var welcome = new Label
            {
                Text = "Bienvenido.\nInicia sesión para gestionar ventas, inventario, recetas y reportes.",
                Font = new Font("Segoe UI", 11, FontStyle.Regular),
                ForeColor = Color.FromArgb(185, 185, 195),
                Size = new Size(leftPanel.Width - 60, 80),
                Location = new Point(42, 255),
                BackColor = Color.Transparent
            };
            leftPanel.Controls.Add(welcome);

            // Control Buttons
            btnClose = MakeWindowButton("✕");
            btnMin = MakeWindowButton("—");
            leftPanel.Controls.Add(btnClose);
            leftPanel.Controls.Add(btnMin);

            // 3. MAIN CARD VIEW
            BuildLoginCard();

            // RESPONSIVE RESIZING INTERFACES
            Resize += (s, e) =>
            {
                int newLeftWidth = (int)(Width * 0.40);
                if (newLeftWidth > 450) newLeftWidth = 450;
                if (newLeftWidth < 320) newLeftWidth = 320;

                leftPanel.Width = newLeftWidth;
                subtitle.Width = leftPanel.Width - 60;
                welcome.Width = leftPanel.Width - 60;

                PositionWindowButtons();
                CenterCard();
            };

            PositionWindowButtons();
            CenterCard();
        }

        private void BuildLoginCard()
        {
            // Panel Customization: Applies rounded corners automatically via Paint pipelines
            card = new Panel
            {
                Size = new Size(360, 430),
                BackColor = Color.White
            };

            // Native Graphics Path modification for beautiful rounded corners (Radius: 18)
            card.Paint += (s, e) =>
            {
                e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
                using (GraphicsPath path = new GraphicsPath())
                {
                    int radius = 18;
                    path.AddArc(0, 0, radius, radius, 180, 90);
                    path.AddArc(card.Width - radius, 0, radius, radius, 270, 90);
                    path.AddArc(card.Width - radius, card.Height - radius, radius, radius, 0, 90);
                    path.AddArc(0, card.Height - radius, radius, radius, 90, 90);
                    path.CloseAllFigures();
                    card.Region = new Region(path);
                }
            };
            rightPanel.Controls.Add(card);

            // Header Elements
            var h1 = new Label
            {
                Text = "Bienvenido",
                Font = new Font("Segoe UI", 22, FontStyle.Bold),
                ForeColor = Color.FromArgb(40, 40, 50),
                AutoSize = true,
                Location = new Point(35, 35),
                BackColor = Color.Transparent
            };
            card.Controls.Add(h1);

            var h2 = new Label
            {
                Text = "Ingresa tus credenciales de acceso",
                Font = new Font(new FontFamily("Segoe UI"), 9.5f, FontStyle.Regular), // Corregido a FontFamily y Regular
                ForeColor = Color.DarkGray,
                AutoSize = true,
                Location = new Point(37, 78),
                BackColor = Color.Transparent
            };
            card.Controls.Add(h2);

            // Input: Username (Minimalist Material Look)
            var lblUser = new Label
            {
                Text = "Usuario",
                Font = new Font("Segoe UI", 9.5f, FontStyle.Bold),
                ForeColor = Color.FromArgb(120, 120, 135),
                AutoSize = true,
                Location = new Point(35, 130),
                BackColor = Color.Transparent
            };
            card.Controls.Add(lblUser);

            txtUser = new TextBox
            {
                Width = 290,
                Font = new Font("Segoe UI", 11.5f),
                Location = new Point(35, 155),
                BorderStyle = BorderStyle.None,
                BackColor = Color.White
            };
            card.Controls.Add(txtUser);

            // Custom bottom line border for Username
            Panel lineUser = new Panel { Size = new Size(290, 2), Location = new Point(35, 180), BackColor = Color.FromArgb(210, 210, 220) };
            txtUser.GotFocus += (s, e) => lineUser.BackColor = Color.FromArgb(240, 128, 148); // Highlights pink on active focus
            txtUser.LostFocus += (s, e) => lineUser.BackColor = Color.FromArgb(210, 210, 220);
            card.Controls.Add(lineUser);

            // Input: Password (Minimalist Material Look)
            var lblPass = new Label
            {
                Text = "Contraseña",
                Font = new Font("Segoe UI", 9.5f, FontStyle.Bold),
                ForeColor = Color.FromArgb(120, 120, 135),
                AutoSize = true,
                Location = new Point(35, 210),
                BackColor = Color.Transparent
            };
            card.Controls.Add(lblPass);

            txtPass = new TextBox
            {
                Width = 260, // Left room for the clean eye icon toggle
                Font = new Font("Segoe UI", 11.5f),
                Location = new Point(35, 235),
                BorderStyle = BorderStyle.None,
                UseSystemPasswordChar = true,
                BackColor = Color.White
            };
            card.Controls.Add(txtPass);

            // Custom bottom line border for Password
            Panel linePass = new Panel { Size = new Size(290, 2), Location = new Point(35, 260), BackColor = Color.FromArgb(210, 210, 220) };
            txtPass.GotFocus += (s, e) => linePass.BackColor = Color.FromArgb(240, 128, 148);
            txtPass.LostFocus += (s, e) => linePass.BackColor = Color.FromArgb(210, 210, 220);
            card.Controls.Add(linePass);

            // Interactive Eye Feature
            btnEye = new Label
            {
                Text = "👁",
                Font = new Font("Segoe UI Emoji", 12),
                AutoSize = true,
                Cursor = Cursors.Hand,
                Location = new Point(300, 233),
                BackColor = Color.Transparent,
                ForeColor = Color.LightGray
            };
            btnEye.Click += (_, __) =>
            {
                txtPass.UseSystemPasswordChar = !txtPass.UseSystemPasswordChar;
                btnEye.ForeColor = txtPass.UseSystemPasswordChar ? Color.LightGray : Color.FromArgb(240, 128, 148);
            };
            card.Controls.Add(btnEye);

            // Login Button (Fluid Pastel Soft Corner Layout)
            btnLogin = new Button
            {
                Text = "Ingresar",
                Width = 290,
                Height = 44,
                Location = new Point(35, 310),
                BackColor = Color.FromArgb(240, 128, 148), // Iconic Pink Theme
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            btnLogin.FlatAppearance.BorderSize = 0;
            btnLogin.MouseEnter += (_, __) => btnLogin.BackColor = Color.FromArgb(225, 110, 130);
            btnLogin.MouseLeave += (_, __) => btnLogin.BackColor = Color.FromArgb(240, 128, 148);
            btnLogin.Click += LoginClick;
            card.Controls.Add(btnLogin);

            // Real-time Error Notification Node
            lblError = new Label
            {
                Text = "",
                ForeColor = Color.FromArgb(230, 75, 90),
                Font = new Font(new FontFamily("Segoe UI"), 9.5f, FontStyle.Bold), // Corregido a FontFamily y cambiado Medium por Bold
                TextAlign = ContentAlignment.MiddleCenter,
                Size = new Size(290, 30),
                Location = new Point(35, 370),
                BackColor = Color.Transparent
            };
        }

        #endregion

            #region === Responsive Layout Helpers ===

        private void CenterCard()
        {
            if (card != null && rightPanel != null)
            {
                card.Left = (rightPanel.Width - card.Width) / 2;
                card.Top = (rightPanel.Height - card.Height) / 2;
            }
        }

        private void PositionWindowButtons()
        {
            if (leftPanel != null && btnClose != null && btnMin != null)
            {
                btnClose.Location = new Point(leftPanel.Width - 45, 12);
                btnMin.Location = new Point(leftPanel.Width - 85, 12);
            }
        }

        private Label MakeWindowButton(string text)
        {
            var b = new Label
            {
                Text = text,
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                ForeColor = Color.FromArgb(200, 200, 210),
                Size = new Size(35, 35),
                TextAlign = ContentAlignment.MiddleCenter,
                Cursor = Cursors.Hand,
                BackColor = Color.Transparent
            };

            b.MouseEnter += (s, e) => b.ForeColor = Color.White;
            b.MouseLeave += (s, e) => b.ForeColor = Color.FromArgb(200, 200, 210);

            b.Click += (s, e) =>
            {
                if (text == "✕") Close();
                else WindowState = FormWindowState.Minimized;
            };

            return b;
        }

        #endregion

        #region === Authentication Logic (ZORRODEV2026) ===
        /// <summary>
        /// Handles login validation and opens the main application window. (≧◡≦)
        /// </summary>
        private void LoginClick(object sender, EventArgs e)
        {
            lblError.Text = "";

            if (string.IsNullOrWhiteSpace(txtUser.Text) || string.IsNullOrWhiteSpace(txtPass.Text))
            {
                lblError.Text = "Por favor, llene todos los campos.";
                return;
            }

            try
            {
                bool ok = _loginService.Validate(txtUser.Text, txtPass.Text);

                if (ok)
                {
                    Hide();
                    FormMain fMain = new FormMain();
                    fMain.FormClosed += (s, args) => Close();
                    fMain.Show();
                }
                else
                {
                    lblError.Text = "Usuario o contraseña incorrectos.";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("ERROR LOGIN: " + ex.Message);
            }

        }
        #endregion





        #region === Seamless Borderless Drag Framework ===

        private void EnableDragRecursive(Control c)
        {
            if (c is Form || c == leftPanel || c == rightPanel)
            {
                c.MouseDown += Drag;
            }

            foreach (Control child in c.Controls)
                EnableDragRecursive(child);
        }

        private void Drag(object sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Left) return;
            ReleaseCapture();
            SendMessage(Handle, 0xA1, (IntPtr)2, IntPtr.Zero);
        }

        [DllImport("user32.dll")] private static extern bool ReleaseCapture();
        [DllImport("user32.dll")] private static extern IntPtr SendMessage(IntPtr hWnd, int msg, IntPtr wParam, IntPtr lParam);

        #endregion
    }
}