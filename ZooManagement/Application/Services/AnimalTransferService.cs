using System;
using System.Collections.Generic;
using ZooManagement.Domain.Entities;
using ZooManagement.Domain.Events;
using ZooManagement.Infrastructure.Repositories;

namespace ZooManagement.Application.Services
{
    public class AnimalTransferService
    {
        private readonly IAnimalRepository _animalRepository;
        private readonly IEnclosureRepository _enclosureRepository;

        public AnimalTransferService(
            IAnimalRepository animalRepository,
            IEnclosureRepository enclosureRepository)
        {
            _animalRepository = animalRepository ?? throw new ArgumentNullException(nameof(animalRepository));
            _enclosureRepository = enclosureRepository ?? throw new ArgumentNullException(nameof(enclosureRepository));
        }

        public void TransferAnimal(Guid animalId, Guid destinationEnclosureId)
        {
            var animal = _animalRepository.GetById(animalId)
                         ?? throw new KeyNotFoundException($"Animal '{animalId}' was not found.");
            var targetEnclosure = _enclosureRepository.GetById(destinationEnclosureId)
                                    ?? throw new KeyNotFoundException($"Enclosure '{destinationEnclosureId}' was not found.");

            // Remove from previous enclosure, if any
            if (animal.CurrentEnclosureId.HasValue)
            {
                var previousEnclosure = _enclosureRepository.GetById(animal.CurrentEnclosureId.Value);
                previousEnclosure?.RemoveAnimal(animal);
            }

            // Transfer to the new enclosure
            targetEnclosure.AddAnimal(animal);
            var previousId = animal.CurrentEnclosureId;
            animal.MoveTo(destinationEnclosureId);

            // Domain event for audit/logging
            var movedEvent = new AnimalMovedEvent(animal, destinationEnclosureId);
            Console.WriteLine(
                $"[{movedEvent.MovedAt:O}] Animal '{animal.Nickname}' moved from '{previousId}' to '{destinationEnclosureId}'.");
        }

        public void TransferAnimals(IEnumerable<Guid> animalIds, Guid destinationEnclosureId)
        {
            if (animalIds == null) throw new ArgumentNullException(nameof(animalIds));

            foreach (var id in animalIds)
            {
                TransferAnimal(id, destinationEnclosureId);
            }
        }
    }
}
