using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using CoffeeManager.Models.Class;
using CoffeeManager.Services;
using CoffeeManager.Services.Logic;

// Timer correcto
using Timer = System.Windows.Forms.Timer;

namespace CoffeeManager.Front
{
    public class FormMain : Form
    {
        // ===========================================================
        //  REGION: DATA & SERVICES
        // ===========================================================
        private readonly Store _store = new();
        private readonly JsonService _jsonService = new();
        private readonly DailyReportService _dailyReportService = new();
        private readonly LoginService _loginService;
        private readonly NotificationService _notificationService = new();

        // ===========================================================
        //  REGION: UI FIELDS
        // ===========================================================
        private BufferedPanel panelMenu;
        private BufferedPanel panelContent;

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
        private const int AnimationStep = 40;

        // ===========================================================
        //  REGION: PERFORMANCE
        // ===========================================================
        protected override CreateParams CreateParams
        {
            get
            {
                // No WS_EX_COMPOSITED (causa paneles negros)
                return base.CreateParams;
            }
        }

        // ===========================================================
        //  REGION: CONSTRUCTOR
        // ===========================================================
        public FormMain()
        {
            Text = "CoffeeManager PRO";
            StartPosition = FormStartPosition.CenterScreen;
            MinimumSize = new Size(1280, 800);
            Size = new Size(1366, 840);

            // DoubleBuffer REAL
            SetStyle(ControlStyles.AllPaintingInWmPaint |
                     ControlStyles.UserPaint |
                     ControlStyles.OptimizedDoubleBuffer, true);

            DoubleBuffered = true;
            UpdateStyles();

            InitializeLayout();
            InitializeSlideTimer();

            typeof(Panel).InvokeMember("DoubleBuffered",
                System.Reflection.BindingFlags.SetProperty |
                System.Reflection.BindingFlags.Instance |
                System.Reflection.BindingFlags.NonPublic,
                null, panelContent, new object[] { true });

            LoadStoreData();
            LoadDashboardModule();
        }

        // ===========================================================
        //  REGION: INITIALIZE LAYOUT (OPTIMIZADO)
        // ===========================================================
        private void InitializeLayout()
        {
            // Fondo del formulario
            this.BackgroundImage = Properties.Resources.bg_main;
            this.BackgroundImageLayout = ImageLayout.Stretch;

            // ============================
            // PANEL MENU (IZQUIERDA)
            // ============================
            panelMenu = new BufferedPanel
            {
                Dock = DockStyle.Left,
                Width = MenuExpandedWidth,
                BackColor = Color.FromArgb(235, 255, 230, 235)
            };

            Controls.Add(panelMenu);
            panelMenu.BringToFront();

            // ============================
            // PANEL CONTENT (CENTRO)
            // ============================
            panelContent = new BufferedPanel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.Transparent,
                Padding = new Padding(0)
            };

            Controls.Add(panelContent);
            panelContent.BringToFront();

            // ============================
            // BOTÓN HAMBURGUESA
            // ============================
            int y = 15;

            btnToggleMenu = new Button
            {
                Text = "☰",
                Width = MenuExpandedWidth - 20,
                Height = 40,
                Location = new Point(10, y),
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.FromArgb(240, 128, 148),
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 14, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            btnToggleMenu.FlatAppearance.BorderSize = 0;
            btnToggleMenu.Click += (s, e) => ToggleMenu();
            panelMenu.Controls.Add(btnToggleMenu);

            y += 55;

            // ============================
            // BOTONES PRINCIPALES
            // ============================
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
            btnEmployees.Click += (s, e) =>
            {
                if (isMenuExpanded && !isAnimating)
                    ShowSubmenu(panelSubEmployees);
            };

            btnWarehouse = CreateMainButton("Almacén", Properties.Resources.icon_inventory, ref y);
            btnWarehouse.Click += (s, e) =>
            {
                if (isMenuExpanded && !isAnimating)
                    ShowSubmenu(panelSubWarehouse);
            };

            btnProducts = CreateMainButton("Productos", Properties.Resources.icon_inventory, ref y);
            btnProducts.Click += (s, e) =>
            {
                if (isMenuExpanded && !isAnimating)
                    ShowSubmenu(panelSubProducts);
            };

            btnSales = CreateMainButton("Ventas", Properties.Resources.icon_sales, ref y);
            btnSales.Click += (s, e) =>
            {
                if (isMenuExpanded && !isAnimating)
                    ShowSubmenu(panelSubSales);
            };

            btnReports = CreateMainButton("Reportes", Properties.Resources.icon_reports, ref y);
            btnReports.Click += (s, e) =>
            {
                if (isMenuExpanded && !isAnimating)
                    ShowSubmenu(panelSubReports);
            };

            btnSettings = CreateMainButton("Configuración", Properties.Resources.icon_settings, ref y);
            btnSettings.Click += (s, e) =>
            {
                if (isMenuExpanded && !isAnimating)
                    ShowSubmenu(panelSubSettings);
            };

            // Submenús
            CreateSubmenus();

            // ============================
            // BOTÓN SALIR
            // ============================
            btnExit = CreateExitButton(ref y);
            panelMenu.Controls.Add(btnExit); 
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
                    int iconSize = 32; // ICONOS GRANDES
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

                            pe.Graphics.DrawString(
                                b.AccessibleName,
                                b.Font,
                                brush,
                                new Rectangle(textX, 0, b.Width - textX, b.Height),
                                sf
                            );
                        }
                    }
                }
            };

            ApplyHover(btn);
            panelMenu.Controls.Add(btn);

            y += 45;
            return btn;
        }




        // ===========================================================
        //  REGION: DATA LOADING
        // ===========================================================
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
                // Silent fail
            }
        }

        // ===========================================================
        //  REGION: DASHBOARD MODULE (OPTIMIZADO)
        // ===========================================================
        private async void LoadDashboardModule()
        {
            panelContent.Controls.Clear();
            panelContent.BackColor = Color.FromArgb(1, 0, 0, 0); // Transparencia falsa


            rootPanel = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                AutoScroll = true,
                FlowDirection = FlowDirection.TopDown,
                WrapContents = false,
                Padding = new Padding(10, 5, 10, 10),
                BackColor = Color.FromArgb(1, 0, 0, 0)

            };

            var panelCenter = new FlowLayoutPanel
            {
                Width = 1100,
                AutoSize = true,
                FlowDirection = FlowDirection.TopDown,
                WrapContents = false,
                BackColor = Color.FromArgb(1, 0, 0, 0)
            };

            // ============================
            // HEADER
            // ============================
            var panelHeader = new FlowLayoutPanel
            {
                Width = 1100,
                Height = 120,
                BackColor = Color.Transparent
            };

            var picLogo = new PictureBox
            {
                Image = Properties.Resources.logo_icon,
                Size = new Size(110, 110),
                SizeMode = PictureBoxSizeMode.Zoom,
                Margin = new Padding(0, 0, 25, 0)
            };

            var lblTitle = new Label
            {
                Text = "Inicio",
                Font = new Font("Segoe UI", 38, FontStyle.Bold),
                ForeColor = Color.White,
                AutoSize = true,
                Margin = new Padding(10, 30, 20, 0)
            };

            var btnRefresh = new Button
            {
                Text = "Actualizar Dashboard",
                Size = new Size(200, 45),
                BackColor = Color.FromArgb(240, 128, 148),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand,
                Margin = new Padding(20, 40, 0, 0)
            };
            btnRefresh.FlatAppearance.BorderSize = 0;
            btnRefresh.Click += (s, e) =>
            {
                LoadStoreData();
                LoadDashboardModule();
            };

            panelHeader.Controls.AddRange(new Control[] { picLogo, lblTitle, btnRefresh });
            panelCenter.Controls.Add(panelHeader);

            // ============================
            // KPI DATA
            // ============================
            decimal ventasHoy = _store.Sales
                .Where(s => s.Date.Date == DateTime.Today)
                .Sum(s => s.Total);

            int productosActivos = _store.Products.Count;
            int recetas = _store.Products.Count(p => p.UsesWarehouse);
            int insumos = _store.Warehouse.Items.Count;

            int prodAgotados = _store.Products.Count(p => !p.UsesWarehouse && p.Stock <= 0);
            int ingAgotados = _store.Warehouse.Items.Count(i => i.IsOutOfStock());
            string stockTexto = (prodAgotados + ingAgotados == 0)
                ? "Sin bajos"
                : $"{prodAgotados + ingAgotados} Agotados";

            // ============================
            // KPI CARDS
            // ============================
            var flowMetrics = new FlowLayoutPanel
            {
                Width = 1100,
                AutoSize = true,
                BackColor = Color.Transparent,
                Margin = new Padding(0, 20, 0, 20)
            };

            flowMetrics.Controls.Add(CreateKpiCard("Ventas Hoy", $"$ {ventasHoy:F2}", Color.FromArgb(240, 128, 148)));
            flowMetrics.Controls.Add(CreateKpiCard("Productos Activos", $"{productosActivos}", Color.FromArgb(70, 180, 160)));
            flowMetrics.Controls.Add(CreateKpiCard("Insumos en Almacén", $"{insumos}", Color.FromArgb(100, 150, 220)));
            flowMetrics.Controls.Add(CreateKpiCard("Recetas", $"{recetas}", Color.FromArgb(120, 180, 240)));
            flowMetrics.Controls.Add(CreateKpiCard("Stock Bajo / Agotado", stockTexto, Color.FromArgb(220, 60, 60)));
            flowMetrics.Controls.Add(CreateKpiCard("Personal Activo", $"{_store.Employees.Count}", Color.FromArgb(255, 180, 80)));
            flowMetrics.Controls.Add(CreateKpiCard("Mermas", $"0", Color.FromArgb(180, 80, 255)));
            flowMetrics.Controls.Add(CreateKpiCard("Pérdidas", $"0", Color.FromArgb(255, 100, 100)));

            panelCenter.Controls.Add(flowMetrics);

            // ============================
            // NOTIFICATIONS
            // ============================
            var panelBottom = new FlowLayoutPanel
            {
                Width = 1100,
                Height = 300,
                BackColor = Color.Transparent
            };

            var btnCorte = new Button
            {
                Text = "Realizar Corte de Caja",
                Size = new Size(350, 60),
                BackColor = Color.FromArgb(240, 128, 148),
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            btnCorte.FlatAppearance.BorderSize = 0;

            var panelNotif = new Panel
            {
                Size = new Size(600, 260),
                BackColor = Color.White,
                Padding = new Padding(15),
                BorderStyle = BorderStyle.FixedSingle
            };

            panelNotif.Controls.Add(new Label
            {
                Text = "Notificaciones Recientes",
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                Location = new Point(10, 10),
                AutoSize = true
            });

            int yNotif = 45;
            var notificaciones = _notificationService
                .LoadNotifications()
                .OrderByDescending(n => n.Date)
                .Take(6)
                .ToList();

            if (notificaciones.Count == 0)
            {
                AddNotificationItem(panelNotif, "Sistema", "No hay novedades por ahora (≧◡≦)", ref yNotif);
            }
            else
            {
                foreach (var n in notificaciones)
                    AddNotificationItem(panelNotif, n.Title, n.Detail, ref yNotif);
            }

            panelBottom.Controls.AddRange(new Control[] { btnCorte, panelNotif });
            panelCenter.Controls.Add(panelBottom);

            rootPanel.Controls.Add(panelCenter);
            panelContent.Controls.Add(rootPanel);

            // Fade‑in suave sin flicker
            await FadeInControl(panelCenter, 300);
        }

        private async Task FadeInControl(Control ctrl, int durationMs)
        {
            ctrl.Visible = false;
            await Task.Delay(50);
            ctrl.Visible = true;
        }

        private Panel CreateKpiCard(string title, string value, Color accentColor)
        {
            var card = new Panel
            {
                Size = new Size(250, 120),
                BackColor = Color.White,
                Margin = new Padding(10),
                Padding = new Padding(0)
            };

            card.Controls.Add(new Panel
            {
                Dock = DockStyle.Top,
                Height = 6,
                BackColor = accentColor
            });

            card.Controls.Add(new Label
            {
                Text = value,
                Font = new Font("Segoe UI", 20, FontStyle.Bold),
                ForeColor = Color.Black,
                Location = new Point(0, 30),
                Size = new Size(250, 40),
                TextAlign = ContentAlignment.MiddleCenter
            });

            card.Controls.Add(new Label
            {
                Text = title,
                Font = new Font("Segoe UI", 10),
                ForeColor = Color.DimGray,
                Location = new Point(0, 75),
                Size = new Size(250, 20),
                TextAlign = ContentAlignment.MiddleCenter
            });

            return card;
        }

        private void AddNotificationItem(Panel parent, string title, string detail, ref int y)
        {
            var bubble = new Panel
            {
                Width = parent.Width - 30,
                Height = 45,
                BackColor = Color.FromArgb(250, 250, 250),
                Location = new Point(10, y),
                BorderStyle = BorderStyle.FixedSingle
            };

            var pic = new PictureBox
            {
                Image = Properties.Resources.logo_icon,
                Size = new Size(28, 28),
                Location = new Point(8, 8),
                SizeMode = PictureBoxSizeMode.Zoom
            };

            var lbl = new Label
            {
                Text = $"[{title}] {detail}",
                Font = new Font("Segoe UI", 9),
                Location = new Point(45, 12),
                AutoSize = true
            };

            bubble.Controls.Add(pic);
            bubble.Controls.Add(lbl);
            parent.Controls.Add(bubble);

            y += 50;
        }


        // ===========================================================
        //  REGION: SUBMENUS
        // ===========================================================
        private void CreateSubmenus()
        {
            panelSubEmployees = CreateSubmenuPanelBelow(btnEmployees);
            AddSubButton(panelSubEmployees, "Agregar empleado",
             (s, e) => LoadModule(new UsersModule(_store)));

            AddSubButton(panelSubEmployees, "Administrar empleados",
                (s, e) => LoadModule(new UsersPanelModule(_store)));



            panelSubWarehouse = CreateSubmenuPanelBelow(btnWarehouse);
            AddSubButton(panelSubWarehouse, "Inventario", (s, e) => LoadPlaceholder("Inventario"));
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

        // ===========================================================
        //  REGION: SLIDE MENU ANIMATION (OPTIMIZADO)
        // ===========================================================
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

        private void SlideTick(object? sender, EventArgs e)
        {
            if (isMenuExpanded)
            {
                panelMenu.Width -= AnimationStep;

                if (panelMenu.Width <= MenuCollapsedWidth)
                {
                    panelMenu.Width = MenuCollapsedWidth;
                    isMenuExpanded = false;

                    panelMenu.ResumeLayout();
                    panelContent.ResumeLayout(true);

                    slideTimer.Stop();
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

                    panelMenu.ResumeLayout();
                    panelContent.ResumeLayout(true);

                    slideTimer.Stop();
                    isAnimating = false;
                }
            }
        }

        private Button CreateExitButton(ref int y)
        {
            var btn = new Button
            {
                Text = string.Empty,
                AccessibleName = "Salir",
                Tag = Properties.Resources.icon_exit,
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
                int iconSize = 32;
                int iconY = (b.Height - iconSize) / 2;

                if (panelMenu.Width <= MenuCollapsedWidth)
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

                        pe.Graphics.DrawString(
                            "Salir",
                            b.Font,
                            brush,
                            new Rectangle(textX, 0, b.Width - textX, b.Height),
                            sf
                        );
                    }
                }
            };

            btn.Click += (s, e) => Close();

            return btn;
        }


        // ===========================================================
        //  REGION: LOAD MODULES
        // ===========================================================
        private void LoadModule(Control module)
        {
            panelContent.Controls.Clear();
            module.Dock = DockStyle.Fill;
            panelContent.Controls.Add(module);
        }

        private void LoadPlaceholder(string text)
        {
            var lbl = new Label
            {
                Text = text,
                Font = new Font("Segoe UI", 28, FontStyle.Bold),
                ForeColor = Color.White,
                AutoSize = true,
                Location = new Point(40, 40)
            };

            panelContent.Controls.Clear();
            panelContent.Controls.Add(lbl);
        }

        // ===========================================================
        //  REGION: HOVER EFFECTS
        // ===========================================================
        private void ApplyHover(Button btn)
        {
            btn.MouseEnter += (s, e) => btn.BackColor = Color.FromArgb(255, 150, 170);
            btn.MouseLeave += (s, e) => btn.BackColor = Color.Transparent;
        }

        private void ApplyHoverSub(Button btn)
        {
            btn.MouseEnter += (s, e) => btn.BackColor = Color.FromArgb(230, 230, 230);
            btn.MouseLeave += (s, e) => btn.BackColor = Color.FromArgb(245, 245, 245);
        }

        // ===========================================================
        //  REGION: DAILY REPORT
        // ===========================================================
        private void GenerateDailyReport()
        {
            _dailyReportService.GenerateDailyReport(_store);
            MessageBox.Show("Corte diario generado correctamente.", "Corte", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
    }
}
