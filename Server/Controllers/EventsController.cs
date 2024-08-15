using Microsoft.AspNetCore.Mvc;
using NCMS_wasm.Server.Repository;
using NCMS_wasm.Shared;

namespace NCMS_wasm.Server.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class EventsController : ControllerBase
    {
        private readonly ILogger<EventsController> _logger;
        private readonly EventsRepository _eventsRepository;

        public EventsController(ILogger<EventsController> logger, EventsRepository eventsRepository)
        {
            _logger = logger;
            _eventsRepository = eventsRepository;
        }


        [HttpPost("AddUpdateEvent")]
        public async Task<ActionResult<int>> AddUpdateEvent(Events events)
        {
            try
            {
                _ = await _eventsRepository.AddEventAsync(events);
                _logger.LogInformation("Event added/updated successfully.");
                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Exception occurred while adding/updating event: {ex.Message}");
                return BadRequest($"Exception occurred while adding/updating event: {ex.Message}");
            }
        }

        [HttpGet("GetEvents")]
        public async Task<ActionResult<List<Events>>> GetEvents()
        {
            try
            {
                var devices = await _eventsRepository.GetAllEventsAsync();
                _logger.LogInformation("Events retrieved successfully.");
                return Ok(devices);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Exception occurred while retrieving events: {ex.Message}");
                return BadRequest($"Exception occurred while retrieving events: {ex.Message}");
            }
        }


    }
}
