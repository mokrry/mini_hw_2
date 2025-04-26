using System;
using System.Collections.Generic;
using ZooManagement.Domain.Entities;

namespace ZooManagement.Infrastructure.Repositories
{
    public interface IAnimalRepository
    {
        void Add(Animal animal);
        Animal? GetById(Guid id);
        IEnumerable<Animal> GetAll();
        void Remove(Guid id);
    }
}