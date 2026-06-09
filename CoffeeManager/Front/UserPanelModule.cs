using System;
using System.Drawing;
using System.Windows.Forms;
using CoffeeManager.Models.Class;
using CoffeeManager.Models.Enums;

namespace CoffeeManager.Front
{
    /// <summary>
    /// Administration panel for managing registered employees.
    /// Allows editing, deleting, previewing and filtering users. (✿◠‿◠) ZORRODEV2026
    /// </summary>
    public class UsersPanelModule : Panel
    {
        #region CONTROLS

        private Panel panelList = null!;
        private Panel panelEdit = null!;
        private ListView listUsers = null!;

        private PictureBox picProfile = null!;
        private Button btnSave = null!;
        private Button btnCancel = null!;

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

        private Employee? selectedEmployee = null;

        #endregion

        #region CONSTRUCTOR
        public UsersPanelModule()
        {
            InitializeModule();
        }
        #endregion

        #region INITIALIZE
        /// <summary>
        /// Builds the visual layout for the user administration panel.
        /// </summary>
        private void InitializeModule()
        {
            Dock = DockStyle.Fill;
            BackColor = Color.FromArgb(40, 40, 40, 180);

            InitializeListPanel();
            InitializeEditPanel();
        }
        #endregion

        #region LIST PANEL
        private void InitializeListPanel()
        {
            panelList = new Panel
            {
                Dock = DockStyle.Left,
                Width = 600,
                BackColor = Color.FromArgb(30, 30, 30, 200)
            };
            Controls.Add(panelList);

            listUsers = new ListView
            {
                View = View.Details,
                Width = panelList.Width - 20,
                Height = panelList.Height - 20,
                Location = new Point(10, 10),
                Font = new Font("Segoe UI", 11),
                BackColor = Color.FromArgb(60, 60, 60),
                ForeColor = Color.White,
                FullRowSelect = true
            };

            listUsers.Columns.Add("Name", 180);
            listUsers.Columns.Add("Type", 120);
            listUsers.Columns.Add("Email", 180);
            listUsers.Columns.Add("Phone", 120);

            listUsers.DoubleClick += EditSelectedUser;

            panelList.Controls.Add(listUsers);

            // TODO: Load real data from database or file
        }
        #endregion

        #region EDIT PANEL
        private void InitializeEditPanel()
        {
            panelEdit = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.FromArgb(20, 20, 20, 180)
            };
            Controls.Add(panelEdit);

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

            picProfile = new PictureBox
            {
                Width = 140,
                Height = 140,
                Location = new Point(350, 20),
                SizeMode = PictureBoxSizeMode.Zoom,
                BorderStyle = BorderStyle.FixedSingle
            };
            panelEdit.Controls.Add(picProfile);

            btnSave = new Button
            {
                Text = "Save Changes",
                Width = 200,
                Height = 40,
                Location = new Point(20, y + 10),
                BackColor = Color.FromArgb(255, 140, 160),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };
            btnSave.FlatAppearance.BorderSize = 0;
            btnSave.Click += SaveChanges;
            panelEdit.Controls.Add(btnSave);

            btnCancel = new Button
            {
                Text = "Cancel",
                Width = 200,
                Height = 40,
                Location = new Point(240, y + 10),
                BackColor = Color.FromArgb(100, 100, 100),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };
            btnCancel.FlatAppearance.BorderSize = 0;
            btnCancel.Click += (s, e) => ClearEditPanel();
            panelEdit.Controls.Add(btnCancel);
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
            panelEdit.Controls.Add(lbl);

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
            panelEdit.Controls.Add(txt);

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
            panelEdit.Controls.Add(lbl);

            y += 25;

            var dt = new DateTimePicker
            {
                Width = 250,
                Location = new Point(20, y),
                Font = new Font("Segoe UI", 11)
            };
            panelEdit.Controls.Add(dt);

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
            panelEdit.Controls.Add(lbl);

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

            panelEdit.Controls.Add(cmb);

            y += 40;
            return cmb;
        }
        #endregion

        #region EVENTS
        private void EditSelectedUser(object? sender, EventArgs e)
        {
            if (listUsers.SelectedItems.Count == 0)
                return;

            // TODO: Replace with real data lookup
            selectedEmployee = new Employee
            {
                FirstName = "Test",
                LastName = "User",
                Email = "test@mail.com",
                Phone = "1234567890",
                Type = EmployeeType.Barista,
                Gender = Gender.Male,
                Salary = 5000,
                Address = "Street 123",
                CURP = "CURP123",
                RFC = "RFC123",
                SocialSecurityNumber = "NSS123"
            };

            LoadEmployeeToEditor(selectedEmployee);
        }

        private void LoadEmployeeToEditor(Employee emp)
        {
            txtFirstName.Text = emp.FirstName;
            txtLastName.Text = emp.LastName;
            dtBirth.Value = emp.DateOfBirth == default ? DateTime.Now : emp.DateOfBirth;
            txtCURP.Text = emp.CURP;
            txtRFC.Text = emp.RFC;
            txtAddress.Text = emp.Address;
            txtPhone.Text = emp.Phone;
            txtEmail.Text = emp.Email;
            cmbGender.SelectedIndex = (int)emp.Gender;
            cmbType.SelectedIndex = (int)emp.Type;
            txtSalary.Text = emp.Salary.ToString();
            txtNSS.Text = emp.SocialSecurityNumber;
            txtPathImage.Text = emp.PathImage;

            if (emp.HasImage())
                picProfile.Image = Image.FromFile(emp.PathImage);
        }

        private void SaveChanges(object? sender, EventArgs e)
        {
            if (selectedEmployee == null)
                return;

            selectedEmployee.FirstName = txtFirstName.Text;
            selectedEmployee.LastName = txtLastName.Text;
            selectedEmployee.DateOfBirth = dtBirth.Value;
            selectedEmployee.CURP = txtCURP.Text;
            selectedEmployee.RFC = txtRFC.Text;
            selectedEmployee.Address = txtAddress.Text;
            selectedEmployee.Phone = txtPhone.Text;
            selectedEmployee.Email = txtEmail.Text;
            selectedEmployee.Gender = (Gender)cmbGender.SelectedIndex;
            selectedEmployee.Type = (EmployeeType)cmbType.SelectedIndex;
            selectedEmployee.Salary = decimal.TryParse(txtSalary.Text, out var s) ? s : 0;
            selectedEmployee.SocialSecurityNumber = txtNSS.Text;
            selectedEmployee.PathImage = txtPathImage.Text;

            MessageBox.Show("Employee updated successfully!", "Success",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void ClearEditPanel()
        {
            foreach (Control c in panelEdit.Controls)
            {
                if (c is TextBox txt)
                    txt.Clear();
            }

            picProfile.Image = null;
            selectedEmployee = null;
        }
        #endregion
    }
}
