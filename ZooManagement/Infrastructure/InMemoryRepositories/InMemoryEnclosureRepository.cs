using System;
using System.Collections.Generic;
using System.Linq;
using ZooManagement.Domain.Entities;

namespace ZooManagement.Infrastructure.Repositories
{
    public class InMemoryEnclosureRepository : IEnclosureRepository
    {
        private readonly List<Enclosure> _store = new();

        public void Add(Enclosure enclosure)
        {
            if (enclosure == null) throw new ArgumentNullException(nameof(enclosure));
            _store.Add(enclosure);
        }

        public Enclosure? GetById(Guid id)
        {
            return _store.FirstOrDefault(e => e.Id == id);
        }

        public IEnumerable<Enclosure> GetAll()
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