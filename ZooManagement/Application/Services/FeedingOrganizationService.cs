using System;
using ZooManagement.Domain.Entities;
using ZooManagement.Infrastructure.Repositories;

namespace ZooManagement.Application.Services
{
    public class FeedingOrganizationService
    {
        private readonly IFeedingScheduleRepository _scheduleRepository;

        public FeedingOrganizationService(IFeedingScheduleRepository scheduleRepository)
        {
            _scheduleRepository = scheduleRepository ?? throw new ArgumentNullException(nameof(scheduleRepository));
        }

        public void AddFeedingSchedule(FeedingSchedule schedule)
        {
            if (schedule == null) throw new ArgumentNullException(nameof(schedule));
            _scheduleRepository.Add(schedule);
        }

        public void CompleteFeeding(Guid scheduleId)
        {
            var schedule = _scheduleRepository.GetById(scheduleId)
                           ?? throw new InvalidOperationException("Feeding schedule not found.");
            schedule.Complete();
        }
    }
}