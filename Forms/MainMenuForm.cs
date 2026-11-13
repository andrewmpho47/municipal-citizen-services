using System;
using System.Drawing;
using System.Windows.Forms;
using MunicipalReporter.Models;
using MunicipalReporter.Services;

namespace MunicipalReporter.Forms
{
    public class MainMenuForm : Form
    {
        private static readonly ServiceRequestRepository IssueRepository = new ServiceRequestRepository();
        private static readonly EventRepository EventRepository = new EventRepository();
        private readonly UserRole userRole;
        private Label summaryLabel;

        public bool SwitchUserRequested { get; private set; }

        public MainMenuForm(UserRole authenticatedRole)
        {
            if (!Enum.IsDefined(typeof(UserRole), authenticatedRole))
            {
                throw new ArgumentOutOfRangeException("authenticatedRole");
            }

            userRole = authenticatedRole;
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
            Size = new Size(900, 650);
            MinimumSize = new Size(820, 600);
            StartPosition = FormStartPosition.CenterScreen;
            BackColor = Color.FromArgb(245, 248, 250);

            TableLayoutPanel root = new TableLayoutPanel();
            root.Dock = DockStyle.Fill;
            root.ColumnCount = 1;
            root.RowCount = 4;
            root.Padding = new Padding(32);
            root.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            root.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
            root.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            root.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            Controls.Add(root);

            TableLayoutPanel headerPanel = new TableLayoutPanel();
            headerPanel.Dock = DockStyle.Top;
            headerPanel.ColumnCount = 1;
            headerPanel.RowCount = 3;
            headerPanel.AutoSize = true;
            headerPanel.Margin = new Padding(0, 0, 0, 20);
            headerPanel.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            headerPanel.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            headerPanel.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            root.Controls.Add(headerPanel, 0, 0);

            Label titleLabel = new Label();
            titleLabel.Text = "Municipal Citizen Services";
            titleLabel.Font = new Font("Segoe UI", 18F, FontStyle.Bold);
            titleLabel.ForeColor = Color.FromArgb(28, 58, 85);
            titleLabel.AutoSize = true;
            titleLabel.Margin = new Padding(0, 0, 0, 6);
            headerPanel.Controls.Add(titleLabel, 0, 0);

            Label instructionLabel = new Label();
            instructionLabel.Text = userRole == UserRole.Admin
                ? "Review service requests and manage municipal events and announcements."
                : "Report a local issue, explore events, or track a service request.";
            instructionLabel.Font = new Font("Segoe UI", 10F);
            instructionLabel.ForeColor = Color.FromArgb(70, 82, 92);
            instructionLabel.AutoSize = true;
            headerPanel.Controls.Add(instructionLabel, 0, 1);

            Label roleLabel = new Label();
            roleLabel.Text = "Signed in as: " + (userRole == UserRole.Admin ? "Municipal Admin" : "Citizen");
            roleLabel.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            roleLabel.ForeColor = Color.FromArgb(36, 106, 163);
            roleLabel.AutoSize = true;
            roleLabel.Margin = new Padding(0, 6, 0, 0);
            headerPanel.Controls.Add(roleLabel, 0, 2);

            TableLayoutPanel menuPanel = new TableLayoutPanel();
            menuPanel.Dock = DockStyle.Fill;
            menuPanel.ColumnCount = 2;
            menuPanel.RowCount = 1;
            menuPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 320));
            menuPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
            menuPanel.Margin = new Padding(0, 0, 0, 12);
            root.Controls.Add(menuPanel, 0, 1);

            PictureBox banner = MunicipalBranding.CreateBannerPictureBox(320, 213);
            banner.Anchor = AnchorStyles.None;
            banner.Margin = new Padding(0, 0, 24, 0);
            menuPanel.Controls.Add(banner, 0, 0);

            FlowLayoutPanel buttonPanel = new FlowLayoutPanel();
            buttonPanel.AutoSize = true;
            buttonPanel.FlowDirection = FlowDirection.TopDown;
            buttonPanel.WrapContents = false;
            buttonPanel.Anchor = AnchorStyles.None;
            menuPanel.Controls.Add(buttonPanel, 1, 0);

            if (userRole == UserRole.Citizen)
            {
                Button reportButton = CreateMenuButton("Report Issues");
                reportButton.Click += ReportButton_Click;
                buttonPanel.Controls.Add(reportButton);
            }
            else
            {
                Button submittedIssuesButton = CreateMenuButton("View Submitted Issues");
                submittedIssuesButton.Click += ViewSubmittedIssuesButton_Click;
                buttonPanel.Controls.Add(submittedIssuesButton);
            }

            Button eventsButton = CreateMenuButton(userRole == UserRole.Admin
                ? "Manage Events and Announcements"
                : "Local Events and Announcements");
            eventsButton.Click += EventsButton_Click;
            buttonPanel.Controls.Add(eventsButton);

            Button statusButton = CreateMenuButton(userRole == UserRole.Admin
                ? "Review Service Requests"
                : "Track Service Request");
            statusButton.Click += StatusButton_Click;
            buttonPanel.Controls.Add(statusButton);

            Button switchUserButton = CreateMenuButton("Switch User / Log Out");
            switchUserButton.BackColor = Color.FromArgb(92, 105, 115);
            switchUserButton.Click += SwitchUserButton_Click;
            buttonPanel.Controls.Add(switchUserButton);

            summaryLabel = new Label();
            summaryLabel.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            summaryLabel.ForeColor = Color.FromArgb(28, 58, 85);
            summaryLabel.AutoSize = true;
            summaryLabel.Margin = new Padding(0, 16, 0, 0);
            root.Controls.Add(summaryLabel, 0, 2);

            Label footerLabel = new Label();
            footerLabel.Text = "Thank you for helping improve municipal services.";
            footerLabel.Font = new Font("Segoe UI", 9F);
            footerLabel.ForeColor = Color.FromArgb(99, 111, 122);
            footerLabel.AutoSize = true;
            footerLabel.Margin = new Padding(0, 8, 0, 0);
            root.Controls.Add(footerLabel, 0, 3);
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
            if (userRole != UserRole.Citizen)
            {
                return;
            }

            using (ReportIssueForm reportForm = new ReportIssueForm(IssueRepository, userRole))
            {
                Hide();
                reportForm.ShowDialog(this);
                Show();
            }
        }

        private void ViewSubmittedIssuesButton_Click(object sender, EventArgs e)
        {
            if (userRole != UserRole.Admin)
            {
                return;
            }

            using (IssueListForm issueListForm = new IssueListForm(IssueRepository, userRole))
            {
                Hide();
                issueListForm.ShowDialog(this);
                Show();
            }
        }

        private void EventsButton_Click(object sender, EventArgs e)
        {
            using (EventAnnouncementsForm eventsForm = new EventAnnouncementsForm(EventRepository, userRole))
            {
                Hide();
                eventsForm.ShowDialog(this);
                Show();
            }
        }

        private void StatusButton_Click(object sender, EventArgs e)
        {
            using (ServiceRequestStatusForm statusForm = new ServiceRequestStatusForm(IssueRepository, userRole))
            {
                Hide();
                statusForm.ShowDialog(this);
                Show();
            }
        }

        private void SwitchUserButton_Click(object sender, EventArgs e)
        {
            SwitchUserRequested = true;
            Close();
        }
    }
}
