using System;
using System.Collections.Generic;
using System.Linq;
using ZooManagement.Domain.Entities;

namespace ZooManagement.Infrastructure.Repositories
{
    public class InMemoryFeedingScheduleRepository : IFeedingScheduleRepository
    {
        private readonly List<FeedingSchedule> _store = new();

        public void Add(FeedingSchedule schedule)
        {
            if (schedule == null) throw new ArgumentNullException(nameof(schedule));
            _store.Add(schedule);
        }

        public FeedingSchedule? GetById(Guid id)
        {
            return _store.FirstOrDefault(s => s.Id == id);
        }

        public IEnumerable<FeedingSchedule> GetAll()
        {
            return _store.AsReadOnly();
        }

        public void Remove(Guid id)
        {
            var item = GetById(id);
            if (item != null) _store.Remove(item);
        }
    }
}