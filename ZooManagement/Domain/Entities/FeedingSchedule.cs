using System;
using ZooManagement.Domain.ValueObject;

namespace ZooManagement.Domain.Entities
{
    public class FeedingSchedule
    {
        public Guid Id { get; private set; }
        public Animal Subject { get; private set; }
        public DateTime ScheduleTime { get; private set; }
        public Food Meal { get; private set; }
        public bool Completed { get; private set; }

        public FeedingSchedule(Animal subject, DateTime scheduleTime, Food meal)
        {
            Id = Guid.NewGuid();
            Subject = subject ?? throw new ArgumentNullException(nameof(subject));
            ScheduleTime = scheduleTime;
            Meal = meal ?? throw new ArgumentNullException(nameof(meal));
            Completed = false;
        }

        public void Reschedule(DateTime newTime)
        {
            ScheduleTime = newTime;
        }

        public void Complete()
        {
            Completed = true;
        }
    }
}