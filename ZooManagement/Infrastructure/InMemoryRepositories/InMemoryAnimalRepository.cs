using System;
using System.Collections.Generic;
using System.Linq;
using ZooManagement.Domain.Entities;

namespace ZooManagement.Infrastructure.Repositories
{
    public class InMemoryAnimalRepository : IAnimalRepository
    {
        private readonly List<Animal> _store = new();

        public void Add(Animal animal)
        {
            if (animal == null) throw new ArgumentNullException(nameof(animal));
            _store.Add(animal);
        }

        public Animal? GetById(Guid id)
        {
            return _store.FirstOrDefault(a => a.Id == id);
        }

        public IEnumerable<Animal> GetAll()
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