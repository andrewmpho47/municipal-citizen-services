using System.Collections.Generic;

namespace MunicipalReporter.Services
{
    public class ReportRepository<T>
    {
        private readonly List<T> items = new List<T>();

        public void Add(T item)
        {
            items.Add(item);
        }

        public void Add(IEnumerable<T> newItems)
        {
            if (newItems == null)
            {
                return;
            }

            foreach (T item in newItems)
            {
                Add(item);
            }
        }

        public List<T> GetAll()
        {
            return new List<T>(items);
        }

        public int Count
        {
            get { return items.Count; }
        }
    }
}
