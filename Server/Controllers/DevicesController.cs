using Auth0.ManagementApi;
using Auth0.ManagementApi.Models;
using Microsoft.AspNetCore.Mvc;
using NCMS_wasm.Shared;
using NCMS_wasm.Server.Repository;

namespace NCMS_wasm.Server.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class DevicesController : ControllerBase
    {        
        private readonly ILogger<DevicesController> _logger;
        private readonly DeviceRepository _deviceRepository;

        public DevicesController(ILogger<DevicesController> logger, DeviceRepository deviceRepository)
        {
            _logger = logger;
            _deviceRepository = deviceRepository;
        }

        [HttpGet("GetNetworkDevices")]
        public async Task<ActionResult<List<Device>>> GetNetworkDevices()
        {
            try
            {
                var devices = await _deviceRepository.GetAllDevicesAsync();
                _logger.LogInformation("Network devices retrieved successfully.");
                return Ok(devices);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Exception occurred while retrieving network devices: {ex.Message}");
                return BadRequest($"Exception occurred while retrieving network devices: {ex.Message}");
            }
        }

        [HttpPost("AddUpdateDevice")]
        public async Task<ActionResult<int>> AddUpdateDevice(Device device)
        {
            try
            {
                int deviceId = await _deviceRepository.AddDeviceAsync(device);
                _logger.LogInformation("Device added/updated successfully.");
                return Ok(deviceId);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Exception occurred while adding/updating device: {ex.Message}");
                return BadRequest($"Exception occurred while adding/updating device: {ex.Message}");
            }
        }


    }
}
