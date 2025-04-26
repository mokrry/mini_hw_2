using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using ZooManagement.Domain.Entities;
using ZooManagement.Domain.ValueObject;
using ZooManagement.Infrastructure.Repositories;
using ZooManagement.Application.Services;

namespace ZooManagement.Presentation.Controllers
{
    [ApiController]
    [Route("v1/animals")]
    public class AnimalsController : ControllerBase
    {
        private readonly IAnimalRepository _repository;
        private readonly AnimalTransferService _transferService;

        public AnimalsController(IAnimalRepository repository, AnimalTransferService transferService)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
            _transferService = transferService ?? throw new ArgumentNullException(nameof(transferService));
        }

        [HttpGet]
        public ActionResult<IEnumerable<Animal>> GetAllAnimals()
        {
            var list = _repository.GetAll();
            return Ok(list);
        }

        [HttpGet("{animalId}")]
        public ActionResult<Animal> GetAnimal(Guid animalId)
        {
            var animal = _repository.GetById(animalId);
            if (animal == null) return NotFound();
            return Ok(animal);
        }

        [HttpPost]
        public ActionResult<Animal> CreateAnimal([FromBody] CreateAnimalRequest request)
        {
            var animal = new Animal(
                request.Species,
                request.Nickname,
                request.BirthDate,
                request.Gender,
                new Food(request.FavoriteFood)
            );
            _repository.Add(animal);
            return CreatedAtAction(nameof(GetAnimal), new { animalId = animal.Id }, animal);
        }

        [HttpDelete("{animalId}")]
        public IActionResult DeleteAnimal(Guid animalId)
        {
            _repository.Remove(animalId);
            return NoContent();
        }

        [HttpPost("{animalId}/feed")]
        public IActionResult FeedAnimal(Guid animalId, [FromBody] FeedRequest request)
        {
            var animal = _repository.GetById(animalId);
            if (animal == null) return NotFound();
            try
            {
                animal.Feed(new Food(request.Food));
                return NoContent();
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("{animalId}/treat")]
        public IActionResult TreatAnimal(Guid animalId, [FromBody] TreatRequest request)
        {
            var animal = _repository.GetById(animalId);
            if (animal == null) return NotFound();
            if (request.IsHealthy) animal.Recover();
            else animal.Infect();
            return NoContent();
        }

        [HttpPost("{animalId}/transfer")]
        public IActionResult TransferAnimal(Guid animalId, [FromBody] TransferRequest request)
        {
            try
            {
                _transferService.TransferAnimal(animalId, request.EnclosureId);
                return NoContent();
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }

    public class CreateAnimalRequest
    {
        public string Species { get; set; }
        public string Nickname { get; set; }
        public DateTime BirthDate { get; set; }
        public string Gender { get; set; }
        public string FavoriteFood { get; set; }
    }

    public class FeedRequest
    {
        public string Food { get; set; }
    }

    public class TreatRequest
    {
        public bool IsHealthy { get; set; }
    }

    public class TransferRequest
    {
        public Guid EnclosureId { get; set; }
    }
}
