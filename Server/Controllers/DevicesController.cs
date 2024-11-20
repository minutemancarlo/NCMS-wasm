using Auth0.ManagementApi;
using Auth0.ManagementApi.Models;
using Microsoft.AspNetCore.Mvc;
using NCMS_wasm.Shared;
using NCMS_wasm.Server.Repository;
using NCMS_wasm.Server.Logger;

namespace NCMS_wasm.Server.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class DevicesController : ControllerBase
    {        
        private readonly ILogger<DevicesController> _logger;
        private readonly DeviceRepository _deviceRepository;
        private readonly FileLogger _fileLogger;

        public DevicesController(ILogger<DevicesController> logger, DeviceRepository deviceRepository, IConfiguration configuration)
        {
            _logger = logger;
            _deviceRepository = deviceRepository;
            _fileLogger = new FileLogger(configuration);

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
                _fileLogger.Log($"Exception Occured in Endpoint [GetNetworkDevices]: {ex.Message}", DateTime.Now.ToString("MM-dd-yyyy") + ".txt", "DevicesController");

                _logger.LogError($"Exception occurred while retrieving network devices: {ex.Message}");
                return BadRequest($"Exception occurred while retrieving network devices: {ex.Message}");
            }
        }

        [HttpGet("GetLeakResult")]
        public async Task<ActionResult<bool>> GetLeakResult()
        {
            try
            {
                var result = await _deviceRepository.GetLeakResult();

                return Ok(result);
            }
            catch (Exception ex)
            {
                _fileLogger.Log($"Exception Occured in Endpoint [GetLeakResult]: {ex.Message}", DateTime.Now.ToString("MM-dd-yyyy") + ".txt", "DevicesController");

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
                _fileLogger.Log($"Exception Occured in Endpoint [AddUpdateDevice]: {ex.Message}", DateTime.Now.ToString("MM-dd-yyyy") + ".txt", "DevicesController");
                _logger.LogError($"Exception occurred while adding/updating device: {ex.Message}");
                return BadRequest($"Exception occurred while adding/updating device: {ex.Message}");
            }
        }

        [HttpPost("UpdateDeviceRoom")]
        public async Task<ActionResult<int>> UpdateDeviceRoom(Device device)
        {
            try
            {
                int deviceId = await _deviceRepository.UpdateDeviceRoom(device);                
                return Ok(deviceId);
            }
            catch (Exception ex)
            {
                _fileLogger.Log($"Exception Occured in Endpoint [UpdateDeviceRoom]: {ex.Message}", DateTime.Now.ToString("MM-dd-yyyy") + ".txt", "DevicesController");
                _logger.LogError($"Exception occurred while updating device: {ex.Message}");
                return BadRequest($"Exception occurred while updating device: {ex.Message}");
            }
        }


    }
}
