using System;
using System.Collections.Generic;
using ZooManagement.Domain.Entities;

namespace ZooManagement.Domain.Entities
{
    public class Enclosure
    {
        public Guid Id { get; private set; }
        public string Category { get; private set; }
        public double Area { get; private set; }
        public int Capacity { get; private set; }

        private readonly List<Animal> _residents = new();
        public IReadOnlyCollection<Animal> Residents => _residents.AsReadOnly();
        public int AnimalsCount => _residents.Count;

        private readonly List<DateTime> _cleanLog = new();
        public IReadOnlyList<DateTime> CleanLog => _cleanLog;

        public Enclosure(string category, double area, int capacity)
        {
            Id = Guid.NewGuid();
            Category = category ?? throw new ArgumentNullException(nameof(category));
            Area = area;
            Capacity = capacity;
        }

        public void AddAnimal(Animal animal)
        {
            if (animal == null) throw new ArgumentNullException(nameof(animal));
            if (_residents.Count >= Capacity)
                throw new InvalidOperationException("Enclosure is at full capacity.");
            _residents.Add(animal);
        }

        public void RemoveAnimal(Animal animal)
        {
            if (animal == null) throw new ArgumentNullException(nameof(animal));
            _residents.Remove(animal);
        }

        public void Clean() => _cleanLog.Add(DateTime.UtcNow);

        public bool IsFull() => _residents.Count >= Capacity;

        public double GetOccupancyRate() =>
            Capacity == 0 ? 0 : (double)_residents.Count / Capacity;
    }
}