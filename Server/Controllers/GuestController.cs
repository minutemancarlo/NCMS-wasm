using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NCMS_wasm.Server.Logger;
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
        private readonly FileLogger _fileLogger;
        public GuestController(ILogger<GuestController> logger, GuestRepository guestRepository, IConfiguration configuration)
        {
            _logger = logger;
            _guestRepository = guestRepository;
            _fileLogger = new FileLogger(configuration);
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
                _fileLogger.Log($"Exception Occured in Endpoint [AddInquiries]: {ex.Message}", DateTime.Now.ToString("MM-dd-yyyy") + ".txt", "GuestController");
                _logger.LogError($"Exception occurred while adding inquiry: {ex.Message}");
                return BadRequest($"Exception occurred while adding inquiry: {ex.Message}");
            }
        }


    }
}
