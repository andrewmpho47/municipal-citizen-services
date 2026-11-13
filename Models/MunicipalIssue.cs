using System;
using System.Collections.Generic;

namespace MunicipalReporter.Models
{
    public class MunicipalIssue
    {
        private static int nextReferenceNumber = 1000;

        public MunicipalIssue()
            : this(string.Empty, string.Empty, string.Empty, new List<ReportAttachment>())
        {
        }

        public MunicipalIssue(string location, string category, string description)
            : this(location, category, description, new List<ReportAttachment>())
        {
        }

        public MunicipalIssue(string location, string category, string description, List<ReportAttachment> attachments)
        {
            ReferenceNumber = "MUN-" + nextReferenceNumber++;
            Location = location;
            Category = category;
            Description = description;
            Attachments = attachments ?? new List<ReportAttachment>();
            CreatedAt = DateTime.Now;
            Status = "Submitted";
            StatusUpdatedAt = CreatedAt;
            StatusHistory = new List<string> { Status + " - " + CreatedAt.ToString("dd MMM yyyy HH:mm") };
        }

        public string ReferenceNumber { get; private set; }
        public string Location { get; set; }
        public string Category { get; set; }
        public string Description { get; set; }
        public List<ReportAttachment> Attachments { get; private set; }
        public DateTime CreatedAt { get; private set; }
        public string Status { get; private set; }
        public DateTime StatusUpdatedAt { get; private set; }
        public List<string> StatusHistory { get; private set; }

        internal void UpdateStatus(string status)
        {
            Status = status;
            StatusUpdatedAt = DateTime.Now;
            StatusHistory.Add(Status + " - " + StatusUpdatedAt.ToString("dd MMM yyyy HH:mm"));
        }

        public void AddAttachment(ReportAttachment attachment)
        {
            if (attachment != null)
            {
                Attachments.Add(attachment);
            }
        }

        public void AddAttachment(string filePath)
        {
            AddAttachment(new ReportAttachment(filePath));
        }

        public override string ToString()
        {
            return ReferenceNumber + " - " + Category + " at " + Location;
        }
    }
}
