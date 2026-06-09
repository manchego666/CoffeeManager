using System;
using System.Drawing;
using System.Windows.Forms;

namespace CoffeeManager.Front
{
    /// <summary>
    /// Main application window for CoffeeManager.
    /// Provides a modern hamburger menu and module hosting. (✿◠‿◠) ZORRODEV2026
    /// </summary>
    public sealed class FormMain : Form
    {
        #region FIELDS

        private Panel panelMenu = null!;
        private Panel panelContent = null!;
        private Button btnToggle = null!;

        private Button btnSales = null!;
        private Button btnUsers = null!;
        private Button btnWarehouse = null!;
        private Button btnProducts = null!;
        private Button btnReports = null!;
        private Button btnSettings = null!;
        private Button btnExit = null!;

        private Panel panelUsersSubMenu = null!;

        private Button btnUserRegister = null!;
        private Button btnUserAdmin = null!;

        private bool menuOpen = true;

        #endregion

        #region CONSTRUCTOR

        /// <summary>
        /// Initializes a new instance of the main form.
        /// </summary>
        public FormMain()
        {
            InitializeForm();
            InitializeMenu();
            InitializeContent();
        }

        #endregion

        #region INITIALIZE FORM

        /// <summary>
        /// Configures base window properties and background.
        /// </summary>
        private void InitializeForm()
        {
            Text = "CoffeeManager — Panel Principal";
            WindowState = FormWindowState.Maximized;
            MinimumSize = new Size(1024, 600);

            // Background image from resources (anime coffee shop).
            BackgroundImage = Properties.Resources.bg_main;
            BackgroundImageLayout = ImageLayout.Stretch;

            DoubleBuffered = true;
        }

        #endregion

        #region INITIALIZE MENU

        /// <summary>
        /// Builds the left hamburger menu with main options and submenus.
        /// </summary>
        private void InitializeMenu()
        {
            panelMenu = new Panel
            {
                Width = 230,
                Dock = DockStyle.Left,
                BackColor = Color.FromArgb(40, 40, 40, 200) // semi-transparent
            };
            Controls.Add(panelMenu);

            btnToggle = new Button
            {
                Text = "☰",
                Width = 50,
                Height = 40,
                Location = new Point(10, 10),
                Font = new Font("Segoe UI", 16, FontStyle.Bold),
                BackColor = Color.FromArgb(60, 60, 60),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };
            btnToggle.FlatAppearance.BorderSize = 0;
            btnToggle.Click += ToggleMenu;
            ApplyHoverEffect(btnToggle);
            panelMenu.Controls.Add(btnToggle);

            int y = 70;

            // Main menu buttons
            btnSales = AddMenuButton("Ventas", y, () => LoadModule(new SalesModule())); y += 50;
            btnUsers = AddMenuButton("Usuarios ▼", y, ToggleUsersSubMenu); y += 50;
            btnWarehouse = AddMenuButton("Almacén", y, () => LoadModule(new WarehouseModule())); y += 50;
            btnProducts = AddMenuButton("Productos", y, () => LoadModule(new ProductsModule())); y += 50;
            btnReports = AddMenuButton("Reportes", y, () => LoadModule(new ReportsModule())); y += 50;
            btnSettings = AddMenuButton("Configuración", y, () => LoadModule(new SettingsModule())); y += 50;
            btnExit = AddMenuButton("Salir", y, () => Close());

            InitializeUsersSubMenu(btnUsers.Bottom);
        }

        /// <summary>
        /// Creates a main menu button with common styling and hover effects.
        /// </summary>
        private Button AddMenuButton(string text, int y, Action onClick)
        {
            var btn = new Button
            {
                Text = text,
                Width = 200,
                Height = 40,
                Location = new Point(15, y),
                Font = new Font("Segoe UI", 11, FontStyle.Bold),
                BackColor = Color.FromArgb(60, 60, 60, 200),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                TextAlign = ContentAlignment.MiddleLeft,
                Padding = new Padding(10, 0, 0, 0)
            };

            btn.FlatAppearance.BorderSize = 0;
            btn.Click += (s, e) => onClick();
            ApplyHoverEffect(btn);

            panelMenu.Controls.Add(btn);
            return btn;
        }

        /// <summary>
        /// Applies a simple hover/press visual effect to a button.
        /// </summary>
        private void ApplyHoverEffect(Button btn)
        {
            btn.MouseEnter += (s, e) =>
            {
                btn.BackColor = Color.FromArgb(80, 80, 80, 220);
            };
            btn.MouseLeave += (s, e) =>
            {
                btn.BackColor = Color.FromArgb(60, 60, 60, 200);
            };
            btn.MouseDown += (s, e) =>
            {
                btn.BackColor = Color.FromArgb(100, 100, 100, 230);
            };
            btn.MouseUp += (s, e) =>
            {
                btn.BackColor = Color.FromArgb(80, 80, 80, 220);
            };
        }

        #endregion

        #region USERS SUBMENU

        /// <summary>
        /// Creates the collapsible submenu for user-related actions.
        /// </summary>
        private void InitializeUsersSubMenu(int startY)
        {
            panelUsersSubMenu = new Panel
            {
                Width = 200,
                Height = 0,
                Location = new Point(30, startY),
                BackColor = Color.FromArgb(30, 30, 30, 200),
                Visible = false
            };
            panelMenu.Controls.Add(panelUsersSubMenu);

            int y = 5;

            btnUserRegister = AddSubMenuButton(panelUsersSubMenu, "Registrar usuario", y,
                () => LoadModule(new UsersModule()));
            y += 35;

            btnUserAdmin = AddSubMenuButton(panelUsersSubMenu, "Administrar usuarios", y,
                () => LoadModule(new UsersPanelModule()));
        }

        /// <summary>
        /// Creates a submenu button inside a given panel.
        /// </summary>
        private Button AddSubMenuButton(Panel parent, string text, int y, Action onClick)
        {
            var btn = new Button
            {
                Text = text,
                Width = parent.Width - 10,
                Height = 30,
                Location = new Point(5, y),
                Font = new Font("Segoe UI", 10, FontStyle.Regular),
                BackColor = Color.FromArgb(50, 50, 50, 220),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                TextAlign = ContentAlignment.MiddleLeft,
                Padding = new Padding(15, 0, 0, 0)
            };

            btn.FlatAppearance.BorderSize = 0;
            btn.Click += (s, e) => onClick();
            ApplyHoverEffect(btn);

            parent.Controls.Add(btn);
            return btn;
        }

        /// <summary>
        /// Expands or collapses the Users submenu.
        /// </summary>
        private void ToggleUsersSubMenu()
        {
            panelUsersSubMenu.Visible = !panelUsersSubMenu.Visible;
            panelUsersSubMenu.Height = panelUsersSubMenu.Visible ? 80 : 0;
        }

        #endregion

        #region TOGGLE MENU

        /// <summary>
        /// Collapses or expands the left menu width.
        /// </summary>
        private void ToggleMenu(object? sender, EventArgs e)
        {
            menuOpen = !menuOpen;
            panelMenu.Width = menuOpen ? 230 : 60;

            foreach (Control c in panelMenu.Controls)
            {
                if (c is Button btn && btn != btnToggle)
                {
                    btn.TextAlign = menuOpen ? ContentAlignment.MiddleLeft : ContentAlignment.MiddleCenter;
                    btn.Padding = menuOpen ? new Padding(10, 0, 0, 0) : new Padding(0);
                    if (btn == btnUsers)
                        btn.Text = menuOpen ? "Usuarios ▼" : "U";
                    if (btn == btnSales)
                        btn.Text = menuOpen ? "Ventas" : "V";
                    if (btn == btnWarehouse)
                        btn.Text = menuOpen ? "Almacén" : "A";
                    if (btn == btnProducts)
                        btn.Text = menuOpen ? "Productos" : "P";
                    if (btn == btnReports)
                        btn.Text = menuOpen ? "Reportes" : "R";
                    if (btn == btnSettings)
                        btn.Text = menuOpen ? "Configuración" : "C";
                    if (btn == btnExit)
                        btn.Text = menuOpen ? "Salir" : "X";
                }
            }

            panelUsersSubMenu.Visible = menuOpen && panelUsersSubMenu.Visible;
        }

        #endregion

        #region CONTENT PANEL

        /// <summary>
        /// Creates the central content panel where modules are loaded.
        /// </summary>
        private void InitializeContent()
        {
            panelContent = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.FromArgb(20, 20, 20, 120) // soft transparent overlay
            };
            Controls.Add(panelContent);
            panelContent.BringToFront();
        }

        /// <summary>
        /// Replaces the current content with the specified module.
        /// </summary>
        /// <param name="module">Control representing the module to display.</param>
        private void LoadModule(Control module)
        {
            panelContent.Controls.Clear();
            module.Dock = DockStyle.Fill;
            panelContent.Controls.Add(module);
        }

        #endregion
    }
}
