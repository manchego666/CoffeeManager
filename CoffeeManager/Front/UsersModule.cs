using System;
using System.Drawing;
using System.Windows.Forms;
using CoffeeManager.Models.Class;
using CoffeeManager.Models.Enums;
using Timer = System.Windows.Forms.Timer;


namespace CoffeeManager.Front
{
    /// <summary>
    /// Module for registering new employees into the system.
    /// Provides full personal and job-related data input. (✿◠‿◠) ZORRODEV2026
    /// </summary>
    public class UsersModule : Panel
    {
        #region CONTROLS

        private Panel panelForm = null!;
        private PictureBox picProfile = null!;
        private Button btnLoadImage = null!;
        private Button btnRegister = null!;

        private TextBox txtFirstName = null!;
        private TextBox txtLastName = null!;
        private DateTimePicker dtBirth = null!;
        private TextBox txtCURP = null!;
        private TextBox txtRFC = null!;
        private TextBox txtAddress = null!;
        private TextBox txtPhone = null!;
        private TextBox txtEmail = null!;
        private ComboBox cmbGender = null!;
        private ComboBox cmbType = null!;
        private TextBox txtSalary = null!;
        private TextBox txtNSS = null!;
        private TextBox txtPathImage = null!;

        private Timer? _fadeTimer;
        private int _alpha = 0;

        #endregion

        #region CONSTRUCTOR

        /// <summary>
        /// Initializes a new instance of the UsersModule.
        /// </summary>
        public UsersModule()
        {
            InitializeModule();
            InitializeFadeAnimation();
        }

        #endregion

        #region INITIALIZE

        /// <summary>
        /// Builds the visual layout of the user registration module.
        /// </summary>
        private void InitializeModule()
        {
            Dock = DockStyle.Fill;

            // Background (can be changed to bg_report, etc. if needed)
            BackgroundImage = Properties.Resources.bg_main;
            BackgroundImageLayout = ImageLayout.Stretch;

            BackColor = Color.FromArgb(40, 40, 40, 180);

            panelForm = new Panel
            {
                Width = 720,
                Height = 620,
                BackColor = Color.FromArgb(30, 30, 30, 220),
                Location = new Point(40, 40)
            };
            panelForm.Anchor = AnchorStyles.Top | AnchorStyles.Left;
            Controls.Add(panelForm);

            int y = 20;

            txtFirstName = CreateField("First Name", ref y);
            txtLastName = CreateField("Last Name", ref y);
            dtBirth = CreateDateField("Birth Date", ref y);
            txtCURP = CreateField("CURP", ref y);
            txtRFC = CreateField("RFC", ref y);
            txtAddress = CreateField("Address", ref y);
            txtPhone = CreateField("Phone", ref y);
            txtEmail = CreateField("Email", ref y);

            cmbGender = CreateCombo("Gender", ref y, Enum.GetNames(typeof(Gender)));
            cmbType = CreateCombo("Employee Type", ref y, Enum.GetNames(typeof(EmployeeType)));

            txtSalary = CreateField("Salary", ref y);
            txtNSS = CreateField("NSS", ref y);
            txtPathImage = CreateField("Image Path", ref y);

            btnLoadImage = new Button
            {
                Text = "Load Image",
                Width = 150,
                Height = 35,
                Location = new Point(300, y - 40),
                BackColor = Color.FromArgb(255, 140, 160),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };
            btnLoadImage.FlatAppearance.BorderSize = 0;
            btnLoadImage.Click += LoadImage;
            ApplyHoverEffect(btnLoadImage);
            panelForm.Controls.Add(btnLoadImage);

            picProfile = new PictureBox
            {
                Width = 140,
                Height = 140,
                Location = new Point(520, 20),
                SizeMode = PictureBoxSizeMode.Zoom,
                BorderStyle = BorderStyle.FixedSingle
            };
            panelForm.Controls.Add(picProfile);

            btnRegister = new Button
            {
                Text = "Register Employee",
                Width = 260,
                Height = 45,
                Location = new Point(20, y + 10),
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                BackColor = Color.FromArgb(255, 140, 160),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };
            btnRegister.FlatAppearance.BorderSize = 0;
            btnRegister.Click += RegisterEmployee;
            ApplyHoverEffect(btnRegister);
            panelForm.Controls.Add(btnRegister);
        }

        /// <summary>
        /// Initializes a simple fade-in animation for the module.
        /// </summary>
        private void InitializeFadeAnimation()
        {
            _alpha = 0;

            _fadeTimer = new Timer();
            _fadeTimer.Interval = 20;
            _fadeTimer.Tick += (s, e) =>
            {
                _alpha += 10;
                if (_alpha >= 180)
                {
                    _alpha = 180;
                    _fadeTimer.Stop();
                }

                BackColor = Color.FromArgb(_alpha, 40, 40, 40);
                panelForm.BackColor = Color.FromArgb(Math.Min(_alpha + 20, 220), 30, 30, 30);
            };

            _fadeTimer.Start();
        }


        #endregion

        #region HELPERS

        /// <summary>
        /// Creates a labeled text field and places it vertically.
        /// </summary>
        private TextBox CreateField(string label, ref int y)
        {
            var lbl = new Label
            {
                Text = label,
                Font = new Font("Segoe UI", 11, FontStyle.Bold),
                ForeColor = Color.White,
                Location = new Point(20, y),
                AutoSize = true
            };
            panelForm.Controls.Add(lbl);

            y += 25;

            var txt = new TextBox
            {
                Width = 260,
                Location = new Point(20, y),
                Font = new Font("Segoe UI", 11),
                BackColor = Color.FromArgb(60, 60, 60),
                ForeColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle
            };
            panelForm.Controls.Add(txt);

            y += 40;
            return txt;
        }

        /// <summary>
        /// Creates a labeled date picker and places it vertically.
        /// </summary>
        private DateTimePicker CreateDateField(string label, ref int y)
        {
            var lbl = new Label
            {
                Text = label,
                Font = new Font("Segoe UI", 11, FontStyle.Bold),
                ForeColor = Color.White,
                Location = new Point(20, y),
                AutoSize = true
            };
            panelForm.Controls.Add(lbl);

            y += 25;

            var dt = new DateTimePicker
            {
                Width = 260,
                Location = new Point(20, y),
                Font = new Font("Segoe UI", 11)
            };
            panelForm.Controls.Add(dt);

            y += 40;
            return dt;
        }

        /// <summary>
        /// Creates a labeled combo box and places it vertically.
        /// </summary>
        private ComboBox CreateCombo(string label, ref int y, string[] items)
        {
            var lbl = new Label
            {
                Text = label,
                Font = new Font("Segoe UI", 11, FontStyle.Bold),
                ForeColor = Color.White,
                Location = new Point(20, y),
                AutoSize = true
            };
            panelForm.Controls.Add(lbl);

            y += 25;

            var cmb = new ComboBox
            {
                Width = 260,
                Location = new Point(20, y),
                Font = new Font("Segoe UI", 11),
                DropDownStyle = ComboBoxStyle.DropDownList,
                BackColor = Color.FromArgb(60, 60, 60),
                ForeColor = Color.White
            };
            cmb.Items.AddRange(items);
            if (items.Length > 0)
                cmb.SelectedIndex = 0;

            panelForm.Controls.Add(cmb);

            y += 40;
            return cmb;
        }

        /// <summary>
        /// Applies a simple hover/press visual effect to a button.
        /// </summary>
        private void ApplyHoverEffect(Button btn)
        {
            btn.MouseEnter += (s, e) =>
            {
                btn.BackColor = Color.FromArgb(255, 160, 180);
            };
            btn.MouseLeave += (s, e) =>
            {
                btn.BackColor = Color.FromArgb(255, 140, 160);
            };
            btn.MouseDown += (s, e) =>
            {
                btn.BackColor = Color.FromArgb(255, 120, 150);
            };
            btn.MouseUp += (s, e) =>
            {
                btn.BackColor = Color.FromArgb(255, 160, 180);
            };
        }

        /// <summary>
        /// Validates required fields before creating an employee.
        /// </summary>
        private bool ValidateForm()
        {
            if (string.IsNullOrWhiteSpace(txtFirstName.Text) ||
                string.IsNullOrWhiteSpace(txtLastName.Text) ||
                string.IsNullOrWhiteSpace(txtCURP.Text) ||
                string.IsNullOrWhiteSpace(txtRFC.Text))
            {
                MessageBox.Show("Please fill in at least First Name, Last Name, CURP and RFC.",
                    "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            return true;
        }

        #endregion

        #region EVENTS

        /// <summary>
        /// Opens a file dialog to select a profile image.
        /// </summary>
        private void LoadImage(object? sender, EventArgs e)
        {
            using var dlg = new OpenFileDialog();
            dlg.Filter = "Images|*.png;*.jpg;*.jpeg";

            if (dlg.ShowDialog() == DialogResult.OK)
            {
                txtPathImage.Text = dlg.FileName;
                picProfile.Image = Image.FromFile(dlg.FileName);
            }
        }

        /// <summary>
        /// Creates a new Employee instance from the form data and shows a confirmation.
        /// </summary>
        private void RegisterEmployee(object? sender, EventArgs e)
        {
            if (!ValidateForm())
                return;

            var emp = new Employee
            {
                FirstName = txtFirstName.Text,
                LastName = txtLastName.Text,
                DateOfBirth = dtBirth.Value,
                CURP = txtCURP.Text,
                RFC = txtRFC.Text,
                Address = txtAddress.Text,
                Phone = txtPhone.Text,
                Email = txtEmail.Text,
                Gender = (Gender)cmbGender.SelectedIndex,
                Type = (EmployeeType)cmbType.SelectedIndex,
                Salary = decimal.TryParse(txtSalary.Text, out var s) ? s : 0,
                SocialSecurityNumber = txtNSS.Text,
                PathImage = txtPathImage.Text,
                Active = true
            };

            // TODO: Persist employee (database, file, etc.) and notify UsersPanelModule if needed.

            MessageBox.Show("Employee registered successfully!", "Success",
                MessageBoxButtons.OK, MessageBoxIcon.Information);

            ClearForm();
        }

        /// <summary>
        /// Clears all input fields after successful registration.
        /// </summary>
        private void ClearForm()
        {
            txtFirstName.Clear();
            txtLastName.Clear();
            txtCURP.Clear();
            txtRFC.Clear();
            txtAddress.Clear();
            txtPhone.Clear();
            txtEmail.Clear();
            txtSalary.Clear();
            txtNSS.Clear();
            txtPathImage.Clear();
            picProfile.Image = null;

            cmbGender.SelectedIndex = 0;
            cmbType.SelectedIndex = 0;
            dtBirth.Value = DateTime.Now;
        }

        #endregion
    }
}
