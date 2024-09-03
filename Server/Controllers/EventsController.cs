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
        public async Task<ActionResult<int>> AddUpdateEvent(LeaveRequests events)
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

        [HttpPost("ApprovalRequest")]
        public async Task<ActionResult<int>> ApprovalRequest(LeaveRequests events)
        {
            try
            {
                _ = await _eventsRepository.ApproveLeaveRequestAsync(events);
                _logger.LogInformation("Leave Request updated successfully.");
                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Exception occurred while updating leave request: {ex.Message}");
                return BadRequest($"Exception occurred while updating leave request: {ex.Message}");
            }
        }

        [HttpPost("DeleteEvent")]
        public async Task<ActionResult<int>> DeleteEvent(Events events)
        {
            try
            {
                _ = await _eventsRepository.DeleteEventAsync(events);
                _logger.LogInformation("Event deleted successfully.");
                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Exception occurred while deleting event: {ex.Message}");
                return BadRequest($"Exception occurred while deleting event: {ex.Message}");
            }
        }

        [HttpPost("GetEvents")]
        public async Task<ActionResult<List<LeaveRequests>>> GetEvents([FromBody] EventFilter filter)
        {
            try
            {
                var events = await _eventsRepository.GetAllEventsAsync(filter);
                _logger.LogInformation("Events retrieved successfully.");
                return Ok(events);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Exception occurred while retrieving events: {ex.Message}");
                return BadRequest($"Exception occurred while retrieving events: {ex.Message}");
            }
        }


    }
}
