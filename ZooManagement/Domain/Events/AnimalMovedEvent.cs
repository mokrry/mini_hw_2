using System;
using ZooManagement.Domain.Entities;

namespace ZooManagement.Domain.Events
{
    public class AnimalMovedEvent
    {
        public Animal Animal { get; }
        public Guid NewEnclosureId { get; }
        public DateTime MovedAt { get; }

        public AnimalMovedEvent(Animal animal, Guid newEnclosureId)
        {
            Animal = animal ?? throw new ArgumentNullException(nameof(animal));
            NewEnclosureId = newEnclosureId;
            MovedAt = DateTime.UtcNow;
        }
    }
}