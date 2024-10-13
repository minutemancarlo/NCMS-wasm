using ArduinoWebAPI.Models;
using ArduinoWebAPI.Repository;
using Microsoft.AspNetCore.Mvc;

namespace ArduinoWebAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class HandshakeController : ControllerBase
    {
        private readonly DeviceRepository _deviceRepository;
        private readonly ILogger<HandshakeController> _logger;
        public HandshakeController(DeviceRepository deviceRepository, ILogger<HandshakeController> logger)
        {            
            _deviceRepository = deviceRepository;
            _logger = logger;
        }

        [HttpGet]
        public IActionResult Get()
        {            
            return Ok(new { Message = "Handshake successful", Timestamp = DateTime.UtcNow });
        }

        [HttpPost("PostHandshakeDetails")]
        public async Task<IActionResult> PostHandshakeDetails(HandshakeDetails details)
        {
            try
            {
                int deviceId = await _deviceRepository.SaveHandshakeDetails(details);                
                return Ok(deviceId);
            }
            catch (Exception ex)
            {
               
                return BadRequest($"Exception occurred while adding/updating device: {ex.Message}");
            }
        }
    }
}
