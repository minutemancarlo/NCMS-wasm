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

    }
}
