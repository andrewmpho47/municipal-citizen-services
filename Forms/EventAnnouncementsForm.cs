using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using MunicipalReporter.Models;
using MunicipalReporter.Services;

namespace MunicipalReporter.Forms
{
    public class EventAnnouncementsForm : Form
    {
        private readonly EventRepository eventRepository;
        private ComboBox categoryComboBox;
        private DateTimePicker fromDatePicker;
        private DateTimePicker toDatePicker;
        private ListView eventListView;
        private ListBox recommendationListBox;
        private ListBox searchHistoryListBox;
        private Label resultCountLabel;
        private Label eventDetailsLabel;

        public EventAnnouncementsForm(EventRepository repository)
        {
            eventRepository = repository;
            InitializeComponent();
            LoadUpcomingEvents();
        }

        private void InitializeComponent()
        {
            Text = "Local Events and Announcements";
            MinimumSize = new Size(940, 680);
            Size = new Size(1180, 820);
            StartPosition = FormStartPosition.CenterParent;
            BackColor = Color.FromArgb(245, 248, 250);

            TableLayoutPanel root = new TableLayoutPanel();
            root.Dock = DockStyle.Fill;
            root.Padding = new Padding(24);
            root.ColumnCount = 1;
            root.RowCount = 5;
            root.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            root.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            root.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            root.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
            root.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            Controls.Add(root);

            PictureBox banner = MunicipalBranding.CreateBannerPictureBox(240, 160);
            banner.Margin = new Padding(0, 0, 0, 8);
            root.Controls.Add(banner, 0, 0);

            Label introLabel = new Label();
            introLabel.Text = "Browse upcoming municipal and community notices, then search by category or date.";
            introLabel.Font = new Font("Segoe UI", 9.5F);
            introLabel.ForeColor = Color.FromArgb(70, 82, 92);
            introLabel.AutoSize = true;
            introLabel.Margin = new Padding(0, 0, 0, 12);
            root.Controls.Add(introLabel, 0, 1);

            FlowLayoutPanel searchPanel = new FlowLayoutPanel();
            searchPanel.Dock = DockStyle.Top;
            searchPanel.AutoSize = true;
            searchPanel.WrapContents = true;
            searchPanel.Margin = new Padding(0, 0, 0, 12);
            root.Controls.Add(searchPanel, 0, 2);

            searchPanel.Controls.Add(CreateFilterLabel("Category"));
            categoryComboBox = new ComboBox();
            categoryComboBox.DropDownStyle = ComboBoxStyle.DropDownList;
            categoryComboBox.Font = new Font("Segoe UI", 9.5F);
            categoryComboBox.Width = 155;
            categoryComboBox.Items.Add("All categories");
            categoryComboBox.Items.AddRange(eventRepository.GetCategories().ToArray());
            categoryComboBox.SelectedIndex = 0;
            searchPanel.Controls.Add(categoryComboBox);

            searchPanel.Controls.Add(CreateFilterLabel("From"));
            fromDatePicker = CreateDatePicker();
            fromDatePicker.Value = DateTime.Today;
            searchPanel.Controls.Add(fromDatePicker);

            searchPanel.Controls.Add(CreateFilterLabel("To"));
            toDatePicker = CreateDatePicker();
            toDatePicker.Value = DateTime.Today.AddMonths(6);
            searchPanel.Controls.Add(toDatePicker);

            Button searchButton = CreateActionButton("Search");
            searchButton.Click += SearchButton_Click;
            searchPanel.Controls.Add(searchButton);

            Button clearButton = CreateSecondaryButton("Clear filters");
            clearButton.Click += ClearButton_Click;
            searchPanel.Controls.Add(clearButton);

            SplitContainer content = new SplitContainer();
            content.Dock = DockStyle.Fill;
            content.Orientation = Orientation.Vertical;
            content.Size = new Size(1100, 550);
            content.SplitterDistance = 600;
            content.Panel1MinSize = 480;
            content.Panel2MinSize = 230;
            root.Controls.Add(content, 0, 3);

            TableLayoutPanel eventsPanel = new TableLayoutPanel();
            eventsPanel.Dock = DockStyle.Fill;
            eventsPanel.Padding = new Padding(0, 0, 12, 0);
            eventsPanel.ColumnCount = 1;
            eventsPanel.RowCount = 3;
            eventsPanel.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            eventsPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
            eventsPanel.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            content.Panel1.Controls.Add(eventsPanel);

            resultCountLabel = CreateSectionLabel("Upcoming events");
            resultCountLabel.Margin = new Padding(0, 0, 0, 8);
            eventsPanel.Controls.Add(resultCountLabel, 0, 0);

            eventListView = new ListView();
            eventListView.Dock = DockStyle.Fill;
            eventListView.View = View.Details;
            eventListView.FullRowSelect = true;
            eventListView.GridLines = true;
            eventListView.HideSelection = false;
            eventListView.MultiSelect = false;
            eventListView.HeaderStyle = ColumnHeaderStyle.Nonclickable;
            eventListView.Font = new Font("Segoe UI", 9F);
            eventListView.Columns.Add("Date and time", 135);
            eventListView.Columns.Add("Event or announcement", 230);
            eventListView.Columns.Add("Category", 115);
            eventListView.Columns.Add("Location", 155);
            eventListView.SelectedIndexChanged += EventListView_SelectedIndexChanged;
            eventsPanel.Controls.Add(eventListView, 0, 1);

            eventDetailsLabel = new Label();
            eventDetailsLabel.Dock = DockStyle.Top;
            eventDetailsLabel.AutoSize = true;
            eventDetailsLabel.MaximumSize = new Size(580, 90);
            eventDetailsLabel.Font = new Font("Segoe UI", 9F);
            eventDetailsLabel.ForeColor = Color.FromArgb(45, 55, 65);
            eventDetailsLabel.Padding = new Padding(2, 8, 2, 0);
            eventDetailsLabel.Text = "Select an event to see its details.";
            eventsPanel.Controls.Add(eventDetailsLabel, 0, 2);

            TableLayoutPanel suggestionsPanel = new TableLayoutPanel();
            suggestionsPanel.Dock = DockStyle.Fill;
            suggestionsPanel.ColumnCount = 1;
            suggestionsPanel.RowCount = 4;
            suggestionsPanel.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            suggestionsPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 55));
            suggestionsPanel.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            suggestionsPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 45));
            content.Panel2.Controls.Add(suggestionsPanel);

            Label recommendationHeader = CreateSectionLabel("Recommended for you");
            recommendationHeader.Margin = new Padding(0, 0, 0, 8);
            suggestionsPanel.Controls.Add(recommendationHeader, 0, 0);

            recommendationListBox = new ListBox();
            recommendationListBox.Dock = DockStyle.Fill;
            recommendationListBox.Font = new Font("Segoe UI", 9F);
            recommendationListBox.IntegralHeight = false;
            recommendationListBox.SelectedIndexChanged += RecommendationListBox_SelectedIndexChanged;
            suggestionsPanel.Controls.Add(recommendationListBox, 0, 1);

            Label historyHeader = CreateSectionLabel("Recent searches");
            historyHeader.Margin = new Padding(0, 12, 0, 8);
            suggestionsPanel.Controls.Add(historyHeader, 0, 2);

            searchHistoryListBox = new ListBox();
            searchHistoryListBox.Dock = DockStyle.Fill;
            searchHistoryListBox.Font = new Font("Segoe UI", 8.5F);
            searchHistoryListBox.IntegralHeight = false;
            suggestionsPanel.Controls.Add(searchHistoryListBox, 0, 3);

            FlowLayoutPanel footer = new FlowLayoutPanel();
            footer.Dock = DockStyle.Fill;
            footer.FlowDirection = FlowDirection.RightToLeft;
            footer.AutoSize = true;
            footer.Margin = new Padding(0, 12, 0, 0);
            root.Controls.Add(footer, 0, 4);

            Button backButton = CreateSecondaryButton("Back to Main Menu");
            backButton.Click += delegate { Close(); };
            footer.Controls.Add(backButton);

            Label sampleDataLabel = new Label();
            sampleDataLabel.Text = "Sample events are provided for demonstration.";
            sampleDataLabel.Font = new Font("Segoe UI", 8.5F);
            sampleDataLabel.ForeColor = Color.FromArgb(99, 111, 122);
            sampleDataLabel.AutoSize = true;
            sampleDataLabel.Margin = new Padding(0, 10, 12, 0);
            footer.Controls.Add(sampleDataLabel);
        }

        private Label CreateFilterLabel(string text)
        {
            Label label = new Label();
            label.Text = text;
            label.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            label.ForeColor = Color.FromArgb(45, 55, 65);
            label.AutoSize = true;
            label.Margin = new Padding(0, 10, 5, 0);
            return label;
        }

        private Label CreateSectionLabel(string text)
        {
            Label label = new Label();
            label.Text = text;
            label.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            label.ForeColor = Color.FromArgb(28, 58, 85);
            label.AutoSize = true;
            return label;
        }

        private DateTimePicker CreateDatePicker()
        {
            DateTimePicker picker = new DateTimePicker();
            picker.Format = DateTimePickerFormat.Short;
            picker.ShowCheckBox = true;
            picker.Checked = false;
            picker.Width = 125;
            picker.Font = new Font("Segoe UI", 9F);
            return picker;
        }

        private Button CreateActionButton(string text)
        {
            Button button = new Button();
            button.Text = text;
            button.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            button.Width = 90;
            button.Height = 30;
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
            button.Width = 125;
            button.BackColor = Color.FromArgb(92, 105, 115);
            return button;
        }

        private void LoadUpcomingEvents()
        {
            DisplayEvents(eventRepository.GetUpcomingEvents());
            UpdateRecommendations(new List<MunicipalEvent>());
        }

        private void DisplayEvents(IEnumerable<MunicipalEvent> events)
        {
            eventListView.BeginUpdate();
            eventListView.Items.Clear();

            foreach (MunicipalEvent municipalEvent in events)
            {
                ListViewItem item = new ListViewItem(municipalEvent.EventDate.ToString("dd MMM yyyy HH:mm"));
                item.SubItems.Add(municipalEvent.Title);
                item.SubItems.Add(municipalEvent.Category);
                item.SubItems.Add(municipalEvent.Location);
                item.Tag = municipalEvent.EventId;
                eventListView.Items.Add(item);
            }

            eventListView.EndUpdate();
            resultCountLabel.Text = eventListView.Items.Count == 1
                ? "1 event or announcement"
                : eventListView.Items.Count + " events and announcements";
            eventDetailsLabel.Text = eventListView.Items.Count == 0
                ? "No upcoming events match those filters. Try a different category or date range."
                : "Select an event to see its details.";
        }

        private void UpdateRecommendations(IEnumerable<MunicipalEvent> displayedEvents)
        {
            HashSet<string> displayedIds = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            foreach (MunicipalEvent municipalEvent in displayedEvents)
            {
                displayedIds.Add(municipalEvent.EventId);
            }

            recommendationListBox.BeginUpdate();
            recommendationListBox.Items.Clear();
            List<MunicipalEvent> recommendations = eventRepository.GetRecommendations(5, displayedIds);
            if (recommendations.Count == 0)
            {
                recommendations = eventRepository.GetRecommendations(5, null);
            }
            foreach (MunicipalEvent municipalEvent in recommendations)
            {
                recommendationListBox.Items.Add(municipalEvent);
            }
            recommendationListBox.EndUpdate();

            searchHistoryListBox.BeginUpdate();
            searchHistoryListBox.Items.Clear();
            foreach (string search in eventRepository.GetRecentSearches(6))
            {
                searchHistoryListBox.Items.Add(search);
            }
            if (searchHistoryListBox.Items.Count == 0)
            {
                searchHistoryListBox.Items.Add("Search by category or date to build recommendations.");
            }
            searchHistoryListBox.EndUpdate();
        }

        private void SearchButton_Click(object sender, EventArgs e)
        {
            DateTime? fromDate = fromDatePicker.Checked ? fromDatePicker.Value.Date : (DateTime?)null;
            DateTime? toDate = toDatePicker.Checked ? toDatePicker.Value.Date : (DateTime?)null;
            if (fromDate.HasValue && toDate.HasValue && fromDate.Value > toDate.Value)
            {
                MessageBox.Show(this, "The 'From' date must be on or before the 'To' date.",
                    "Check the date range", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string category = categoryComboBox.SelectedItem == null
                ? "All categories"
                : categoryComboBox.SelectedItem.ToString();
            List<MunicipalEvent> results = eventRepository.SearchEvents(category, fromDate, toDate);
            DisplayEvents(results);
            UpdateRecommendations(results);
        }

        private void ClearButton_Click(object sender, EventArgs e)
        {
            categoryComboBox.SelectedIndex = 0;
            fromDatePicker.Checked = false;
            toDatePicker.Checked = false;
            LoadUpcomingEvents();
        }

        private void EventListView_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (eventListView.SelectedItems.Count == 0)
            {
                return;
            }

            string eventId = eventListView.SelectedItems[0].Tag as string;
            ShowEventDetails(eventRepository.GetById(eventId));
        }

        private void RecommendationListBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            MunicipalEvent municipalEvent = recommendationListBox.SelectedItem as MunicipalEvent;
            if (municipalEvent != null)
            {
                ShowEventDetails(municipalEvent);
            }
        }

        private void ShowEventDetails(MunicipalEvent municipalEvent)
        {
            if (municipalEvent == null)
            {
                eventDetailsLabel.Text = "Select an event to see its details.";
                return;
            }

            eventDetailsLabel.Text = municipalEvent.Title + " - " + municipalEvent.EventDate.ToString("dddd, dd MMMM yyyy 'at' HH:mm") +
                Environment.NewLine + municipalEvent.Location + " | " + municipalEvent.Category +
                Environment.NewLine + municipalEvent.Description;
        }
    }
}
