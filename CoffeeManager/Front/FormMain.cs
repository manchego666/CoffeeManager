// ===============================================================
//  ZORRODEV 2026 — Main Form Engine
//  Authors: Christopher (≧◡≦), Daniel (ง'̀-'́)ง, Brayan (✧ω✧), Jesús (◕‿◕✿)
//  Description: Main application window with animated hamburger menu,
//               dynamic module loading, real KPIs and live notifications.
// ===============================================================

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Windows.Forms;
using CoffeeManager.Models.Class;
using CoffeeManager.Services;
using CoffeeManager.Services.Logic;

// Force correct Timer type
using Timer = System.Windows.Forms.Timer;

namespace CoffeeManager.Front
{
    /// <summary>
    /// Main application window with animated hamburger menu,
    /// dynamic module loading, responsive dashboards, and fluid UI transitions.
    /// ZORRODEV 2026 — Premium Performance Edition.
    /// </summary>
    public class FormMain : Form
    {
        #region === DATA & SERVICES [ZORRODEV 2026] ===

        /// <summary>
        /// Central in-memory store for products, employees, sales and warehouse. (≧◡≦)
        /// </summary>
        private readonly Store _store = new();

        /// <summary>
        /// JSON helper for loading and saving data. (≧◡≦)
        /// </summary>
        private readonly JsonService _jsonService = new();

        /// <summary>
        /// Service used to generate daily TXT reports. (≧◡≦)
        /// </summary>
        private readonly DailyReportService _dailyReportService = new();

        /// <summary>
        /// Handles login persistence and validation. (≧◡≦)
        /// </summary>
        private readonly LoginService _loginService;

        /// <summary>
        /// Centralized notification engine for dashboard alerts. (✧ω✧)
        /// </summary>
        private readonly NotificationService _notificationService = new();

        #endregion

        #region === UI FIELDS [ZORRODEV 2026] ===

        private Panel panelMenu = null!;
        private Panel panelContent = null!;
        private FlowLayoutPanel rootPanel = null!;

        private Panel panelSubEmployees = null!;
        private Panel panelSubWarehouse = null!;
        private Panel panelSubProducts = null!;
        private Panel panelSubSales = null!;
        private Panel panelSubReports = null!;
        private Panel panelSubSettings = null!;

        private Button btnToggleMenu = null!;
        private Button btnHome = null!;
        private Button btnEmployees = null!;
        private Button btnWarehouse = null!;
        private Button btnProducts = null!;
        private Button btnSales = null!;
        private Button btnReports = null!;
        private Button btnSettings = null!;
        private Button btnExit = null!;

        private Timer slideTimer = null!;
        private bool isMenuExpanded = true;
        private bool isAnimating = false;

        private const int MenuExpandedWidth = 230;
        private const int MenuCollapsedWidth = 60;
        private const int AnimationStep = 25;

        #endregion

        #region === PERFORMANCE BOOSTERS [ZORRODEV 2026] ===

        /// <summary>
        /// Enables WS_EX_COMPOSITED to reduce flickering on complex layouts. (✧ω✧)
        /// </summary>
        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams cp = base.CreateParams;
                cp.ExStyle |= 0x02000000; // WS_EX_COMPOSITED (Elimina parpadeos por completo)
                return cp;
            }
        }

        #endregion

        #region === CONSTRUCTOR [ZORRODEV 2026] ===

        /// <summary>
        /// Initializes the main form, services, layout and dashboard. (≧◡≦)
        /// </summary>
        public FormMain()
        {
            Text = "CoffeeManager PRO";
            StartPosition = FormStartPosition.CenterScreen;

            // Professional wide default size
            MinimumSize = new Size(1280, 800);
            Size = new Size(1366, 840);

            SetStyle(ControlStyles.AllPaintingInWmPaint |
                     ControlStyles.UserPaint |
                     ControlStyles.OptimizedDoubleBuffer, true);
            DoubleBuffered = true;

            Icon = Properties.Resources.logo_ico;
            _loginService = new LoginService("Data/login.json");

            InitializeSlideTimer();
            InitializeLayout();

            // Load real data into Store and show real dashboard
            LoadStoreData();
            LoadDashboardModule();
        }

        #endregion

        #region === DATA LOADING (STORE + NOTIFICATIONS) ===

        /// <summary>
        /// Loads products, employees, sales and warehouse from JSON into the Store. (≧◡≦)
        /// </summary>
        private void LoadStoreData()
        {
            try
            {
                _store.Products = _jsonService.Load<Product>(PathService.Products);
                _store.Employees = _jsonService.Load<Employee>(PathService.Employees);
                _store.Sales = _jsonService.Load<Sale>(PathService.Sales);

                var warehouse = _jsonService.LoadObject<Warehouse>("Data/warehouse.json");
                if (warehouse != null)
                    _store.Warehouse = warehouse;
            }
            catch
            {
                // For the final project we keep it silent; could log in a real system.
            }
        }

        #endregion

        #region === INITIALIZE LAYOUT (MENU + CONTENT) ===

        /// <summary>
        /// Builds the left hamburger menu and the main content panel. (≧◡≦)
        /// </summary>
        private void InitializeLayout()
        {
            BackgroundImage = Properties.Resources.bg_main;
            BackgroundImageLayout = ImageLayout.Stretch;

            // === Left Menu Panel (Rosa Traslúcido Premium) ===
            panelMenu = new Panel
            {
                Dock = DockStyle.Left,
                Width = MenuExpandedWidth,
                BackColor = Color.FromArgb(235, 255, 230, 235) // Rosa suave traslúcido refinado
            };
            Controls.Add(panelMenu);

            // === Content Panel ===
            panelContent = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.Transparent,
                Padding = new Padding(30, 25, 30, 25) // Margen de seguridad física absoluto
            };
            Controls.Add(panelContent);

            int y = 15;

            // === Hamburger Button ===
            btnToggleMenu = new Button
            {
                Text = "☰",
                Width = MenuExpandedWidth - 20,
                Height = 40,
                Location = new Point(10, y),
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.FromArgb(240, 128, 148), // Tonalidad rosa del prototipo
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            btnToggleMenu.FlatAppearance.BorderSize = 0;
            btnToggleMenu.Click += (s, e) => ToggleMenu();
            panelMenu.Controls.Add(btnToggleMenu);

            y += 55;

            // === Main Navigation Buttons ===
            btnHome = CreateMainButton("Inicio", Properties.Resources.logo_icon, ref y);
            btnHome.Click += (s, e) =>
            {
                if (!isAnimating)
                {
                    HideAllSubmenus();
                    LoadDashboardModule();
                }
            };

            btnEmployees = CreateMainButton("Empleados", Properties.Resources.logo_icon, ref y);
            btnEmployees.Click += (s, e) => { if (isMenuExpanded && !isAnimating) ShowSubmenu(panelSubEmployees); };

            btnWarehouse = CreateMainButton("Almacén", Properties.Resources.icon_inventory, ref y);
            btnWarehouse.Click += (s, e) => { if (isMenuExpanded && !isAnimating) ShowSubmenu(panelSubWarehouse); };

            btnProducts = CreateMainButton("Productos", Properties.Resources.icon_inventory, ref y);
            btnProducts.Click += (s, e) => { if (isMenuExpanded && !isAnimating) ShowSubmenu(panelSubProducts); };

            btnSales = CreateMainButton("Ventas", Properties.Resources.icon_sales, ref y);
            btnSales.Click += (s, e) => { if (isMenuExpanded && !isAnimating) ShowSubmenu(panelSubSales); };

            btnReports = CreateMainButton("Reportes", Properties.Resources.icon_reports, ref y);
            btnReports.Click += (s, e) => { if (isMenuExpanded && !isAnimating) ShowSubmenu(panelSubReports); };

            btnSettings = CreateMainButton("Configuración", Properties.Resources.icon_settings, ref y);
            btnSettings.Click += (s, e) => { if (isMenuExpanded && !isAnimating) ShowSubmenu(panelSubSettings); };

            CreateSubmenus();

            // === Exit Button con Icono Limpio ===
            btnExit = new Button
            {
                Text = string.Empty,
                AccessibleName = "Salir",
                Tag = Properties.Resources.icon_exit,
                Width = MenuExpandedWidth - 20,
                Height = 40,
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.FromArgb(240, 240, 240),
                ForeColor = Color.FromArgb(40, 40, 40),
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                Location = new Point(10, panelMenu.Height - 55),
                Anchor = AnchorStyles.Left | AnchorStyles.Bottom,
                Cursor = Cursors.Hand
            };
            btnExit.FlatAppearance.BorderSize = 0;
            btnExit.Click += (s, e) => Close();

            btnExit.Paint += (sender, pe) =>
            {
                Button b = (Button)sender;
                pe.Graphics.SmoothingMode = SmoothingMode.HighQuality;
                Image img = b.Tag as Image;

                if (img != null)
                {
                    int iconSize = 22;
                    int iconY = (b.Height - iconSize) / 2;

                    if (b.Width <= MenuCollapsedWidth)
                    {
                        int iconX = (b.Width - iconSize) / 2;
                        pe.Graphics.DrawImage(img, iconX, iconY, iconSize, iconSize);
                    }
                    else
                    {
                        int iconX = 12;
                        pe.Graphics.DrawImage(img, iconX, iconY, iconSize, iconSize);

                        int textX = iconX + iconSize + 12;
                        using (Brush brush = new SolidBrush(b.ForeColor))
                        {
                            StringFormat sf = new StringFormat
                            {
                                LineAlignment = StringAlignment.Center,
                                Alignment = StringAlignment.Near
                            };
                            pe.Graphics.DrawString(b.AccessibleName, b.Font, brush,
                                new Rectangle(textX, 0, b.Width - textX, b.Height), sf);
                        }
                    }
                }
            };
            panelMenu.Controls.Add(btnExit);
        }

        /// <summary>
        /// Creates a main navigation button with icon + text and custom painting. (≧◡≦)
        /// </summary>
        private Button CreateMainButton(string text, Image icon, ref int y)
        {
            var btn = new Button
            {
                Text = string.Empty,
                AccessibleName = text,
                Tag = icon,
                Width = MenuExpandedWidth - 20,
                Height = 40,
                Location = new Point(10, y),
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.Transparent,
                ForeColor = Color.FromArgb(50, 50, 50),
                Font = new Font("Segoe UI", 11, FontStyle.Regular),
                Cursor = Cursors.Hand
            };
            btn.FlatAppearance.BorderSize = 0;

            btn.Paint += (sender, pe) =>
            {
                Button b = (Button)sender;
                pe.Graphics.SmoothingMode = SmoothingMode.HighQuality;

                Image img = b.Tag as Image;
                if (img != null)
                {
                    int iconSize = 22;
                    int iconY = (b.Height - iconSize) / 2;

                    if (b.Width <= MenuCollapsedWidth)
                    {
                        int iconX = (b.Width - iconSize) / 2;
                        pe.Graphics.DrawImage(img, iconX, iconY, iconSize, iconSize);
                    }
                    else
                    {
                        int iconX = 12;
                        pe.Graphics.DrawImage(img, iconX, iconY, iconSize, iconSize);

                        int textX = iconX + iconSize + 12;
                        using (Brush brush = new SolidBrush(b.ForeColor))
                        {
                            StringFormat sf = new StringFormat
                            {
                                LineAlignment = StringAlignment.Center,
                                Alignment = StringAlignment.Near
                            };
                            pe.Graphics.DrawString(b.AccessibleName, b.Font, brush,
                                new Rectangle(textX, 0, b.Width - textX, b.Height), sf);
                        }
                    }
                }
            };

            ApplyHover(btn);
            panelMenu.Controls.Add(btn);

            y += 45;
            return btn;
        }

        #endregion

        #region === DYNAMIC DASHBOARD (INICIO PERFECTO) ===

        /// <summary>
        /// Builds the main dashboard (Inicio) with real KPIs and notifications. (≧◡≦)
        /// </summary>
        private void LoadDashboardModule()
        {
            panelContent.Controls.Clear();

            // FlowLayoutPanel principal
            rootPanel = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                AutoScroll = true,
                Padding = new Padding(10, 5, 10, 10),
                BackColor = Color.Transparent
            };
            rootPanel.GetType()
                .GetProperty("DoubleBuffered",
                    System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic)
                ?.SetValue(rootPanel, true, null);

            // --- Header Section ---
            var panelHeader = new FlowLayoutPanel
            {
                Width = 1000,
                Height = 65,
                BackColor = Color.Transparent,
                FlowDirection = FlowDirection.LeftToRight,
                Margin = new Padding(0, 0, 0, 15)
            };

            var picHeaderLogo = new PictureBox
            {
                Image = Properties.Resources.logo_icon,
                SizeMode = PictureBoxSizeMode.Zoom,
                Size = new Size(55, 55),
                Margin = new Padding(0, 0, 15, 0)
            };

            var lblTitle = new Label
            {
                Text = "Inicio",
                Font = new Font("Segoe UI", 26, FontStyle.Bold),
                ForeColor = Color.White,
                AutoSize = true,
                Anchor = AnchorStyles.Left
            };

            // Refresh button (top-right of header)
            var btnRefresh = new Button
            {
                Text = "Actualizar",
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                Size = new Size(110, 32),
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.FromArgb(240, 128, 148),
                ForeColor = Color.White,
                Margin = new Padding(40, 18, 0, 0),
                Cursor = Cursors.Hand
            };
            btnRefresh.FlatAppearance.BorderSize = 0;
            btnRefresh.Click += (s, e) =>
            {
                LoadStoreData();
                LoadDashboardModule();
            };

            panelHeader.Controls.Add(picHeaderLogo);
            panelHeader.Controls.Add(lblTitle);
            panelHeader.Controls.Add(btnRefresh);
            rootPanel.SetFlowBreak(panelHeader, true);
            rootPanel.Controls.Add(panelHeader);

            // --- Subtítulo Resumen Diario ---
            var lblResumenTitle = new Label
            {
                Text = "Resumen Diario",
                Font = new Font("Segoe UI", 16, FontStyle.Bold),
                ForeColor = Color.White,
                Size = new Size(1000, 30),
                Margin = new Padding(0, 0, 0, 10)
            };
            rootPanel.SetFlowBreak(lblResumenTitle, true);
            rootPanel.Controls.Add(lblResumenTitle);

            // === REAL KPIs FROM STORE ===
            decimal ventasHoy = _store.Sales
                .Where(s => s.Date.Date == DateTime.Today)
                .Sum(s => s.Total);

            int productosActivos = _store.Products.Count;
            int recetas = _store.Products.Count(p => p.UsesWarehouse);
            int empleadosActivos = _store.Employees.Count(e => e.Active);

            // --- KPI CARDS ---
            var panelMetricsGroup = new FlowLayoutPanel
            {
                Width = 1020,
                Height = 115,
                Margin = new Padding(0, 0, 0, 30),
                BackColor = Color.Transparent
            };
            rootPanel.SetFlowBreak(panelMetricsGroup, true);

            panelMetricsGroup.Controls.Add(CreateKpiCard("Ventas Hoy", $"$ {ventasHoy:F2}", Color.FromArgb(240, 128, 148)));
            panelMetricsGroup.Controls.Add(CreateKpiCard("Productos", $"{productosActivos} Activos", Color.FromArgb(70, 180, 160)));
            panelMetricsGroup.Controls.Add(CreateKpiCard("Recetas", $"{recetas} Registros", Color.FromArgb(100, 150, 220)));
            panelMetricsGroup.Controls.Add(CreateKpiCard("Empleados", $"{empleadosActivos} Personal", Color.FromArgb(220, 160, 90)));
            rootPanel.Controls.Add(panelMetricsGroup);

            // --- ACCIONES RÁPIDAS ---
            var panelActions = new FlowLayoutPanel
            {
                Width = 460,
                Height = 320,
                BackColor = Color.Transparent,
                FlowDirection = FlowDirection.TopDown,
                Margin = new Padding(0, 0, 40, 0)
            };

            var lblActionsTitle = new Label
            {
                Text = "Acciones Rápidas",
                Font = new Font("Segoe UI", 15, FontStyle.Bold),
                ForeColor = Color.White,
                Size = new Size(450, 35)
            };
            panelActions.Controls.Add(lblActionsTitle);
            panelActions.Controls.Add(CreateQuickActionButton("Nuevo Pedido"));
            panelActions.Controls.Add(CreateQuickActionButton("Revisar Inventario"));
            panelActions.Controls.Add(CreateQuickActionButton("Corte de Caja"));
            rootPanel.Controls.Add(panelActions);

            // --- NOTIFICACIONES RECIENTES (REALES) ---
            var panelNotifications = new Panel
            {
                Width = 480,
                Height = 265,
                BackColor = Color.FromArgb(248, 248, 248),
                Padding = new Padding(15),
                Margin = new Padding(0, 5, 0, 0)
            };

            var lblNotifTitle = new Label
            {
                Text = "Notificaciones Recientes",
                Font = new Font("Segoe UI", 13, FontStyle.Bold),
                ForeColor = Color.Black,
                Location = new Point(15, 15),
                Size = new Size(250, 25)
            };
            panelNotifications.Controls.Add(lblNotifTitle);

            int notifY = 55;

            List<Notification> notifications = _notificationService.LoadNotifications()
                .OrderByDescending(n => n.Date)
                .Take(6)
                .ToList();

            if (notifications.Count == 0)
            {
                AddNotificationItem(panelNotifications,
                    "Sistema",
                    "No hay notificaciones registradas todavía. (≧◡≦)",
                    ref notifY);
            }
            else
            {
                foreach (var n in notifications)
                {
                    string title = $"{n.Title} — {n.Date:HH:mm}";
                    AddNotificationItem(panelNotifications, title, n.Detail, ref notifY);
                }
            }

            rootPanel.Controls.Add(panelNotifications);

            panelContent.Controls.Add(rootPanel);
        }

        /// <summary>
        /// Creates a KPI card with title, value and accent color. (≧◡≦)
        /// </summary>
        private Panel CreateKpiCard(string title, string value, Color accentColor)
        {
            var card = new Panel
            {
                Size = new Size(220, 105),
                BackColor = Color.White,
                Margin = new Padding(0, 0, 20, 0)
            };

            var borderLine = new Panel { Dock = DockStyle.Top, Height = 4, BackColor = accentColor };
            card.Controls.Add(borderLine);

            var lblValue = new Label
            {
                Text = value,
                Font = new Font("Segoe UI", 18, FontStyle.Bold),
                ForeColor = Color.Black,
                Location = new Point(10, 15),
                Size = new Size(200, 40),
                TextAlign = ContentAlignment.MiddleCenter
            };
            card.Controls.Add(lblValue);

            var lblTitle = new Label
            {
                Text = title,
                Font = new Font("Segoe UI", 10, FontStyle.Regular),
                ForeColor = Color.DimGray,
                Location = new Point(10, 60),
                Size = new Size(200, 25),
                TextAlign = ContentAlignment.MiddleCenter
            };
            card.Controls.Add(lblTitle);

            return card;
        }

        /// <summary>
        /// Creates a quick action button with hover effects. (≧◡≦)
        /// </summary>
        private Button CreateQuickActionButton(string text)
        {
            var btn = new Button
            {
                Text = text,
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                Size = new Size(450, 55),
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.White,
                ForeColor = Color.FromArgb(60, 60, 60),
                Margin = new Padding(0, 12, 0, 0),
                Cursor = Cursors.Hand
            };
            btn.FlatAppearance.BorderSize = 0;
            btn.Click += (s, e) =>
                MessageBox.Show($"{text} se encuentra actualmente en desarrollo.",
                    "Módulo", MessageBoxButtons.OK, MessageBoxIcon.Information);

            btn.MouseEnter += (s, e) => btn.BackColor = Color.FromArgb(240, 128, 148);
            btn.MouseLeave += (s, e) => btn.BackColor = Color.White;
            return btn;
        }

        /// <summary>
        /// Adds a notification row with icon + text to the notifications panel. (≧◡≦)
        /// </summary>
        private void AddNotificationItem(Panel parent, string title, string detail, ref int y)
        {
            var picIcon = new PictureBox
            {
                Image = Properties.Resources.logo_icon,
                Size = new Size(32, 32),
                Location = new Point(15, y + 4),
                SizeMode = PictureBoxSizeMode.Zoom
            };

            var lblItem = new Label
            {
                Text = $"• [{title}] {detail}",
                Font = new Font("Segoe UI", 9, FontStyle.Regular),
                ForeColor = Color.FromArgb(70, 70, 70),
                Location = new Point(55, y),
                Size = new Size(400, 40),
                TextAlign = ContentAlignment.MiddleLeft
            };

            parent.Controls.Add(picIcon);
            parent.Controls.Add(lblItem);
            y += 45;
        }

        #endregion

        #region === SUBMENUS ===

        /// <summary>
        /// Creates all submenu panels and their buttons. (≧◡≦)
        /// </summary>
        private void CreateSubmenus()
        {
            panelSubEmployees = CreateSubmenuPanelBelow(btnEmployees);
            AddSubButton(panelSubEmployees, "Agregar empleado", (s, e) => LoadPlaceholder("Agregar Empleado"));
            AddSubButton(panelSubEmployees, "Administrar empleados", (s, e) => LoadPlaceholder("Administrar Empleados"));

            panelSubWarehouse = CreateSubmenuPanelBelow(btnWarehouse);
            AddSubButton(panelSubWarehouse, "Inventario", (s, e) => LoadPlaceholder("Administrar Inventario"));
            AddSubButton(panelSubWarehouse, "Agregar insumo", (s, e) => LoadPlaceholder("Agregar Insumo"));
            AddSubButton(panelSubWarehouse, "Recetas", (s, e) => LoadPlaceholder("Recetas"));

            panelSubProducts = CreateSubmenuPanelBelow(btnProducts);
            AddSubButton(panelSubProducts, "Agregar producto", (s, e) => LoadPlaceholder("Agregar Producto"));
            AddSubButton(panelSubProducts, "Administrar productos", (s, e) => LoadPlaceholder("Administrar Productos"));

            panelSubSales = CreateSubmenuPanelBelow(btnSales);
            AddSubButton(panelSubSales, "Punto de venta", (s, e) => LoadPlaceholder("Punto de Venta"));
            AddSubButton(panelSubSales, "Historial de ventas", (s, e) => LoadPlaceholder("Historial de Ventas"));

            panelSubReports = CreateSubmenuPanelBelow(btnReports);
            AddSubButton(panelSubReports, "Reporte de ventas", (s, e) => LoadPlaceholder("Reporte de Ventas"));
            AddSubButton(panelSubReports, "Reporte de inventario", (s, e) => LoadPlaceholder("Reporte de Inventario"));
            AddSubButton(panelSubReports, "Reporte de pérdidas", (s, e) => LoadPlaceholder("Reporte de Pérdidas"));
            AddSubButton(panelSubReports, "Corte diario", (s, e) => GenerateDailyReport());

            panelSubSettings = CreateSubmenuPanelBelow(btnSettings);
            AddSubButton(panelSubSettings, "Información del negocio", (s, e) => LoadPlaceholder("Información del Negocio"));
            AddSubButton(panelSubSettings, "Cambiar contraseña", (s, e) => LoadPlaceholder("Cambiar Contraseña"));
            AddSubButton(panelSubSettings, "Preferencias", (s, e) => LoadPlaceholder("Preferencias"));

            HideAllSubmenus();
        }

        /// <summary>
        /// Creates a submenu panel positioned below a parent button. (≧◡≦)
        /// </summary>
        private Panel CreateSubmenuPanelBelow(Button parent)
        {
            var panel = new Panel
            {
                Width = MenuExpandedWidth - 20,
                AutoSize = true,
                Location = new Point(10, parent.Bottom),
                BackColor = Color.FromArgb(245, 245, 245)
            };
            panelMenu.Controls.Add(panel);
            panel.BringToFront();
            return panel;
        }

        /// <summary>
        /// Adds a submenu button to a submenu panel. (≧◡≦)
        /// </summary>
        private void AddSubButton(Panel parent, string text, EventHandler onClick)
        {
            int y = parent.Controls.Count * 35;

            var btn = new Button
            {
                Text = "    " + text,
                TextAlign = ContentAlignment.MiddleLeft,
                Width = parent.Width,
                Height = 30,
                Location = new Point(0, y),
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.FromArgb(245, 245, 245),
                ForeColor = Color.Black,
                Cursor = Cursors.Hand
            };
            btn.FlatAppearance.BorderSize = 0;
            ApplyHoverSub(btn);
            btn.Click += onClick;
            parent.Controls.Add(btn);
        }

        /// <summary>
        /// Shows a specific submenu and hides the others. (≧◡≦)
        /// </summary>
        private void ShowSubmenu(Panel submenu)
        {
            if (!submenu.Visible)
            {
                HideAllSubmenus();
                submenu.Visible = true;
                submenu.BringToFront();
            }
            else submenu.Visible = false;
        }

        /// <summary>
        /// Hides all submenu panels. (≧◡≦)
        /// </summary>
        private void HideAllSubmenus()
        {
            if (panelSubEmployees != null) panelSubEmployees.Visible = false;
            if (panelSubWarehouse != null) panelSubWarehouse.Visible = false;
            if (panelSubProducts != null) panelSubProducts.Visible = false;
            if (panelSubSales != null) panelSubSales.Visible = false;
            if (panelSubReports != null) panelSubReports.Visible = false;
            if (panelSubSettings != null) panelSubSettings.Visible = false;
        }

        #endregion

        #region === SLIDE MENU ANIMATION ===

        /// <summary>
        /// Initializes the slide animation timer. (≧◡≦)
        /// </summary>
        private void InitializeSlideTimer()
        {
            slideTimer = new Timer { Interval = 10 };
            slideTimer.Tick += SlideTick;
        }

        /// <summary>
        /// Starts the menu expand/collapse animation. (≧◡≦)
        /// </summary>
        private void ToggleMenu()
        {
            if (isAnimating) return;

            isAnimating = true;
            HideAllSubmenus();
            panelMenu.SuspendLayout();
            panelContent.SuspendLayout();
            slideTimer.Start();
        }

        /// <summary>
        /// Handles each animation step for the sliding menu. (≧◡≦)
        /// </summary>
        private void SlideTick(object? sender, EventArgs e)
        {
            if (isMenuExpanded)
            {
                panelMenu.Width -= AnimationStep;
                if (panelMenu.Width <= MenuCollapsedWidth)
                {
                    panelMenu.Width = MenuCollapsedWidth;
                    isMenuExpanded = false;
                    slideTimer.Stop();
                    CollapseMenu();
                    panelMenu.ResumeLayout();
                    panelContent.ResumeLayout(true);
                    isAnimating = false;
                }
            }
            else
            {
                panelMenu.Width += AnimationStep;
                if (panelMenu.Width >= MenuExpandedWidth)
                {
                    panelMenu.Width = MenuExpandedWidth;
                    isMenuExpanded = true;
                    slideTimer.Stop();
                    ExpandMenu();
                    panelMenu.ResumeLayout();
                    panelContent.ResumeLayout(true);
                    isAnimating = false;
                }
            }
        }

        /// <summary>
        /// Adjusts button sizes and positions for collapsed menu. (≧◡≦)
        /// </summary>
        private void CollapseMenu()
        {
            btnToggleMenu.Width = MenuCollapsedWidth - 16;
            btnToggleMenu.Location = new Point(8, btnToggleMenu.Location.Y);

            foreach (Control c in panelMenu.Controls)
            {
                if (c is Button btn)
                {
                    btn.Width = MenuCollapsedWidth - 16;
                    btn.Location = new Point(8, btn.Location.Y);
                    btn.Invalidate();
                }
            }
        }

        /// <summary>
        /// Adjusts button sizes and positions for expanded menu. (≧◡≦)
        /// </summary>
        private void ExpandMenu()
        {
            btnToggleMenu.Width = MenuExpandedWidth - 20;
            btnToggleMenu.Location = new Point(10, btnToggleMenu.Location.Y);

            foreach (Control c in panelMenu.Controls)
            {
                if (c is Button btn)
                {
                    btn.Width = MenuExpandedWidth - 20;
                    btn.Location = new Point(10, btn.Location.Y);
                    btn.Invalidate();
                }
            }
        }

        #endregion

        #region === MODULE LOADING ===

        /// <summary>
        /// Loads a module control into the main content panel. (≧◡≦)
        /// </summary>
        private void LoadModule(Control module)
        {
            panelContent.Controls.Clear();
            module.Dock = DockStyle.Fill;
            panelContent.Controls.Add(module);
        }

        /// <summary>
        /// Loads a simple placeholder panel for modules not implemented yet. (≧◡≦)
        /// </summary>
        private void LoadPlaceholder(string name)
        {
            var p = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.FromArgb(40, 40, 40)
            };

            var lbl = new Label
            {
                Text = $"{name}",
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 16, FontStyle.Bold),
                AutoSize = true,
                Location = new Point(40, 40)
            };
            p.Controls.Add(lbl);

            LoadModule(p);
        }

        #endregion

        #region === REPORTS ===

        /// <summary>
        /// Generates and saves a daily TXT report using DailyReportService. (≧◡≦)
        /// </summary>
        private void GenerateDailyReport()
        {
            using var dlg = new SaveFileDialog
            {
                Filter = "Text files|*.txt",
                FileName = $"daily_report_{DateTime.Now:yyyyMMdd}.txt"
            };

            if (dlg.ShowDialog() == DialogResult.OK)
            {
                _dailyReportService.SaveDailyReportToFile(_store, DateTime.Now, dlg.FileName);
                MessageBox.Show("¡Reporte diario generado correctamente!",
                    "Reporte", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        #endregion

        #region === HOVER EFFECTS ===

        /// <summary>
        /// Applies hover effect for main menu buttons. (≧◡≦)
        /// </summary>
        private void ApplyHover(Button btn)
        {
            btn.MouseEnter += (s, e) => btn.BackColor = Color.FromArgb(220, 255, 210, 220);
            btn.MouseLeave += (s, e) => btn.BackColor = Color.Transparent;
        }

        /// <summary>
        /// Applies hover effect for submenu buttons. (≧◡≦)
        /// </summary>
        private void ApplyHoverSub(Button btn)
        {
            btn.MouseEnter += (s, e) => btn.BackColor = Color.FromArgb(230, 230, 230);
            btn.MouseLeave += (s, e) => btn.BackColor = Color.FromArgb(245, 245, 245);
        }

        #endregion
    }
}
