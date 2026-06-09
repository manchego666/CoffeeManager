using System;
using System.Drawing;
using System.Drawing.Drawing2D;
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

        private readonly Store _store = new();
        private readonly JsonService _jsonService = new();
        private readonly DailyReportService _dailyReportService = new();
        private readonly LoginService _loginService;

        #endregion

        #region === UI FIELDS [ZORRODEV 2026] ===

        private Panel panelMenu;
        private Panel panelContent;
        private FlowLayoutPanel rootPanel;

        private Panel panelSubEmployees;
        private Panel panelSubWarehouse;
        private Panel panelSubProducts;
        private Panel panelSubSales;
        private Panel panelSubReports;
        private Panel panelSubSettings;

        private Button btnToggleMenu;
        private Button btnHome;
        private Button btnEmployees;
        private Button btnWarehouse;
        private Button btnProducts;
        private Button btnSales;
        private Button btnReports;
        private Button btnSettings;
        private Button btnExit;

        private Timer slideTimer;
        private bool isMenuExpanded = true;
        private bool isAnimating = false;

        private const int MenuExpandedWidth = 230;
        private const int MenuCollapsedWidth = 60;
        private const int AnimationStep = 25;

        #endregion

        #region === PERFORMANCE BOOSTERS [ZORRODEV 2026] ===

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

        public FormMain()
        {
            Text = "CoffeeManager PRO";
            StartPosition = FormStartPosition.CenterScreen;

            // Incrementamos el tamaño por defecto del Form para dar un aspecto de pantalla profesional amplia
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
        }

        #endregion

        #region === INITIALIZE LAYOUT (MENU + CONTENT) ===

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
            btnHome.Click += (s, e) => { if (!isAnimating) { HideAllSubmenus(); LoadDashboardModule(); } };

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
                            pe.Graphics.DrawString(b.AccessibleName, b.Font, brush, new Rectangle(textX, 0, b.Width - textX, b.Height), sf);
                        }
                    }
                }
            };
            panelMenu.Controls.Add(btnExit);

            LoadDashboardModule();
        }

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
                BackColor = Color.Transparent, // Botones limpios que adoptan el color rosa traslúcido de fondo
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
                            pe.Graphics.DrawString(b.AccessibleName, b.Font, brush, new Rectangle(textX, 0, b.Width - textX, b.Height), sf);
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

        private void LoadDashboardModule()
        {
            panelContent.Controls.Clear();

            // FlowLayoutPanel principal configurado con anclajes estables a la derecha
            rootPanel = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                AutoScroll = true,
                Padding = new Padding(10, 5, 10, 10),
                BackColor = Color.Transparent
            };
            rootPanel.GetType().GetProperty("DoubleBuffered", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic)?.SetValue(rootPanel, true, null);

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

            panelHeader.Controls.Add(picHeaderLogo);
            panelHeader.Controls.Add(lblTitle);
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

            // --- KPI CARDS (Tamaños exactos fijos idénticos a tu mockup) ---
            var panelMetricsGroup = new FlowLayoutPanel
            {
                Width = 1020,
                Height = 115,
                Margin = new Padding(0, 0, 0, 30),
                BackColor = Color.Transparent
            };
            rootPanel.SetFlowBreak(panelMetricsGroup, true);

            panelMetricsGroup.Controls.Add(CreateKpiCard("Ventas Hoy", "$ 12,450.00", Color.FromArgb(240, 128, 148)));
            panelMetricsGroup.Controls.Add(CreateKpiCard("Productos", "148 Activos", Color.FromArgb(70, 180, 160)));
            panelMetricsGroup.Controls.Add(CreateKpiCard("Recetas", "32 Registros", Color.FromArgb(100, 150, 220)));
            panelMetricsGroup.Controls.Add(CreateKpiCard("Empleados", "8 Personal", Color.FromArgb(220, 160, 90)));
            rootPanel.Controls.Add(panelMetricsGroup);

            // --- ACCIONES RÁPIDAS ---
            var panelActions = new FlowLayoutPanel
            {
                Width = 460,
                Height = 320,
                BackColor = Color.Transparent,
                FlowDirection = FlowDirection.TopDown,
                Margin = new Padding(0, 0, 40, 0) // Espacio a la derecha antes de las notificaciones
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

            // --- NOTIFICACIONES RECIENTES ---
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
            AddNotificationItem(panelNotifications, "Alerta de Stock", "Insumo 'Leche Entera' por debajo del mínimo.", ref notifY);
            AddNotificationItem(panelNotifications, "Asistencia", "Empleado Carlos Mendoza inició turno.", ref notifY);
            AddNotificationItem(panelNotifications, "Sistema", "Copia de seguridad realizada con éxito.", ref notifY);
            rootPanel.Controls.Add(panelNotifications);

            panelContent.Controls.Add(rootPanel);
        }

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
            btn.Click += (s, e) => MessageBox.Show($"{text} se encuentra actualmente en desarrollo.", "Módulo", MessageBoxButtons.OK, MessageBoxIcon.Information);

            btn.MouseEnter += (s, e) => btn.BackColor = Color.FromArgb(240, 128, 148);
            btn.MouseLeave += (s, e) => btn.BackColor = Color.White;
            return btn;
        }

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

        private void InitializeSlideTimer()
        {
            slideTimer = new Timer { Interval = 10 };
            slideTimer.Tick += SlideTick;
        }

        private void ToggleMenu()
        {
            if (isAnimating) return;

            isAnimating = true;
            HideAllSubmenus();
            panelMenu.SuspendLayout();
            panelContent.SuspendLayout();
            slideTimer.Start();
        }

        private void SlideTick(object sender, EventArgs e)
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

        private void LoadModule(Control module)
        {
            panelContent.Controls.Clear();
            module.Dock = DockStyle.Fill;
            panelContent.Controls.Add(module);
        }

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

        private void ApplyHover(Button btn)
        {
            btn.MouseEnter += (s, e) => btn.BackColor = Color.FromArgb(220, 255, 210, 220);
            btn.MouseLeave += (s, e) => btn.BackColor = Color.Transparent;
        }

        private void ApplyHoverSub(Button btn)
        {
            btn.MouseEnter += (s, e) => btn.BackColor = Color.FromArgb(230, 230, 230);
            btn.MouseLeave += (s, e) => btn.BackColor = Color.FromArgb(245, 245, 245);
        }

        #endregion
    }
}