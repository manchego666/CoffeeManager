using System;
using System.Drawing;
using System.Windows.Forms;
using CoffeeManager.Models.Class;
using CoffeeManager.Models.Enums;

namespace CoffeeManager.Front
{
    internal class EditEmployeeModal : UserControl
    {
        private readonly Employee _emp;
        private readonly Store _store;

        public event Action? OnClose;

        private Panel panelCard = null!;
        private PictureBox picPhoto = null!;
        private Button btnLoadPhoto = null!;
        private TextBox txtFirstName = null!;
        private TextBox txtLastName = null!;
        private TextBox txtCURP = null!;
        private TextBox txtRFC = null!;
        private TextBox txtAddress = null!;
        private TextBox txtPhone = null!;
        private TextBox txtEmail = null!;
        private DateTimePicker dtBirth = null!;
        private ComboBox cbGender = null!;
        private ComboBox cbType = null!;
        private TextBox txtSalary = null!;
        private TextBox txtSSN = null!;
        private CheckBox chkActive = null!;
        private Button btnSave = null!;
        private Button btnCancel = null!;

        private string? _photoPath;

        public EditEmployeeModal(Employee emp, Store store)
        {
            _emp = emp;
            _store = store;

            DoubleBuffered = true;
            SetStyle(ControlStyles.AllPaintingInWmPaint |
                     ControlStyles.UserPaint |
                     ControlStyles.OptimizedDoubleBuffer, true);

            Size = new Size(720, 520);
            Anchor = AnchorStyles.None;
            BackColor = Color.Transparent;

            InitializeLayout();
            LoadEmployeeData();
        }

        private void InitializeLayout()
        {
            panelCard = new Panel
            {
                Size = new Size(700, 500),
                BackColor = Color.FromArgb(235, 255, 255, 255),
                Padding = new Padding(25)
            };
            panelCard.Paint += PanelCard_Paint;
            Controls.Add(panelCard);

            var lblTitle = new Label
            {
                Text = "Editar empleado",
                Font = new Font("Segoe UI", 20, FontStyle.Bold),
                ForeColor = Color.FromArgb(40, 40, 40),
                AutoSize = true,
                Location = new Point(10, 5)
            };
            panelCard.Controls.Add(lblTitle);

            int top = 60;
            int col1 = 20;
            int col2 = 330;
            int rowH = 45;

            txtFirstName = CreateText(panelCard, "Nombre", col1, top);
            txtLastName = CreateText(panelCard, "Apellidos", col1, top + rowH);
            txtCURP = CreateText(panelCard, "CURP", col1, top + rowH * 2);
            txtRFC = CreateText(panelCard, "RFC", col1, top + rowH * 3);
            txtAddress = CreateText(panelCard, "Dirección", col1, top + rowH * 4);

            txtPhone = CreateText(panelCard, "Teléfono", col2, top);
            txtEmail = CreateText(panelCard, "Correo", col2, top + rowH);

            dtBirth = new DateTimePicker
            {
                Location = new Point(col2, top + rowH * 2 + 18),
                Width = 260,
                Font = new Font("Segoe UI", 10)
            };
            panelCard.Controls.Add(CreateLabel("Fecha de nacimiento", col2, top + rowH * 2));
            panelCard.Controls.Add(dtBirth);

            cbGender = new ComboBox
            {
                Location = new Point(col2, top + rowH * 3 + 18),
                Width = 260,
                DropDownStyle = ComboBoxStyle.DropDownList,
                Font = new Font("Segoe UI", 10)
            };
            cbGender.Items.Add("Masculino");
            cbGender.Items.Add("Femenino");
            cbGender.Items.Add("No binario");
            panelCard.Controls.Add(CreateLabel("Género", col2, top + rowH * 3));
            panelCard.Controls.Add(cbGender);

            cbType = new ComboBox
            {
                Location = new Point(col2, top + rowH * 4 + 18),
                Width = 260,
                DropDownStyle = ComboBoxStyle.DropDownList,
                Font = new Font("Segoe UI", 10)
            };
            cbType.Items.Add("Mesero");
            cbType.Items.Add("Barista");
            cbType.Items.Add("Auxiliar");
            cbType.Items.Add("Administrador");
            panelCard.Controls.Add(CreateLabel("Tipo de empleado", col2, top + rowH * 4));
            panelCard.Controls.Add(cbType);

            txtSalary = CreateText(panelCard, "Salario mensual", col1, top + rowH * 5);
            txtSSN = CreateText(panelCard, "NSS", col2, top + rowH * 5);

            chkActive = new CheckBox
            {
                Text = "Empleado activo",
                Location = new Point(col1, top + rowH * 6 + 10),
                Font = new Font("Segoe UI", 10)
            };
            panelCard.Controls.Add(chkActive);

            picPhoto = new PictureBox
            {
                Size = new Size(140, 140),
                Location = new Point(col2 + 200, top),
                SizeMode = PictureBoxSizeMode.Zoom,
                BackColor = Color.White
            };
            panelCard.Controls.Add(picPhoto);

            btnLoadPhoto = new Button
            {
                Text = "Cambiar foto",
                Size = new Size(140, 32),
                Location = new Point(col2 + 200, top + 160),
                BackColor = Color.FromArgb(240, 128, 148),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };
            btnLoadPhoto.FlatAppearance.BorderSize = 0;
            btnLoadPhoto.Click += LoadPhoto;
            panelCard.Controls.Add(btnLoadPhoto);

            btnSave = new Button
            {
                Text = "Guardar cambios",
                Size = new Size(150, 40),
                Location = new Point(panelCard.Width - 330, panelCard.Height - 70),
                BackColor = Color.FromArgb(70, 180, 160),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };
            btnSave.FlatAppearance.BorderSize = 0;
            btnSave.Click += SaveChanges;
            panelCard.Controls.Add(btnSave);

            btnCancel = new Button
            {
                Text = "Cancelar",
                Size = new Size(150, 40),
                Location = new Point(panelCard.Width - 170, panelCard.Height - 70),
                BackColor = Color.FromArgb(220, 60, 60),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };
            btnCancel.FlatAppearance.BorderSize = 0;
            btnCancel.Click += (s, e) => OnClose?.Invoke();
            panelCard.Controls.Add(btnCancel);
        }

        private void LoadEmployeeData()
        {
            txtFirstName.Text = _emp.FirstName;
            txtLastName.Text = _emp.LastName;
            txtCURP.Text = _emp.CURP;
            txtRFC.Text = _emp.RFC;
            txtAddress.Text = _emp.Address;
            txtPhone.Text = _emp.Phone;
            txtEmail.Text = _emp.Email;
            dtBirth.Value = _emp.DateOfBirth;

            cbGender.SelectedIndex = _emp.Gender switch
            {
                Gender.Male => 0,
                Gender.Female => 1,
                _ => 2
            };

            cbType.SelectedIndex = _emp.Type switch
            {
                EmployeeType.Waiter => 0,
                EmployeeType.Barista => 1,
                EmployeeType.Assistant => 2,
                _ => 3
            };

            txtSalary.Text = _emp.Salary.ToString();
            txtSSN.Text = _emp.SocialSecurityNumber;
            chkActive.Checked = _emp.Active;

            if (!string.IsNullOrWhiteSpace(_emp.PathImage) && System.IO.File.Exists(_emp.PathImage))
                picPhoto.Image = Image.FromFile(_emp.PathImage);
        }

        private void SaveChanges(object? sender, EventArgs e)
        {
            _emp.FirstName = txtFirstName.Text;
            _emp.LastName = txtLastName.Text;
            _emp.CURP = txtCURP.Text;
            _emp.RFC = txtRFC.Text;
            _emp.Address = txtAddress.Text;
            _emp.Phone = txtPhone.Text;
            _emp.Email = txtEmail.Text;
            _emp.DateOfBirth = dtBirth.Value;

            _emp.Gender = cbGender.SelectedIndex switch
            {
                0 => Gender.Male,
                1 => Gender.Female,
                _ => Gender.NonBinary
            };

            _emp.Type = cbType.SelectedIndex switch
            {
                0 => EmployeeType.Waiter,
                1 => EmployeeType.Barista,
                2 => EmployeeType.Assistant,
                _ => EmployeeType.Administrator
            };

            _emp.Salary = decimal.TryParse(txtSalary.Text, out var s) ? s : 0;
            _emp.SocialSecurityNumber = txtSSN.Text;
            _emp.Active = chkActive.Checked;

            if (_photoPath != null)
                _emp.PathImage = _photoPath;

            _store.UpdateEmployee(_emp);

            MessageBox.Show("Cambios guardados correctamente.", "Éxito",
                MessageBoxButtons.OK, MessageBoxIcon.Information);

            OnClose?.Invoke();
        }

        private void LoadPhoto(object? sender, EventArgs e)
        {
            using var ofd = new OpenFileDialog
            {
                Filter = "Imágenes|*.png;*.jpg;*.jpeg;*.bmp"
            };

            if (ofd.ShowDialog() == DialogResult.OK)
            {
                _photoPath = ofd.FileName;
                picPhoto.Image = Image.FromFile(_photoPath);
            }
        }

        private Label CreateLabel(string text, int x, int y)
        {
            return new Label
            {
                Text = text,
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                ForeColor = Color.FromArgb(60, 60, 60),
                Location = new Point(x, y),
                AutoSize = true
            };
        }

        private TextBox CreateText(Control parent, string label, int x, int y)
        {
            var lbl = CreateLabel(label, x, y);
            parent.Controls.Add(lbl);

            var txt = new TextBox
            {
                Location = new Point(x, y + 18),
                Width = 260,
                Font = new Font("Segoe UI", 10),
                BorderStyle = BorderStyle.FixedSingle
            };
            parent.Controls.Add(txt);
            return txt;
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
