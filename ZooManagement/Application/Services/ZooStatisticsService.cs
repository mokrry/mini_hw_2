using System;
using System.Linq;
using ZooManagement.Infrastructure.Repositories;

namespace ZooManagement.Application.Services
{
    public class ZooStatisticsService
    {
        private readonly IAnimalRepository _animalRepository;
        private readonly IEnclosureRepository _enclosureRepository;

        public ZooStatisticsService(
            IAnimalRepository animalRepository,
            IEnclosureRepository enclosureRepository)
        {
            _animalRepository = animalRepository ?? throw new ArgumentNullException(nameof(animalRepository));
            _enclosureRepository = enclosureRepository ?? throw new ArgumentNullException(nameof(enclosureRepository));
        }

        public int GetTotalAnimals()
        {
            return _animalRepository.GetAll().Count();
        }

        public int GetFreeEnclosuresCount()
        {
            return _enclosureRepository
                .GetAll()
                .Count(e => e.AnimalsCount == 0);
        }
    }
}