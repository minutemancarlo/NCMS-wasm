using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NCMS_wasm.Server.Repository;
using NCMS_wasm.Shared;

namespace NCMS_wasm.Server.Controllers
{
   
    [Route("[controller]")]
    [ApiController]
    public class GuestController : ControllerBase
    {
        private readonly ILogger<GuestController> _logger;
        private readonly GuestRepository _guestRepository;
        public GuestController(ILogger<GuestController> logger, GuestRepository guestRepository)
        {
            _logger = logger;
            _guestRepository = guestRepository;
        }

        [AllowAnonymous]
        [HttpPost("AddInquiries")]
        public async Task<ActionResult<int>> AddInquiries(Inquiries inquiries)
        {
            try
            {
                var result = await _guestRepository.AddInquiriesAsync(inquiries);
                _logger.LogInformation("Inquiry added successfully.");
                return Ok(result); 
            }
            catch (Exception ex)
            {
                _logger.LogError($"Exception occurred while adding inquiry: {ex.Message}");
                return BadRequest($"Exception occurred while adding inquiry: {ex.Message}");
            }
        }


    }
}
