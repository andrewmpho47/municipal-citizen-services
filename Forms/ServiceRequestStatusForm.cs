using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using MunicipalReporter.Models;
using MunicipalReporter.Services;

namespace MunicipalReporter.Forms
{
    public class ServiceRequestStatusForm : Form
    {
        private readonly ServiceRequestRepository repository;
        private readonly UserRole userRole;
        private TextBox residentDetailsTextBox;
        private ListView residentRequestListView;
        private ListView requestListView;
        private TextBox adminDetailsTextBox;
        private readonly Dictionary<string, Button> transitionButtons =
            new Dictionary<string, Button>(StringComparer.OrdinalIgnoreCase);

        public ServiceRequestStatusForm(ServiceRequestRepository serviceRequestRepository, UserRole authenticatedRole)
        {
            if (!Enum.IsDefined(typeof(UserRole), authenticatedRole))
            {
                throw new ArgumentOutOfRangeException("authenticatedRole");
            }

            repository = serviceRequestRepository;
            userRole = authenticatedRole;
            InitializeComponent();
            if (userRole == UserRole.Admin)
            {
                LoadRequests();
            }
            else
            {
                LoadCitizenRequests();
            }
        }

        private void InitializeComponent()
        {
            Text = "Service Request Status";
            Size = new Size(1000, 720);
            MinimumSize = new Size(820, 600);
            StartPosition = FormStartPosition.CenterParent;
            BackColor = Color.FromArgb(245, 248, 250);

            TableLayoutPanel root = new TableLayoutPanel();
            root.Dock = DockStyle.Fill;
            root.Padding = new Padding(24);
            root.ColumnCount = 1;
            root.RowCount = 3;
            root.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            root.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
            root.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            Controls.Add(root);

            Label title = new Label();
            title.Text = "Service Request Status";
            title.Font = new Font("Segoe UI", 18F, FontStyle.Bold);
            title.ForeColor = Color.FromArgb(28, 58, 85);
            title.AutoSize = true;
            title.Margin = new Padding(0, 0, 0, 12);
            root.Controls.Add(title, 0, 0);

            TabControl tabs = new TabControl();
            tabs.Dock = DockStyle.Fill;
            tabs.Font = new Font("Segoe UI", 10F);
            root.Controls.Add(tabs, 0, 1);

            if (userRole == UserRole.Citizen)
            {
                TabPage citizenTab = new TabPage("Citizen");
                citizenTab.BackColor = Color.FromArgb(245, 248, 250);
                tabs.TabPages.Add(citizenTab);
                BuildResidentTab(citizenTab);
            }
            else
            {
                TabPage adminTab = new TabPage("Municipal Admin");
                adminTab.BackColor = Color.FromArgb(245, 248, 250);
                tabs.TabPages.Add(adminTab);
                BuildAdminTab(adminTab);
            }

            Button closeButton = CreateButton("Close", Color.FromArgb(92, 105, 115), 120);
            closeButton.Anchor = AnchorStyles.Right;
            closeButton.Margin = new Padding(0, 12, 0, 0);
            closeButton.Click += delegate { Close(); };
            root.Controls.Add(closeButton, 0, 2);
        }

        private void BuildResidentTab(TabPage tab)
        {
            TableLayoutPanel content = new TableLayoutPanel();
            content.Dock = DockStyle.Fill;
            content.Padding = new Padding(12);
            content.ColumnCount = 1;
            content.RowCount = 3;
            content.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            content.RowStyles.Add(new RowStyle(SizeType.Percent, 55));
            content.RowStyles.Add(new RowStyle(SizeType.Percent, 45));
            tab.Controls.Add(content);

            Label instructions = new Label();
            instructions.Text = "Your submitted issues and their latest status are listed below. Select an issue to see its update history.";
            instructions.Font = new Font("Segoe UI", 10F);
            instructions.AutoSize = true;
            instructions.Margin = new Padding(0, 0, 0, 12);
            content.Controls.Add(instructions, 0, 0);

            residentRequestListView = CreateRequestListView();
            residentRequestListView.Columns.Add("Reference", 115);
            residentRequestListView.Columns.Add("Category", 125);
            residentRequestListView.Columns.Add("Location", 210);
            residentRequestListView.Columns.Add("Status", 125);
            residentRequestListView.Columns.Add("Submitted", 145);
            residentRequestListView.SelectedIndexChanged += ResidentRequestListView_SelectedIndexChanged;
            content.Controls.Add(residentRequestListView, 0, 1);

            residentDetailsTextBox = CreateDetailsTextBox();
            residentDetailsTextBox.Text = "Select an issue to see its details and status history.";
            content.Controls.Add(residentDetailsTextBox, 0, 2);
        }

        private void BuildAdminTab(TabPage tab)
        {
            TableLayoutPanel content = new TableLayoutPanel();
            content.Dock = DockStyle.Fill;
            content.Padding = new Padding(12);
            content.ColumnCount = 1;
            content.RowCount = 4;
            content.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            content.RowStyles.Add(new RowStyle(SizeType.Percent, 58));
            content.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            content.RowStyles.Add(new RowStyle(SizeType.Percent, 42));
            tab.Controls.Add(content);

            Label instructions = new Label();
            instructions.Text = "Select a request to review it. You can approve or reject a new report directly, or move it to Under Review first.";
            instructions.Font = new Font("Segoe UI", 9.5F);
            instructions.AutoSize = true;
            instructions.Margin = new Padding(0, 0, 0, 8);
            content.Controls.Add(instructions, 0, 0);

            requestListView = CreateRequestListView();
            requestListView.Columns.Add("Reference", 115);
            requestListView.Columns.Add("Category", 115);
            requestListView.Columns.Add("Location", 190);
            requestListView.Columns.Add("Status", 125);
            requestListView.Columns.Add("Submitted", 145);
            requestListView.SelectedIndexChanged += RequestListView_SelectedIndexChanged;
            content.Controls.Add(requestListView, 0, 1);

            FlowLayoutPanel actions = new FlowLayoutPanel();
            actions.AutoSize = true;
            actions.Dock = DockStyle.Fill;
            actions.WrapContents = true;
            actions.Margin = new Padding(0, 8, 0, 8);
            content.Controls.Add(actions, 0, 2);

            Button refreshButton = CreateButton("Refresh", Color.FromArgb(92, 105, 115), 100);
            refreshButton.Click += delegate { LoadRequests(); };
            actions.Controls.Add(refreshButton);

            AddTransitionButton(actions, "Under Review");
            AddTransitionButton(actions, "Approved");
            AddTransitionButton(actions, "Rejected");
            AddTransitionButton(actions, "In Progress");
            AddTransitionButton(actions, "Resolved");

            adminDetailsTextBox = CreateDetailsTextBox();
            adminDetailsTextBox.Text = "Select a request to review its details and status history.";
            content.Controls.Add(adminDetailsTextBox, 0, 3);
        }

        private void AddTransitionButton(Control parent, string status)
        {
            Button button = CreateButton(status, Color.FromArgb(36, 106, 163), 112);
            button.Enabled = false;
            button.Click += delegate { ChangeSelectedStatus(status); };
            transitionButtons.Add(status, button);
            parent.Controls.Add(button);
        }

        private void LoadRequests()
        {
            string selectedReference = requestListView.SelectedItems.Count == 0
                ? null
                : requestListView.SelectedItems[0].Tag as string;

            ListViewItem itemToSelect = null;
            requestListView.BeginUpdate();
            requestListView.Items.Clear();
            foreach (MunicipalIssue issue in repository.GetAll())
            {
                ListViewItem item = new ListViewItem(issue.ReferenceNumber);
                item.SubItems.Add(issue.Category);
                item.SubItems.Add(issue.Location);
                item.SubItems.Add(issue.Status);
                item.SubItems.Add(issue.CreatedAt.ToString("dd MMM yyyy HH:mm"));
                item.Tag = issue.ReferenceNumber;
                requestListView.Items.Add(item);

                if (string.Equals(issue.ReferenceNumber, selectedReference, StringComparison.OrdinalIgnoreCase))
                {
                    itemToSelect = item;
                }
            }

            if (itemToSelect == null && requestListView.Items.Count > 0)
            {
                itemToSelect = requestListView.Items[0];
            }

            requestListView.EndUpdate();

            if (itemToSelect != null)
            {
                itemToSelect.Selected = true;
                itemToSelect.Focused = true;
            }

            if (requestListView.Items.Count == 0)
            {
                adminDetailsTextBox.Text = "There are no submitted issues to review yet.";
            }

            UpdateTransitionButtons();
        }

        private ListView CreateRequestListView()
        {
            ListView listView = new ListView();
            listView.Dock = DockStyle.Fill;
            listView.View = View.Details;
            listView.FullRowSelect = true;
            listView.GridLines = true;
            listView.HideSelection = false;
            listView.MultiSelect = false;
            listView.Font = new Font("Segoe UI", 9F);
            return listView;
        }

        private void LoadCitizenRequests()
        {
            string selectedReference = residentRequestListView.SelectedItems.Count == 0
                ? null
                : residentRequestListView.SelectedItems[0].Tag as string;
            ListViewItem itemToSelect = null;

            residentRequestListView.BeginUpdate();
            residentRequestListView.Items.Clear();
            foreach (MunicipalIssue issue in repository.GetAll())
            {
                ListViewItem item = CreateRequestListItem(issue);
                residentRequestListView.Items.Add(item);
                if (string.Equals(issue.ReferenceNumber, selectedReference, StringComparison.OrdinalIgnoreCase))
                {
                    itemToSelect = item;
                }
            }
            residentRequestListView.EndUpdate();

            if (itemToSelect == null && residentRequestListView.Items.Count > 0)
            {
                itemToSelect = residentRequestListView.Items[0];
            }

            if (itemToSelect != null)
            {
                itemToSelect.Selected = true;
                itemToSelect.Focused = true;
            }
            else
            {
                residentDetailsTextBox.Text = "You have not submitted any issues yet.";
            }
        }

        private ListViewItem CreateRequestListItem(MunicipalIssue issue)
        {
            ListViewItem item = new ListViewItem(issue.ReferenceNumber);
            item.SubItems.Add(issue.Category);
            item.SubItems.Add(issue.Location);
            item.SubItems.Add(issue.Status);
            item.SubItems.Add(issue.CreatedAt.ToString("dd MMM yyyy HH:mm"));
            item.Tag = issue.ReferenceNumber;
            return item;
        }

        private void RequestListView_SelectedIndexChanged(object sender, EventArgs e)
        {
            MunicipalIssue issue = GetSelectedIssue();
            adminDetailsTextBox.Text = issue == null
                ? "Select a request to review its details and status history."
                : FormatIssueDetails(issue);
            UpdateTransitionButtons();
        }

        private void ResidentRequestListView_SelectedIndexChanged(object sender, EventArgs e)
        {
            MunicipalIssue issue = GetSelectedCitizenIssue();
            residentDetailsTextBox.Text = issue == null
                ? "Select an issue to see its details and status history."
                : FormatIssueDetails(issue);
        }

        private void ChangeSelectedStatus(string status)
        {
            if (userRole != UserRole.Admin)
            {
                throw new UnauthorizedAccessException("Only a municipal admin may change request status.");
            }

            MunicipalIssue issue = GetSelectedIssue();
            if (issue == null)
            {
                return;
            }

            DialogResult confirmation = MessageBox.Show(
                this,
                "Change request " + issue.ReferenceNumber + " (" + issue.Category + " at " + issue.Location +
                ") from " + issue.Status + " to " + status + "?",
                "Confirm status change",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question,
                MessageBoxDefaultButton.Button2);
            if (confirmation != DialogResult.Yes)
            {
                return;
            }

            repository.ChangeStatus(issue.ReferenceNumber, status);
            LoadRequests();
            MunicipalIssue updatedIssue = repository.GetByReference(issue.ReferenceNumber);
            adminDetailsTextBox.Text = FormatIssueDetails(updatedIssue);
            UpdateTransitionButtons();
            MessageBox.Show(
                this,
                "Request " + updatedIssue.ReferenceNumber + " is now " + updatedIssue.Status + ".",
                "Request updated",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information);
        }

        private MunicipalIssue GetSelectedIssue()
        {
            if (requestListView.SelectedItems.Count == 0)
            {
                return null;
            }

            return repository.GetByReference(requestListView.SelectedItems[0].Tag as string);
        }

        private MunicipalIssue GetSelectedCitizenIssue()
        {
            if (residentRequestListView.SelectedItems.Count == 0)
            {
                return null;
            }

            return repository.GetByReference(residentRequestListView.SelectedItems[0].Tag as string);
        }

        private void UpdateTransitionButtons()
        {
            MunicipalIssue issue = GetSelectedIssue();
            List<string> allowedStatuses = issue == null
                ? new List<string>()
                : repository.GetAllowedNextStatuses(issue.ReferenceNumber);

            foreach (KeyValuePair<string, Button> transitionButton in transitionButtons)
            {
                bool isAllowed = allowedStatuses.Contains(transitionButton.Key);
                transitionButton.Value.Enabled = isAllowed;
                transitionButton.Value.BackColor = isAllowed
                    ? Color.FromArgb(36, 106, 163)
                    : Color.FromArgb(210, 216, 221);
                transitionButton.Value.ForeColor = isAllowed
                    ? Color.White
                    : Color.FromArgb(95, 105, 113);
            }
        }

        private string FormatIssueDetails(MunicipalIssue issue)
        {
            List<string> workflowPath = repository.GetWorkflowPath(issue.Status);
            return "Reference: " + issue.ReferenceNumber +
                Environment.NewLine + "Category: " + issue.Category +
                Environment.NewLine + "Location: " + issue.Location +
                Environment.NewLine + "Description: " + issue.Description +
                Environment.NewLine + "Current status: " + issue.Status +
                Environment.NewLine + "Last updated: " + issue.StatusUpdatedAt.ToString("dd MMM yyyy HH:mm") +
                Environment.NewLine + "Progress: " + string.Join(" > ", workflowPath) +
                Environment.NewLine + Environment.NewLine + "Status history:" +
                Environment.NewLine + string.Join(Environment.NewLine, issue.StatusHistory);
        }

        private TextBox CreateDetailsTextBox()
        {
            TextBox textBox = new TextBox();
            textBox.Dock = DockStyle.Fill;
            textBox.Multiline = true;
            textBox.ReadOnly = true;
            textBox.ScrollBars = ScrollBars.Vertical;
            textBox.Font = new Font("Segoe UI", 10F);
            textBox.BackColor = Color.White;
            return textBox;
        }

        private Button CreateButton(string text, Color backColor, int width)
        {
            Button button = new Button();
            button.Text = text;
            button.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            button.Width = width;
            button.Height = 34;
            button.Margin = new Padding(0, 0, 8, 6);
            button.BackColor = backColor;
            button.ForeColor = Color.White;
            button.FlatStyle = FlatStyle.Flat;
            button.FlatAppearance.BorderSize = 0;
            button.UseVisualStyleBackColor = false;
            return button;
        }
    }
}
