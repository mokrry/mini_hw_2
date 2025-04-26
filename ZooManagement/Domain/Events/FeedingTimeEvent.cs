using System;
using ZooManagement.Domain.Entities;

namespace ZooManagement.Domain.Events
{
    public class FeedingTimeEvent
    {
        public FeedingSchedule Schedule { get; }
        public DateTime OccurredAt { get; }

        public FeedingTimeEvent(FeedingSchedule schedule)
        {
            Schedule = schedule ?? throw new ArgumentNullException(nameof(schedule));
            OccurredAt = DateTime.UtcNow;
        }
    }
}