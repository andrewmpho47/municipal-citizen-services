using System;
using System.Drawing;
using System.Windows.Forms;

namespace MunicipalReporter.Forms
{
    public class AnnouncementForm : Form
    {
        private TextBox titleTextBox;
        private TextBox categoryTextBox;
        private TextBox locationTextBox;
        private TextBox descriptionTextBox;
        private DateTimePicker eventDatePicker;
        private DateTimePicker eventTimePicker;

        public AnnouncementForm()
        {
            InitializeComponent();
        }

        public string AnnouncementTitle
        {
            get { return titleTextBox.Text.Trim(); }
        }

        public string Category
        {
            get { return categoryTextBox.Text.Trim(); }
        }

        public string AnnouncementLocation
        {
            get { return locationTextBox.Text.Trim(); }
        }

        public string Description
        {
            get { return descriptionTextBox.Text.Trim(); }
        }

        public DateTime EventDate
        {
            get { return eventDatePicker.Value.Date.Add(eventTimePicker.Value.TimeOfDay); }
        }

        private void InitializeComponent()
        {
            Text = "Add Announcement";
            Size = new Size(680, 580);
            MinimumSize = new Size(580, 500);
            StartPosition = FormStartPosition.CenterParent;
            BackColor = Color.FromArgb(245, 248, 250);

            TableLayoutPanel root = new TableLayoutPanel();
            root.Dock = DockStyle.Fill;
            root.Padding = new Padding(24);
            root.ColumnCount = 1;
            root.RowCount = 2;
            root.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
            root.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            Controls.Add(root);

            TableLayoutPanel fields = new TableLayoutPanel();
            fields.Dock = DockStyle.Fill;
            fields.ColumnCount = 2;
            fields.RowCount = 5;
            fields.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 115));
            fields.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
            fields.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            fields.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            fields.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            fields.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            fields.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
            root.Controls.Add(fields, 0, 0);

            titleTextBox = CreateTextBox();
            AddField(fields, "Title", titleTextBox, 0);

            categoryTextBox = CreateTextBox();
            AddField(fields, "Category", categoryTextBox, 1);

            locationTextBox = CreateTextBox();
            AddField(fields, "Location", locationTextBox, 2);

            FlowLayoutPanel dateTimePanel = new FlowLayoutPanel();
            dateTimePanel.AutoSize = true;
            dateTimePanel.WrapContents = false;
            dateTimePanel.Dock = DockStyle.Fill;

            eventDatePicker = new DateTimePicker();
            eventDatePicker.Format = DateTimePickerFormat.Short;
            eventDatePicker.MinDate = DateTime.Today;
            eventDatePicker.Value = DateTime.Today.AddDays(1);
            eventDatePicker.Width = 135;
            eventDatePicker.Font = new Font("Segoe UI", 9.5F);
            dateTimePanel.Controls.Add(eventDatePicker);

            eventTimePicker = new DateTimePicker();
            eventTimePicker.Format = DateTimePickerFormat.Custom;
            eventTimePicker.CustomFormat = "HH:mm";
            eventTimePicker.ShowUpDown = true;
            eventTimePicker.Value = DateTime.Today.AddHours(9);
            eventTimePicker.Width = 90;
            eventTimePicker.Font = new Font("Segoe UI", 9.5F);
            dateTimePanel.Controls.Add(eventTimePicker);
            AddField(fields, "Date and time", dateTimePanel, 3);

            Label descriptionLabel = CreateLabel("Description");
            descriptionLabel.Anchor = AnchorStyles.Top | AnchorStyles.Left;
            fields.Controls.Add(descriptionLabel, 0, 4);
            descriptionTextBox = new TextBox();
            descriptionTextBox.Multiline = true;
            descriptionTextBox.ScrollBars = ScrollBars.Vertical;
            descriptionTextBox.Dock = DockStyle.Fill;
            descriptionTextBox.Font = new Font("Segoe UI", 9.5F);
            descriptionTextBox.Margin = new Padding(0, 5, 0, 12);
            fields.Controls.Add(descriptionTextBox, 1, 4);

            FlowLayoutPanel buttons = new FlowLayoutPanel();
            buttons.AutoSize = true;
            buttons.FlowDirection = FlowDirection.RightToLeft;
            buttons.Dock = DockStyle.Fill;
            buttons.Margin = new Padding(0, 12, 0, 0);
            root.Controls.Add(buttons, 0, 1);

            Button cancelButton = CreateButton("Cancel", Color.FromArgb(92, 105, 115), 110);
            cancelButton.DialogResult = DialogResult.Cancel;
            buttons.Controls.Add(cancelButton);

            Button saveButton = CreateButton("Publish announcement", Color.FromArgb(36, 106, 163), 185);
            saveButton.Click += SaveButton_Click;
            buttons.Controls.Add(saveButton);
            AcceptButton = saveButton;
            CancelButton = cancelButton;
        }

        private void AddField(TableLayoutPanel panel, string labelText, Control field, int row)
        {
            Label label = CreateLabel(labelText);
            panel.Controls.Add(label, 0, row);
            field.Dock = DockStyle.Fill;
            field.Margin = new Padding(0, 5, 0, 12);
            panel.Controls.Add(field, 1, row);
        }

        private TextBox CreateTextBox()
        {
            TextBox textBox = new TextBox();
            textBox.Font = new Font("Segoe UI", 9.5F);
            return textBox;
        }

        private Label CreateLabel(string text)
        {
            Label label = new Label();
            label.Text = text;
            label.Font = new Font("Segoe UI", 9.5F, FontStyle.Bold);
            label.ForeColor = Color.FromArgb(28, 58, 85);
            label.AutoSize = true;
            label.Margin = new Padding(0, 8, 8, 0);
            return label;
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

        private void SaveButton_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(AnnouncementTitle) ||
                string.IsNullOrWhiteSpace(Category) ||
                string.IsNullOrWhiteSpace(AnnouncementLocation) ||
                string.IsNullOrWhiteSpace(Description))
            {
                MessageBox.Show(this, "Complete the title, category, location, and description before publishing.",
                    "Announcement incomplete", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (EventDate < DateTime.Now)
            {
                MessageBox.Show(this, "Choose a future date and time for the announcement.",
                    "Check the date and time", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            DialogResult = DialogResult.OK;
            Close();
        }
    }
}
