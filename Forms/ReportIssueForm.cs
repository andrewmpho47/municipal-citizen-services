using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using MunicipalReporter.Models;
using MunicipalReporter.Services;

namespace MunicipalReporter.Forms
{
    public class ReportIssueForm : Form
    {
        private readonly ReportRepository<MunicipalIssue> issueRepository;
        private readonly List<ReportAttachment> attachments = new List<ReportAttachment>();
        private readonly string[] categories = new string[] { "Sanitation", "Roads", "Utilities", "Water", "Electricity", "Public Safety", "Other" };
        private readonly string[] encouragementMessages = new string[]
        {
            "Start by adding the issue location.",
            "Good start. Choose the category that fits best.",
            "Almost there. Add a clear description for the team.",
            "Great detail. Attach evidence if you have it.",
            "Ready to submit. Thank you for taking part."
        };

        private TextBox locationTextBox;
        private ComboBox categoryComboBox;
        private RichTextBox descriptionRichTextBox;
        private ListBox attachmentListBox;
        private ProgressBar progressBar;
        private Label engagementLabel;

        public ReportIssueForm(ReportRepository<MunicipalIssue> repository)
        {
            issueRepository = repository;
            InitializeComponent();
            UpdateEngagement();
        }

        private void InitializeComponent()
        {
            Text = "Report Municipal Issue";
            MinimumSize = new Size(720, 620);
            StartPosition = FormStartPosition.CenterParent;
            BackColor = Color.FromArgb(245, 248, 250);

            TableLayoutPanel root = new TableLayoutPanel();
            root.Dock = DockStyle.Fill;
            root.ColumnCount = 1;
            root.RowCount = 8;
            root.Padding = new Padding(28);
            root.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            root.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            root.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            root.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            root.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
            root.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            root.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            root.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            Controls.Add(root);

            Label titleLabel = new Label();
            titleLabel.Text = "Report an Issue";
            titleLabel.Font = new Font("Segoe UI", 18F, FontStyle.Bold);
            titleLabel.ForeColor = Color.FromArgb(28, 58, 85);
            titleLabel.AutoSize = true;
            titleLabel.Margin = new Padding(0, 0, 0, 8);
            root.Controls.Add(titleLabel, 0, 0);

            Label introLabel = new Label();
            introLabel.Text = "Provide the location, category, description, and optional supporting files.";
            introLabel.Font = new Font("Segoe UI", 10F);
            introLabel.ForeColor = Color.FromArgb(70, 82, 92);
            introLabel.AutoSize = true;
            introLabel.MaximumSize = new Size(760, 0);
            introLabel.Margin = new Padding(0, 0, 0, 18);
            root.Controls.Add(introLabel, 0, 1);

            locationTextBox = CreateTextBox();
            locationTextBox.TextChanged += InputChanged;
            root.Controls.Add(CreateInputRow("Location", locationTextBox), 0, 2);

            categoryComboBox = new ComboBox();
            categoryComboBox.DropDownStyle = ComboBoxStyle.DropDownList;
            categoryComboBox.Font = new Font("Segoe UI", 10F);
            categoryComboBox.Items.AddRange(categories);
            categoryComboBox.SelectedIndexChanged += InputChanged;
            root.Controls.Add(CreateInputRow("Category", categoryComboBox), 0, 3);

            descriptionRichTextBox = new RichTextBox();
            descriptionRichTextBox.Font = new Font("Segoe UI", 10F);
            descriptionRichTextBox.Dock = DockStyle.Fill;
            descriptionRichTextBox.MinimumSize = new Size(0, 140);
            descriptionRichTextBox.TextChanged += InputChanged;
            root.Controls.Add(CreateDescriptionPanel(), 0, 4);

            Button attachButton = CreateActionButton("Attach Image or Document");
            attachButton.Click += AttachButton_Click;
            root.Controls.Add(attachButton, 0, 5);

            attachmentListBox = new ListBox();
            attachmentListBox.Font = new Font("Segoe UI", 9F);
            attachmentListBox.Height = 70;
            attachmentListBox.Dock = DockStyle.Top;
            attachmentListBox.Margin = new Padding(0, 8, 0, 16);
            root.Controls.Add(attachmentListBox, 0, 6);

            TableLayoutPanel bottomPanel = new TableLayoutPanel();
            bottomPanel.ColumnCount = 2;
            bottomPanel.RowCount = 3;
            bottomPanel.Dock = DockStyle.Fill;
            bottomPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50));
            bottomPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50));
            bottomPanel.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            bottomPanel.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            bottomPanel.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            root.Controls.Add(bottomPanel, 0, 7);

            engagementLabel = new Label();
            engagementLabel.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            engagementLabel.ForeColor = Color.FromArgb(28, 58, 85);
            engagementLabel.AutoSize = true;
            engagementLabel.Margin = new Padding(0, 0, 0, 8);
            bottomPanel.Controls.Add(engagementLabel, 0, 0);
            bottomPanel.SetColumnSpan(engagementLabel, 2);

            progressBar = new ProgressBar();
            progressBar.Dock = DockStyle.Fill;
            progressBar.Height = 22;
            progressBar.Margin = new Padding(0, 0, 0, 16);
            bottomPanel.Controls.Add(progressBar, 0, 1);
            bottomPanel.SetColumnSpan(progressBar, 2);

            FlowLayoutPanel navigationPanel = new FlowLayoutPanel();
            navigationPanel.Dock = DockStyle.Fill;
            navigationPanel.FlowDirection = FlowDirection.RightToLeft;
            navigationPanel.AutoSize = true;
            bottomPanel.Controls.Add(navigationPanel, 0, 2);
            bottomPanel.SetColumnSpan(navigationPanel, 2);

            Button submitButton = CreateActionButton("Submit");
            submitButton.Click += SubmitButton_Click;
            navigationPanel.Controls.Add(submitButton);

            Button backButton = CreateSecondaryButton("Back to Main Menu");
            backButton.Click += delegate { Close(); };
            navigationPanel.Controls.Add(backButton);
        }

        private TextBox CreateTextBox()
        {
            TextBox textBox = new TextBox();
            textBox.Font = new Font("Segoe UI", 10F);
            textBox.Dock = DockStyle.Fill;
            return textBox;
        }

        private Control CreateInputRow(string labelText, Control input)
        {
            TableLayoutPanel panel = new TableLayoutPanel();
            panel.ColumnCount = 2;
            panel.RowCount = 1;
            panel.Dock = DockStyle.Top;
            panel.AutoSize = true;
            panel.Margin = new Padding(0, 0, 0, 12);
            panel.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 110));
            panel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));

            Label label = new Label();
            label.Text = labelText;
            label.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            label.ForeColor = Color.FromArgb(45, 55, 65);
            label.AutoSize = true;
            label.Margin = new Padding(0, 4, 16, 0);

            panel.Controls.Add(label, 0, 0);
            panel.Controls.Add(input, 1, 0);
            return panel;
        }

        private Control CreateDescriptionPanel()
        {
            TableLayoutPanel panel = new TableLayoutPanel();
            panel.ColumnCount = 1;
            panel.RowCount = 2;
            panel.Dock = DockStyle.Fill;
            panel.Margin = new Padding(0, 0, 0, 12);
            panel.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            panel.RowStyles.Add(new RowStyle(SizeType.Percent, 100));

            Label label = new Label();
            label.Text = "Description";
            label.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            label.ForeColor = Color.FromArgb(45, 55, 65);
            label.AutoSize = true;
            label.Margin = new Padding(0, 0, 0, 6);

            panel.Controls.Add(label, 0, 0);
            panel.Controls.Add(descriptionRichTextBox, 0, 1);
            return panel;
        }

        private Button CreateActionButton(string text)
        {
            Button button = new Button();
            button.Text = text;
            button.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            button.Width = 190;
            button.Height = 38;
            button.Margin = new Padding(8, 0, 0, 0);
            button.BackColor = Color.FromArgb(36, 106, 163);
            button.ForeColor = Color.White;
            button.FlatStyle = FlatStyle.Flat;
            button.FlatAppearance.BorderSize = 0;
            button.UseVisualStyleBackColor = false;
            return button;
        }

        private Button CreateSecondaryButton(string text)
        {
            Button button = CreateActionButton(text);
            button.BackColor = Color.FromArgb(92, 105, 115);
            return button;
        }

        private void AttachButton_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog fileDialog = new OpenFileDialog())
            {
                fileDialog.Title = "Attach supporting image or document";
                fileDialog.Multiselect = true;
                fileDialog.Filter = "Supported Files|*.jpg;*.jpeg;*.png;*.gif;*.bmp;*.pdf;*.doc;*.docx;*.txt|All Files|*.*";

                if (fileDialog.ShowDialog(this) == DialogResult.OK)
                {
                    foreach (string fileName in fileDialog.FileNames)
                    {
                        if (File.Exists(fileName))
                        {
                            ReportAttachment attachment = new ReportAttachment(fileName);
                            attachments.Add(attachment);
                            attachmentListBox.Items.Add(attachment);
                        }
                    }

                    UpdateEngagement();
                }
            }
        }

        private void SubmitButton_Click(object sender, EventArgs e)
        {
            string validationError = ValidateReport();

            if (!string.IsNullOrEmpty(validationError))
            {
                MessageBox.Show(this, validationError, "Report incomplete", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            MunicipalIssue issue = new MunicipalIssue(
                locationTextBox.Text.Trim(),
                categoryComboBox.SelectedItem.ToString(),
                descriptionRichTextBox.Text.Trim(),
                new List<ReportAttachment>(attachments));

            issueRepository.Add(issue);

            MessageBox.Show(
                this,
                "Your issue has been submitted successfully.\nReference number: " + issue.ReferenceNumber,
                "Report submitted",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information);

            ClearForm();
        }

        private string ValidateReport()
        {
            if (string.IsNullOrWhiteSpace(locationTextBox.Text))
            {
                return "Please enter the location of the issue.";
            }

            if (categoryComboBox.SelectedIndex < 0)
            {
                return "Please select an issue category.";
            }

            if (string.IsNullOrWhiteSpace(descriptionRichTextBox.Text))
            {
                return "Please describe the issue before submitting.";
            }

            return string.Empty;
        }

        private void ClearForm()
        {
            locationTextBox.Clear();
            categoryComboBox.SelectedIndex = -1;
            descriptionRichTextBox.Clear();
            attachments.Clear();
            attachmentListBox.Items.Clear();
            UpdateEngagement();
        }

        private void InputChanged(object sender, EventArgs e)
        {
            UpdateEngagement();
        }

        private void UpdateEngagement()
        {
            bool hasLocation = !string.IsNullOrWhiteSpace(locationTextBox.Text);
            bool hasCategory = categoryComboBox.SelectedIndex >= 0;
            bool hasDescription = !string.IsNullOrWhiteSpace(descriptionRichTextBox.Text);
            bool hasAttachment = attachments.Count > 0;

            int progress = ReportingProgressCalculator.CalculateProgress(hasLocation, hasCategory, hasDescription, hasAttachment);
            progressBar.Value = progress;
            engagementLabel.Text = encouragementMessages[progress / 25];
        }
    }
}
