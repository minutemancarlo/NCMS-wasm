using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NCMS_wasm.Server.Repository;
using NCMS_wasm.Shared;

namespace NCMS_wasm.Server.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class HotelManagementController : ControllerBase
    {
        private readonly ILogger<HotelManagementController> _logger;
        private readonly HotelRepository _hotelRepository;

        public HotelManagementController(ILogger<HotelManagementController> logger, HotelRepository hotelRepository)
        {
            _logger = logger;
            _hotelRepository = hotelRepository;
        }

        [HttpPost("AddAccomodations")]
        public async Task<ActionResult<int>> AddAccomodations(Accomodations info)
        {
            try
            {
                int accId = await _hotelRepository.AddAccomodationAsync(info);
                _logger.LogInformation("Accomodation added successfully.");
                return Ok(accId);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Exception occurred while adding accomodation info: {ex.Message}");
                return BadRequest($"Exception occurred while adding accomodation info: {ex.Message}");
            }
        }

        [HttpPost("AddRooms")]
        public async Task<ActionResult<int>> AddRooms(HotelRoom rooms)
        {
            try
            {
                int accId = await _hotelRepository.AddRoomsAsync(rooms);
                _logger.LogInformation("Room added successfully.");
                return Ok(accId);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Exception occurred while adding room info: {ex.Message}");
                return BadRequest($"Exception occurred while adding room info: {ex.Message}");
            }
        }

        [HttpGet("GetRooms")]
        public async Task<ActionResult<List<HotelRoom>>> GetAllRooms()
        {
            try
            {
                var devices = await _hotelRepository.GetAllRoomsAsync();
                _logger.LogInformation("Rooms retrieved successfully.");
                return Ok(devices);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Exception occurred while retrieving rooms: {ex.Message}");
                return BadRequest($"Exception occurred while retrieving rooms: {ex.Message}");
            }
        }
        [AllowAnonymous]
        [HttpGet("GetRoomInfo")]
        public async Task<ActionResult<List<RoomInfo>>> GetRoomInfo()
        {
            try
            {
                var devices = await _hotelRepository.GetAllRoomsInfoAsync();
                _logger.LogInformation("Rooms retrieved successfully.");
                return Ok(devices);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Exception occurred while retrieving rooms: {ex.Message}");
                return BadRequest($"Exception occurred while retrieving rooms: {ex.Message}");
            }
        }

    }
}
