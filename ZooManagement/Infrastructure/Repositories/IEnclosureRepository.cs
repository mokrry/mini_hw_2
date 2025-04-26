using System;
using System.Collections.Generic;
using ZooManagement.Domain.Entities;

namespace ZooManagement.Infrastructure.Repositories
{
    public interface IEnclosureRepository
    {
        void Add(Enclosure enclosure);
        Enclosure? GetById(Guid id);
        IEnumerable<Enclosure> GetAll();
        void Remove(Guid id);
    }
}