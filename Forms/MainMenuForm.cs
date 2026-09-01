using System;
using System.Drawing;
using System.Windows.Forms;
using MunicipalReporter.Models;
using MunicipalReporter.Services;

namespace MunicipalReporter.Forms
{
    public class MainMenuForm : Form
    {
        private static readonly ReportRepository<MunicipalIssue> IssueRepository = new ReportRepository<MunicipalIssue>();
        private Label summaryLabel;

        public MainMenuForm()
        {
            InitializeComponent();
        }

        protected override void OnActivated(EventArgs e)
        {
            base.OnActivated(e);
            summaryLabel.Text = "Reports submitted this session: " + IssueRepository.Count;
        }

        private void InitializeComponent()
        {
            Text = "Municipal Citizen Services";
            MinimumSize = new Size(560, 420);
            StartPosition = FormStartPosition.CenterScreen;
            BackColor = Color.FromArgb(245, 248, 250);

            TableLayoutPanel root = new TableLayoutPanel();
            root.Dock = DockStyle.Fill;
            root.ColumnCount = 1;
            root.RowCount = 5;
            root.Padding = new Padding(32);
            root.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            root.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            root.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
            root.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            root.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            Controls.Add(root);

            Label titleLabel = new Label();
            titleLabel.Text = "Municipal Citizen Services";
            titleLabel.Font = new Font("Segoe UI", 18F, FontStyle.Bold);
            titleLabel.ForeColor = Color.FromArgb(28, 58, 85);
            titleLabel.AutoSize = true;
            titleLabel.Margin = new Padding(0, 0, 0, 8);
            root.Controls.Add(titleLabel, 0, 0);

            Label instructionLabel = new Label();
            instructionLabel.Text = "Choose a service below. Reporting issues is available now; other services will be added later.";
            instructionLabel.Font = new Font("Segoe UI", 10F);
            instructionLabel.ForeColor = Color.FromArgb(70, 82, 92);
            instructionLabel.AutoSize = true;
            instructionLabel.MaximumSize = new Size(680, 0);
            instructionLabel.Margin = new Padding(0, 0, 0, 24);
            root.Controls.Add(instructionLabel, 0, 1);

            FlowLayoutPanel buttonPanel = new FlowLayoutPanel();
            buttonPanel.Dock = DockStyle.Top;
            buttonPanel.FlowDirection = FlowDirection.TopDown;
            buttonPanel.WrapContents = false;
            buttonPanel.AutoSize = true;
            root.Controls.Add(buttonPanel, 0, 2);

            Button reportButton = CreateMenuButton("Report Issues");
            reportButton.Click += ReportButton_Click;
            buttonPanel.Controls.Add(reportButton);

            Button eventsButton = CreateMenuButton("Local Events and Announcements");
            eventsButton.Enabled = false;
            buttonPanel.Controls.Add(eventsButton);

            Button statusButton = CreateMenuButton("Service Request Status");
            statusButton.Enabled = false;
            buttonPanel.Controls.Add(statusButton);

            summaryLabel = new Label();
            summaryLabel.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            summaryLabel.ForeColor = Color.FromArgb(28, 58, 85);
            summaryLabel.AutoSize = true;
            summaryLabel.Margin = new Padding(0, 16, 0, 0);
            root.Controls.Add(summaryLabel, 0, 3);

            Label footerLabel = new Label();
            footerLabel.Text = "Thank you for helping improve municipal services.";
            footerLabel.Font = new Font("Segoe UI", 9F);
            footerLabel.ForeColor = Color.FromArgb(99, 111, 122);
            footerLabel.AutoSize = true;
            footerLabel.Margin = new Padding(0, 8, 0, 0);
            root.Controls.Add(footerLabel, 0, 4);
        }

        private Button CreateMenuButton(string text)
        {
            Button button = new Button();
            button.Text = text;
            button.Font = new Font("Segoe UI", 11F, FontStyle.Bold);
            button.Width = 360;
            button.Height = 48;
            button.Margin = new Padding(0, 0, 0, 12);
            button.BackColor = Color.FromArgb(36, 106, 163);
            button.ForeColor = Color.White;
            button.FlatStyle = FlatStyle.Flat;
            button.FlatAppearance.BorderSize = 0;
            button.UseVisualStyleBackColor = false;
            return button;
        }

        private void ReportButton_Click(object sender, EventArgs e)
        {
            using (ReportIssueForm reportForm = new ReportIssueForm(IssueRepository))
            {
                Hide();
                reportForm.ShowDialog(this);
                Show();
            }
        }
    }
}
