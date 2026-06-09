using System;
using System.Drawing;
using System.Windows.Forms;
using CoffeeManager.Models.Class;
using CoffeeManager.Models.Enums;

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
        #endregion

        #region CONSTRUCTOR
        public UsersModule()
        {
            InitializeModule();
        }
        #endregion

        #region INITIALIZE
        /// <summary>
        /// Builds the visual layout of the user registration module.
        /// </summary>
        private void InitializeModule()
        {
            Dock = DockStyle.Fill;
            BackColor = Color.FromArgb(40, 40, 40, 180);

            panelForm = new Panel
            {
                Width = 700,
                Height = 600,
                BackColor = Color.FromArgb(30, 30, 30, 200),
                Location = new Point(40, 40)
            };
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
            panelForm.Controls.Add(btnLoadImage);

            picProfile = new PictureBox
            {
                Width = 140,
                Height = 140,
                Location = new Point(500, 20),
                SizeMode = PictureBoxSizeMode.Zoom,
                BorderStyle = BorderStyle.FixedSingle
            };
            panelForm.Controls.Add(picProfile);

            btnRegister = new Button
            {
                Text = "Register Employee",
                Width = 250,
                Height = 45,
                Location = new Point(20, y + 10),
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                BackColor = Color.FromArgb(255, 140, 160),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };
            btnRegister.FlatAppearance.BorderSize = 0;
            btnRegister.Click += RegisterEmployee;
            panelForm.Controls.Add(btnRegister);
        }
        #endregion

        #region HELPERS
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
                Width = 250,
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
                Width = 250,
                Location = new Point(20, y),
                Font = new Font("Segoe UI", 11)
            };
            panelForm.Controls.Add(dt);

            y += 40;
            return dt;
        }

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
                Width = 250,
                Location = new Point(20, y),
                Font = new Font("Segoe UI", 11),
                DropDownStyle = ComboBoxStyle.DropDownList,
                BackColor = Color.FromArgb(60, 60, 60),
                ForeColor = Color.White
            };
            cmb.Items.AddRange(items);
            cmb.SelectedIndex = 0;

            panelForm.Controls.Add(cmb);

            y += 40;
            return cmb;
        }
        #endregion

        #region EVENTS
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

        private void RegisterEmployee(object? sender, EventArgs e)
        {
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

            MessageBox.Show("Employee registered successfully!", "Success",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        #endregion
    }
}
