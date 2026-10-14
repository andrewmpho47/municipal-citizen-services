using System;

namespace MunicipalReporter.Models
{
    public class MunicipalEvent
    {
        public MunicipalEvent(string eventId, string title, string category, string description, DateTime eventDate, string location)
        {
            EventId = eventId;
            Title = title;
            Category = category;
            Description = description;
            EventDate = eventDate;
            Location = location;
        }

        public string EventId { get; private set; }
        public string Title { get; private set; }
        public string Category { get; private set; }
        public string Description { get; private set; }
        public DateTime EventDate { get; private set; }
        public string Location { get; private set; }

        public override string ToString()
        {
            return EventDate.ToString("dd MMM yyyy") + " - " + Title + " (" + Category + ")";
        }
    }
}
