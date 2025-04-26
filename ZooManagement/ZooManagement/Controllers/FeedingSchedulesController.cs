using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using ZooManagement.Domain.Entities;
using ZooManagement.Domain.ValueObject;
using ZooManagement.Infrastructure.Repositories;

namespace ZooManagement.Presentation.Controllers
{
    [ApiController]
    [Route("v1/feedschedules")]
    public class FeedingSchedulesController : ControllerBase
    {
        private readonly IFeedingScheduleRepository _scheduleRepository;
        private readonly IAnimalRepository _animalRepository;

        public FeedingSchedulesController(
            IFeedingScheduleRepository scheduleRepository,
            IAnimalRepository animalRepository)
        {
            _scheduleRepository = scheduleRepository ?? throw new ArgumentNullException(nameof(scheduleRepository));
            _animalRepository = animalRepository ?? throw new ArgumentNullException(nameof(animalRepository));
        }

        [HttpGet]
        public ActionResult<IEnumerable<FeedingSchedule>> GetAllSchedules()
        {
            var list = _scheduleRepository.GetAll();
            return Ok(list);
        }

        [HttpGet("{scheduleId}")]
        public ActionResult<FeedingSchedule> GetSchedule(Guid scheduleId)
        {
            var schedule = _scheduleRepository.GetById(scheduleId);
            if (schedule == null) return NotFound();
            return Ok(schedule);
        }

        [HttpPost]
        public ActionResult<FeedingSchedule> CreateSchedule([FromBody] CreateFeedingScheduleRequest request)
        {
            var animal = _animalRepository.GetById(request.AnimalId)
                         ?? throw new KeyNotFoundException($"Animal '{request.AnimalId}' not found.");
            var schedule = new FeedingSchedule(animal, request.ScheduleTime, new Food(request.Food));
            _scheduleRepository.Add(schedule);
            return CreatedAtAction(nameof(GetSchedule), new { scheduleId = schedule.Id }, schedule);
        }

        [HttpPut("{scheduleId}")]
        public IActionResult Reschedule(Guid scheduleId, [FromBody] RescheduleRequest request)
        {
            var schedule = _scheduleRepository.GetById(scheduleId);
            if (schedule == null) return NotFound();
            schedule.Reschedule(request.NewTime);
            return NoContent();
        }

        [HttpDelete("{scheduleId}")]
        public IActionResult DeleteSchedule(Guid scheduleId)
        {
            _scheduleRepository.Remove(scheduleId);
            return NoContent();
        }
    }

    public class CreateFeedingScheduleRequest
    {
        public Guid AnimalId { get; set; }
        public DateTime ScheduleTime { get; set; }
        public string Food { get; set; }
    }

    public class RescheduleRequest
    {
        public DateTime NewTime { get; set; }
    }
}
