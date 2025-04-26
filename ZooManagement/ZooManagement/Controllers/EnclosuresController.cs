using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using ZooManagement.Domain.Entities;
using ZooManagement.Infrastructure.Repositories;

namespace ZooManagement.Presentation.Controllers
{
    [ApiController]
    [Route("v1/enclosures")]
    public class EnclosuresController : ControllerBase
    {
        private readonly IEnclosureRepository _repository;

        public EnclosuresController(IEnclosureRepository repository)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        }

        [HttpGet]
        public ActionResult<IEnumerable<Enclosure>> GetAllEnclosures()
        {
            var items = _repository.GetAll();
            return Ok(items);
        }

        [HttpGet("{enclosureId}")]
        public ActionResult<Enclosure> GetEnclosure(Guid enclosureId)
        {
            var enclosure = _repository.GetById(enclosureId);
            if (enclosure == null) return NotFound();
            return Ok(enclosure);
        }

        [HttpPost]
        public ActionResult<Enclosure> CreateEnclosure([FromBody] CreateEnclosureRequest request)
        {
            var enclosure = new Enclosure(request.Category, request.Area, request.Capacity);
            _repository.Add(enclosure);
            return CreatedAtAction(nameof(GetEnclosure), new { enclosureId = enclosure.Id }, enclosure);
        }

        [HttpDelete("{enclosureId}")]
        public IActionResult DeleteEnclosure(Guid enclosureId)
        {
            _repository.Remove(enclosureId);
            return NoContent();
        }

        [HttpPost("{enclosureId}/clean")]
        public IActionResult CleanEnclosure(Guid enclosureId)
        {
            var enclosure = _repository.GetById(enclosureId);
            if (enclosure == null) return NotFound();
            enclosure.Clean();
            return NoContent();
        }
    }

    public class CreateEnclosureRequest
    {
        public string Category { get; set; }
        public double Area { get; set; }
        public int Capacity { get; set; }
    }
}
