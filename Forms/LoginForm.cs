using System;
using System.Drawing;
using System.Windows.Forms;
using MunicipalReporter.Models;

namespace MunicipalReporter.Forms
{
    public class LoginForm : Form
    {
        private const string CitizenUsername = "citizen";
        private const string CitizenPassword = "Citizen@123";
        private const string AdminUsername = "admin";
        private const string AdminPassword = "Admin@123";

        private ComboBox roleComboBox;
        private TextBox usernameTextBox;
        private TextBox passwordTextBox;

        public UserRole AuthenticatedRole { get; private set; }

        public LoginForm()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            Text = "Municipal Services Login";
            Size = new Size(480, 360);
            MinimumSize = new Size(440, 340);
            StartPosition = FormStartPosition.CenterScreen;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            BackColor = Color.FromArgb(245, 248, 250);

            TableLayoutPanel root = new TableLayoutPanel();
            root.Dock = DockStyle.Fill;
            root.Padding = new Padding(28);
            root.ColumnCount = 1;
            root.RowCount = 3;
            root.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            root.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
            root.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            Controls.Add(root);

            Label title = new Label();
            title.Text = "Municipal Citizen Services";
            title.Font = new Font("Segoe UI", 17F, FontStyle.Bold);
            title.ForeColor = Color.FromArgb(28, 58, 85);
            title.AutoSize = true;
            title.Margin = new Padding(0, 0, 0, 18);
            root.Controls.Add(title, 0, 0);

            TableLayoutPanel fields = new TableLayoutPanel();
            fields.Dock = DockStyle.Top;
            fields.ColumnCount = 2;
            fields.RowCount = 3;
            fields.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 105));
            fields.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
            for (int row = 0; row < fields.RowCount; row++)
            {
                fields.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            }
            root.Controls.Add(fields, 0, 1);

            roleComboBox = new ComboBox();
            roleComboBox.DropDownStyle = ComboBoxStyle.DropDownList;
            roleComboBox.Font = new Font("Segoe UI", 10F);
            roleComboBox.Items.AddRange(new object[] { "Citizen", "Admin" });
            roleComboBox.SelectedIndex = 0;
            AddField(fields, "Sign in as", roleComboBox, 0);

            usernameTextBox = new TextBox();
            usernameTextBox.Font = new Font("Segoe UI", 10F);
            AddField(fields, "Username", usernameTextBox, 1);

            passwordTextBox = new TextBox();
            passwordTextBox.Font = new Font("Segoe UI", 10F);
            passwordTextBox.UseSystemPasswordChar = true;
            AddField(fields, "Password", passwordTextBox, 2);

            FlowLayoutPanel buttons = new FlowLayoutPanel();
            buttons.AutoSize = true;
            buttons.FlowDirection = FlowDirection.RightToLeft;
            buttons.Dock = DockStyle.Fill;
            buttons.Margin = new Padding(0, 18, 0, 0);
            root.Controls.Add(buttons, 0, 2);

            Button exitButton = CreateButton("Exit", Color.FromArgb(92, 105, 115), 100);
            exitButton.DialogResult = DialogResult.Cancel;
            buttons.Controls.Add(exitButton);

            Button loginButton = CreateButton("Sign in", Color.FromArgb(36, 106, 163), 120);
            loginButton.Click += LoginButton_Click;
            buttons.Controls.Add(loginButton);
            AcceptButton = loginButton;
            CancelButton = exitButton;
        }

        private void AddField(TableLayoutPanel panel, string labelText, Control field, int row)
        {
            Label label = new Label();
            label.Text = labelText;
            label.Font = new Font("Segoe UI", 9.5F, FontStyle.Bold);
            label.ForeColor = Color.FromArgb(28, 58, 85);
            label.AutoSize = true;
            label.Margin = new Padding(0, 8, 8, 0);
            panel.Controls.Add(label, 0, row);

            field.Dock = DockStyle.Fill;
            field.Margin = new Padding(0, 4, 0, 10);
            panel.Controls.Add(field, 1, row);
        }

        private Button CreateButton(string text, Color backColor, int width)
        {
            Button button = new Button();
            button.Text = text;
            button.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            button.Width = width;
            button.Height = 36;
            button.Margin = new Padding(8, 0, 0, 0);
            button.BackColor = backColor;
            button.ForeColor = Color.White;
            button.FlatStyle = FlatStyle.Flat;
            button.FlatAppearance.BorderSize = 0;
            button.UseVisualStyleBackColor = false;
            return button;
        }

        private void LoginButton_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(usernameTextBox.Text) ||
                string.IsNullOrEmpty(passwordTextBox.Text) ||
                roleComboBox.SelectedIndex < 0)
            {
                MessageBox.Show(this, "Select a role and enter both username and password.",
                    "Sign-in details required", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string username = usernameTextBox.Text.Trim();
            string password = passwordTextBox.Text;
            bool isCitizen = string.Equals(roleComboBox.SelectedItem.ToString(), "Citizen", StringComparison.Ordinal);
            bool isValidCitizen = isCitizen &&
                string.Equals(username, CitizenUsername, StringComparison.OrdinalIgnoreCase) &&
                string.Equals(password, CitizenPassword, StringComparison.Ordinal);
            bool isValidAdmin = !isCitizen &&
                string.Equals(username, AdminUsername, StringComparison.OrdinalIgnoreCase) &&
                string.Equals(password, AdminPassword, StringComparison.Ordinal);

            if (!isValidCitizen && !isValidAdmin)
            {
                MessageBox.Show(this, "The selected role and credentials do not match.",
                    "Sign-in failed", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                passwordTextBox.Clear();
                passwordTextBox.Focus();
                return;
            }

            AuthenticatedRole = isValidAdmin ? UserRole.Admin : UserRole.Citizen;
            DialogResult = DialogResult.OK;
            Close();
        }
    }
}
