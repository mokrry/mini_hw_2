using Microsoft.AspNetCore.Mvc;
using ZooManagement.Application.Services;

namespace ZooManagement.Presentation.Controllers
{
    [ApiController]
    [Route("v1/statistics")]
    public class StatisticsController : ControllerBase
    {
        private readonly ZooStatisticsService _statsService;

        public StatisticsController(ZooStatisticsService statsService)
        {
            _statsService = statsService ?? throw new ArgumentNullException(nameof(statsService));
        }

        [HttpGet]
        public ActionResult<object> GetStatistics()
        {
            var total = _statsService.GetTotalAnimals();
            var free = _statsService.GetFreeEnclosuresCount();
            return Ok(new { totalCount = total, freeEnclosures = free });
        }
    }
}