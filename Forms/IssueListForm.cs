using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using MunicipalReporter.Models;
using MunicipalReporter.Services;

namespace MunicipalReporter.Forms
{
    public class IssueListForm : Form
    {
        private readonly ReportRepository<MunicipalIssue> issueRepository;
        private ListView issueListView;
        private Label statusLabel;

        public IssueListForm(ReportRepository<MunicipalIssue> repository)
        {
            issueRepository = repository;
            InitializeComponent();
            LoadIssues();
        }

        private void InitializeComponent()
        {
            Text = "Submitted Issues";
            MinimumSize = new Size(760, 500);
            StartPosition = FormStartPosition.CenterParent;
            BackColor = Color.FromArgb(245, 248, 250);

            TableLayoutPanel root = new TableLayoutPanel();
            root.Dock = DockStyle.Fill;
            root.Padding = new Padding(24);
            root.ColumnCount = 1;
            root.RowCount = 4;
            root.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            root.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            root.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
            root.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            Controls.Add(root);

            Label titleLabel = new Label();
            titleLabel.Text = "Submitted Issues";
            titleLabel.Font = new Font("Segoe UI", 18F, FontStyle.Bold);
            titleLabel.ForeColor = Color.FromArgb(28, 58, 85);
            titleLabel.AutoSize = true;
            titleLabel.Margin = new Padding(0, 0, 0, 8);
            root.Controls.Add(titleLabel, 0, 0);

            statusLabel = new Label();
            statusLabel.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            statusLabel.ForeColor = Color.FromArgb(28, 58, 85);
            statusLabel.AutoSize = true;
            statusLabel.Margin = new Padding(0, 0, 0, 12);
            root.Controls.Add(statusLabel, 0, 1);

            issueListView = new ListView();
            issueListView.Dock = DockStyle.Fill;
            issueListView.View = View.Details;
            issueListView.FullRowSelect = true;
            issueListView.GridLines = true;
            issueListView.HideSelection = false;
            issueListView.MultiSelect = false;
            issueListView.HeaderStyle = ColumnHeaderStyle.Nonclickable;
            issueListView.Columns.Add("Reference", 110);
            issueListView.Columns.Add("Location", 150);
            issueListView.Columns.Add("Category", 120);
            issueListView.Columns.Add("Description", 260);
            issueListView.Columns.Add("Status", 90);
            issueListView.Columns.Add("Submitted", 120);
            issueListView.ForeColor = Color.FromArgb(40, 55, 72);
            issueListView.Font = new Font("Segoe UI", 10F);
            root.Controls.Add(issueListView, 0, 2);

            FlowLayoutPanel actions = new FlowLayoutPanel();
            actions.Dock = DockStyle.Fill;
            actions.FlowDirection = FlowDirection.RightToLeft;
            actions.AutoSize = true;
            actions.Margin = new Padding(0, 12, 0, 0);
            root.Controls.Add(actions, 0, 3);

            Button refreshButton = new Button();
            refreshButton.Text = "Refresh";
            refreshButton.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            refreshButton.Width = 120;
            refreshButton.Height = 38;
            refreshButton.BackColor = Color.FromArgb(36, 106, 163);
            refreshButton.ForeColor = Color.White;
            refreshButton.FlatStyle = FlatStyle.Flat;
            refreshButton.FlatAppearance.BorderSize = 0;
            refreshButton.UseVisualStyleBackColor = false;
            refreshButton.Click += RefreshButton_Click;
            actions.Controls.Add(refreshButton);

            Button backButton = new Button();
            backButton.Text = "Back to Main Menu";
            backButton.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            backButton.Width = 170;
            backButton.Height = 38;
            backButton.BackColor = Color.FromArgb(92, 105, 115);
            backButton.ForeColor = Color.White;
            backButton.FlatStyle = FlatStyle.Flat;
            backButton.FlatAppearance.BorderSize = 0;
            backButton.UseVisualStyleBackColor = false;
            backButton.Click += delegate { Close(); };
            actions.Controls.Add(backButton);
        }

        private void LoadIssues()
        {
            issueListView.BeginUpdate();
            issueListView.Items.Clear();

            List<MunicipalIssue> issues = issueRepository.GetAll();
            issues.Sort((first, second) => second.CreatedAt.CompareTo(first.CreatedAt));

            foreach (MunicipalIssue issue in issues)
            {
                ListViewItem item = new ListViewItem(issue.ReferenceNumber);
                item.SubItems.Add(issue.Location);
                item.SubItems.Add(issue.Category);
                item.SubItems.Add(FormatDescription(issue.Description));
                item.SubItems.Add(issue.Status);
                item.SubItems.Add(issue.CreatedAt.ToString("dd MMM yyyy HH:mm"));
                issueListView.Items.Add(item);
            }

            issueListView.EndUpdate();

            if (issues.Count == 0)
            {
                statusLabel.Text = "No reports have been submitted yet.";
            }
            else
            {
                statusLabel.Text = "Showing " + issues.Count + " submitted report(s).";
            }
        }

        private string FormatDescription(string description)
        {
            if (string.IsNullOrWhiteSpace(description))
            {
                return "No description provided";
            }

            const int maxLength = 90;
            if (description.Length <= maxLength)
            {
                return description;
            }

            return description.Substring(0, maxLength - 3).TrimEnd() + "...";
        }

        private void RefreshButton_Click(object sender, EventArgs e)
        {
            LoadIssues();
        }
    }
}
