using System;
using System.Collections.Generic;
using System.Linq;
using MunicipalReporter.Models;

namespace MunicipalReporter.Services
{
    public class EventRepository
    {
        private readonly SortedDictionary<DateTime, List<MunicipalEvent>> eventsByDate =
            new SortedDictionary<DateTime, List<MunicipalEvent>>();
        private readonly Dictionary<string, MunicipalEvent> eventsById =
            new Dictionary<string, MunicipalEvent>(StringComparer.OrdinalIgnoreCase);
        private readonly Dictionary<string, HashSet<string>> eventIdsByCategory =
            new Dictionary<string, HashSet<string>>(StringComparer.OrdinalIgnoreCase);
        private readonly HashSet<string> categories = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        private readonly Queue<MunicipalEvent> upcomingEvents = new Queue<MunicipalEvent>();
        private readonly Stack<string> recentSearches = new Stack<string>();
        private readonly Dictionary<string, int> categorySearchCounts =
            new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);

        public EventRepository()
        {
            AddSampleEvents();
            RebuildUpcomingQueue();
        }

        public List<string> GetCategories()
        {
            List<string> result = new List<string>(categories);
            result.Sort(StringComparer.CurrentCultureIgnoreCase);
            return result;
        }

        public List<MunicipalEvent> GetUpcomingEvents()
        {
            return upcomingEvents.ToList();
        }

        public List<MunicipalEvent> SearchEvents(string category, DateTime? fromDate, DateTime? toDate)
        {
            List<MunicipalEvent> results = new List<MunicipalEvent>();
            string selectedCategory = string.IsNullOrWhiteSpace(category) || category == "All categories"
                ? null
                : category;

            if (selectedCategory != null)
            {
                HashSet<string> eventIds;
                if (eventIdsByCategory.TryGetValue(selectedCategory, out eventIds))
                {
                    foreach (string eventId in eventIds)
                    {
                        MunicipalEvent municipalEvent = eventsById[eventId];
                        if (IsWithinDateRange(municipalEvent, fromDate, toDate))
                        {
                            results.Add(municipalEvent);
                        }
                    }
                }
                results.Sort((first, second) => first.EventDate.CompareTo(second.EventDate));
            }
            else
            {
                foreach (KeyValuePair<DateTime, List<MunicipalEvent>> dateGroup in eventsByDate)
                {
                    DateTime eventDay = dateGroup.Key.Date;
                    if ((fromDate.HasValue && eventDay < fromDate.Value.Date) ||
                        (toDate.HasValue && eventDay > toDate.Value.Date))
                    {
                        continue;
                    }

                    results.AddRange(dateGroup.Value);
                }
            }

            RecordSearch(selectedCategory, fromDate, toDate, results);
            return results;
        }

        public MunicipalEvent GetById(string eventId)
        {
            MunicipalEvent municipalEvent;
            return eventId != null && eventsById.TryGetValue(eventId, out municipalEvent)
                ? municipalEvent
                : null;
        }

        public List<string> GetRecentSearches(int maximumCount)
        {
            List<string> result = new List<string>();
            foreach (string search in recentSearches)
            {
                if (result.Count == maximumCount)
                {
                    break;
                }

                result.Add(search);
            }

            return result;
        }

        public List<MunicipalEvent> GetRecommendations(int maximumCount, IEnumerable<string> excludedEventIds)
        {
            HashSet<string> excluded = new HashSet<string>(
                excludedEventIds ?? Enumerable.Empty<string>(),
                StringComparer.OrdinalIgnoreCase);
            EventRecommendationPriorityQueue recommendations = new EventRecommendationPriorityQueue();

            foreach (MunicipalEvent municipalEvent in GetUpcomingEvents())
            {
                if (excluded.Contains(municipalEvent.EventId))
                {
                    continue;
                }

                int categoryInterest;
                categorySearchCounts.TryGetValue(municipalEvent.Category, out categoryInterest);
                int daysUntilEvent = Math.Max(0, (municipalEvent.EventDate.Date - DateTime.Today).Days);
                int priority = (categoryInterest * 100) + Math.Max(0, 30 - daysUntilEvent);
                recommendations.Enqueue(municipalEvent, priority);
            }

            List<MunicipalEvent> result = new List<MunicipalEvent>();
            while (result.Count < maximumCount && recommendations.Count > 0)
            {
                result.Add(recommendations.Dequeue());
            }

            return result;
        }

        private bool IsWithinDateRange(MunicipalEvent municipalEvent, DateTime? fromDate, DateTime? toDate)
        {
            DateTime eventDay = municipalEvent.EventDate.Date;
            return (!fromDate.HasValue || eventDay >= fromDate.Value.Date) &&
                (!toDate.HasValue || eventDay <= toDate.Value.Date);
        }

        private void RecordSearch(string category, DateTime? fromDate, DateTime? toDate, List<MunicipalEvent> results)
        {
            string searchDescription = "Category: " + (category ?? "All categories") +
                "; dates: " + (fromDate.HasValue ? fromDate.Value.ToString("dd MMM yyyy") : "any") +
                " to " + (toDate.HasValue ? toDate.Value.ToString("dd MMM yyyy") : "any") +
                "; " + results.Count + " result(s)";
            recentSearches.Push(searchDescription);

            if (category != null)
            {
                int searchCount;
                categorySearchCounts.TryGetValue(category, out searchCount);
                categorySearchCounts[category] = searchCount + 1;
            }
        }

        private void AddEvent(MunicipalEvent municipalEvent)
        {
            eventsById.Add(municipalEvent.EventId, municipalEvent);
            categories.Add(municipalEvent.Category);

            List<MunicipalEvent> eventsOnDate;
            if (!eventsByDate.TryGetValue(municipalEvent.EventDate.Date, out eventsOnDate))
            {
                eventsOnDate = new List<MunicipalEvent>();
                eventsByDate.Add(municipalEvent.EventDate.Date, eventsOnDate);
            }
            eventsOnDate.Add(municipalEvent);
            eventsOnDate.Sort((first, second) => first.EventDate.CompareTo(second.EventDate));

            HashSet<string> eventIds;
            if (!eventIdsByCategory.TryGetValue(municipalEvent.Category, out eventIds))
            {
                eventIds = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
                eventIdsByCategory.Add(municipalEvent.Category, eventIds);
            }
            eventIds.Add(municipalEvent.EventId);
        }

        private void RebuildUpcomingQueue()
        {
            foreach (KeyValuePair<DateTime, List<MunicipalEvent>> dateGroup in eventsByDate)
            {
                foreach (MunicipalEvent municipalEvent in dateGroup.Value)
                {
                    if (municipalEvent.EventDate >= DateTime.Now)
                    {
                        upcomingEvents.Enqueue(municipalEvent);
                    }
                }
            }
        }

        private void AddSampleEvents()
        {
            DateTime today = DateTime.Today;
            AddEvent(new MunicipalEvent("EVT-1001", "Community clean-up day", "Community",
                "Join local volunteers to clean public spaces. Gloves and refuse bags will be provided.", today.AddDays(7).AddHours(9), "Ward 12 Community Hall, Johannesburg"));
            AddEvent(new MunicipalEvent("EVT-1002", "Water services maintenance notice", "Water",
                "Scheduled maintenance may temporarily interrupt water supply. Residents are encouraged to store water in advance.", today.AddDays(10).AddHours(8), "Central and surrounding suburbs, Cape Town"));
            AddEvent(new MunicipalEvent("EVT-1003", "Youth skills and career fair", "Youth",
                "Meet local training providers and employers. Bring a copy of your CV and identification.", today.AddDays(14).AddHours(10), "Durban City Hall"));
            AddEvent(new MunicipalEvent("EVT-1004", "Road safety awareness session", "Public Safety",
                "A community session covering pedestrian safety, road rules and how to report hazardous road conditions.", today.AddDays(18).AddHours(11), "Polokwane Civic Centre"));
            AddEvent(new MunicipalEvent("EVT-1005", "Mobile health screening", "Health",
                "Free basic health checks and wellness information. Services are offered on a first-come basis.", today.AddDays(21).AddHours(9), "Gqeberha Community Clinic"));
            AddEvent(new MunicipalEvent("EVT-1006", "Recycling collection and drop-off", "Waste",
                "Drop off separated recyclable materials and learn about the municipal recycling programme.", today.AddDays(25).AddHours(8), "Soweto Regional Library"));
            AddEvent(new MunicipalEvent("EVT-1007", "Electricity safety information session", "Electricity",
                "Learn how to report electricity faults and stay safe around damaged cables and electrical equipment.", today.AddDays(30).AddHours(14), "Bloemfontein Civic Hall"));
            AddEvent(new MunicipalEvent("EVT-1008", "Integrated development plan community meeting", "Community",
                "Residents can share priorities for local services and municipal planning. Public participation is welcome.", today.AddDays(35).AddHours(18), "Mbombela Municipal Offices"));
            AddEvent(new MunicipalEvent("EVT-1009", "Pothole reporting and road maintenance update", "Roads",
                "Municipal staff will share road maintenance updates and explain how residents can submit location details.", today.AddDays(42).AddHours(10), "Kimberley Library Hall"));
            AddEvent(new MunicipalEvent("EVT-1010", "Fire preparedness and safety demonstration", "Public Safety",
                "A practical demonstration on household fire prevention and emergency contacts.", today.AddDays(50).AddHours(9), "Rustenburg Sports Grounds"));
            AddEvent(new MunicipalEvent("EVT-1011", "Public library digital services workshop", "Education",
                "Get help using public library e-resources and learn about free digital literacy support.", today.AddDays(58).AddHours(13), "East London Main Library"));
            AddEvent(new MunicipalEvent("EVT-1012", "Municipal budget public participation meeting", "Community",
                "Residents are invited to comment on service delivery priorities and proposed municipal spending.", today.AddDays(65).AddHours(17), "Polokwane Civic Centre"));
        }
    }

    internal sealed class EventRecommendationPriorityQueue
    {
        private readonly List<Recommendation> heap = new List<Recommendation>();

        public int Count
        {
            get { return heap.Count; }
        }

        public void Enqueue(MunicipalEvent municipalEvent, int priority)
        {
            heap.Add(new Recommendation(municipalEvent, priority));
            int childIndex = heap.Count - 1;

            while (childIndex > 0)
            {
                int parentIndex = (childIndex - 1) / 2;
                if (ComesBefore(heap[parentIndex], heap[childIndex]))
                {
                    break;
                }

                Swap(parentIndex, childIndex);
                childIndex = parentIndex;
            }
        }

        public MunicipalEvent Dequeue()
        {
            if (heap.Count == 0)
            {
                throw new InvalidOperationException("The recommendation queue is empty.");
            }

            MunicipalEvent result = heap[0].Event;
            Recommendation last = heap[heap.Count - 1];
            heap.RemoveAt(heap.Count - 1);

            if (heap.Count > 0)
            {
                heap[0] = last;
                int parentIndex = 0;

                while (true)
                {
                    int leftIndex = (parentIndex * 2) + 1;
                    int rightIndex = leftIndex + 1;
                    int bestIndex = parentIndex;

                    if (leftIndex < heap.Count && ComesBefore(heap[leftIndex], heap[bestIndex]))
                    {
                        bestIndex = leftIndex;
                    }
                    if (rightIndex < heap.Count && ComesBefore(heap[rightIndex], heap[bestIndex]))
                    {
                        bestIndex = rightIndex;
                    }
                    if (bestIndex == parentIndex)
                    {
                        break;
                    }

                    Swap(parentIndex, bestIndex);
                    parentIndex = bestIndex;
                }
            }

            return result;
        }

        private static bool ComesBefore(Recommendation first, Recommendation second)
        {
            if (first.Priority != second.Priority)
            {
                return first.Priority > second.Priority;
            }
            if (first.Event.EventDate != second.Event.EventDate)
            {
                return first.Event.EventDate < second.Event.EventDate;
            }
            return string.Compare(first.Event.EventId, second.Event.EventId, StringComparison.OrdinalIgnoreCase) < 0;
        }

        private void Swap(int firstIndex, int secondIndex)
        {
            Recommendation temporary = heap[firstIndex];
            heap[firstIndex] = heap[secondIndex];
            heap[secondIndex] = temporary;
        }

        private sealed class Recommendation
        {
            public Recommendation(MunicipalEvent municipalEvent, int priority)
            {
                Event = municipalEvent;
                Priority = priority;
            }

            public MunicipalEvent Event { get; private set; }
            public int Priority { get; private set; }
        }
    }
}
