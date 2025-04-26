using System;
using System.Collections.Generic;
using ZooManagement.Domain.Entities;

namespace ZooManagement.Infrastructure.Repositories
{
    public interface IFeedingScheduleRepository
    {
        void Add(FeedingSchedule schedule);
        FeedingSchedule? GetById(Guid id);
        IEnumerable<FeedingSchedule> GetAll();
        void Remove(Guid id);
    }
}